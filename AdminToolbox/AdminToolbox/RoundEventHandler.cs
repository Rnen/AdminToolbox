using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System;

namespace AdminToolbox
{
    class RoundEventHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerCheckRoundEnd
    {
        private Plugin plugin;

        public RoundEventHandler(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void WritePlayerToServer(string str)
        {
            plugin.Info(str);
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            AdminToolbox.isRoundFinished = false;
            AdminToolbox.roundCount++;
            if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_round_info", true, false))
            {
                plugin.Info("Round: " + AdminToolbox.roundCount + " started.");
                plugin.Info("Players this round: " + ev.Server.GetPlayers().Count);
            }
            AdminToolbox.AddMissingPlayerVariables();
            AdminToolbox._roundStartTime = DateTime.Now.Year.ToString() + "-" + ((DateTime.Now.Month >= 10) ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString())) + "-" + ((DateTime.Now.Day >= 10) ? DateTime.Now.Day.ToString() : ("0" + DateTime.Now.Day.ToString())) + " " + ((DateTime.Now.Hour >= 10) ? DateTime.Now.Hour.ToString() : ("0" + DateTime.Now.Hour.ToString())) + "." + ((DateTime.Now.Minute >= 10) ? DateTime.Now.Minute.ToString() : ("0" + DateTime.Now.Minute.ToString())) + "." + ((DateTime.Now.Second >= 10) ? DateTime.Now.Second.ToString() : ("0" + DateTime.Now.Second.ToString()));

        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (AdminToolbox.lockRound)
            {
                ev.Status = ROUND_END_STATUS.ON_GOING;
            }
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (ev.Round.Duration >= 3)
            {
                AdminToolbox.isRoundFinished = true;
                AdminToolbox.lockRound = false;
                if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_round_info", true, false))
                {
                    plugin.Info("Round: " + AdminToolbox.roundCount + " has ended.");
                    int minutes = (int)(ev.Round.Duration / 60), duration = ev.Round.Duration;
                    if (duration < 60)
                        plugin.Info("Round lasted for: " + duration + " sec");
                    else
                        plugin.Info("Round lasted for: " + minutes + " min, " + (duration - (minutes * 60)) + " sec");
                }
                if (AdminToolbox.warpVectors.Count > 0)
                    AdminToolbox.warpVectors.Clear();
            }
            foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
            {
                AdminToolbox.AddSpesificPlayer(pl);
                if(AdminToolbox.playerStats.ContainsKey(pl.SteamId))
                    AdminToolbox.playerStats[pl.SteamId][3]++;

            }
        }

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            if (AdminToolbox.playerdict.Count > 0)
            {
                List<string> steamIDsToRemove = new List<string>();
                foreach (KeyValuePair<string, List<bool>> item in AdminToolbox.playerdict)
                {
                    if (!item.Value[4]) steamIDsToRemove.Add(item.Key);
                }
                if (steamIDsToRemove.Count > 0)
                {
                    foreach (var item in steamIDsToRemove)
                    {
                        AdminToolbox.playerdict.Remove(item);
                    }
                    steamIDsToRemove.Clear();
                }
            }
        }
    }
}
