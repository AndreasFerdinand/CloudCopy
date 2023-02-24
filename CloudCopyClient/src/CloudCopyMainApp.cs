namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class CloudCopyMainApp
    {
        private readonly ICloudClientFactory cloudClientFactory;

        public CloudCopyMainApp(ICloudClientFactory cloudClientFactory)
        {
            this.cloudClientFactory = cloudClientFactory;
        }

        public Task<int> Configure(string Hostname, string Username, bool MaintainPassword)
        {
            ConfigFileHandler configFileHandler;

            try
            {
                try
                {
                    configFileHandler = new ConfigFileHandler();
                }
                catch (FileProcessingError)
                {
                    configFileHandler = new ConfigFileHandler(true);
                }

                if (!string.IsNullOrEmpty(Hostname))
                {
                    configFileHandler.Hostname = Hostname;
                    configFileHandler.Password = "";
                }

                if (!string.IsNullOrEmpty(Username))
                {
                    configFileHandler.Username = Username;
                    configFileHandler.Password = "";
                }

                if (MaintainPassword)
                {
                    if (string.IsNullOrEmpty(configFileHandler.Hostname))
                    {
                        throw new CloudCopyParametrizationException("Hostname must be provided from commandline or already maintained in the configuration file");
                    }

                    Console.WriteLine($"Please enter Password for user '{configFileHandler.Username}' to connect to host '{configFileHandler.Hostname}'");

                    configFileHandler.Password = ConsoleCredentialHandler.ReadPassword();
                }

                configFileHandler.SaveConfigurationFile();

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("WARNING: Password is saved unencrypted in configuration file!");
                    Console.ResetColor();
                }

                Console.WriteLine($"Configuration successfully written to '{configFileHandler.GetConfigFilePath()}'.");

                return Task.FromResult(0);

            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine(ex.Message);

                if ( ex.InnerException != null )
                {
                    Console.WriteLine(ex.InnerException.Message);
                }

                Console.ResetColor();

                return Task.FromResult(1);
            }
        }

        public async Task DownloadFiles(string Hostname, string Username, string FilterPattern, string FilterRegex, uint Threads, OutputFormat OutputFormat, DirectoryInfo TargetDir, string TargetEntry, InvocationContext ctx)
        {
            C4CHttpClient cloudClient;

            try
            {
                if (!TargetDir.Exists)
                {
                    throw new CloudCopyParametrizationException("Target directory doesn't exist");
                }

                cloudClient = retrieveCloudClient(Hostname, Username);

                var entryToRead = TargetFactory.CreateC4CTarget(TargetEntry, cloudClient);

                var remoteFiles = await cloudClient.GetFileListingAsync(entryToRead);

                if (!string.IsNullOrEmpty(FilterRegex))
                {
                    remoteFiles = remoteFiles.RemoveNotMatchingRegex(x => x.Filename, FilterRegex);
                }

                if (!string.IsNullOrEmpty(FilterPattern))
                {
                    remoteFiles = remoteFiles.RemoveNotMatchingWildcards(x => x.Filename, FilterPattern);
                }

                // we have to remove empty URIs since we need them to download the files.
                remoteFiles = remoteFiles.Where(x => x.DownloadURI != null).ToList();

                var downloadTasks = new List<Task>();
                var sSlim = new SemaphoreSlim(initialCount: (int)Threads);

                foreach (var fileMetadata in remoteFiles)
                {
                    await sSlim.WaitAsync().ConfigureAwait(false);

                    downloadTasks.Add(
                        Task.Run(async () =>
                        {
                            try
                            {
                                await cloudClient.DownloadFileAsync(fileMetadata, new FileSystemResource(Path.Combine(TargetDir.FullName, fileMetadata.Filename)));
                                Console.WriteLine(fileMetadata.Filename);
                            }
                            finally
                            {
                                sSlim.Release();
                            }
                        }));
                }

                await Task.WhenAll(downloadTasks).ConfigureAwait(false);

                ctx.ExitCode = 0;

            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine(ex.Message);

                if ( ex.InnerException != null )
                {
                    Console.WriteLine(ex.InnerException.Message);
                }

                Console.ResetColor();

                ctx.ExitCode = 100;
            }
        }

        public async Task ListFiles(string Hostname, string Username, OutputFormat OutputFormat, string FilterPattern, string FilterRegex, SortByOption SortBy, string TargetEntry, InvocationContext ctx)
        {
            C4CHttpClient cloudClient;

            try
            {
                cloudClient = retrieveCloudClient(Hostname, Username);

                var entryToRead = TargetFactory.CreateC4CTarget(TargetEntry, cloudClient);

                var remoteFiles = await cloudClient.GetFileListingAsync(entryToRead);

                if (!string.IsNullOrEmpty(FilterRegex))
                {
                    remoteFiles = remoteFiles.RemoveNotMatchingRegex(x => x.Filename, FilterRegex);
                }

                if (!string.IsNullOrEmpty(FilterPattern))
                {
                    remoteFiles = remoteFiles.RemoveNotMatchingWildcards(x => x.Filename, FilterPattern);
                }

                remoteFiles = remoteFiles.SortByProperty(SortBy);

                printMetadataList(remoteFiles.ToList<IRemoteFileMetadata>(), OutputFormat);

                ctx.ExitCode = 0;

            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine(ex.Message);

                if ( ex.InnerException != null )
                {
                    Console.WriteLine(ex.InnerException.Message);
                }

                Console.ResetColor();

                ctx.ExitCode = 100;
            }
        }

        public async Task UploadFiles(string Hostname, string Username, string TypeCode, OutputFormat OutputFormat, string TargetEntry, List<FileInfo> FilesToUpload, InvocationContext ctx)
        {
            C4CHttpClient cloudClient;
            IRemoteResource c4ctarget;
            List<IRemoteFileMetadata> metadataList = new List<IRemoteFileMetadata>();

            try
            {
                cloudClient = retrieveCloudClient(Hostname, Username);

                var allFiles = ExpandWildcards(FilesToUpload);

                c4ctarget = TargetFactory.CreateC4CTarget(TargetEntry, cloudClient, TypeCode);

                foreach (var file in allFiles)
                {
                    IRemoteFileMetadata fileMetadata = await cloudClient.UploadFileAsync(new FileSystemResource(file.ToString()), c4ctarget);

                    metadataList.Add(fileMetadata);
                }

                printMetadataList(metadataList, OutputFormat);

                if (OutputFormat == OutputFormat.human)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"\nSuccessfully uploadad {metadataList.Count} file(s).");
                }

                ctx.ExitCode = 0;

            } catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.WriteLine(ex.Message);

                if ( ex.InnerException != null )
                {
                    Console.WriteLine(ex.InnerException.Message);
                }

                Console.ResetColor();

                ctx.ExitCode = 100;
            }
        }

        private static void printMetadataList(List<IRemoteFileMetadata> metadataList, OutputFormat outputFormat)
        {
            if (outputFormat == OutputFormat.human)
            {
                foreach (var metadata in metadataList)
                {
                    printKVP("Remote Filename:", metadata.Filename);
                    printKVP("UUID:", metadata.UUID);
                    printKVP("MimeType:", metadata.MimeType);
                    printKVP("Metadata URI:", metadata.MetadataURI.ToString());

                    if (metadata.DownloadURI == null)
                    {
                        printKVP("Download URI:", "[download URI unknown]");
                    }
                    else
                    {
                        printKVP("Download URI:", metadata.DownloadURI.ToString());
                    }
                }
            }

            if (outputFormat == OutputFormat.table)
            {
                foreach (var metadata in metadataList)
                {
                    Console.WriteLine("{0} {1} {2}", metadata.UUID.Truncate(38).PadRight(38), metadata.MimeType.Truncate(18).PadRight(18), metadata.Filename);
                }
            }

            if (outputFormat == OutputFormat.jsoncompressed)
            {
                Console.WriteLine(System.Text.RegularExpressions.Regex.Unescape(JsonSerializer.Serialize(metadataList)));
            }

            if (outputFormat == OutputFormat.json)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };

                Console.WriteLine(System.Text.RegularExpressions.Regex.Unescape(JsonSerializer.Serialize(metadataList, options)));
            }
        }

        private static void printKVP(string key, string value)
        {
            if (value == null)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(key.PadRight(17, ' '));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(value);
        }

        private C4CHttpClient retrieveCloudClient(string Hostname, string Username)
        {
            return this.cloudClientFactory.CreateCloudClient(Hostname, Username);
        }

        public static List<FileInfo> ExpandWildcards(List<FileInfo> files)
        {
            List<FileInfo> ExpandedFiles = new List<FileInfo>();

            foreach (var file in files)
            {
                string dir = file.DirectoryName;

                dir = dir == string.Empty ? "." : dir;

                var di = new DirectoryInfo(dir);

                ExpandedFiles.AddRange(di.GetFiles(file.Name, SearchOption.TopDirectoryOnly));
            }

            return ExpandedFiles.Distinct(new FileNameComparer()).ToList();
        }
    }
}
