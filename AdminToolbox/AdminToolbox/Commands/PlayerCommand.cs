using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	class PlayerCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public PlayerCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Gets toolbox info about spesific player";
		}

		public string GetUsage()
		{
			return "PLAYER [PLAYERNAME]";
		}

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            AdminToolbox.AddMissingPlayerVariables();
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0 && server.GetPlayers().Count>0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
                string x = "Player info: \n " +
                    "\n Player: " + myPlayer.Name +
                    "\n - SteamID: " + myPlayer.SteamId +
                    "\n - Health: " + myPlayer.GetHealth() +
                    "\n - Role: " + myPlayer.TeamRole.Role +
                    "\n - Server Rank: " + myPlayer.GetRankName() +
                    "\n - AdminToolbox Toggables: " +
                    "\n     - SpectatorOnly: " + AdminToolbox.playerdict[myPlayer.SteamId][0] +
                    "\n     - Godmode: " + AdminToolbox.playerdict[myPlayer.SteamId][1] +
                    "\n     - NoDmg: " + AdminToolbox.playerdict[myPlayer.SteamId][2] +
                    "\n     - BreakDoors: " + AdminToolbox.playerdict[myPlayer.SteamId][3] +
                    "\n     - KeepSettings: " + AdminToolbox.playerdict[myPlayer.SteamId][4] +
                    "\n     - PlayerLockDown: " + AdminToolbox.playerdict[myPlayer.SteamId][5] +
                    "\n     - InstantKill: " + AdminToolbox.playerdict[myPlayer.SteamId][6] +
                    "\n - Stats:" +
                    "\n     - Kills: " + AdminToolbox.playerStats[myPlayer.SteamId][0] +
                    "\n     - TeamKills: " + AdminToolbox.playerStats[myPlayer.SteamId][1] +
                    "\n     - Deaths: " + AdminToolbox.playerStats[myPlayer.SteamId][2] +
                    "\n     - Rounds Played: " + AdminToolbox.playerStats[myPlayer.SteamId][3] +
                    "\n - Position:" +
                        " - X:" + (int)myPlayer.GetPosition().x +
                        " - Y:" + (int)myPlayer.GetPosition().y +
                        " - Z:" + (int)myPlayer.GetPosition().z;
                //plugin.Info(x);
                return new string[] { x };
            }
            return new string[] { GetUsage() };
        }
	}
}
