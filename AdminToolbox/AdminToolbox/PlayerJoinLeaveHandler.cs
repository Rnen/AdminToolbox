﻿using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using Smod2;
using System.Collections.Generic;

namespace AdminToolbox
{
    class PlayerJoinHandler : IEventHandlerPlayerJoin
    {
        private Plugin plugin;

        public PlayerJoinHandler(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            AdminToolbox.AddSpesificPlayer(ev.Player);

            if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_player_joinANDleave", false, false))
            {
                plugin.Info(ev.Player.Name + " just joined the server!");
            }
            //AdminToolbox.godMode[player.SteamId][0] = false;
        }
    }
    //class PlayerLeaveHandler : IEventHandlerPlayerLeave
    //{
    //    public void OnPlayerLeave(Player player)
    //    {
    //        if(!AdminToolbox.playerdict[player.SteamId][0])
    //            AdminToolbox.playerdict.Remove(player.SteamId);
    //    }
    //}
}
