namespace CloudCopy
{
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var download_Hostname = new Option<string>(
                    new [] { "--Hostname", "-H" },
                    description: "Specifies the hostname of the C4C target system");

            var download_Username = new Option<string>(
                    new [] { "--Username", "-U" },
                    description: "Specifies the user used for authentication with the C4C target system");

            var download_FilterPattern = new Option<string>(
                    new [] { "--FilterPattern", "-p" },
                    description: "Filter files using pattern");

            var download_FilterRegex = new Option<string>(
                    new [] { "--FilterRegex", "-r" },
                    description: "Filter files using regular expressions");

            var download_Threads = new Option<ushort>(
                    new [] { "--Threads", "-t" },
                    getDefaultValue: () => 4,
                    description: "Threads to use for parallel download");

            var download_OutputFormat = new Option<OutputFormat>(
                    new [] { "--OutputFormat", "-o" },
                    getDefaultValue: () => OutputFormat.human,
                    description: "Output Format");

            var download_TargetDir = new Option<DirectoryInfo>(
                    new [] { "--TargetDir", "-d" },
                    getDefaultValue: () => new DirectoryInfo("."),
                    description: "Target Directory where the files should be downloaded to");

            var download_TargetEntry = new Argument<string>("TargetEntry", "Target entry where the files should be uploaded to")
                {
                    Arity = ArgumentArity.ExactlyOne,
                };

            var download_command = new Command("download","Download files from an entry in C4C")
            {
                download_Hostname,
                download_Username,
                download_FilterPattern,
                download_FilterRegex,
                download_Threads,
                download_OutputFormat,
                download_TargetDir,
                download_TargetEntry,                
            };

            var upload_Hostname = new Option<string>(
                    new [] { "--Hostname", "-H" },
                    description: "Specifies the hostname of the C4C target system");

            var upload_Username = new Option<string>(
                    new [] { "--Username", "-U" },
                    description: "Specifies the user used for authentication with the C4C target system");

            var upload_TypeCode = new Option<string>(
                    new [] { "--TypeCode", "-c" },
                    getDefaultValue: () => "10001",
                    description: "Attachment TypeCode");

            var upload_OutputFormat = new Option<OutputFormat>(
                    new [] { "--OutputFormat", "-o" },
                    getDefaultValue: () => OutputFormat.human,
                    description: "Output Format");

            var upload_TargetEntry = new Argument<string>("TargetEntry", "Target entry where the files should be uploaded to")
            {
                Arity = ArgumentArity.ExactlyOne,
            };
            
            var upload_FilesToUpload = new Argument<List<FileInfo>>("FilesToUpload", "List of files to be uploaded")
            {
                Arity = ArgumentArity.OneOrMore,
            };

            var upload_command = new Command("upload", "Upload files to an entry in C4C")
            {
                upload_Hostname,
                upload_Username,
                upload_TypeCode,
                upload_OutputFormat,
                upload_TargetEntry,
                upload_FilesToUpload
            };

            var list_Hostname = new Option<string>(
                    new [] { "--Hostname", "-H" },
                    description: "Specifies the hostname of the C4C target system");

            var list_Username = new Option<string>(
                    new [] { "--Username", "-U" },
                    description: "Specifies the user used for authentication with the C4C target system");

            var list_OutputFormat = new Option<OutputFormat>(
                    new [] { "--OutputFormat", "-o" },
                    getDefaultValue: () => OutputFormat.table,
                    description: "Output Format");

            var list_FilterPattern = new Option<string>(
                    new [] { "--FilterPattern", "-p" },
                    description: "Filter files using pattern");

            var list_FilterRegex = new Option<string>(
                    new [] { "--FilterRegex", "-r" },
                    description: "Filter files using regular expressions");

            var list_SortBy = new Option<SortByOption>(
                    new [] { "--SortBy", "-s" },
                    getDefaultValue: () => SortByOption.Filename,
                    description: "Sort file listing by");

            var list_TargetEntry =  new Argument<string>("TargetEntry", string.Empty)
            {
                Arity = ArgumentArity.ExactlyOne,
            };

            var list_command = new Command("list", "List files attached to an entry in C4C")
            {
                list_Hostname,
                list_Username,
                list_OutputFormat,
                list_FilterPattern,
                list_FilterRegex,
                list_SortBy,
                list_TargetEntry
            };

            var configure_Hostname = new Option<string>(
                    new [] { "--Hostname", "-H" },
                    description: "Specifies the hostname of the C4C target system");

            var configure_Username = new Option<string>(
                    new [] { "--Username", "-U" },
                    description: "Specifies the user used for authentication with the C4C target system");

            var configure_MaintainPassword = new Option<bool>(
                    new [] { "--Maintain-Password", "-M" },
                    description: "Allows user to input the type password");

            var configure_command = new Command("configure", "Maintain configuration file")
            {
                configure_Hostname,
                configure_Username,
                configure_MaintainPassword                
            };

            var cloudCopyMainApp = new CloudCopyMainApp(new CloudClientFactory());

            var rootCommand = new RootCommand("CloudCopy - A command-line tool to manage attachments in SAP Cloud for Customer");

            rootCommand.AddCommand(download_command);
            rootCommand.AddCommand(upload_command);
            rootCommand.AddCommand(list_command);
            rootCommand.AddCommand(configure_command);

            list_command.SetHandler(async (string hostname, string username, OutputFormat outputformat, string filterpattern, string filterregex, SortByOption sortby, string targetentry, InvocationContext ctx )
                                    => { await cloudCopyMainApp.ListFiles(hostname, username, outputformat, filterpattern, filterregex, sortby, targetentry, ctx); },
                                    list_Hostname,
                                    list_Username,
                                    list_OutputFormat,
                                    list_FilterPattern,
                                    list_FilterRegex,
                                    list_SortBy,
                                    list_TargetEntry);

            configure_command.SetHandler((string hostname, string username, bool maintpassword)
                                         => { cloudCopyMainApp.Configure(hostname,username,maintpassword); },
                                         configure_Hostname,
                                         configure_Username,
                                         configure_MaintainPassword);

            upload_command.SetHandler(async (string hostname, string username, string typecode, OutputFormat outputformat, string targetentry, List<FileInfo> filestoupload, InvocationContext ctx )
                                      => { await cloudCopyMainApp.UploadFiles(hostname,username,typecode,outputformat,targetentry,filestoupload,ctx); },
                                      upload_Hostname,
                                      upload_Username,
                                      upload_TypeCode,
                                      upload_OutputFormat,
                                      upload_TargetEntry,
                                      upload_FilesToUpload);

            download_command.SetHandler(async (string hostname, string username, string filepattern, string filterregex, ushort threads, OutputFormat outputFormat, DirectoryInfo targetDir, string targetentry, InvocationContext ctx)
                                        => { await cloudCopyMainApp.DownloadFiles(hostname,username,filepattern,filterregex,threads,outputFormat,targetDir,targetentry, ctx); },
                                        download_Hostname,
                                        download_Username,
                                        download_FilterPattern,
                                        download_FilterRegex,
                                        download_Threads,
                                        download_OutputFormat,
                                        download_TargetDir,
                                        download_TargetEntry);

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
