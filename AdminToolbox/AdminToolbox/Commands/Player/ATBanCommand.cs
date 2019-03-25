using System;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using System.Linq;
namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;

	class ATBanCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		public ATBanCommand(AdminToolbox plugin) => this.plugin = plugin;
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [NAME] [IP/SteamID] [MINUTES] <OPTIONAL REASON>";
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
					int minutes = ConfigManager.Manager.Config.GetIntValue("admintoolbox_atban_default_duration", 43800); //Default 4 weeks
					if(args.Length > 2)
						minutes = int.TryParse(args[2], out int x) ? x : 0;
					string reason = (args.Length > 3) ? string.Join(" ", args, startIndex: 3, count: args.Length - 3) : "";

					if (minutes < 1)
						return new string[] { "Wrong time format: \"" + args[2] + "\"" };


					if (input.Contains("."))
					{
						if (input.Split('.').Length != 4) return new string[] { "Invalid IP: " + input, GetUsage() };
						string ip = (input.Contains("::ffff:")) ? input : "::ffff:" + input;

						PluginManager.Manager.Server.BanIpAddress(bannedPlayer, ip, minutes, reason, IssuingPlayer);

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
						PluginManager.Manager.Server.BanSteamId(bannedPlayer, input, minutes, reason, IssuingPlayer);

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
