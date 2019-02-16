using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	class SpectatorCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Switch on/off always spectator for player";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] <BOOL>";

		public static readonly string[] CommandAliases = new string[] {  "SPECTATOR", "SPEC", "ATOVERWATCH" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				Server server = PluginManager.Manager.Server;
				if (args.Length > 0)
				{
					if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
					{
						AdminToolbox.AddMissingPlayerVariables();
						if (args.Length > 1)
						{
							if (bool.TryParse(args[1], out bool j))
							{
								int playerNum = 0;
								foreach (Player pl in server.GetPlayers())
								{
									AdminToolbox.ATPlayerDict[pl.SteamId].overwatchMode = j;
									pl.OverwatchMode = j;
									playerNum++;
								}
								if (playerNum > 1)
									return new string[] { playerNum + " player's Overwatch status set to: " + j };
								else
									return new string[] { playerNum + " player Overwatch status set to: " + j };
							}
							else
								return new string[] { "Not a valid bool!" };
						}
						else
						{
							int playerNum = 0;
							foreach (Player pl in server.GetPlayers()) { AdminToolbox.ATPlayerDict[pl.SteamId].overwatchMode = !AdminToolbox.ATPlayerDict[pl.SteamId].overwatchMode; pl.OverwatchMode = !pl.OverwatchMode; playerNum++; }
							return new string[] { "Toggled " + playerNum + " player's \"OverwatchMode\"" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						AdminToolbox.AddMissingPlayerVariables();
						string str = "\nPlayers with \"Overwatch\" enabled: \n";
						List<string> myPlayerList = new List<string>();
						foreach (Player pl in server.GetPlayers())
						{
							if (pl.OverwatchMode)
								myPlayerList.Add(pl.Name);
						}
						if (myPlayerList.Count > 0)
						{
							myPlayerList.Sort();
							foreach (var item in myPlayerList)
								str += "\n - " + item;
						}
						else str = "\nNo players with \"Overwatch\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
					AdminToolbox.AddMissingPlayerVariables(myPlayer);
					if (args.Length > 1)
					{
						if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].overwatchMode = true; myPlayer.OverwatchMode = true; }
						else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].overwatchMode = false; myPlayer.OverwatchMode = false; }
						return new string[] { myPlayer.Name + " Overwatch: " + myPlayer.OverwatchMode };
					}
					else
					{
						AdminToolbox.ATPlayerDict[myPlayer.SteamId].overwatchMode = !AdminToolbox.ATPlayerDict[myPlayer.SteamId].overwatchMode;
						myPlayer.OverwatchMode = !myPlayer.OverwatchMode;
						return new string[] { myPlayer.Name + " Overwatch: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].overwatchMode };
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