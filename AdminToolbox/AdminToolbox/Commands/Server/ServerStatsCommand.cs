using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace AdminToolbox.Command
{
	class ServerStatsCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		static IConfigFile Config => ConfigManager.Manager.Config;
		Server Server => PluginManager.Manager.Server;

		public ServerStatsCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "Gets the server's round stats since last server restart";

		public string GetUsage() => "SERVERSTATS";

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			string reply = Environment.NewLine + " ";
			foreach (FieldInfo field in AdminToolbox.roundStats.GetType().GetFields()
				.OrderBy(s => s.GetValue(AdminToolbox.roundStats)).ThenBy(s => s.Name))
			{
				reply += "\n - " + field.Name.Replace("_", " ") + ": " + field.GetValue(AdminToolbox.roundStats) + "";
			}
			return new string[] { reply };
		}
	}
}