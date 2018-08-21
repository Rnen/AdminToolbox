﻿using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
    class LockdownCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "Locks all the doors for specified players";
        }

        public string GetUsage()
        {
            return "LOCKDOWN [PLAYER] [BOOLEAN]";
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
                                AdminToolbox.playerdict[pl.SteamId].lockDown = j;
                                playerNum++;
                            }
                            outPut += "\nSet " + playerNum + " player's Lockdown to " + j;
                            return new string[] { outPut };
                        }
                        else
                            return new string[] { "Not a valid bool!" };
                    }
                    else
                    {
                        foreach (Player pl in server.GetPlayers()) { AdminToolbox.playerdict[pl.SteamId].lockDown = !AdminToolbox.playerdict[pl.SteamId].lockDown; }
                        return new string[] { "Toggled all players Lockdown" };
                    }
                }
                else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
                {
                    string str = "\nPlayers with Lockdown enabled: \n";
                    List<string> myPlayerList = new List<string>();
                    foreach (Player pl in server.GetPlayers())
                    {
                        if (AdminToolbox.playerdict[pl.SteamId].lockDown)
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
                    else str = "\nNo players with \"LockDown\" enabled!";
                    return new string[] { str };
                }
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
                if (args.Length > 1)
                {
                    if (bool.TryParse(args[1], out bool g)) AdminToolbox.playerdict[myPlayer.SteamId].lockDown = g;
                    else if (args[1].ToLower() == "on") { AdminToolbox.playerdict[myPlayer.SteamId].lockDown = true; }
                    else if (args[1].ToLower() == "off") { AdminToolbox.playerdict[myPlayer.SteamId].lockDown = false; }
                    else return new string[] { GetUsage() };
                    return new string[] { myPlayer.Name + " Lockdown: " + AdminToolbox.playerdict[myPlayer.SteamId].lockDown };
                }
                else
                {
                    AdminToolbox.playerdict[myPlayer.SteamId].lockDown = !AdminToolbox.playerdict[myPlayer.SteamId].lockDown;
                    return new string[] { myPlayer.Name + " Lockdown: " + AdminToolbox.playerdict[myPlayer.SteamId].lockDown };
                }

            }
            return new string[] { GetUsage() };
        }
    }
}
