namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.CommandLine.Invocation;
    using System.CommandLine;

    public class CloudCopyMainApp
    {
        public async Task DownloadFiles(string Hostname ,string Username, string FilterPattern, string FilterRegex, uint Threads, OutputFormat OutputFormat, DirectoryInfo TargetDir, string TargetEntry)
        {
            C4CHttpClient cloudClient;

            if ( !TargetDir.Exists )
            {
                //ToDo RAISE EXCEPTION
            }

            cloudClient = retrieveCloudClient(Hostname, Username);

            var entryToRead = TargetFactory.CreateC4CTarget(TargetEntry,cloudClient);

            var remoteFiles = await cloudClient.GetFileListingAsync(entryToRead);

            if (!string.IsNullOrEmpty(FilterRegex))
            {
                remoteFiles.RemoveNotMatchingRegex( FilterRegex );
            }

            if (!string.IsNullOrEmpty(FilterPattern))
            {
                remoteFiles.RemoveNotMatchingWildcard( FilterPattern );
            }

            remoteFiles.RemoveEmptyURIs();


            var downloadTasks = new List<Task>();
            var sSlim = new SemaphoreSlim(initialCount: (int)Threads);

            foreach (var fileMetadata in remoteFiles)
            {
                await sSlim.WaitAsync();

                downloadTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            await cloudClient.DownloadFileAsync(fileMetadata,new FileSystemResource(Path.Combine(TargetDir.FullName,fileMetadata.Filename)));
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

        public async Task ListFiles(string Hostname,string Username,OutputFormat OutputFormat,string FilterPattern,string FilterRegex,SortByOption SortBy, string TargetEntry)
        {
            C4CHttpClient cloudClient;

            cloudClient = retrieveCloudClient(Hostname, Username);

            var entryToRead = TargetFactory.CreateC4CTarget(TargetEntry,cloudClient);

            var remoteFilesX = await cloudClient.GetFileListingAsync(entryToRead);

            var remoteFiles = remoteFilesX.ToList<IRemoteFileMetadata>();

            if (!string.IsNullOrEmpty(FilterRegex))
            {
                remoteFiles = remoteFiles.RemoveNotMatchingRegex(x => x.Filename, FilterRegex);
            }

            if (!string.IsNullOrEmpty(FilterPattern))
            {
                remoteFiles = remoteFiles.RemoveNotMatchingWildcards( x => x.Filename, FilterPattern);
            }

            remoteFiles = remoteFiles.SortByProperty( SortBy );

            printMetadataList( remoteFiles.ToList<IRemoteFileMetadata>(),OutputFormat);
        }

        public async Task UploadFiles(string Hostname ,string Username,string TypeCode,OutputFormat OutputFormat, string TargetEntry, List<FileInfo> FilesToUpload)
        {
            C4CHttpClient cloudClient;
            IRemoteResource c4ctarget;
            List<IRemoteFileMetadata> metadataList = new List<IRemoteFileMetadata>();

            cloudClient = retrieveCloudClient(Hostname, Username);

            var allFiles = ExpandWildcards( FilesToUpload );

            c4ctarget = TargetFactory.CreateC4CTarget(TargetEntry,cloudClient,TypeCode);

            foreach (var file in allFiles)
            {
                IRemoteFileMetadata fileMetadata = await cloudClient.UploadFileAsync(new FileSystemResource(file.ToString()), c4ctarget);

                metadataList.Add( fileMetadata );
            }

            printMetadataList(metadataList,OutputFormat);

            if (OutputFormat == OutputFormat.human)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"\nSuccessfully uploadad {metadataList.Count} file(s).");
            }
        }

        private static void printMetadataList(List<IRemoteFileMetadata> metadataList, OutputFormat outputFormat)
        {
            if (outputFormat == OutputFormat.human)
            {
                foreach( var metadata in metadataList )
                {
                    printKVP("Remote Filename:",metadata.Filename);
                    printKVP("UUID:",metadata.UUID);
                    printKVP("MimeType:",metadata.MimeType);
                    printKVP("Metadata URI:",metadata.MetadataURI.ToString());
                    printKVP("Download URI:",metadata.DownloadURI.ToString());
                }
            }

            if (outputFormat == OutputFormat.table)
            {
                foreach( var metadata in metadataList )
                {
                    Console.WriteLine( "{0} {1} {2}", metadata.UUID.Truncate(38).PadRight(38), metadata.MimeType.Truncate(18).PadRight(18), metadata.Filename ); 
                }
            }

            if (outputFormat == OutputFormat.jsoncompressed)
            {
                Console.WriteLine( System.Text.RegularExpressions.Regex.Unescape(JsonSerializer.Serialize(metadataList) ) );
            }

            if (outputFormat == OutputFormat.json)
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true

                };
                Console.WriteLine( System.Text.RegularExpressions.Regex.Unescape(JsonSerializer.Serialize(metadataList,options) ) );
            }
        }

        private static void printKVP(string key, string value)
        {
            if ( value == null )
            return;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(key.PadRight(17, ' '));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(value);
        }

        private static C4CHttpClient retrieveCloudClient(string Hostname, string Username)
        {
            C4CHttpClient cloudClient;
            INetworkCredentialHandler credentialHandler = null;

            string C4CHostName = "";
            /*
                    HOSTNAME leer    & USERNAME leer    => Hostname von Configfile    Username von Configfile   Passwort von Configfile
                    HOSTNAME leer    & USERNAME gefüllt => Hostname von Configfile    Username übernehmen       Passwort von Console
                    HOSTNAME gefüllt & USERNAME leer    => Hostname übernehmen        Username von Console      Passwort von Console
                    HOSTNAME gefüllt & USERNAME gefüllt => Hostname übernehmen        Username übernehmen       Passwort von Console
            */

            if (string.IsNullOrEmpty(Hostname) && string.IsNullOrEmpty(Username))
            {
                var configFileHandler = new ConfigFileHandler();

                C4CHostName = configFileHandler.Hostname;

                if (string.IsNullOrEmpty(configFileHandler.Password) || string.IsNullOrEmpty(configFileHandler.Username))
                {
                    credentialHandler = new ConsoleCredentialHandler(configFileHandler.Username);
                }
                else
                {
                    credentialHandler = configFileHandler;
                }
            }

            if (string.IsNullOrEmpty(Hostname) && !string.IsNullOrEmpty(Username))
            {
                var configFileHandler = new ConfigFileHandler();

                C4CHostName = configFileHandler.Hostname;

                credentialHandler = new ConsoleCredentialHandler(Username);
            }

            if (!string.IsNullOrEmpty(Hostname) && string.IsNullOrEmpty(Username))
            {
                C4CHostName = Hostname;

                credentialHandler = new ConsoleCredentialHandler();
            }

            if (!string.IsNullOrEmpty(Hostname) && !string.IsNullOrEmpty(Username))
            {
                C4CHostName = Hostname;

                credentialHandler = new ConsoleCredentialHandler(Username);
            }

            if (string.IsNullOrEmpty(C4CHostName))
            {
                //TODO ERROR
            }

            if (credentialHandler == null)
            {
                //TODO ERROR
            }

            IClientFactory factory = new ClientFactory();

            cloudClient = factory.CreateC4CHttpClient(C4CHostName, credentialHandler);
            return cloudClient;
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

        public static List<FileInfo> ExpandWildcards(List<FileInfo> files)
        {
            List<FileInfo> ExpandedFiles = new List<FileInfo>();

            foreach (var file in files)
            {
                string dir = file.DirectoryName;

                dir = dir == "" ? "." : dir;

                var di = new DirectoryInfo(dir);

                ExpandedFiles.AddRange( di.GetFiles(file.Name,SearchOption.TopDirectoryOnly) );
            }

            return ExpandedFiles.Distinct( new FileNameComparer()).ToList();;
        }
    }
}
