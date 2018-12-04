using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Net;
using System;
using System.Linq;
using System.IO;
using System.Threading;

namespace AdminToolbox.Command
{
	class ATCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		static IConfigFile Config => ConfigManager.Manager.Config;
		Server Server => PluginManager.Manager.Server;
		ICommandManager CommandManager = PluginManager.Manager.CommandManager;

		public ATCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "For plugin related stuff";

		public string GetUsage() => "AT HELP / AT INFO";

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			Player caller = (sender is Player _p) ? _p : null;
			if (caller != null) return new string[] { "This command is for only for use in the server window!" };
			if (args.Length > 0)
			{
				switch (args[0].ToUpper())
				{
					case "HELP":
						return CommandManager.CallCommand(sender, "athelp", null);
					case "INFO":
						string onlineV = "";
				return new string[] { "\n \n Version: " + plugin.Details.version, "Online Version: " + onlineV + "\n"};
					default:
						return new string[] { args[0] + " is not a valid arguement!", GetUsage() };
				}
			}
			else
				return new string[] { GetUsage() };
		}
	}
}