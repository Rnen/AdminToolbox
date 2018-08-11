using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
	class PlayerCommand : ICommandHandler
	{
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
            bool isPlayer()
            {
                if (sender.GetType() == typeof(Player))
                    return true;
                else
                    return false; 
            }

            AdminToolbox.AddMissingPlayerVariables();
            Server server = PluginManager.Manager.Server;
            string ColoredBools(bool input)
            {
                if (isPlayer() && input)
                    return "<color=green>" + input + "</color>";
                else if (isPlayer() && !input)
                    return "<color=red>" + input + "</color>";
                else
                    return input.ToString();
            }
            if (args.Length > 0 && server.GetPlayers().Count>0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
                TimeSpan myTimespan = AdminToolbox.playerdict[myPlayer.SteamId].JailedToTime.Subtract(DateTime.Now);
                string myCurrentJailTime()
                {
                    int minutes = (int)(myTimespan.TotalSeconds / 60);
                    int seconds = (int)(myTimespan.TotalSeconds - (minutes * 60));
                    if (seconds <= 0) return "N/A";
                    if (minutes > 0)
                        return minutes + " minutes, " + seconds + " seconds";
                    else
                        return seconds + " seconds";
                }
                
                string x = "Player info: \n " +
                    "\n Player: " + myPlayer.Name +
                    "\n - SteamID: " + myPlayer.SteamId +
                    "\n - Health: " + myPlayer.GetHealth() +
                    "\n - Role: " + myPlayer.TeamRole.Role +
                    "\n - Server Rank: " + "<color=" + myPlayer.GetUserGroup().Color+ ">" + myPlayer.GetRankName()+"</color>" +
                    "\n - AdminToolbox Toggables: " +
                    "\n     - SpectatorOnly: " + ColoredBools(AdminToolbox.playerdict[myPlayer.SteamId].spectatorOnly) +
                    "\n     - Godmode: " + ColoredBools(AdminToolbox.playerdict[myPlayer.SteamId].godMode) +
                    "\n     - NoDmg: " + ColoredBools(AdminToolbox.playerdict[myPlayer.SteamId].dmgOff) +
                    "\n     - BreakDoors: " + ColoredBools(AdminToolbox.playerdict[myPlayer.SteamId].destroyDoor) +
                    "\n     - KeepSettings: " + ColoredBools(AdminToolbox.playerdict[myPlayer.SteamId].keepSettings) +
                    "\n     - PlayerLockDown: " + ColoredBools(AdminToolbox.playerdict[myPlayer.SteamId].lockDown) +
                    "\n     - InstantKill: " + ColoredBools(AdminToolbox.playerdict[myPlayer.SteamId].instantKill) +
                    "\n     - IsJailed: " + ColoredBools(AdminToolbox.playerdict[myPlayer.SteamId].isJailed) +
                    "\n         - Released In: "+ myCurrentJailTime() + 
                    /*"\n     - IsInJail: " + ColoredBools(AdminToolbox.playerdict[myPlayer.SteamId].isInJail) +*/
                    "\n - Stats:" +
                    "\n     - Kills: " + AdminToolbox.playerdict[myPlayer.SteamId].Kills +
                    "\n     - TeamKills: " + AdminToolbox.playerdict[myPlayer.SteamId].TeamKills +
                    "\n     - Deaths: " + AdminToolbox.playerdict[myPlayer.SteamId].Deaths +
                    "\n     - Rounds Played: " + AdminToolbox.playerdict[myPlayer.SteamId].RoundsPlayed +
                    "\n - Position:" +
                        " - X:" + (int)myPlayer.GetPosition().x +
                        "   Y:" + (int)myPlayer.GetPosition().y +
                        "   Z:" + (int)myPlayer.GetPosition().z;
                return new string[] { x };
            }
            return new string[] { GetUsage() };
        }
	}
}
