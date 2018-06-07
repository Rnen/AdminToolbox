using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	class KeepSettings : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public KeepSettings(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Switch on/off keep settings for player";
		}

		public string GetUsage()
		{
			return "KEEPSETTINGS [PLAYER] (BOOL)";
		}

        public string[] OnCall(ICommandSender manager, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { /*plugin.Info("Couldn't find player: " + args[0]);*/ return new string[] { "Couldn't find player: " + args[0] }; }
                if (args.Length > 1)
                {
                    if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.playerdict[myPlayer.SteamId][0] = true; }
                    else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.playerdict[myPlayer.SteamId][0] = false; }
                    //plugin.Info(myPlayer.Name + " Keep settings: " + AdminToolbox.playerdict[myPlayer.SteamId][0]);
                    return new string[] { myPlayer.Name + " Keep settings: " + AdminToolbox.playerdict[myPlayer.SteamId][0] };
                }
                else
                {
                    AdminToolbox.playerdict[myPlayer.SteamId][0] = !AdminToolbox.playerdict[myPlayer.SteamId][0];
                    //plugin.Info(myPlayer.Name + " Keep settings: " + AdminToolbox.playerdict[myPlayer.SteamId][0]);
                    return new string[] { myPlayer.Name + " Keep settings: " + AdminToolbox.playerdict[myPlayer.SteamId][0] };
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
