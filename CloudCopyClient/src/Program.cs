namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.Builder;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var download_command = new Command("download","Download files from an entry in C4C")
            {
                new Option<string>(
                    new string[] { "--Hostname", "-H" },
                    description: "Specifies the hostname of the C4C target system"),

                new Option<string>(
                    new string[] { "--Username", "-U" },
                    description: "Specifies the user used for authentication with the C4C target system"),

                new Option<string>(
                    new string[] { "--FilterPattern", "-p" },
                    description: "Output Format"),

                new Option<string>(
                    new string[] { "--FilterRegex", "-r" },
                    description: "Output Format"),

                new Option<uint>(
                    new string[] { "--Threads", "-t" },
                    getDefaultValue: () => 4,
                    description: "Threads to use for parallel download"),

                new Option<OutputFormat>(
                    new string[] { "--OutputFormat", "-o" },
                    getDefaultValue: () => OutputFormat.human,
                    description: "Output Format"),

                new Option<DirectoryInfo>(
                    new string[] { "--TargetDir", "-d" },
                    getDefaultValue: () => new DirectoryInfo("."),
                    description: "Target Directory where the files should be downloaded to"),

                new Argument<string>("TargetEntry", "Target entry where the files should be uploaded to")
                {
                    Arity = ArgumentArity.ExactlyOne,
                }
            };

            var upload_command = new Command("upload", "Upload files to an entry in C4C")
            {
                new Option<string>(
                    new string[] { "--Hostname", "-H" },
                    description: "Specifies the hostname of the C4C target system"),

                new Option<string>(
                    new string[] { "--Username", "-U" },
                    description: "Specifies the user used for authentication with the C4C target system"),

                new Option<string>(
                    new string[] { "--TypeCode", "-c" },
                    getDefaultValue: () => "10001",
                    description: "Attachment TypeCode"),

                new Option<OutputFormat>(
                    new string[] { "--OutputFormat", "-o" },
                    getDefaultValue: () => OutputFormat.human,
                    description: "Output Format"),

                new Argument<string>("TargetEntry", "Target entry where the files should be uploaded to")
                {
                    Arity = ArgumentArity.ExactlyOne,
                },

                new Argument<List<FileInfo>>("FilesToUpload", "List of files to be uploaded")
                {
                    Arity = ArgumentArity.OneOrMore,
                }
            };

            var list_command = new Command("list", "List files attached to an entry in C4C")
            {
                new Option<string>(
                    new string[] { "--Hostname", "-H" },
                    description: "Specifies the hostname of the C4C target system"),

                new Option<string>(
                    new string[] { "--Username", "-U" },
                    description: "Specifies the user used for authentication with the C4C target system"),

                new Option<OutputFormat>(
                    new string[] { "--OutputFormat", "-o" },
                    getDefaultValue: () => OutputFormat.table,
                    description: "Output Format"),

                new Option<string>(
                    new string[] { "--FilterPattern", "-p" },
                    description: "Output Format"),

                new Option<string>(
                    new string[] { "--FilterRegex", "-r" },
                    description: "Output Format"),

                new Option<SortByOption>(
                    new string[] { "--SortBy", "-s" },
                    getDefaultValue: () => SortByOption.Filename,
                    description: "Sort file listing by"),

                new Argument<string>("TargetEntry", string.Empty)
                {
                    Arity = ArgumentArity.ExactlyOne,
                }
            };

            var configure_command = new Command("configure", "Maintain configuratoin file")
            {
                new Option<string>(
                    new string[] { "--Hostname", "-H" },
                    description: "Specifies the hostname of the C4C target system"),

                new Option<string>(
                    new string[] { "--Username", "-U" },
                    description: "Specifies the user used for authentication with the C4C target system"),

                new Option(
                    new string[] { "--Maintain-Password", "-M" },
                    description: "Allows user to input the type password"),
            };

            var cloudCopyMainApp = new CloudCopyMainApp(new CloudClientFactory());

            upload_command.Handler = CommandHandler.Create<string, string, string, OutputFormat, string, List<FileInfo>>(cloudCopyMainApp.UploadFiles);
            list_command.Handler = CommandHandler.Create<string, string, OutputFormat, string, string, SortByOption, string>(cloudCopyMainApp.ListFiles);
            download_command.Handler = CommandHandler.Create<string,string, string, string, uint, OutputFormat, DirectoryInfo, string>(cloudCopyMainApp.DownloadFiles);
            configure_command.Handler = CommandHandler.Create<string,string,bool>(cloudCopyMainApp.Configure);


            var rootCommand = new RootCommand()
            {
                download_command, upload_command, list_command, configure_command,
            };

            // needed to handle exceptions find details at
            // https://github.com/dotnet/command-line-api/issues/796
            var _ = new CommandLineBuilder(rootCommand)
                .UseVersionOption()
                .UseHelp()
                .UseEnvironmentVariableDirective()
                .UseParseDirective()
                .UseDebugDirective()
                .UseSuggestDirective()
                .RegisterWithDotnetSuggest()
                .UseTypoCorrections()
                .UseParseErrorReporting()
                .CancelOnProcessTermination()
                .Build();

            try
            {
                int result = rootCommand.InvokeAsync(args).Result;

                Console.ResetColor();

                return result;
            }
            catch (AggregateException exa)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                foreach (var ex in exa.InnerExceptions)
                {
                    Console.WriteLine(ex.Message.ToString());

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine(ex.InnerException.ToString());
                    }
                }

                Console.ResetColor();

                return 1;
            }
        }
    }
}
