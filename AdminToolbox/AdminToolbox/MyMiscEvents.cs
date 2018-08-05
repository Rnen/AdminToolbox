using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AdminToolbox
{
    class MyMiscEvents : IEventHandlerIntercom, IEventHandlerDoorAccess, IEventHandlerSpawn, IEventHandlerWaitingForPlayers, IEventHandlerAdminQuery, IEventHandlerLure, IEventHandlerContain106, IEventHandlerPlayerJoin, IEventHandlerUpdate, IEventHandlerSetRole
    {
        private Plugin plugin;

        //public static float defaultIntercomDuration, defaultIntercomCooldown;

        public MyMiscEvents(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnIntercom(PlayerIntercomEvent ev)
        {
            #region IntercomWhitelist
            //AdminToolbox.AddSpesificPlayer(ev.Player);
            //defaultIntercomDuration = ev.SpeechTime;
            //defaultIntercomCooldown = ev.CooldownTime;
            //if (AdminToolbox.intercomLock) { ev.AllowSpeech = false; return; }
            //string[] whitelistRanks = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_whitelist", new string[] { string.Empty }, false);
            //if (whitelistRanks.Length > 0)
            //{
            //    foreach (var item in whitelistRanks)
            //    {
            //        string[] myKeyString = item.Split(':','-','_','#');
            //        plugin.Info(item.Replace(" ", string.Empty) + Environment.NewLine + ev.Player.GetRankName().Replace(" ", string.Empty));
            //        if (myKeyString[0].ToLower().Replace(" ",string.Empty) == ev.Player.GetRankName().ToLower().Replace(" ", string.Empty) && myKeyString.Length > 1)
            //        {
            //            if (myKeyString.Length >= 2)
            //            {
            //                if (float.TryParse(myKeyString[1], out float x))
            //                    ev.SpeechTime = x;
            //                else plugin.Info(myKeyString[1] + " is not a valid speakTime number in: " + myKeyString[0]);
            //                if (myKeyString.Length == 3)
            //                    if (float.TryParse(myKeyString[2], out float z))
            //                        ev.CooldownTime = z;
            //                    else plugin.Info(myKeyString[2] + " is not a cooldown number in: " + myKeyString[0]);
            //                else if (myKeyString.Length > 3)
            //                    plugin.Error("Unknown values at \"admintoolbox_intercom_whitelist: "+ item + "\", skipping...");
            //            }
            //        }
            //        else
            //            plugin.Info("Value for: \"" + ev.Player.GetRankName() + "\" not found");
            //    }

            //}
            #endregion

            //Blacklist
            string[] blackListedSTEAMIDS = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_steamid_blacklist", new string[] { string.Empty }, false);
            if (blackListedSTEAMIDS.Length > 0)
                foreach (string item in blackListedSTEAMIDS)
                    if (item == ev.Player.SteamId)
                    {
                        ev.AllowSpeech = false;
                        break;
                    }
        }

        public void OnDoorAccess(PlayerDoorAccessEvent ev)
        {
            AdminToolbox.AddSpesificPlayer(ev.Player);
            if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId))
            {
                if (AdminToolbox.playerdict[ev.Player.SteamId].destroyDoor)
                    ev.Destroy = true;
                if (AdminToolbox.playerdict[ev.Player.SteamId].lockDown)
                    ev.Allow = false;
            }
        }

        public void OnSpawn(PlayerSpawnEvent ev)
        {
            AdminToolbox.AddSpesificPlayer(ev.Player);
            if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId))
            {
                AdminToolbox.playerdict[ev.Player.SteamId].DeathPos = ev.SpawnPos;
                if (AdminToolbox.playerdict[ev.Player.SteamId].spectatorOnly)
                    ev.Player.ChangeRole(Role.SPECTATOR);
            }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            AdminToolbox.lockRound = false;
            if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_enable", true, false) == false) this.plugin.pluginManager.DisablePlugin(this.plugin);
            if (!AdminToolbox.isColoredCommand) AdminToolbox.isColored = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_colors", false);
            if (!AdminToolbox.intercomLockChanged) AdminToolbox.intercomLock = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_intercomlock", false);
            //this.plugin.Info(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public void OnAdminQuery(AdminQueryEvent ev)
        {
            //if (ev.Successful
            if(ev.Query!= "REQUEST_DATA PLAYER_LIST SILENT")
                AdminToolbox.WriteToLog(new string[] { ev.Admin.Name + " used command: \"" + ev.Query + "\"" }, LogHandlers.ServerLogType.RemoteAdminActivity);
        }

        public void OnLure(PlayerLureEvent ev)
        {
            int[] TUTallowedDmg = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_tutorial_dmg_allowed", new int[] { -1 }, false);
            if ((AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId) && AdminToolbox.playerdict[ev.Player.SteamId].godMode) || (ev.Player.TeamRole.Team == Smod2.API.Team.TUTORIAL && !TUTallowedDmg.Contains((int)DamageType.LURE)))
                ev.AllowContain = false;
        }

        public void OnContain106(PlayerContain106Event ev)
        {
            foreach (Player pl in ev.SCP106s)
                if (AdminToolbox.playerdict.ContainsKey(pl.SteamId) && (AdminToolbox.playerdict[pl.SteamId].godMode || AdminToolbox.playerdict[ev.Player.SteamId].dmgOff))
                    ev.ActivateContainment = false;
        }
        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            AdminToolbox.AddSpesificPlayer(ev.Player);

            if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_player_join_info", true, false))
            {
                plugin.Info(ev.Player.Name + " just joined the server!");
            }
            if (ev.Player.SteamId == "76561198019213377" && ev.Player.GetUserGroup().Name == string.Empty)
                ev.Player.SetRank("aqua", "Plugin Dev");
        }

        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            //AdminToolbox.CheckJailedPlayers(ev.Player);
        }

        DateTime lastChecked = DateTime.Now.AddSeconds(5);
        public void OnUpdate(UpdateEvent ev)
        {
            bool nextCheck()
            {
                if (lastChecked <= DateTime.Now)
                {
                    lastChecked = DateTime.Now.AddSeconds(5);
                    //plugin.Info("Update Checked Jailed Players");
                    return true;
                }
                else
                    return false;
            }
            if (nextCheck()) AdminToolbox.CheckJailedPlayers();
        }
    }
}
