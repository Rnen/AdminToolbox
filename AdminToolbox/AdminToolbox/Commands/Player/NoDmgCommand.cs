using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class NoDmgCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Switch on/off damageOutput for player";
		}

		public string GetUsage()
		{
			return "NODMG [PLAYER] (BOOL)";
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
							int playerNum = 0;
							foreach (Player pl in server.GetPlayers())
							{
								AdminToolbox.ATPlayerDict[pl.SteamId].dmgOff = j;
								playerNum++;
							}
							return new string[] { "Set " + playerNum + " player's \"No Dmg\" to " + j };
						}
						else
							return new string[] { "Not a valid bool!" };
					}
					else
					{
						foreach (Player pl in server.GetPlayers()) { AdminToolbox.ATPlayerDict[pl.SteamId].dmgOff = !AdminToolbox.ATPlayerDict[pl.SteamId].dmgOff; }
						return new string[] { "Toggled all player's \"No Dmg\"" };
					}
				}
				else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
				{
					string str = "\nPlayers with \"No Dmg\" enabled: \n";
					List<string> myPlayerList = new List<string>();
					foreach (Player pl in server.GetPlayers())
					{
						if (AdminToolbox.ATPlayerDict[pl.SteamId].dmgOff)
							myPlayerList.Add(pl.Name);
					}
					if (myPlayerList.Count > 0)
					{
						myPlayerList.Sort();
						foreach (var item in myPlayerList)
						{
							str += "\n - " + item;
						}
					}
					else str = "\nNo players with \"No Dmg\" enabled!";
					return new string[] { str };
				}
				Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
				if (args.Length > 1)
				{
					bool changedValue = false;
					if (args.Length > 2) { if (args[2].ToLower() == "godmode") { changedValue = true; } }
					if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff = true; }
					else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff = false; }
					if (changedValue)
					{
						AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode = AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff;
						return new string[] { myPlayer.Name + " No Dmg: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff, myPlayer.Name + " Godmode: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].godMode };
					}
					return new string[] { myPlayer.Name + " No Dmg: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff };
				}
				else
				{
					AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff = !AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff;
					return new string[] { myPlayer.Name + " No Dmg: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].dmgOff };
				}
			}
			else
				return new string[] { GetUsage() };
		}
	}
}