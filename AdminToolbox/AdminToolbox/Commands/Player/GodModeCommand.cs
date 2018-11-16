using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class GodModeCommand : ICommandHandler
	{
		public bool noDmg = false;

		public string GetCommandDescription()
		{
			return "Switch on/off godmode for player";
		}

		public string GetUsage()
		{
			return "(ATGOD / ATGODMODE / AT-GOD) [PLAYER] (BOOL)";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			AdminToolbox.AddMissingPlayerVariables();
			if (sender is Player p) AdminToolbox.AddMissingPlayerVariables(new List<Player> { p });
			Server server = PluginManager.Manager.Server;
			if (args.Length > 0)
			{
				if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
				{
					if (args.Length > 1)
					{
						if (bool.TryParse(args[1], out bool j))
						{
							string outPut = null;
							bool changedState = false;
							if (args.Length > 2) { if (args[2].ToLower() == "nodmg") { noDmg = j; changedState = true; } }
							int playerNum = 0;
							foreach (Player pl in server.GetPlayers())
							{
								if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId))
								{
									AdminToolbox.ATPlayerDict[pl.SteamId].godMode = j;
									if (changedState) AdminToolbox.ATPlayerDict[pl.SteamId].dmgOff = j;
									playerNum++;
								}
							}
							outPut += "\nSet " + playerNum + " player's AT-Godmode to " + j;
							if (changedState) return new string[] { "\n" + "Set " + playerNum + " player's AT-Godmode to " + j, "\n" + "NoDmg for theese " + playerNum + " players set to: " + j };
							return new string[] { "\n" + "Set " + playerNum + " player's AT-Godmode to " + j };
						}
						else
						{
							return new string[] { "Not a valid bool!" };
						}
					}
					else
					{
						foreach (Player pl in server.GetPlayers()) { AdminToolbox.ATPlayerDict[pl.SteamId].godMode = !AdminToolbox.ATPlayerDict[pl.SteamId].godMode; }
						return new string[] { "Toggled all players AT-Godmodes" };
					}
				}
				else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
				{
					string str = "\n" + "Players with AT-Godmode enabled: " + "\n";
					List<string> myPlayerList = new List<string>();
					foreach (Player pl in server.GetPlayers())
					{
						if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && AdminToolbox.ATPlayerDict[pl.SteamId].godMode)
						{
							myPlayerList.Add(pl.Name);
							//str += " - " +pl.Name + "\n";
						}
					}
					if (myPlayerList.Count > 0)
					{
						myPlayerList.Sort();
						foreach (var item in myPlayerList)
						{
							str += "\n - " + item;
						}
					}
					else str = "\n" + "No players with \"AT-Godmode\" enabled!";
					return new string[] { str };
				}
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) return new string[] { "Couldn't find player: " + args[0] };
				if (!AdminToolbox.ATPlayerDict.ContainsKey(myPlayer.SteamId)) return new string[] { "Player not in dictionary" };
				if (args.Length > 1)
				{
					bool changedValue = false;
					if (args.Length > 2) { if (args[2].ToLower() == "nodmg") { changedValue = true; } }
					if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode = true; }
					else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode = false; }
					if (changedValue)
					{
						AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff = AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode;
						return new string[] { myPlayer.Name + " AT-Godmode: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode, myPlayer.Name + " No Dmg: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff };
					}
					else
						return new string[] { myPlayer.Name + " AT-Godmode: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode };
				}
				else
				{
					AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode = !AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode;
					return new string[] { myPlayer.Name + " AT-Godmode: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode };
				}

			}
			else
			{
				return new string[] { GetUsage() };
			}
		}
	}
}