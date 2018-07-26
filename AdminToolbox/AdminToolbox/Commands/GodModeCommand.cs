using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class GodModeCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        public bool noDmg = false;

        public GodModeCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Switch on/off godmode for player";
		}

		public string GetUsage()
		{
			return "GOD [PLAYER] (BOOL)";
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
                            bool changedState = false;
                            if (args.Length > 2) { if (args[2].ToLower() == "nodmg") { noDmg = j; changedState = true; } }
                            int playerNum = 0;
                            foreach (Player pl in server.GetPlayers())
                            {
                                AdminToolbox.playerdict[pl.SteamId].godMode = j;
                                if(changedState) AdminToolbox.playerdict[pl.SteamId].dmgOff = j;
                                playerNum++;
                            }
                            outPut += "\nSet " + playerNum + " player's Godmode to " + j;
                            //plugin.Info("Set " + playerNum + " player's Godmode to " + j);
                            if (changedState) { /*plugin.Info("\nNoDmg for theese " + playerNum + " players set to: " + j);*/ return new string[] { "\nSet " + playerNum + " player's Godmode to " + j, "\nNoDmg for theese " + playerNum + " players set to: " + j }; }
                            return new string[] { "\nSet " + playerNum + " player's Godmode to " + j };
                        }
                        else
                        {
                            //plugin.Info("Not a valid bool!");
                            return new string[] { "Not a valid bool!" };
                        }
                    }
                    else
                    {
                        foreach (Player pl in server.GetPlayers()) { AdminToolbox.playerdict[pl.SteamId].godMode = !AdminToolbox.playerdict[pl.SteamId].godMode; }
                        //plugin.Info("Toggled all players godmodes");
                        return new string[] { "Toggled all players godmodes" };
                    }
                }
                else if(args[0].ToLower() == "list" || args[0].ToLower() == "get")
                {
                    string str = "\nPlayers with Godmode enabled: \n";
                    List<string> myPlayerList = new List<string>();
                    foreach(Player pl in server.GetPlayers())
                    {
                        if (AdminToolbox.playerdict[pl.SteamId].godMode)
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
                    else str = "\nNo players with \"Godmode\" enabled!";
                    //plugin.Info(str);
                    return new string[] { str };
                }
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { /*plugin.Info("Couldn't find player: " + args[0]);*/ return new string[] { "Couldn't find player: " + args[0] }; }
                if (args.Length > 1)
                {
                    bool changedValue=false;
                    if (args.Length > 2) { if (args[2].ToLower() == "nodmg") { changedValue = true; } }
                    if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.playerdict[myPlayer.SteamId].godMode = true; }
                    else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.playerdict[myPlayer.SteamId].godMode = false; }
                    //plugin.Info(myPlayer.Name + " godmode: " + AdminToolbox.playerdict[myPlayer.SteamId][1]);
                    if (changedValue)
                    {
                        AdminToolbox.playerdict[myPlayer.SteamId].dmgOff = AdminToolbox.playerdict[myPlayer.SteamId].godMode;
                        //plugin.Info(myPlayer.Name + " No Dmg: " + AdminToolbox.playerdict[myPlayer.SteamId][2]);
                        return new string[] { myPlayer.Name + " godmode: " + AdminToolbox.playerdict[myPlayer.SteamId].godMode, myPlayer.Name + " No Dmg: " + AdminToolbox.playerdict[myPlayer.SteamId].dmgOff };
                    }
                    else
                        return new string[] { myPlayer.Name + " godmode: " + AdminToolbox.playerdict[myPlayer.SteamId].godMode };
                }
                else
                {
                    AdminToolbox.playerdict[myPlayer.SteamId].godMode = !AdminToolbox.playerdict[myPlayer.SteamId].godMode;
                    //plugin.Info(myPlayer.Name + " Godmode: " + AdminToolbox.playerdict[myPlayer.SteamId][1]);
                    return new string[] { myPlayer.Name + " Godmode: " + AdminToolbox.playerdict[myPlayer.SteamId].godMode };
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
