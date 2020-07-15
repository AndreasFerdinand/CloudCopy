namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class CloudCopyMainApp
    {
        string[] args;

        public async Task<int> Run(string[] args)
        {
            try
            {
                this.args = args;

                if (this.args.Length == 0)
                {
                    throw new Exception("Unknown command or options provided.");
                }
                else if (this.args.Length > 0 && (this.args[0] == "--help" || this.args[0] == "help"))
                {
                    PrintUsage();
                }
                else if (this.args[0] == "upload" && this.args.Length > 2)
                {
                    await this.Upload().ConfigureAwait(false);
                }
                else if (this.args[0] == "list" && this.args.Length > 1)
                {
                    await this.List().ConfigureAwait(false);
                }
                else if (this.args[0] == "download" && this.args.Length > 1)
                {
                    await this.Download().ConfigureAwait(false);
                }
                else if (this.args[0] == "version")
                {
                    PrintVersion();
                }
                else
                {
                    throw new Exception("Unknown command or options provided.");
                }
            }
            catch (C4CClientException c4cException)
            {
                TextWriter errorWriter = Console.Error;

                errorWriter.WriteLine(c4cException.Message);

                if (c4cException.InnerException != null)
                {
                    errorWriter.WriteLine(c4cException.InnerException.Message);
                }

                return 2;
            }
            catch (Exception ex)
            {
                TextWriter errorWriter = Console.Error;

                errorWriter.WriteLine(ex.Message);

                return 1;
            }

            return 0;
        }

        private async Task List()
        {
            IClient cloudClient;

            IRemoteResource directory2Read;

            List<string> optionsAndParameter = new List<string>(this.args);

            string targetArg = optionsAndParameter[optionsAndParameter.Count - 1];

            optionsAndParameter.RemoveAt(optionsAndParameter.Count - 1); // Remove Target
            optionsAndParameter.RemoveAt(0); // Remove "list"

            TargetDescription targetDescription = new TargetDescription(targetArg);

            IOutputOptions outputOptions = new OutputOptions();

            string filterPattern = string.Empty;
            bool filterByWildcard = false;
            bool filterByRegex = false;

            foreach (var element in optionsAndParameter)
            {
                if ((filterByWildcard || filterByRegex) && filterPattern == string.Empty)
                {
                    filterPattern = element;
                    continue;
                }

                if (element == "-r") // reverse sort order
                {
                    outputOptions.SortDirection = ListSortDirection.Descending;
                    continue;
                }

                if (element == "-X") // sort by file extension
                {
                    outputOptions.SortAttribute = "FilenameExtension";
                    continue;
                }

                if (element == "-M") // sort by Mime type
                {
                    outputOptions.SortAttribute = "MimeType";
                    continue;
                }

                if (element == "-U") // sort by UUID
                {
                    outputOptions.SortAttribute = "UUID";
                    continue;
                }

                if (element == "-P") // pattern
                {
                    filterByWildcard = true;
                }

                if (element == "-R") // REGEX
                {
                    filterByRegex = true;
                }
            }

            cloudClient = CreateCloudClient(targetDescription);

            if (targetDescription.Collection == string.Empty || targetDescription.Identifier == string.Empty)
            {
                throw new Exception("Target collection or identifier not set.");
            }

            if (targetDescription.Identifier[0] == '#')
            {
                directory2Read = TargetFactory.CreateC4CTarget(targetDescription.Collection, targetDescription.Identifier.Substring(1), (C4CHttpClient)cloudClient);
            }
            else
            {
                directory2Read = TargetFactory.CreateC4CTarget(targetDescription.Collection, targetDescription.Identifier);
            }

            var fileListing = await cloudClient.GetFileListingAsync(directory2Read);

            if (filterByWildcard)
            {
                fileListing.RemoveNotMatchingWildcard(filterPattern);
            }

            if (filterByRegex)
            {
                fileListing.RemoveNotMatchingRegex(filterPattern);
            }

            fileListing.ListFiles(outputOptions);
        }

        private async Task Download()
        {
            C4CHttpClient cloudClient;
            List<string> optionsAndParameter = new List<string>(this.args);

            IRemoteResource directory2Read;

            string filterPattern = string.Empty;
            bool filterByWildcard = false;
            bool filterByRegex = false;

            bool overrideDefaultParallelism = false;
            int maxParallelism = 4;

            string targetArg = optionsAndParameter[optionsAndParameter.Count - 1];

            optionsAndParameter.RemoveAt(optionsAndParameter.Count - 1); //Remove Target
            optionsAndParameter.RemoveAt(0); // Remove "download"

            TargetDescription targetDescription = new TargetDescription(targetArg);

            foreach (var element in optionsAndParameter)
            {
                if ((filterByWildcard || filterByRegex) && filterPattern == string.Empty)
                {
                    filterPattern = element;
                    continue;
                }

                if (overrideDefaultParallelism)
                {
                    maxParallelism = int.Parse(element);
                    overrideDefaultParallelism = false;
                }

                if (element == "-P") // pattern
                {
                    filterByWildcard = true;
                }

                if (element == "-R") // REGEX
                {
                    filterByRegex = true;
                }

                if (element == "-T") // Max number of download threads
                {
                    overrideDefaultParallelism = true;
                }
            }

            cloudClient = CreateCloudClient(targetDescription);

            if (targetDescription.Collection == string.Empty || targetDescription.Identifier == string.Empty)
            {
                throw new Exception("Target collection or identifier not set.");
            }

            if (targetDescription.Identifier[0] == '#')
            {
                directory2Read = TargetFactory.CreateC4CTarget(targetDescription.Collection, targetDescription.Identifier.Substring(1), cloudClient);
            }
            else
            {
                directory2Read = TargetFactory.CreateC4CTarget(targetDescription.Collection, targetDescription.Identifier);
            }

            var fileListing = await cloudClient.GetFileListingAsync(directory2Read);

            if (filterByWildcard)
            {
                fileListing.RemoveNotMatchingWildcard(filterPattern);
            }

            if (filterByRegex)
            {
                fileListing.RemoveNotMatchingRegex(filterPattern);
            }

            // Links cannot be downloaded and dont have a DownloadURI set
            fileListing.RemoveEmptyURIs();

            var downloadTasks = new List<Task>();
            var sSlim = new SemaphoreSlim(initialCount: maxParallelism);

            foreach (var fileMetadata in fileListing)
            {
                await sSlim.WaitAsync();

                downloadTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            await cloudClient.DownloadFileAsync(fileMetadata,new FileSystemResource(fileMetadata.Filename));
                            Console.WriteLine(fileMetadata.Filename);
                        }
                        finally
                        {
                            sSlim.Release();
                        }
                    }));
            }

            await Task.WhenAll(downloadTasks);
        }

        private async Task Upload()
        {
            IRemoteResource target;
            C4CHttpClient cloudClient;

            bool silentOptionSet = false;
            bool overrideDefaultTypeCode = false;
            string typeCode = "10001";

            List<string> optionsAndParameter = new List<string>(this.args);

            string targetArg = optionsAndParameter[optionsAndParameter.Count - 1];

            optionsAndParameter.RemoveAt(optionsAndParameter.Count - 1); // Remove Target
            optionsAndParameter.RemoveAt(0); // Remove "upload"

            TargetDescription targetDescription = new TargetDescription(targetArg); // parseTargetDescription(TargetArg);

            cloudClient = CreateCloudClient(targetDescription);

            List<string> files2Upload = new List<string>();

            foreach (var element in optionsAndParameter)
            {
                if (overrideDefaultTypeCode)
                {
                    typeCode = element;
                    overrideDefaultTypeCode = false;

                    continue;
                }

                if (element == "-s")
                {
                    silentOptionSet = true;

                    continue;
                }

                if (element == "-C")
                {
                    overrideDefaultTypeCode = true;

                    continue;
                }

                files2Upload.AddRange(GetFiles(element));
            }

            if (files2Upload.Count == 0)
            {
                // ToDo: Error?
            }

            files2Upload = files2Upload.Distinct().ToList();

            if (targetDescription.Collection == string.Empty || targetDescription.Identifier == string.Empty)
            {
                throw new Exception("Target collection or identifier not set.");
            }

            if (targetDescription.Identifier[0] == '#')
            {
                target = TargetFactory.CreateC4CTarget(targetDescription.Collection,targetDescription.Identifier.Substring(1),cloudClient, typeCode);
            }
            else
            {
                target = TargetFactory.CreateC4CTarget(targetDescription.Collection,targetDescription.Identifier, typeCode);
            }

            foreach (string filePath in files2Upload)
            {
                var uploadTask = await cloudClient.UploadFileAsync(new FileSystemResource(filePath), target);

                if (!silentOptionSet)
                {
                    uploadTask.PrintMetdata();

                    Console.WriteLine();
                }
            }
        }

        private static C4CHttpClient CreateCloudClient(TargetDescription targetDescription)
        {
            IClientFactory factory = new ClientFactory();

            C4CHttpClient cloudClient;

            if (targetDescription.Hostname != string.Empty && targetDescription.Username != string.Empty)
            {
                cloudClient = factory.CreateC4CHttpClient(targetDescription.Hostname, new ConsoleCredentialHandler(targetDescription.Username));
            }
            else if (targetDescription.Hostname == string.Empty && targetDescription.Username == string.Empty)
            {
                ConfigFileHandler configFileHandler = new ConfigFileHandler();

                cloudClient = factory.CreateC4CHttpClient(configFileHandler.Hostname, configFileHandler);
            }
            else
            {
                throw new Exception("Either target username or target hostname missing.");
            }

            return cloudClient;
        }

        public static List<string> GetFiles(string path)
        {
            string pattern = System.IO.Path.GetFileName(path);

            string dir = path.Substring(0, path.Length - pattern.Length);

            if (dir == string.Empty)
            {
                dir = ".";
            }

            List<string> files = new List<string>(System.IO.Directory.GetFiles(dir, pattern, System.IO.SearchOption.TopDirectoryOnly));

            return files;
        }

        public static string GetResource(string resourceName)
        {
            Stream resource = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            StreamReader reader = new StreamReader(resource);

            return reader.ReadToEnd();
        }

        private static void PrintUsage()
        {
            string helpText = GetResource("CloudCopy.Help");

            helpText = helpText.Replace("~~CONFIGFILE~~", ConfigFileHandler.GetDefaultConfigFilePath());

            Console.Write(helpText);
        }

        private static void PrintVersion()
        {
            Console.Write(GetResource("CloudCopy.VersionName"));
        }
    }
}
