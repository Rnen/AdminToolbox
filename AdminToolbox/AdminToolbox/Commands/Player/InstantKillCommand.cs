using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class InstantKillCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Lets specified players instantly kill targets";
		}

		public string GetUsage()
		{
			return "INSTANTKILL [PLAYER] [BOOLEAN]";
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
								if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId))
								{
									AdminToolbox.ATPlayerDict[pl.SteamId].instantKill = j;
									playerNum++;
								}
							}
							outPut += "\nSet " + playerNum + " player's InstantKill to " + j;
							return new string[] { outPut };
						}
						else
						{
							return new string[] { "Not a valid bool!" };
						}
					}
					else
					{
						foreach (Player pl in server.GetPlayers()) { AdminToolbox.ATPlayerDict[pl.SteamId].instantKill = !AdminToolbox.ATPlayerDict[pl.SteamId].instantKill; }
						return new string[] { "Toggled all players InstantKill" };
					}
				}
				else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
				{
					string str = "\nPlayers with InstantKill enabled: \n";
					List<string> myPlayerList = new List<string>();
					foreach (Player pl in server.GetPlayers())
					{
						if (AdminToolbox.ATPlayerDict[pl.SteamId].instantKill)
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
					else str = "\nNo players with \"InstantKill\" enabled!";
					return new string[] { str };
				}
				Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
				if (args.Length > 1)
				{
					if (bool.TryParse(args[1], out bool g)) AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill = g;
					else if (args[1].ToLower() == "on") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill = true; }
					else if (args[1].ToLower() == "off") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill = false; }
					else return new string[] { GetUsage() };
					return new string[] { myPlayer.Name + " InstantKill: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill };
				}
				else
				{
					AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill = !AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill;
					return new string[] { myPlayer.Name + " InstantKill: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill };
				}

			}
			return new string[] { GetUsage() };
		}
	}
}