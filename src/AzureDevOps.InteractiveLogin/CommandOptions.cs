using CommandLine;

namespace AzureDevOps.InteractiveLogin
{
    public class CommandOptions
    {
        [Option("organization", Required = true, HelpText = "The name of Azure DevOps organization. For example: https://dev.azure.com/{organization}, pass in only the name and not the URL")]
        public string Organization { get; set; }

        public static CommandOptions ParseArguments(string[] args)
        {
            // Parse command line options
            CommandOptions commandOptions = new CommandOptions();
            ParserResult<CommandOptions> results = Parser.Default.ParseArguments<CommandOptions>(args);

            // Map results after parsing
            results.MapResult<CommandOptions, object>((CommandOptions opts) => commandOptions = opts, (errs) => 1);

            // Return null if not able to parse
            if (results.Tag == ParserResultType.NotParsed)
            {
                return null;
            }

            return commandOptions;
        }
    }
}
