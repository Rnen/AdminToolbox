using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
    class TeleportCommand : ICommandHandler
    {
        private AdminToolbox plugin;

        public TeleportCommand(AdminToolbox plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "Teleports player to player2";
        }

        public string GetUsage()
        {
            return "TPX [PLAYER] [PLAYER2]";
        }

        public void OnCall(ICommandManager manager, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 1)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { plugin.Info("Couldn't find player: " + args[0]); return; }
                Player myPlayer2 = GetPlayerFromString.GetPlayer(args[1], out myPlayer2);
                if (myPlayer2 == null) { plugin.Info("Couldn't find player: " + args[1]); return; }
                if (args[1] != null)
                {
                    myPlayer.Teleport(myPlayer2.GetPosition());
                    plugin.Info("Teleported: " + myPlayer.Name + " to " + myPlayer2.Name /*+ " at " + System.DateTime.Now.ToString()*/);
                }
                else plugin.Info(GetUsage());
            }
            else
            {
                plugin.Info(GetUsage());
            }
        }
    }
}
