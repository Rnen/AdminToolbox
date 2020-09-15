using Smod2;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API.Extentions;
	public class WarpsCommmand : ICommandHandler
	{
		public string GetCommandDescription() => "Returns a list of warps. Use arguement \"Refresh\" to reload warps from file";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") <REFRESH / R>";

		public static readonly string[] CommandAliases = new string[] { "WARPS", "ATWARPS", "WARPLIST" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length >= 1 && (args[0].ToUpper() == "R" || args[0].ToUpper() == "REFRESH"))
				{
					AdminToolbox.WarpManager.RefreshWarps();
					return new string[] { "Warps was refreshed!" };
				}
				else
					return PluginManager.Manager.CommandManager.CallCommand(sender, "warp", new string[] { "list" });
			}
			else
				return deniedReply;
		}
	}
}
