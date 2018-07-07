using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.Linq;
using System;

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
            AdminToolbox.AddSpesificPlayer(ev.Player);
            defaultIntercomDuration = ev.SpeechTime;
            defaultIntercomCooldown = ev.CooldownTime;
            Dictionary<string, string> whitelistRanks = ConfigManager.Manager.Config.GetDictValue("admintoolbox_intercom_whitelist", new Dictionary<string, string> { { "", defaultIntercomDuration+"-"+defaultIntercomCooldown } }, false);
            if (whitelistRanks.Count > 0)
            {
                foreach (KeyValuePair<string, string> item in whitelistRanks)
                {
                    if (item.Key.ToLower().Replace(" ", "") == ev.Player.GetRankName().ToLower().Replace(" ", ""))
                    {
                        string[] myString = item.Value.Split('.', '-', '#', '_',' ');
                        if (myString.Length == 1)
                        {
                            if (Int32.TryParse(myString[0], out int x))
                            {
                                ev.SpeechTime = x;
                            }
                        }
                        else if (myString.Length == 2)
                        {
                            if (Int32.TryParse(myString[0], out int x))
                            {
                                ev.SpeechTime = x;
                            }
                            if (Int32.TryParse(myString[1], out int z))
                            {
                                ev.CooldownTime = z;
                            }
                        }
                    }
                }
            }
            //string[] playersAllowed = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_extended_whitelist_rolebadges", new string[] { "" }, false);
            //if (playersAllowed.Length > 0)
            //{
            //    foreach (string x in playersAllowed)
            //    {
            //        if (ev.Player.GetRankName().ToLower().Replace(" ", "") == x.ToLower().Replace(" ", ""))
            //        {
            //            ev.SpeechTime = ConfigManager.Manager.Config.GetFloatValue("admintoolbox_intercom_extended_duration", defaultIntercomDuration);
            //            ev.CooldownTime = ConfigManager.Manager.Config.GetFloatValue("admintoolbox_intercom_extended_cooldown", defaultIntercomCooldown);
            //        }
            //    }
            //}
        }

        public void OnIntercomCooldownCheck(PlayerIntercomCooldownCheckEvent ev)
        {
            AdminToolbox.AddSpesificPlayer(ev.Player);
            defaultIntercomCurrentCooldown = ev.CurrentCooldown;
            string[] blackListedPlayers = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_steamid_blacklist", false);
            List<string> blacklistedIDs = new List<string>();
            if (blackListedPlayers.Length > 0)
            {
                foreach(var item in blackListedPlayers)
                {
                    blacklistedIDs.Add(item);
                }
                if (blacklistedIDs.Contains(ev.Player.SteamId))
                {
                    ev.CurrentCooldown = 0.1f;
                    return;
                }
            }

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
            AdminToolbox.AddSpesificPlayer(ev.Player);
            if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId))
            {
                if (AdminToolbox.playerdict[ev.Player.SteamId][3])
                    ev.Destroy = true;
                if (AdminToolbox.playerdict[ev.Player.SteamId][5] && !AdminToolbox.playerdict[ev.Player.SteamId][6])
                    ev.Allow = false;
                if (AdminToolbox.playerdict[ev.Player.SteamId][6])
                    ev.Allow = true;
            }
        }

        public void OnSpawn(PlayerSpawnEvent ev)
        {
            AdminToolbox.AddSpesificPlayer(ev.Player);
            if (AdminToolbox.playerdict[ev.Player.SteamId][0])
                    ev.Player.ChangeRole(Role.SPECTATOR);
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_enable", true, false) == false) this.plugin.pluginManager.DisablePlugin(this.plugin);
        }
    }
}
