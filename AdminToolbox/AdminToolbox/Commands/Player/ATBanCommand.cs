using System;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using System.Linq;
namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;

	public class ATBanCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		private static IConfigFile Config => ConfigManager.Manager.Config;
		private static Server Server => PluginManager.Manager.Server;

		public ATBanCommand(AdminToolbox plugin) => this.plugin = plugin;
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [NAME] [IP/SteamID] <MINUTES> <OPTIONAL REASON>";
		public string GetCommandDescription() => "Alternative ban for offline users";

		public static readonly string[] CommandAliases = new string[] { "ATBAN", "OBAN", "OFFLINEBAN" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if(sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				try
				{
					if (args.Length < 2) return new string[] { GetUsage() };

					string IssuingPlayer = (sender is Player pl && !string.IsNullOrEmpty(pl.SteamId)) ? pl.Name : "Server";
					string bannedPlayer = args[0];
					string input = args[1];
					int minutes = Config.GetIntValue("admintoolbox_atban_duration_default", 43800); //Default 4 weeks
					if(args.Length > 2)
						int.TryParse(args[2], out minutes);
					string reason = (args.Length > 3) ? string.Join(" ", args, startIndex: 3, count: args.Length - 3) : "";

					if (minutes < 1)
						return new string[] { "Wrong time format: \"" + args[2] + "\"" };


					if (input.Contains("."))
					{
						if (input.Split('.').Length != 4) return new string[] { "Invalid IP: " + input, GetUsage() };
						string ip = input.Contains("::ffff:") ? input : "::ffff:" + input;

						Player[] plist = Server.GetPlayers().ToArray();
						foreach(Player player in plist)
						{
							if(player.IpAddress.Contains(input))
							{
								bannedPlayer = player.Name;
								player.Ban(0, "You have been banned from this server!");
								break;
							}
						}

						Server.BanIpAddress(bannedPlayer, ip, minutes, reason, IssuingPlayer);

						string response = "\n" +
							"Player with name: " + bannedPlayer + "\n" +
							"IP: " + ip + "\n" +
							"Was banned for: " + minutes + " minutes \n" +
							"By: " + IssuingPlayer;

						if (IssuingPlayer != "Server")
							plugin.Info(response);
						return new string[] { response };
					}
					else if (input.Trim().Length == 17 && long.TryParse(input.Trim(), out long sID))
					{
						Player[] plist = Server.GetPlayers().ToArray();
						foreach (Player player in plist)
						{
							if (player.SteamId == sID.ToString())
							{
								bannedPlayer = player.Name;
								player.Ban(0, "You have been banned from this server!");
								break;
							}
						}

						Server.BanSteamId(bannedPlayer, input, minutes, reason, IssuingPlayer);

						string response = "\n" +
							"Player with name: " + bannedPlayer + "\n" +
							"SteamID64: " + input.Trim() + "\n" +
							"Was banned for: " + minutes + " minutes \n" +
							"By: " + IssuingPlayer;

						if (IssuingPlayer != "Server")
							plugin.Info(response);
						return new string[] { response };
					}
					else
						return new string[] { "SteamID / IP not in correct format!", GetUsage() };
				}
				catch (Exception e)
				{
					plugin.Error(e.StackTrace);
					return new string[] { "Encountered an error! Check server window." };
				}
			}
			else
				return deniedReply;
		}
	}
}
