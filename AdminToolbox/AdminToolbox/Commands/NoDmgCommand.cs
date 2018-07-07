﻿using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class NoDmgCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public NoDmgCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

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
                                AdminToolbox.playerdict[pl.SteamId][2] = j;
                                playerNum++;
                            }
                            //plugin.Info("Set " + playerNum + " player's \"No Dmg\" to " + j);
                            return new string[] { "Set " + playerNum + " player's \"No Dmg\" to " + j };
                        }
                        else
                        {
                            //plugin.Info("Not a valid bool!");
                            return new string[] { "Not a valid bool!" };
                        }
                    }
                    else
                    {
                        foreach (Player pl in server.GetPlayers()) { AdminToolbox.playerdict[pl.SteamId][2] = !AdminToolbox.playerdict[pl.SteamId][2]; }
                        //plugin.Info("Toggled all player's \"No Dmg\"");
                        return new string[] { "Toggled all player's \"No Dmg\"" };
                    }
                }
                else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
                {
                    string str = "\nPlayers with \"No Dmg\" enabled: \n";
                    List<string> myPlayerList = new List<string>();
                    foreach (Player pl in server.GetPlayers())
                    {
                        if (AdminToolbox.playerdict[pl.SteamId][2])
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
                    else str = "\nNo players with \"No Dmg\" enabled!";
                    //plugin.Info(str);
                    return new string[] { str };
                }
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
                if (args.Length > 1)
                {
                    bool changedValue = false;
                    if (args.Length > 2) { if (args[2].ToLower() == "godmode") { changedValue = true; } }
                    if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.playerdict[myPlayer.SteamId][2] = true; }
                    else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.playerdict[myPlayer.SteamId][2] = false; }
                    //plugin.Info(myPlayer.Name + " No Dmg: " + AdminToolbox.playerdict[myPlayer.SteamId][2]);
                    if (changedValue)
                    {
                        AdminToolbox.playerdict[myPlayer.SteamId][1] = AdminToolbox.playerdict[myPlayer.SteamId][2];
                        return new string[] { myPlayer.Name + " No Dmg: " + AdminToolbox.playerdict[myPlayer.SteamId][2], myPlayer.Name + " Godmode: " + AdminToolbox.playerdict[myPlayer.SteamId][1] };
                        //plugin.Info(myPlayer.Name + " Godmode: " + AdminToolbox.playerdict[myPlayer.SteamId][1]);
                    }
                    return new string[] { myPlayer.Name + " No Dmg: " + AdminToolbox.playerdict[myPlayer.SteamId][2] };
                }
                else
                {
                    AdminToolbox.playerdict[myPlayer.SteamId][2] = !AdminToolbox.playerdict[myPlayer.SteamId][2];
                    return new string[] { myPlayer.Name + " No Dmg: " + AdminToolbox.playerdict[myPlayer.SteamId][2] };
                    //plugin.Info(myPlayer.Name + " No Dmg: " + AdminToolbox.playerdict[myPlayer.SteamId][2]);
                }
            }
            else
            {
                return new string[] { GetUsage() };
            }
        }
	}
}
