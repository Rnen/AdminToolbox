using System;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace AdminToolbox
{
	using API;
	using API.Extentions;

	public class RoundEventHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerRoundRestart, IEventHandlerCheckRoundEnd
	{
		private readonly AdminToolbox plugin;

		private static IConfigFile Config => ConfigManager.Manager.Config;

		private void Debug(string message) => plugin.Debug(message);

		internal string
			intercomReady = Config.GetStringValue("admintoolbox_intercomready_text", string.Empty),
			intercomRestart = Config.GetStringValue("admintoolbox_intercomrestart_text", string.Empty),
			intercomTransmit = Config.GetStringValue("admintoolbox_intercomtransmit_text", string.Empty);

		public RoundEventHandler(AdminToolbox plugin) => this.plugin = plugin;

		public void OnRoundStart(RoundStartEvent ev)
		{
			AdminToolbox.isRoundFinished = false;
			if (Config.GetBoolValue("admintoolbox_round_info", true, false))
			{
				plugin.Info("Round: " + ++AdminToolbox.RoundCount + " started.");
				plugin.Info("Players this round: " + ev.Server.NumPlayers);
			}
			AdminToolbox.AddMissingPlayerVariables();
			AdminToolbox.atfileManager.PlayerStatsFileManager(Managers.ATFile.PlayerFile.Write);
			AdminToolbox.logManager.SetLogStartTime();
			AdminToolbox.warpManager.RefreshWarps();

			AdminToolbox.roundStatsRecorded = false;

			if (intercomReady != string.Empty)
			{
				ev.Server.Map.SetIntercomContent(IntercomStatus.Ready, intercomReady);
			}
			if (intercomRestart != string.Empty)
			{
				ev.Server.Map.SetIntercomContent(IntercomStatus.Restarting, intercomRestart);
			}
			if (intercomTransmit != string.Empty)
			{
				ev.Server.Map.SetIntercomContent(IntercomStatus.Transmitting, intercomTransmit);
			}
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
			if (plugin.Server.Round.Duration >= 3)
			{
				AdminToolbox.isRoundFinished = true;
				AdminToolbox.lockRound = false;

				if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_round_info", true, false))
				{
					plugin.Info("Round: " + AdminToolbox.RoundCount + " has ended.");
					int minutes = ev.Round.Duration / 60, duration = ev.Round.Duration;
					if (duration < 60)
						plugin.Info("Round lasted for: " + duration + " sec");
					else
						plugin.Info("Round lasted for: " + minutes + " min, " + (duration - (minutes * 60)) + " sec");
				}

				AdminToolbox.AddMissingPlayerVariables();

				string[] keys = AdminToolbox.ATPlayerDict.Keys.ToArray();
				if (keys.Length > 0)
					foreach (string key in keys)
					{
						if (AdminToolbox.ATPlayerDict.ContainsKey(key))
						{
							PlayerSettings ps = AdminToolbox.ATPlayerDict[key];
							ps.PlayerStats.RoundsPlayed++;
							AdminToolbox.ATPlayerDict[key] = ps;
						}
					}
			}

		}

		public void OnRoundRestart(RoundRestartEvent ev)
		{
			AdminToolbox.lockRound = false;
			if (AdminToolbox.ATPlayerDict.Count > 0)
			{
				AdminToolbox.ATPlayerDict.Keys.ResetPlayerBools();
			}

			string[] keys = AdminToolbox.ATPlayerDict.Keys.ToArray();
			if (keys.Length > 0)
				foreach (string key in keys)
				{
					if (AdminToolbox.ATPlayerDict.ContainsKey(key))
					{
						PlayerSettings ps = AdminToolbox.ATPlayerDict[key];
						ps.PlayerStats.MinutesPlayed += DateTime.Now.Subtract(ps.JoinTime).TotalSeconds;
						AdminToolbox.ATPlayerDict[key] = ps;
					}
				}

			AdminToolbox.atfileManager.PlayerStatsFileManager(AdminToolbox.ATPlayerDict.Keys.ToArray(), Managers.ATFile.PlayerFile.Write);
		}
	}
}
