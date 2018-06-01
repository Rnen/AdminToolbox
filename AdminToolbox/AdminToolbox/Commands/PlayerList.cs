using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
    class PlayerList : ICommandHandler
    {
        private AdminToolbox plugin;

        public PlayerList(AdminToolbox plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "Lists current players to server console";
        }

        public string GetUsage()
        {
            return "PLAYERS";
        }

        public void OnCall(ICommandManager manager, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (server.NumPlayers - 1 < 1) { plugin.Info("No players"); return; }
            string input = server.NumPlayers - 1 + " - Players in server: \n";
            foreach (Player pl in server.GetPlayers())
            {
                input += pl.Name + "  IP: " + pl.IpAddress + " STEAMID: " + pl.SteamId + "\n";
            }
            plugin.Info(input);
        }
    }
}
