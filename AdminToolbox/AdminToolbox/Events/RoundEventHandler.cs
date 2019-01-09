using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace AdminToolbox
{
	class RoundEventHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerCheckRoundEnd
	{
		private Plugin plugin;
		static IConfigFile Config => ConfigManager.Manager.Config;
		internal string intercomReady = Config.GetStringValue("admintoolbox_intercomready_text", string.Empty),
			intercomRestart = Config.GetStringValue("admintoolbox_intercomrestart_text", string.Empty),
			intercomTransmit = Config.GetStringValue("admintoolbox_intercomtransmit_text", string.Empty);

		public RoundEventHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}
		public void OnRoundStart(RoundStartEvent ev)
		{
			AdminToolbox.isRoundFinished = false;
			if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_round_info", true, false))
			{
				plugin.Info("Round: " + ++AdminToolbox.RoundCount + " started.");
				plugin.Info("Players this round: " + ev.Server.GetPlayers().Count);
			}
			AdminToolbox.AddMissingPlayerVariables();
			AdminToolbox.atfileManager.PlayerStatsFileManager(ev.Server.GetPlayers(), Managers.ATFileManager.PlayerFile.Write);
			AdminToolbox._roundStartTime = DateTime.Now.Year.ToString() + "-" + ((DateTime.Now.Month >= 10) ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString())) + "-" + ((DateTime.Now.Day >= 10) ? DateTime.Now.Day.ToString() : ("0" + DateTime.Now.Day.ToString())) + " " + ((DateTime.Now.Hour >= 10) ? DateTime.Now.Hour.ToString() : ("0" + DateTime.Now.Hour.ToString())) + "." + ((DateTime.Now.Minute >= 10) ? DateTime.Now.Minute.ToString() : ("0" + DateTime.Now.Minute.ToString())) + "." + ((DateTime.Now.Second >= 10) ? DateTime.Now.Second.ToString() : ("0" + DateTime.Now.Second.ToString()));
			AdminToolbox.warpManager.RefreshWarps();



			if (intercomReady != string.Empty)
				ev.Server.Map.SetIntercomContent(IntercomStatus.Ready, intercomReady);
			if (intercomRestart != string.Empty)
				ev.Server.Map.SetIntercomContent(IntercomStatus.Restarting, intercomRestart);
			if (intercomTransmit != string.Empty)
				ev.Server.Map.SetIntercomContent(IntercomStatus.Transmitting, intercomTransmit);
		}


		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (AdminToolbox.lockRound)
				ev.Status = ROUND_END_STATUS.ON_GOING;
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{
			bool realRoundEnd(RoundEndEvent myEvent)
			{
				//Temp fix for the OnRoundEnd triggering on RoundStart bug
				if (myEvent.Round.Duration >= 3)
					return true;
				else
					return false;
			}
			if (realRoundEnd(ev))
			{
				AdminToolbox.isRoundFinished = true;
				AdminToolbox.lockRound = false;
				if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_round_info", true, false))
				{
					plugin.Info("Round: " + AdminToolbox.RoundCount + " has ended.");
					int minutes = (int)(ev.Round.Duration / 60), duration = ev.Round.Duration;
					if (duration < 60)
						plugin.Info("Round lasted for: " + duration + " sec");
					else
						plugin.Info("Round lasted for: " + minutes + " min, " + (duration - (minutes * 60)) + " sec");
				}
				AdminToolbox.AddMissingPlayerVariables();
				foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
				{
					if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId))
						AdminToolbox.ATPlayerDict[pl.SteamId].RoundsPlayed++;
				}
				AdminToolbox.atfileManager.PlayerStatsFileManager(AdminToolbox.ATPlayerDict.Keys.ToList(), Managers.ATFileManager.PlayerFile.Write);
			}

		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			List<string> keysToChange = AdminToolbox.ATPlayerDict.Where(kp => !kp.Value.keepSettings).Select(p => p.Key).ToList() ?? new List<string>();
			AdminToolbox.lockRound = false;
			if (keysToChange.Count > 0)
				foreach(string key in keysToChange)
					Managers.SetPlayerVariables.SetPlayerBools(key, godMode: false, dmgOff: false, destroyDoor: false, lockDown: false, instantKill: false);
			//foreach (Player player in ev.Server.GetPlayers())
			//	if (AdminToolbox.playerdict.ContainsKey(player.SteamId))
			//		AdminToolbox.playerdict[player.SteamId].playTime += DateTime.Now.Subtract(AdminToolbox.playerdict[player.SteamId].joinTime).TotalSeconds;
			AdminToolbox.logManager.ManageDatedATLogs();
		}
	}
}
