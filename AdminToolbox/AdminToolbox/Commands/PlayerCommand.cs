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
			return "(P / PLAYER) [PLAYERNAME]";
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
                    "\n     - SpectatorOnly: " + AdminToolbox.playerdict[myPlayer.SteamId].spectatorOnly +
                    "\n     - Godmode: " + AdminToolbox.playerdict[myPlayer.SteamId].godMode +
                    "\n     - NoDmg: " + AdminToolbox.playerdict[myPlayer.SteamId].godMode +
                    "\n     - BreakDoors: " + AdminToolbox.playerdict[myPlayer.SteamId].destroyDoor +
                    "\n     - KeepSettings: " + AdminToolbox.playerdict[myPlayer.SteamId].keepSettings +
                    "\n     - PlayerLockDown: " + AdminToolbox.playerdict[myPlayer.SteamId].lockDown +
                    "\n     - InstantKill: " + AdminToolbox.playerdict[myPlayer.SteamId].instantKill +
                    "\n - Stats:" +
                    "\n     - Kills: " + AdminToolbox.playerdict[myPlayer.SteamId].Kills +
                    "\n     - TeamKills: " + AdminToolbox.playerdict[myPlayer.SteamId].TeamKills +
                    "\n     - Deaths: " + AdminToolbox.playerdict[myPlayer.SteamId].Deaths +
                    "\n     - Rounds Played: " + AdminToolbox.playerdict[myPlayer.SteamId].RoundsPlayed +
                    "\n - Position:" +
                        " - X:" + (int)myPlayer.GetPosition().x +
                        "   Y:" + (int)myPlayer.GetPosition().y +
                        "   Z:" + (int)myPlayer.GetPosition().z;
                //plugin.Info(x);
                return new string[] { x };
            }
            return new string[] { GetUsage() };
        }
	}
}
