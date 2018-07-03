using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;

namespace AdminToolbox
{
    class MyMiscEvents : IEventHandlerIntercom, IEventHandlerIntercomCooldownCheck, IEventHandlerDoorAccess, IEventHandlerSpawn, IEventHandlerWaitingForPlayers
    {
        private Plugin plugin;

        public static float defaultIntercomDuration, defaultIntercomCooldown, defaultIntercomCurrentCooldown;

        public MyMiscEvents(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnIntercom(PlayerIntercomEvent ev)
        {
            defaultIntercomDuration = ev.SpeechTime;
            defaultIntercomCooldown = ev.CooldownTime;
            string[] playersAllowed = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_extended_whitelist_rolebadges", new string[] { "" }, false);
            if (playersAllowed.Length < 1) return;
            foreach (string x in playersAllowed)
            {
                if (ev.Player.GetUserGroup().Name.ToLower().Replace(" ", "") == x.ToLower().Replace(" ", ""))
                {
                    ev.SpeechTime = ConfigManager.Manager.Config.GetFloatValue("admintoolbox_intercom_extended_duration", defaultIntercomDuration);
                    ev.CooldownTime = ConfigManager.Manager.Config.GetFloatValue("admintoolbox_intercom_extended_cooldown", defaultIntercomCooldown);
                }
            }
        }
        public void OnIntercomCooldownCheck(PlayerIntercomCooldownCheckEvent ev)
        {
            defaultIntercomCurrentCooldown = ev.CurrentCooldown;
            string[] playersAllowed = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_extended_whitelist_rolebadges", new string[] { "" }, false);
            if (playersAllowed.Length < 1) return;
            foreach (string x in playersAllowed)
            {
                if (ev.Player.GetUserGroup().Name.ToLower().Replace(" ", "") == x.ToLower().Replace(" ", "") && ConfigManager.Manager.Config.GetBoolValue("admintoolbox_intercom_extended_forcereset", true, false))
                {
                    ev.CurrentCooldown = 1f;
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
