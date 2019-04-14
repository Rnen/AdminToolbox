using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	public class AT_HelpCommand : ICommandHandler
	{
		private static Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Opens the AdminToolbox GitHub page";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";

		public static readonly string[] CommandAliases = new string[] { "ATHELP", "ATBHELP", "AT-HELP", "ADMINTOOLBOXHELP", "ADMINTOOLBOX-HELP" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (!(sender is Player p) || (p.IpAddress == Server.IpAddress))
			{
				try
				{
					System.Diagnostics.Process.Start("https://github.com/Rnen/AdminToolbox");
					return new string[] { "Opening GitHub page..." };
				}
				catch
				{
					return new string[] { "Could not open browser!", "Visit: " + "https://github.com/Rnen/AdminToolbox" };
				}
			}
			else
				return new string[] { "Can only be used on host computer!", "Visit: " + "https://github.com/Rnen/AdminToolbox" };
		}
	}
}
