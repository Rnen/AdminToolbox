using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class ATColorCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		public ATColorCommand(AdminToolbox plugin) => this.plugin = plugin;
		public string GetCommandDescription() => "Enables/Disables color for Admintoolbox in the server window";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") <Bool>";

		public static readonly string[] CommandAliases = new string[] { "ATCOLOR", "ATCOLOUR" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				Server server = PluginManager.Manager.Server;
				if (args.Length >= 1)
				{
					if (bool.TryParse(args[0], out bool x))
					{
						AdminToolbox.isColored = x;
						AdminToolbox.isColoredCommand = true;
						if (AdminToolbox.isColored)
							plugin.Info("@#fg=Yellow;AdminToolbox@#fg=Default; colors is set to @#fg=Green;" + AdminToolbox.isColored + "@#fg=Default;");
						else
							plugin.Info("AdminToolbox colors set to" + AdminToolbox.isColored);
						return new string[] { "AdminToolbox colors set to" + AdminToolbox.isColored };
					}
					else
						return new string[] { "\"ATCOLOR " + args[0] + "\"  is not a valid bool" };
				}
				else if (args.Length == 0)
				{
					AdminToolbox.isColored = !AdminToolbox.isColored;
					AdminToolbox.isColoredCommand = true;
					if (AdminToolbox.isColored)
						plugin.Info("@#fg=Yellow;AdminToolbox@#fg=Default; colors is set to @#fg=Green;" + AdminToolbox.isColored + "@#fg=Default;");
					else
						plugin.Info("AdminToolbox colors set to " + AdminToolbox.isColored);
					return new string[] { "AdminToolbox colors set to " + AdminToolbox.isColored };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
