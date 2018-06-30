﻿using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;

namespace AdminToolbox
{
    class MyMiscEvents : IEventHandlerIntercom, IEventHandlerDoorAccess, IEventHandlerSpawn, IEventHandlerWaitingForPlayers
    {
        private Plugin plugin;

        public static float defaultIntercomDuration, defaultIntercomCooldown;

        public MyMiscEvents(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnIntercom(PlayerIntercomEvent ev)
        {
            defaultIntercomDuration = ev.SpeechTime;
            defaultIntercomCooldown = ev.CooldownTime;
            string[] playersAllowed = ConfigManager.Manager.Config.GetListValue("admintoolbox_allow_extended_Intercom", new string[] { "" }, false);
            if (playersAllowed.Length < 1) return;
            foreach (string x in playersAllowed)
            {
                if (ev.Player.GetUserGroup().Name.ToLower() == x.ToLower())
                {
                    ev.SpeechTime = ConfigManager.Manager.Config.GetFloatValue("admintoolbox_intercom_extended_duration", defaultIntercomDuration);
                    ev.CooldownTime = ConfigManager.Manager.Config.GetFloatValue("admintoolbox_intercom_extended_cooldown", defaultIntercomCooldown);
                }
            }
        }
        public void OnDoorAccess(PlayerDoorAccessEvent ev)
        { 
            if (AdminToolbox.playerdict[ev.Player.SteamId][3])
                ev.Destroy = true;
            if (AdminToolbox.playerdict[ev.Player.SteamId][5] && !AdminToolbox.playerdict[ev.Player.SteamId][6])
                ev.Allow = false;
            if (AdminToolbox.playerdict[ev.Player.SteamId][6])
                ev.Allow = true;
        }

        public void OnSpawn(PlayerSpawnEvent ev)
        {
            if (AdminToolbox.playerdict[ev.Player.SteamId][0])
                ev.Player.ChangeRole(Role.SPECTATOR);
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_enable", true, false) == false) this.plugin.pluginManager.DisablePlugin(this.plugin);
        }
    }
}
