using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class SpectatorCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public SpectatorCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Switch on/off always spectator for player";
		}

		public string GetUsage()
		{
			return "SPECTATOR [PLAYER] (BOOL)";
		}

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
                {
                    if (args.Length > 1)
                    {
                        bool j;
                        if (bool.TryParse(args[1], out j))
                        {
                            int playerNum = 0;
                            foreach (Player pl in server.GetPlayers())
                            {
                                AdminToolbox.playerdict[pl.SteamId][0] = j;
                                playerNum++;
                            }
                            if (playerNum > 1)
                                return new string[] { playerNum + " players set to AlwaysSpectator: " + j };
                              //plugin.Info(playerNum + " roles set to " + (Role)14);
                            else
                                return new string[] { playerNum + " player set to AlwaysSpectator: " + j };
                              //plugin.Info(playerNum + " role set to " + (Role)14);
                        }
                        else
                        {
                            
                            return new string[] { "Not a valid bool!" };
                        }
                    }
                    else
                    {
                        int playerNum = 0;
                        foreach (Player pl in server.GetPlayers()) { AdminToolbox.playerdict[pl.SteamId][0] = !AdminToolbox.playerdict[pl.SteamId][0]; playerNum++; }
                        //plugin.Info("Toggled all player's \"No Dmg\"");
                        return new string[] { "Toggled " + playerNum + " player's \"AlwaysSpectator\"" };
                    }
                }
                else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
                {
                    string str = "\nPlayers with \"AlwaysSpectator\" enabled: \n";
                    List<string> myPlayerList = new List<string>();
                    foreach (Player pl in server.GetPlayers())
                    {
                        if (AdminToolbox.playerdict[pl.SteamId][0])
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
                    else str = "\nNo players with \"AlwaysSpectator\" enabled!";
                    //plugin.Info(str);
                    return new string[] { str };
                }
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
                if (args.Length > 1)
                {
                    if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.playerdict[myPlayer.SteamId][0] = true; }
                    else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.playerdict[myPlayer.SteamId][0] = false; }
                    //plugin.Info(myPlayer.Name + " Keep settings: " + AdminToolbox.playerdict[myPlayer.SteamId][0]);
                    return new string[] { myPlayer.Name + " AlwaysSpectator: " + AdminToolbox.playerdict[myPlayer.SteamId][0] };
                }
                else
                {
                    AdminToolbox.playerdict[myPlayer.SteamId][0] = !AdminToolbox.playerdict[myPlayer.SteamId][0];
                    //plugin.Info(myPlayer.Name + " Keep settings: " + AdminToolbox.playerdict[myPlayer.SteamId][0]);
                    return new string[] { myPlayer.Name + " AlwaysSpectator: " + AdminToolbox.playerdict[myPlayer.SteamId][0] };
                }

            }
            else
            {
                return new string[] { GetUsage() };
                //plugin.Info(GetUsage());
            }
        }
	}
}
