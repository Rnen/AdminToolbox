using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class BreakDoorsCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Toggles that players break doors when interacting with them";
		}

		public string GetUsage()
		{
			return "BREAKDOOR [PLAYER] [BOOLEAN]";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			AdminToolbox.AddMissingPlayerVariables();
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
							int playerNum = 0;
							foreach (Player pl in server.GetPlayers())
							{
								AdminToolbox.playerdict[pl.SteamId].destroyDoor = j;
								playerNum++;
							}
							outPut += "Set " + playerNum + " player's BreakDoors to " + j;
							return new string[] { "Set " + playerNum + " player's BreakDoors to " + j };
						}
						else
							return new string[] { "Not a valid bool!" };
					}
					else
					{
						foreach (Player pl in server.GetPlayers()) { AdminToolbox.playerdict[pl.SteamId].destroyDoor = !AdminToolbox.playerdict[pl.SteamId].destroyDoor; }
						return new string[] { "Toggled all players BreakDoors" };
					}
				}
				else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
				{
					string str = "Players with BreakDoors enabled: \n";
					List<string> myPlayerList = new List<string>();
					foreach (Player pl in server.GetPlayers())
					{
						if (AdminToolbox.playerdict[pl.SteamId].destroyDoor)
							myPlayerList.Add(pl.Name);
					}
					if (myPlayerList.Count > 0)
					{
						myPlayerList.Sort();
						foreach (var item in myPlayerList)
							str += "\n - " + item;
					}
					else str = "No players with \"BreakDoors\" enabled!";
					return new string[] { str };
				}
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
				if (args.Length > 1)
				{
					if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.playerdict[myPlayer.SteamId].destroyDoor = true; }
					else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.playerdict[myPlayer.SteamId].destroyDoor = false; }
					return new string[] { myPlayer.Name + " BreakDoors: " + AdminToolbox.playerdict[myPlayer.SteamId].destroyDoor };
				}
				else
				{
					AdminToolbox.playerdict[myPlayer.SteamId].destroyDoor = !AdminToolbox.playerdict[myPlayer.SteamId].destroyDoor;
					return new string[] { myPlayer.Name + " BreakDoors: " + AdminToolbox.playerdict[myPlayer.SteamId].destroyDoor };
				}

			}
			return new string[] { GetUsage() };
		}
	}
}