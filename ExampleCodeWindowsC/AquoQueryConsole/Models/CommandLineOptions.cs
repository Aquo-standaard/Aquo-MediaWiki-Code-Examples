using CommandLine;

namespace AquoQueryConsole.Models
{
	public sealed class CommandLineOptions
	{
		[Option("baseuri", Required    = true, HelpText  = "Base uri.")]                                         public string BaseUrl        { set; get; } = string.Empty;
		[Option("tablenames", Required = false, HelpText = "Retrieve table names only.", SetName     = "list")]  public bool   TableNamesOnly { set; get; }
		[Option("table", Required      = false, HelpText = "Retrieve specific table.", SetName       = "table")] public bool   Table          { set; get; }
		[Option("name", Required       = false, HelpText = "Name of the table to retrieve.", SetName = "table")] public string Name           { set; get; } = string.Empty;
	}
}