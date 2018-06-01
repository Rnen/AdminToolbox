using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	class NoDmgCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public NoDmgCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Switch on/off damageOutput for player";
		}

		public string GetUsage()
		{
			return "NODMG [PLAYER] (BOOL)";
		}

        public void OnCall(ICommandManager manager, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { plugin.Info("Couldn't find player: " + args[0]); return; }
                if (args.Length > 1)
                {
                    if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.playerdict[myPlayer.SteamId][2] = true; }
                    else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.playerdict[myPlayer.SteamId][2] = false; }
                    plugin.Info(myPlayer.Name + " No Dmg: " + AdminToolbox.playerdict[myPlayer.SteamId][2]);
                }
                else
                {
                    AdminToolbox.playerdict[myPlayer.SteamId][2] = !AdminToolbox.playerdict[myPlayer.SteamId][2];
                    plugin.Info(myPlayer.Name + " No Dmg: " + AdminToolbox.playerdict[myPlayer.SteamId][2]);
                }
            }
            else
            {
                plugin.Info(GetUsage());
            }
        }
	}
}
