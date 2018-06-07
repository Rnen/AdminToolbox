﻿using Smod2.Commands;
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

        public string[] OnCall(ICommandSender manger, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0 && server.GetPlayers().Count>0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
                string x = "Player info: \n \n Player: "+myPlayer.Name+"\n - SteamID: "+myPlayer.SteamId+"\n - Health: "+myPlayer.GetHealth()+"\n - Role: "+myPlayer.GetUserGroup().Name+"\n - KeepInfo: "+ AdminToolbox.playerdict[myPlayer.SteamId][0]+"\n - Godmode: "+ AdminToolbox.playerdict[myPlayer.SteamId][1]+"\n - NoDmg: "+ AdminToolbox.playerdict[myPlayer.SteamId][2]+"\n - Position: X:"+myPlayer.GetPosition().x+" Y:"+ myPlayer.GetPosition().y+" Z:"+ myPlayer.GetPosition().z;
                //plugin.Info(x);
                return new string[] { x };
            }
            return new string[] { GetUsage() };
        }
	}
}