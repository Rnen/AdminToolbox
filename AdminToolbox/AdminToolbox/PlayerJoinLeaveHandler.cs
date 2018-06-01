using Smod2.Events;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox
{
    class PlayerJoinHandler : IEventPlayerJoin
    {
        public void OnPlayerJoin(Player player)
        {
            if (!AdminToolbox.playerdict.ContainsKey(player.SteamId))
                AdminToolbox.playerdict.Add(player.SteamId,new List<bool>(new bool[3] { false, false, false}));
            //AdminToolbox.godMode[player.SteamId][0] = false;
        }
    }
    class PlayerLeaveHandler : IEventPlayerLeave
    {
        public void OnPlayerLeave(Player player)
        {
            if(!AdminToolbox.playerdict[player.SteamId][0])
                AdminToolbox.playerdict.Remove(player.SteamId);
        }
    }
}
