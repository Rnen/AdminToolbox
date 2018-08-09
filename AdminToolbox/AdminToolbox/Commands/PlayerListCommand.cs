using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
    class PlayerListCommand : ICommandHandler
    {
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
            string str = server.NumPlayers - 1 + " - Players in server: \n";
            List<string> myPlayerList = new List<string>();
            foreach (Player pl in server.GetPlayers())
            {
                myPlayerList.Add(pl.TeamRole.Role+"("+(int)pl.TeamRole.Role+")" + "  " + pl.Name + "  IP: " + pl.IpAddress + " STEAMID: " + pl.SteamId + "\n");
            }
            myPlayerList.Sort();
            foreach (var item in myPlayerList)
            {
                str += "\n - " + item;
            }
            return new string[] { str };
        }
    }
}
