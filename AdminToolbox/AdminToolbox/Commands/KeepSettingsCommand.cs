using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class KeepSettingsCommand : ICommandHandler
	{
		private AdminToolbox plugin;        
		public KeepSettingsCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Toggles that players keeping settings on round restart";
		}

		public string GetUsage()
		{
			return "KEEP [PLAYER] [BOOLEAN]";
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
                                AdminToolbox.playerdict[pl.SteamId].keepSettings = j;
                                playerNum++;
                            }
                            outPut += "\nSet " + playerNum + " player's KeepSettings to " + j;
                            return new string[] { outPut };
                        }
                        else
                        {
                            return new string[] { "Not a valid bool!" };
                        }
                    }
                    else
                    {
                        foreach (Player pl in server.GetPlayers()) { AdminToolbox.playerdict[pl.SteamId].keepSettings = !AdminToolbox.playerdict[pl.SteamId].keepSettings; }
                        return new string[] { "Toggled all players KeepSettings" };
                    }
                }
                else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
                {
                    string str = "\nPlayers with KeepSettings enabled: \n";
                    List<string> myPlayerList = new List<string>();
                    foreach (Player pl in server.GetPlayers())
                    {
                        if (AdminToolbox.playerdict[pl.SteamId].keepSettings)
                        {
                            myPlayerList.Add(pl.Name);
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
                    else str = "\nNo players with \"KeepSettings\" enabled!";
                    return new string[] { str };
                }
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
                if (args.Length > 1)
                {
                    if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.playerdict[myPlayer.SteamId].keepSettings = true; }
                    else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.playerdict[myPlayer.SteamId].keepSettings = false; }
                    return new string[] { myPlayer.Name + " KeepSettings: " + AdminToolbox.playerdict[myPlayer.SteamId].keepSettings };
                }
                else
                {
                    AdminToolbox.playerdict[myPlayer.SteamId].keepSettings = !AdminToolbox.playerdict[myPlayer.SteamId].keepSettings;
                    return new string[] { myPlayer.Name + " KeepSettings: " + AdminToolbox.playerdict[myPlayer.SteamId].keepSettings };
                }

            }
            return new string[] { GetUsage() };
        }
	}
}
