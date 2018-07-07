using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class BreakDoorsCommand : ICommandHandler
	{
		private AdminToolbox plugin;        
		public BreakDoorsCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

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
                        bool j;
                        if (bool.TryParse(args[1], out j))
                        {
                            string outPut = null;
                            int playerNum = 0;
                            foreach (Player pl in server.GetPlayers())
                            {
                                AdminToolbox.playerdict[pl.SteamId][3] = j;
                                playerNum++;
                            }
                            outPut += "\nSet " + playerNum + " player's BreakDoors to " + j;
                            //plugin.Info("Set " + playerNum + " player's Godmode to " + j);
                            return new string[] { "\nSet " + playerNum + " player's BreakDoors to " + j };
                        }
                        else
                        {
                            //plugin.Info("Not a valid bool!");
                            return new string[] { "Not a valid bool!" };
                        }
                    }
                    else
                    {
                        foreach (Player pl in server.GetPlayers()) { AdminToolbox.playerdict[pl.SteamId][3] = !AdminToolbox.playerdict[pl.SteamId][3]; }
                        //plugin.Info("Toggled all players godmodes");
                        return new string[] { "Toggled all players BreakDoors" };
                    }
                }
                else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
                {
                    string str = "\nPlayers with BreakDoors enabled: \n";
                    List<string> myPlayerList = new List<string>();
                    foreach (Player pl in server.GetPlayers())
                    {
                        if (AdminToolbox.playerdict[pl.SteamId][3])
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
                    else str = "\nNo players with \"BreakDoors\" enabled!";
                    return new string[] { str };
                }
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
                if (args.Length > 1)
                {
                    if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.playerdict[myPlayer.SteamId][3] = true; }
                    else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.playerdict[myPlayer.SteamId][3] = false; }
                    return new string[] { myPlayer.Name + " BreakDoors: " + AdminToolbox.playerdict[myPlayer.SteamId][3] };
                }
                else
                {
                    AdminToolbox.playerdict[myPlayer.SteamId][3] = !AdminToolbox.playerdict[myPlayer.SteamId][3];
                    return new string[] { myPlayer.Name + " BreakDoors: " + AdminToolbox.playerdict[myPlayer.SteamId][3] };
                }

            }
            return new string[] { GetUsage() };
        }
	}
}
