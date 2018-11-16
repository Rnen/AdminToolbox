using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.IO;

namespace AdminToolbox.Command
{
	class ATDisableCommand : ICommandHandler
	{
		private AdminToolbox plugin;

		public ATDisableCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Disables Admintoolbox";
		}

		public string GetUsage()
		{
			return "ATDISABLE";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			plugin.Info(sender + " ran the " + GetUsage() + " command!");
			this.plugin.pluginManager.DisablePlugin(this.plugin);
			return new string[] { "AdminToolbox Disabled" };
		}
	}
}