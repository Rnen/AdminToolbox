using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.IO;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class ATDisableCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;
		public ATDisableCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "Disables Admintoolbox";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";

		public static readonly string[] CommandAliases = new string[] { "ATDISABLE" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, true, out string[] deniedReply))
			{
				plugin.Info(sender + " ran the " + GetUsage() + " command!");
				this.plugin.pluginManager.DisablePlugin(this.plugin);
				return new string[] { "AdminToolbox Disabled" };
			}
			else
				return deniedReply;
		}
	}
}
