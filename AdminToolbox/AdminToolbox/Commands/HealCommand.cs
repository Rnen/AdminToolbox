using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
	class HealCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public HealCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Heals player. Use int for spesific amount (optional)";
		}

		public string GetUsage()
		{
			return "HEAL [PLAYER] (AMOUNT)";
		}

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            AdminToolbox.AddMissingPlayerVariables();
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                if(args[0].ToLower() == "all"||args[0].ToLower() == "*")
                {
                    if (args.Length > 1)
                    {
                        if (Int32.TryParse(args[1], out int j))
                        {
                            int playerNum = 0;
                            foreach (Player pl in server.GetPlayers())
                            {
                                pl.AddHealth(j);
                                playerNum++;
                            }
                            if (playerNum > 1)
                                return new string[] { "Added " + j + " HP to " + playerNum + " player(s)" };
                            else
                                return new string[] { "Added " + j + " HP to " + playerNum + " player" };
                        }
                        else
                        {
                            return new string[] { "Not a valid number!" };
                        }
                    }
                    else
                    {
                        foreach (Player pl in server.GetPlayers()) { pl.SetHealth(pl.TeamRole.MaxHP); }
                        plugin.Info("Set all players to their default max HP");
                        return new string[] { "Set all players to their default max HP" };
                    }
                }
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { /*plugin.Info("Couldn't find player: " + args[0]);*/ return new string[] { "Couldn't find player: " + args[0] }; }
                if (args.Length > 1)
                {
                    if (Int32.TryParse(args[1], out int j))
                    {
                        myPlayer.AddHealth(j);
                        return new string[] { "Added " + j + " HP " + " to " + myPlayer.Name };
                    }
                    else
                        return new string[] { "Not a valid number!" };
                }
                else
                {
                    myPlayer.SetHealth(myPlayer.TeamRole.MaxHP);
                    return new string[] { "Set " + myPlayer.Name + " to full HP" };
                }
            }
            else
            {
                return new string[] { GetUsage() };
            }
        }
	}
}
