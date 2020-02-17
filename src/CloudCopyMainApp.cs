using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;

namespace CloudCopy
{
    class CloudCopyMainApp
    {
        string[] _args;

        Boolean _SilentOptionSet = false;

        public void run(string[] args)
        {
            _args = args;

            if ( _args.Length > 0 && ( _args[0] == "--help" || _args[0] == "help" ) )
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

            foreach (var element in OptionsAndParameter)
            {
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

            var GetFileListingAsyncTask = Client.GetFileListingAsync(Directory2Read);

            GetFileListingAsyncTask.Result.listFiles(outputOptions);

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
                var UploadTask = Client.UploadFileAsync(new FileSystemSource(FilePath), Target);

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
            Console.WriteLine("SYNOPSIS");
            Console.WriteLine("  CloudCopy <command> [options] [parameter]");
            Console.WriteLine("");
            Console.WriteLine("DESCRIPTION");
            Console.WriteLine("  CloudCopy copies files between SAP C4C and the local host. It uses the OData Service (https) of the remote host.");
            Console.WriteLine("");
            Console.WriteLine("  Uploading files:");
            Console.WriteLine("  CloudCopy upload [options] <sourcefile> ... [user@host:]<TargetEntityName>:{<UUID>|#<ID>}");
            Console.WriteLine("");
            Console.WriteLine("    Options:");
            Console.WriteLine("    -s\tSilent");
            Console.WriteLine("");
            Console.WriteLine("  Listing files:");
            Console.WriteLine("  CloudCopy list [user@host:]<TargetEntityName>:{<UUID>|#<ID>}");
            Console.WriteLine("");
            Console.WriteLine("    Options:");
            Console.WriteLine("    -X\tsort by file extension");
            Console.WriteLine("    -M\tsort by MimeType");
            Console.WriteLine("    -U\tsort by UUID");
            Console.WriteLine("    -r\tsort in reversed order");
            Console.WriteLine("");
            Console.WriteLine("  General:");
            Console.WriteLine("  If user and host are not provided as argument, it must be specified in the user specific configuration file.");
            Console.WriteLine("  Configuration file for the current user: " + ConfigFileHandler.getDefaultConfigFilePath() );
            Console.WriteLine("  ATTENTION: Please be advised that the credentials for the remote service are stored as plain text (unencrypted).");
            Console.WriteLine("             Therefore this method is not recommended. Set at least appropriate file permissions.");
            Console.WriteLine("");
            Console.WriteLine("EXAMPLES");
            Console.WriteLine("  CloudCopy upload Document.pdf hans@my123456.crm.ondemand.com:ServiceRequest:bb11aa2b4ffdd7744cc2734aa33c6be");
            Console.WriteLine("  CloudCopy upload * ServiceRequest:#1234");
            Console.WriteLine("  CloudCopy list Contact:#1234");
            Console.WriteLine("");
            Console.WriteLine("STATUS CODE");
            Console.WriteLine("  CloudCopy returns 0 on success, and >0 otherwise.");
            Console.WriteLine("");
            Console.WriteLine("LICENSE");
            Console.WriteLine("  CloudCopy is licensed under the MIT License.");
            Console.WriteLine("");
            Console.WriteLine("AUTHORS");
            Console.WriteLine("  Andreas Ferdinand Kasper (froeschler.net@gmail.com)");

        }

    }
}
