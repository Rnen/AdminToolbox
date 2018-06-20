using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;

namespace AdminToolbox
{
    class RoundEventHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerCheckRoundEnd
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
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            AdminToolbox.isRoundFinished = true;
            if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_round_info", true, false))
            {
                int minutes = (int)(ev.Round.Duration / 60), duration = ev.Round.Duration;
                if (duration < 60)
                    plugin.Info("Round lasted for: " + duration + " sec");
                else
                    plugin.Info("Round lasted for: " + minutes + " min, " + (duration - (minutes * 60)) + " sec");
                if (AdminToolbox.warpVectors.Count > 0)
                    AdminToolbox.warpVectors.Clear();
                plugin.Info("Round: " + AdminToolbox.roundCount + " has ended.");
            }
            foreach(Player pl in this.plugin.pluginManager.Server.GetPlayers())
            {
                if (AdminToolbox.playerdict[pl.SteamId][4])
                    AdminToolbox.SetPlayerBools(pl, false, false, false, false);
            }
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (AdminToolbox.lockRound)
            {
                ev.Status = ROUND_END_STATUS.ON_GOING;
            }
        }
    }
}
