using System;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace AdminToolbox.Events
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
				plugin.Info("Round: " + ++AdminToolbox.RoundCount + " start");
				plugin.Info("Players this round: " + ev.Server.NumPlayers);
			}
			Managers.ATFile.AddMissingPlayerVariables();
			AdminToolbox.FileManager.PlayerStatsFileManager(Managers.ATFile.PlayerFile.Write);
			AdminToolbox.LogManager.SetLogStartTime();
			AdminToolbox.WarpManager.RefreshWarps();

			AdminToolbox.roundStatsRecorded = false;

			if (!string.IsNullOrEmpty(intercomReady))
			{
				ev.Server.Map.SetIntercomContent(IntercomStatus.READY, intercomReady);
			}
			if (!string.IsNullOrEmpty(intercomRestart))
			{
				ev.Server.Map.SetIntercomContent(IntercomStatus.RESTARTING, intercomRestart);
			}
			if (!string.IsNullOrEmpty(intercomTransmit))
			{
				ev.Server.Map.SetIntercomContent(IntercomStatus.TRANSMITTING, intercomTransmit);
			}
		}


		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (AdminToolbox.lockRound)
			{
				ev.Status = RoundEndStatus.ON_GOING;
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

				Managers.ATFile.AddMissingPlayerVariables();

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
				AdminToolbox.ATPlayerDict.ResetPlayerBools();
			}

			string[] keys = AdminToolbox.ATPlayerDict.Keys.ToArray();
			if (keys.Length > 0)
				foreach (string key in keys)
				{
					if (AdminToolbox.ATPlayerDict.ContainsKey(key))
					{
						PlayerSettings ps = AdminToolbox.ATPlayerDict[key];
						ps.PlayerStats.MinutesPlayed += DateTime.UtcNow.Subtract(ps.JoinTime).TotalSeconds;
						AdminToolbox.ATPlayerDict[key] = ps;
					}
				}

			AdminToolbox.FileManager.PlayerStatsFileManager(AdminToolbox.ATPlayerDict.Keys.ToArray(), Managers.ATFile.PlayerFile.Write);
		}
	}
}
