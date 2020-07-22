using System;
using System.Linq;
using System.Reflection;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API.Extentions;
	public class ServerStatsCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		private static IConfigFile Config => ConfigManager.Manager.Config;

		private Server Server => PluginManager.Manager.Server;

		public ServerStatsCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "Gets the server's round stats since last server restart";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";

		public static readonly string[] CommandAliases = new string[] { "SERVERSTATS", "SSTATS", "RSTATS", "ROUNDSTATS" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				string reply = Environment.NewLine + " ";
				foreach (FieldInfo field in AdminToolbox.roundStats.GetType().GetFields()
					.OrderBy(s => s.GetValue(AdminToolbox.roundStats)).ThenBy(s => s.Name))
				{
					reply += "\n - " + field.Name.Replace("_", " ") + ": " + field.GetValue(AdminToolbox.roundStats) + "";
				}
				return new string[] { reply };
			}
			else
				return deniedReply;
		}
	}
}
