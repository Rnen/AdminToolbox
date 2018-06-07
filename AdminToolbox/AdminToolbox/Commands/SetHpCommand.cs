using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
	class SetHpCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public SetHpCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Sets player HP. Use int for amount";
		}

		public string GetUsage()
		{
			return "HP [PLAYER] (AMOUNT)";
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
                        int j;
                        if (Int32.TryParse(args[1], out j))
                        {
                            int playerNum = 0;
                            foreach (Player pl in server.GetPlayers())
                            {
                                pl.SetHealth(j);
                                playerNum++;
                            }
                            if (playerNum > 1)
                                return new string[] { "Set " + playerNum + " players HP to " + j + "HP" };
                            //plugin.Info("Set " + playerNum + " players HP to " + j + "HP");
                            else
                                return new string[] { "Set " + playerNum + " players HP to " + j + "HP" };
                                //plugin.Info("Set " + playerNum + " player HP to " + j + "HP");
                        }
                        else
                        {
                            return new string[] { "Not a valid number!" };
                            //plugin.Info("Not a valid number!");
                        }
                    }
                    else
                    {
                        foreach (Player pl in server.GetPlayers()) { pl.SetHealth(pl.TeamRole.MaxHP); }
                        return new string[] { "Set all players to their default max HP" };
                        //plugin.Info("Set all players to their default max HP");
                    }
                }
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
                if (args.Length > 1)
                {
                    int j;
                    if (Int32.TryParse(args[1], out j))
                    {
                        //plugin.Info("Set " + myPlayer.Name + "'s HP to " + j + "HP");
                        myPlayer.SetHealth(j);
                        return new string[] { "Set " + myPlayer.Name + "'s HP to " + j + "HP" };
                    }
                    else
                        return new string[] { "Not a valid number!" };
                    //plugin.Info("Not a valid number!");
                }
                else
                {
                    //plugin.Info(GetUsage());
                    //plugin.Info("Set " + myPlayer.Name + " to default HP");
                    myPlayer.SetHealth(myPlayer.TeamRole.MaxHP);
                    return new string[] { "Set " + myPlayer.Name + " to default ("+myPlayer.TeamRole.MaxHP +") HP" };
                }
            }
            else
                return new string[] { GetUsage() };
                //plugin.Info(GetUsage());
        }
	}
}
