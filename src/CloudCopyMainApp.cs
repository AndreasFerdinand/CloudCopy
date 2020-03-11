using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.IO;

namespace CloudCopy
{
    class CloudCopyMainApp
    {
        string[] _args;

        Boolean _SilentOptionSet = false;

        public void run(string[] args)
        {
            _args = args;

            if ( _args.Length == 0 )
            {
                throw new Exception("Unknown command or options provided.");
            }
            else if ( _args.Length > 0 && ( _args[0] == "--help" || _args[0] == "help" ) )
            {
                printUsage();
            }
            else if ( _args[0] == "upload" && _args.Length > 2 )
            {
                upload();
            }
            else if ( _args[0] == "list" && _args.Length > 1 )
            {
                list();
            }
            else if ( _args[0] == "download" && _args.Length > 1 )
            {
                download();
            }
            else
            {
                throw new Exception("Unknown command or options provided.");
            }
        }

        private void list()
        {
            C4CHttpClient Client;

            IRemoteResource Directory2Read;

            List<string> OptionsAndParameter = new List<string>(_args);

            string TargetArg = OptionsAndParameter[OptionsAndParameter.Count - 1];

            OptionsAndParameter.RemoveAt(OptionsAndParameter.Count - 1); //Remove Target
            OptionsAndParameter.RemoveAt(0); // Remove "list"

            TargetDescription targetDescription = parseTargetDescription(TargetArg);

            IOutputOptions outputOptions = new OutputOptions();

            string FilterPattern = "";
            bool FilterByWildcard = false;
            bool FilterByRegex = false;

            foreach (var element in OptionsAndParameter)
            {
                if ( (FilterByWildcard || FilterByRegex ) && FilterPattern == "" )
                {
                    FilterPattern = element;
                    continue;
                }

                if (element == "-r") //reverse sort order
                {
                    outputOptions.sortDirection = ListSortDirection.Descending;
                    continue;
                }

                if (element == "-X") //sort by file extension
                {
                    outputOptions.sortAttribute = "FilenameExtension";
                    continue;
                }

                if (element == "-M") //sort by Mime type
                {
                    outputOptions.sortAttribute = "MimeType";
                    continue;
                }

                if (element == "-U") //sort by UUID
                {
                    outputOptions.sortAttribute = "UUID";
                    continue;
                }

                if (element == "-P") //pattern
                {
                    FilterByWildcard = true;
                }

                if (element == "-R" ) //REGEX
                {
                    FilterByRegex = true;
                }

            }

            Client = createCloudClient(targetDescription);

            if (targetDescription.Collection == "" || targetDescription.Identifier == "")
            {
                throw new Exception("Target collection or identifier not set.");
            }

            if (targetDescription.Identifier[0] == '#')
            {
                //Directory2Read = new C4CTarget(targetDescription.Collection, targetDescription.Identifier.Substring(1), "ServiceRequestAttachmentFolder", Client);
                Directory2Read = TargetFactory.createC4CTarget(targetDescription.Collection,targetDescription.Identifier.Substring(1),Client);
            }
            else
            {
                //Directory2Read = new C4CTarget(targetDescription.Collection, targetDescription.Identifier, "ServiceRequestAttachmentFolder");
                Directory2Read = TargetFactory.createC4CTarget(targetDescription.Collection,targetDescription.Identifier);
            }

            var fileListing = Client.GetFileListingAsync(Directory2Read).Result;

            if ( FilterByWildcard )
            {
                fileListing.removeNotMatchingWildcard( FilterPattern );
            }

            if ( FilterByRegex )
            {
                fileListing.removeNotMatchingRegex( FilterPattern );
            }

            fileListing.listFiles(outputOptions);

        }

        private void download()
        {
            C4CHttpClient Client;
            List<string> OptionsAndParameter = new List<string>(_args);

            IRemoteResource Directory2Read;

            string FilterPattern = "";
            bool FilterByWildcard = false;
            bool FilterByRegex = false;

            string TargetArg = OptionsAndParameter[OptionsAndParameter.Count - 1];

            OptionsAndParameter.RemoveAt(OptionsAndParameter.Count - 1); //Remove Target
            OptionsAndParameter.RemoveAt(0); // Remove "download"

            TargetDescription targetDescription = parseTargetDescription(TargetArg);

            foreach (var element in OptionsAndParameter)
            {
                if ( (FilterByWildcard || FilterByRegex ) && FilterPattern == "" )
                {
                    FilterPattern = element;
                    continue;
                }

                if (element == "-P") //pattern
                {
                    FilterByWildcard = true;
                }

                if (element == "-R" ) //REGEX
                {
                    FilterByRegex = true;
                }

            }

            
            Client = createCloudClient(targetDescription);

            if (targetDescription.Collection == "" || targetDescription.Identifier == "")
            {
                throw new Exception("Target collection or identifier not set.");
            }

            if (targetDescription.Identifier[0] == '#')
            {
                Directory2Read = TargetFactory.createC4CTarget(targetDescription.Collection,targetDescription.Identifier.Substring(1),Client);
            }
            else
            {
                Directory2Read = TargetFactory.createC4CTarget(targetDescription.Collection,targetDescription.Identifier);
            }

            var fileListing = Client.GetFileListingAsync(Directory2Read).Result;

            if ( FilterByWildcard )
            {
                fileListing.removeNotMatchingWildcard( FilterPattern );
            }

            if ( FilterByRegex )
            {
                fileListing.removeNotMatchingRegex( FilterPattern );
            }

            foreach( var fileMetadata in fileListing )
            {
                if ( fileMetadata.DownloadURI != null ) //Links cannot be downloaded and dont have a DownloadURI set
                {
                    var DownloadTask = Client.DownloadFileAsync(fileMetadata,new FileSystemResource(fileMetadata.Filename));
                    DownloadTask.Wait();
                }
            }

        }

        private void upload()
        {
            IRemoteResource Target;
            C4CHttpClient Client;

            List<string> OptionsAndParameter = new List<string>(_args);

            string TargetArg = OptionsAndParameter[OptionsAndParameter.Count - 1];

            OptionsAndParameter.RemoveAt(OptionsAndParameter.Count - 1); //Remove Target
            OptionsAndParameter.RemoveAt(0); // Remove "upload"

            TargetDescription targetDescription = parseTargetDescription(TargetArg);

            Client = createCloudClient(targetDescription);

            List<string> Files2Upload = new List<string>();

            foreach (var element in OptionsAndParameter)
            {
                if (element == "-s")
                {
                    _SilentOptionSet = true;

                    continue;
                }

                Files2Upload.AddRange(getFiles(element));
            }

            if (Files2Upload.Count == 0)
            {
                //ToDo: Error?
            }

            Files2Upload = Files2Upload.Distinct().ToList();

            
            if (targetDescription.Collection == "" || targetDescription.Identifier == "")
            {
                throw new Exception("Target collection or identifier not set.");
            }

            if (targetDescription.Identifier[0] == '#')
            {
                //Target = new C4CTarget(targetDescription.Collection, targetDescription.Identifier.Substring(1), "ServiceRequestAttachmentFolder", Client);
                Target = TargetFactory.createC4CTarget(targetDescription.Collection,targetDescription.Identifier.Substring(1),Client);
            }
            else
            {
                //Target = new C4CTarget(targetDescription.Collection, targetDescription.Identifier, "ServiceRequestAttachmentFolder");
                Target = TargetFactory.createC4CTarget(targetDescription.Collection,targetDescription.Identifier);
            }

            foreach (string FilePath in Files2Upload)
            {
                var UploadTask = Client.UploadFileAsync(new FileSystemResource(FilePath), Target);

                if (!_SilentOptionSet)
                {
                    UploadTask.Result.printMetdata();

                    Console.WriteLine();
                }
            }
        }

        private static C4CHttpClient createCloudClient(TargetDescription targetDescription)
        {
            IClientFactory Factory = new ClientFactory();

            C4CHttpClient Client;

            if (targetDescription.Hostname != "" && targetDescription.Username != "")
            {
                Client = Factory.createC4CHttpClient(targetDescription.Hostname, new ConsoleCredentialHandler(targetDescription.Username));
            }
            else if (targetDescription.Hostname == "" && targetDescription.Username == "")
            {
                ConfigFileHandler Configuration = new ConfigFileHandler();

                Client = Factory.createC4CHttpClient(Configuration.Hostname, Configuration);
            }
            else
            {
                throw new Exception("Either target username or target hostname missing.");
            }

            return Client;
        }

        private static TargetDescription parseTargetDescription(string targetDescription)
        {
            //[User]@[Hostname]:<Collection>:<Identifier>
            //<Identifier> :== #<ID> | <UUID>

            // admin@my123456.crm.ondemand.com:ServiceRequestCollection:bb0812c2b4174491bca22734aa33c6be
            // @my123456.crm.ondemand.com:ServiceRequestCollection:#8311
            // @:ServiceRequestCollection:#4431

            //Console.WriteLine(targetArg);

            string TempTarget = targetDescription;

            string Username = "";
            string Hostname = "";
            string Collection = "";
            string Identifier = "";

            string[] firstSplit = targetDescription.Split('@');

            if ( firstSplit.Length == 1) 
            {
                TempTarget = firstSplit[0];
            }
            else if ( firstSplit.Length == 2 )
            {
                Username = firstSplit[0];
                TempTarget = firstSplit[1];
            }
            else
            {
                throw new Exception("Cannot parse target description.");
            }

            string[] TargetDescription = TempTarget.Split(':');

            if ( TargetDescription.Length == 2 )
            {
                Collection = TargetDescription[0];
                Identifier = TargetDescription[1];
            }
            else if ( TargetDescription.Length == 3 )
            {
                Hostname = TargetDescription[0];
                Collection = TargetDescription[1];
                Identifier = TargetDescription[2];
            }
            else
            {
                throw new Exception("Cannot parse target description.");
            }


            return new TargetDescription(Username,Hostname,Collection,Identifier);
        }

        static List<string> getFiles(string path)
        {
            string pattern = System.IO.Path.GetFileName(path);

            string dir = path.Substring ( 0, path.Length - pattern.Length );

            if ( dir == "" )
            {
                dir = ".";
            }

            List<string> files = new List<string>( System.IO.Directory.GetFiles ( dir, pattern, System.IO.SearchOption.TopDirectoryOnly ) );

            return files;
        }

        private static void printUsage()
        {
            Stream EntityMappingStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("CloudCopy.Help");
            
            StreamReader reader = new StreamReader(EntityMappingStream);

            String helpText = reader.ReadToEnd();

            helpText = helpText.Replace( "~~CONFIGFILE~~",ConfigFileHandler.getDefaultConfigFilePath());

            Console.Write(helpText);
        }

    }
}
