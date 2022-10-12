using DanbooruDownloader.Commands;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.PlatformAbstractions;
using System;

namespace DanbooruDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineApplication application = new CommandLineApplication(true)
            {
                FullName = "Danbooru Downloader",
            };

            application.HelpOption("-?|-h|--help");

            CommandOption versionOption = application.VersionOption("-v|--version", PlatformServices.Default.Application.ApplicationVersion);

            application.Command("dump", command =>
            {
                command.Description = "Download entire images on the server of specified source.";
                command.HelpOption("-h|--help");

                var outputPathArgument = command.Argument("path", "Output path.", false);
                var startIdOption = command.Option("-s|--start-id <id>", "Starting Id. Default is 1.", CommandOptionType.SingleValue);
                var endIdOption = command.Option("-e|--end-id <id>", "Ending Id. Default is 0 (unlimited).).", CommandOptionType.SingleValue);
                var parallelDownloadsOption = command.Option("-p|--parallel-downloads <value>", "Number of images to download simultaneously. Default is 5.", CommandOptionType.SingleValue);
                var ignoreHashCheckOption = command.Option("-i|--ignore-hash-check", "Ignore hash check.", CommandOptionType.NoValue);
                var includeDeletedOption = command.Option("-d|--deleted", "Include deleted posts.", CommandOptionType.NoValue);
                var customtags = command.Argument("tags", "Custom Tags", false);

                command.OnExecute(() =>
                {
                    string path = outputPathArgument.Value;
                    string ctags = customtags.Value;
                    long startId = 1;
                    long endId = 0;
                    int parallelDownloads = 5;
                    bool ignoreHashCheck = ignoreHashCheckOption.HasValue();
                    bool includeDeleted = includeDeletedOption.HasValue();

                    if (startIdOption.HasValue() && !long.TryParse(startIdOption.Value(), out startId))
                    {
                        Console.WriteLine("Invalid start id.");
                        return -2;
                    }
                    if (parallelDownloadsOption.HasValue() && !int.TryParse(parallelDownloadsOption.Value(), out parallelDownloads))
                    {
                        Console.WriteLine("Invalid number of parallel downloads.");
                        return -2;
                    }

                    if (endIdOption.HasValue() && !long.TryParse(endIdOption.Value(), out endId))
                    {
                        Console.WriteLine("Invalid end id.");
                        return -2;
                    }

                    DumpCommand.Run(path, startId, endId, parallelDownloads, ignoreHashCheck, includeDeleted, ctags).Wait();

                    return 0;
                });
            });

            application.OnExecute(() =>
            {
                application.ShowHint();

                return 0;
            });

            try
            {
                int exitCode = application.Execute(args);

                if (exitCode == -2)
                {
                    application.ShowHint();
                }

                Environment.ExitCode = exitCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.ExitCode = -1;
            }
        }
    }
}
