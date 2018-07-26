using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AdminToolbox
{
    class MyMiscEvents : IEventHandlerIntercom, IEventHandlerDoorAccess, IEventHandlerSpawn, IEventHandlerWaitingForPlayers, IEventHandlerAdminQuery
    {
        private Plugin plugin;

        public static float defaultIntercomDuration, defaultIntercomCooldown;

        public MyMiscEvents(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnIntercom(PlayerIntercomEvent ev)
        {
            AdminToolbox.AddSpesificPlayer(ev.Player);
            defaultIntercomDuration = ev.SpeechTime;
            defaultIntercomCooldown = ev.CooldownTime;

            string[] whitelistRanks = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_whitelist", new string[] { string.Empty }, false);
            if (whitelistRanks.Length > 0)
            {
                foreach (var item in whitelistRanks)
                {
                    string[] myKeyString = item.Split(':');
                    if (myKeyString[0] == ev.Player.GetRankName())
                    {
                        if (myKeyString.Length == 2)
                        {
                            string[] myString = myKeyString[1].Split('.', '-', '#', '_', ' ');
                            if (myString.Length >= 1)
                            {
                                if (Int32.TryParse(myString[0], out int x))
                                    ev.SpeechTime = x;
                                if (myString.Length == 2)
                                    if (Int32.TryParse(myString[1], out int z))
                                        ev.CooldownTime = z;
                                if (myString.Length > 2)
                                    plugin.Error("Unknown values at \"admintoolbox_intercom_whitelist: " + ev.Player.GetRankName() + ":" + myKeyString[1] + "\", skipping...");
                            }
                        }
                        else
                            plugin.Info("Value for: \"" + ev.Player.GetRankName() + "\" not found");
                    }
                }
            }

            //Blacklist
            string[] blackListedSTEAMIDS = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_steamid_blacklist", new string[] { string.Empty }, false);
            if (blackListedSTEAMIDS.Length > 0)
                foreach (string item in blackListedSTEAMIDS)
                    if (item == ev.Player.SteamId)
                    {
                        ev.AllowSpeech = false;
                        ev.CooldownTime = 5f;
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
                if (AdminToolbox.playerdict[ev.Player.SteamId].isJailed) ev.Player.Teleport(AdminToolbox.warpVectors["jail"]);
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
        }

        public void OnAdminQuery(AdminQueryEvent ev)
        {
            //if (ev.Successful
            if(ev.Query!= "REQUEST_DATA PLAYER_LIST SILENT")
                AdminToolbox.WriteToLog(new string[] { ev.Admin.Name + " used command: \"" + ev.Query + "\"" }, LogHandlers.ServerLogType.RemoteAdminActivity);
        }
    }
}
