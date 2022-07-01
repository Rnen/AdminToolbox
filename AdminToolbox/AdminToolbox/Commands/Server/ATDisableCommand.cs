using Smod2;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API.Extentions;
	public class ATDisableCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;
		public ATDisableCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "Disables AdminToolbox";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";

		public static readonly string[] CommandAliases = new string[] { "ATDISABLE" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, true, out string[] deniedReply))
			{
				plugin.Info(sender + " ran the " + GetUsage() + " command!");
				PluginManager.Manager.DisablePlugin(plugin);
				return new string[] { "AdminToolbox Disabled" };
			}
			else
				return deniedReply;
		}
	}
}
