using System;
using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using SMRoleType = Smod2.API.RoleType;
using SMItemType = Smod2.API.ItemType;

namespace AdminToolbox.Command
{
	using API.Extentions;

	public class ATCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;
		private static ICommandManager CommandManager => PluginManager.Manager.CommandManager;

		public ATCommand(AdminToolbox plugin) => this.plugin = plugin;
		public string GetCommandDescription() => "Command with sub-commands";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") (HELP / INFO / DOWNLOAD / DEBUG)";

		public static readonly string[] CommandAliases = new string[] { "AT", "ADMINTOOLBOX", "ATB" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length > 0)
				{
					switch (args[0].ToUpper())
					{
						case "HELP":
						case "H":
							return CommandManager.CallCommand(sender, "athelp", new string[] { });

						case "VERSION":
						case "V":
						case "INFO":
						case "I":
							return new string[] { "[AdminToolbox Info]", "Your Local Version: " + plugin.Details.version, "Latest GitHub Version: " + plugin.GetGitReleaseInfo().Version };

						case "DOWNLOAD" when !(sender is Player) || (sender is Player p && p.IpAddress == plugin.Server.IpAddress):
						case "DL" when !(sender is Player) || (sender is Player p2 && p2.IpAddress == plugin.Server.IpAddress):
							try
							{
								System.Diagnostics.Process.Start(plugin.GetGitReleaseInfo().DownloadLink);
								return new string[] { "Opening browser..." };
							}
							catch
							{
								return new string[] { "Failed to open browser! Please visit GitHub or use \"AT_AutoUpdate.bat\" instead" };
							}

						case "DEBUG":
							if (!sender.IsPermitted(new string[] { "ATDEBUG" }, true, out string[] denied))
								return denied;
							AdminToolbox.DebugMode = !AdminToolbox.DebugMode;
							return new string[] { "AdminToolbox Debugmode: " + AdminToolbox.DebugMode };
						case "ITEMS":
							Dictionary<int, string> dict = new Dictionary<int, string>();
							string str = "Items:";
							foreach (SMItemType i in Enum.GetValues(typeof(SMItemType)))
							{
								if (!dict.ContainsKey((int)i))
								{
									dict.Add((int)i, i.ToString());
								}
							}
							foreach(KeyValuePair<int,string> kvp in dict.OrderBy(s => s.Key))
								str += "\n" + kvp.Key + " - " + kvp.Value;
							return new string[] { str };
						case "ROLES":
							Dictionary<int, string> dict2 = new Dictionary<int, string>();
							string str2 = "Items:";
							foreach (SMRoleType i in Enum.GetValues(typeof(SMRoleType)))
							{
								if (!dict2.ContainsKey((int)i))
								{
									dict2.Add((int)i, i.ToString());
								}
							}
							foreach (KeyValuePair<int, string> kvp in dict2.OrderBy(s => s.Key))
								str2 += "\n" + kvp.Key + " - " + kvp.Value;
							return new string[] { str2 };
						default:
							return new string[] { args[0] + " is not a valid arguement!", GetUsage() };
					}
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
