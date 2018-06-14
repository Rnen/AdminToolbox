using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
    class PlayerListCommand : ICommandHandler
    {
        private AdminToolbox plugin;

        public PlayerListCommand(AdminToolbox plugin)
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

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (server.NumPlayers - 1 < 1) { return new string[] { "No players" }; }
            string input = server.NumPlayers - 1 + " - Players in server: \n";
            foreach (Player pl in server.GetPlayers())
            {
                input += pl.Name + "  IP: " + pl.IpAddress + " STEAMID: " + pl.SteamId + "\n";
            }
            return new string[] { input };
            //plugin.Info(input);
        }
    }
}
