using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using Unity;
using AdminToolbox.API;

namespace AdminToolbox
{
	internal class MyMiscEvents : IEventHandlerIntercom, IEventHandlerDoorAccess, IEventHandlerSpawn, 
		IEventHandlerWaitingForPlayers, IEventHandlerAdminQuery, IEventHandlerLure, IEventHandlerContain106, 
		IEventHandlerPlayerJoin, IEventHandlerUpdate, IEventHandlerWarheadStartCountdown, IEventHandlerSetServerName, 
		IEventHandlerHandcuffed, IEventHandlerBan, IEventHandlerSetRole
	{
		private readonly AdminToolbox plugin;
		private static IConfigFile Config => ConfigManager.Manager.Config;

		public MyMiscEvents(AdminToolbox plugin) => this.plugin = plugin;

		public void OnIntercom(PlayerIntercomEvent ev)
		{
			if (AdminToolbox.intercomLock) ev.SpeechTime = 0f;
			#region Blacklist
			string[] blackListedSTEAMIDS = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_steamid_blacklist", new string[] { string.Empty }, false);
			if (blackListedSTEAMIDS.Length > 0)
				foreach (string item in blackListedSTEAMIDS)
					if (item == ev.Player.SteamId)
					{
						ev.SpeechTime = 0f;
						break;
					}
			#endregion
			#region IntercomWhitelist
			string[] whitelistRanks = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_whitelist", new string[] { string.Empty }, false);
			if (whitelistRanks.Length > 0)
			{
				foreach (var item in whitelistRanks)
				{
					string[] myKeyString = item.Split(':', '-', '_', '#');
					if (myKeyString[0].ToLower().Trim() == ev.Player.GetRankName().ToLower().Trim() || myKeyString[0].ToLower().Trim() == ev.Player.GetUserGroup().Name.ToLower().Trim())
					{
						if (myKeyString.Length >= 2)
						{
							if (float.TryParse(myKeyString[1], out float x))
								ev.SpeechTime = (x <= 0) ? 300 : x;
							else plugin.Info(myKeyString[1] + " is not a valid speakTime number in: " + myKeyString[0]);
							if (myKeyString.Length == 3)
								if (float.TryParse(myKeyString[2], out float z))
									ev.CooldownTime = z;
								else plugin.Info(myKeyString[2] + " is not a cooldown number in: " + myKeyString[0]);
							else if (myKeyString.Length > 3)
								plugin.Error("Unknown values at \"admintoolbox_intercom_whitelist: " + item + "\", skipping...");
						}
					}
				}
			}
			#endregion
			string intercomTransmit = ConfigManager.Manager.Config.GetStringValue("admintoolbox_intercomtransmit_text", string.Empty);
			if (intercomTransmit != string.Empty && ev.SpeechTime > 0f)
				plugin.pluginManager.Server.Map.SetIntercomContent(IntercomStatus.Transmitting, intercomTransmit
					.Replace("$playerid",ev.Player.PlayerId.ToString())
					.Replace("$playerrole",ev.Player.TeamRole.Role.ToString())
					.Replace("$playerteam", ev.Player.TeamRole.Team.ToString())
					.Replace("$playerhp",ev.Player.GetHealth().ToString())
					.Replace("$playerhealth", ev.Player.GetHealth().ToString())
					.Replace("$playerrank", ev.Player.GetRankName())
					.Replace("$player", ev.Player.Name)
					.Replace("\n", Environment.NewLine));
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			if (ev.Player != null && ev.Player is Player player)
			{
				AdminToolbox.AddMissingPlayerVariables(player);
				PlayerSettings playerSetting = (AdminToolbox.ATPlayerDict.ContainsKey(player.SteamId) ? AdminToolbox.ATPlayerDict[player.SteamId] : null);

				if (playerSetting != null)
				{
					if (playerSetting.destroyDoor)
						ev.Destroy = true;
					if (playerSetting.lockDown)
						ev.Allow = false;
				}
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			//if (ev.Player.TeamRole.Role == Role.TUTORIAL)
			//{
			//	Vector ppos = ev.Player.GetPosition();
			//	if (Physics.Raycast(new Vector3(ppos.x + 3, ppos.y - 3, ppos.z), Vector3.down, out RaycastHit hitInfo))
			//	{
			//		plugin.Info(string.Format("X: {0}, Y: {1}, Z: {2}", hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.transform.position.z));
			//		if (hitInfo.collider.gameObject.name.ToLower() == "classname=brush.003")
			//		{
			//			Vector3 hitPos = hitInfo.transform.position;

			//			Vector newPos = new ServerMod2.API.SmodVector(hitPos.x, hitPos.y + 10, hitPos.z);

			//			ev.Player.Teleport(newPos);
			//		}
			//		else
			//			plugin.Info(hitInfo.collider.gameObject.name);
			//	}
			//	else
			//		plugin.Info("No hit!");
			//}
		}

		public void OnSpawn(PlayerSpawnEvent ev)
		{
			ev.Player.SetGhostMode(false); //Temp fix for default *True* ghostmode
			if (ev.Player != null && ev.Player is Player)
				AdminToolbox.AddMissingPlayerVariables(ev.Player);
			if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.SteamId))
			{
				PlayerSettings pSettings = AdminToolbox.ATPlayerDict[ev.Player.SteamId];
				if (pSettings.overwatchMode)
				{
					pSettings.DeathPos = ev.SpawnPos;
					ev.Player.OverwatchMode = true;
				}
				else if (ev.Player.TeamRole.Role != Role.TUTORIAL
					&& pSettings.isJailed && !pSettings.IsInsideJail)
				{
					JailHandler.SendToJail(ev.Player, pSettings.JailedToTime);
				}
			}
		}

		int checkNewVersion = 8;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (AdminToolbox.isStarting) AdminToolbox.isStarting = false;
			AdminToolbox.lockRound = false;
			if (!Config.GetBoolValue("admintoolbox_enable", true, false)) this.plugin.pluginManager.DisablePlugin(plugin);
			if (!AdminToolbox.isColoredCommand) AdminToolbox.isColored = Config.GetBoolValue("admintoolbox_colors", false);
			if (!AdminToolbox.intercomLockChanged) AdminToolbox.intercomLock = Config.GetBoolValue("admintoolbox_intercomlock", false);
			//this.plugin.Info(System.Reflection.Assembly.GetExecutingAssembly().Location);
			if (checkNewVersion >= 8)
			{
				checkNewVersion = 0;
				if (this.plugin.NewerVersionAvailable())
					plugin.Info("\n\n [New Version of AdminToolbox avaiable for download!] [V:" + this.plugin.GetGitReleaseInfo().Version + "]\n " + " Either update via \"AT_AutoUpdate.bat\" or write \"AT DOWNLOAD\"" + "\n\n");
			}
			else
				checkNewVersion++;

			if (AdminToolbox.RoundCount == 0)
				AdminToolbox.warpManager.RefreshWarps();
		}

		public void OnAdminQuery(AdminQueryEvent ev)
		{
			if (ev.Query != "REQUEST_DATA PLAYER_LIST SILENT")
				AdminToolbox.logManager.WriteToLog(new string[] { ev.Admin.Name + " used command: \"" + ev.Query + "\"" }, Managers.LogManager.ServerLogType.RemoteAdminActivity);
		}

		public void OnLure(PlayerLureEvent ev)
		{
			int[] TUTallowedDmg = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_tutorial_dmg_allowed", new int[] { -1 }, false);
			if ((AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.SteamId) && AdminToolbox.ATPlayerDict[ev.Player.SteamId].godMode) || (ev.Player.TeamRole.Team == Smod2.API.Team.TUTORIAL && !TUTallowedDmg.Contains((int)DamageType.LURE)))
				ev.AllowContain = false;
		}

		public void OnContain106(PlayerContain106Event ev)
		{
			foreach (Player pl in ev.SCP106s)
				if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && (AdminToolbox.ATPlayerDict[pl.SteamId].godMode || AdminToolbox.ATPlayerDict[ev.Player.SteamId].dmgOff))
				{
					ev.ActivateContainment = false;
					break;
				}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			ev.Player.SetGhostMode(false); //Temp fix for default *True* ghostmode
			if (!AdminToolbox.isStarting && ev.Player != null && ev.Player is Player player)
			{
				AdminToolbox.AddMissingPlayerVariables(player);
				AdminToolbox.atfileManager.PlayerStatsFileManager(player, Managers.ATFileManager.PlayerFile.Read);

				if (Config.GetBoolValue("admintoolbox_player_join_info_extended", true, false))
				{
					int bancount = (AdminToolbox.ATPlayerDict.ContainsKey(player.SteamId)) ? AdminToolbox.ATPlayerDict[player.SteamId].banCount : 0;
					string str = Environment.NewLine +
						ev.Player.Name + " joined as player (" + player.PlayerId + ")" + Environment.NewLine +
						"From IP: " + (player.IpAddress).Replace("::ffff:",string.Empty) + Environment.NewLine +
						"Using steamID: " + player.SteamId + Environment.NewLine;
					if (bancount > 0) str += "Player has: \"" + bancount + "\" ban(s) on record" + Environment.NewLine;
					plugin.Info(str);
				}
				else if (Config.GetBoolValue("admintoolbox_player_join_info", true, false))
				{
					plugin.Info(player.Name + " just joined the server!");
				}
				if (AdminToolbox.ATPlayerDict.ContainsKey(player.SteamId))
				{
					if (AdminToolbox.ATPlayerDict[player.SteamId].overwatchMode)
						ev.Player.OverwatchMode = true;
					AdminToolbox.ATPlayerDict[player.SteamId].JoinTime = DateTime.Now;
				}
			}
		}

		private readonly static int 
			JailCheckInterval = Config.GetIntValue("admintoolbox_jailcheck_interval", 5),
			WritePlayerFileInterval = Config.GetIntValue("admintoolbox_writeplayerfile_interval",180),
			DictCleanupInterval = Config.GetIntValue("admintoolbox_dictcleanup_interval",300);

		private DateTime /*oneSecTimer = DateTime.Now,*/ 
			fiveSecTimer = DateTime.Now.AddSeconds(5),
			oneMinuteTimer = DateTime.Now.AddSeconds(30), 
			threeMinTimer = DateTime.Now.AddMinutes(1), 
			fiveMinTimer = DateTime.Now.AddMinutes(2);

		public void OnUpdate(UpdateEvent ev)
		{ /*
			if (oneSecTimer < DateTime.Now)
			{
				if (plugin.scheduledCommands.Count > 0)
					foreach (ScheduledCommandCall schm in plugin.scheduledCommands.Where(s => !s.hasExecuted && s.timeToExecute <= DateTime.Now))
					{
						string[] rawResponse = schm.CallCommand();
						string response = string.Empty;
						if (rawResponse.Length > 0)
							for (int i = 0; i < rawResponse.Length; i++)
								response += rawResponse[i] + " ";
						if (!string.IsNullOrEmpty(response))
							plugin.Info("Scheduled " + schm.command + ": " + response);
						schm.hasExecuted = true;
					}
				//if (plugin.scheduledRestart.enabled && plugin.scheduledRestart.restartTime <= DateTime.Now)
				//plugin.scheduledRestart.CallRestart();
				oneSecTimer = DateTime.Now.AddSeconds(1);
			}*/
			if (fiveSecTimer <= DateTime.Now)
			{
				if (plugin.Server.Round.Duration > 0)
				{
					JailHandler.CheckJailedPlayers();
				}
				//if(plugin.scheduledCommands.Count > 0)
					//plugin.scheduledCommands.RemoveAll(sch => sch.hasExecuted);
				fiveSecTimer = DateTime.Now.AddSeconds(JailCheckInterval);
			}
			if(oneMinuteTimer <= DateTime.Now)
			{
				AdminToolbox.ATPlayerDict.Cleanup();
			}
			if (threeMinTimer <= DateTime.Now)
			{
				List<string> keys = AdminToolbox.ATPlayerDict.Keys.ToList();
				if (keys?.Count > 0)
					AdminToolbox.atfileManager.PlayerStatsFileManager(keys, Managers.ATFileManager.PlayerFile.Write);
				threeMinTimer = DateTime.Now.AddSeconds(WritePlayerFileInterval);
			}
			//if (fiveMinTimer <= DateTime.Now)
			//{

			//}
		}

		public void OnStartCountdown(WarheadStartEvent ev)
		{
			if (Config.GetBoolValue("admintoolbox_custom_nuke_cards", false))
			{
				int[] allowedCards = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_nuke_card_list", new int[] { 6, 9, 11 }, false);
				ev.Cancel = !allowedCards.Contains((int)ev.Activator.GetCurrentItem().ItemType);
			}
		}

		public void OnSetServerName(SetServerNameEvent ev)
		{
			ev.ServerName = ev.ServerName.Replace("$atversion", "AT:" + plugin.Details.version);
			ev.ServerName = (Config.GetBoolValue("admintoolbox_tracking", true)) ? ev.ServerName += "<color=#ffffff00><size=1>AT:" + plugin.Details.version + "</size></color>" : ev.ServerName;
		}

		public void OnHandcuffed(PlayerHandcuffedEvent ev)
		{
			//if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId) && AdminToolbox.playerdict[ev.Player.SteamId].godMode || ev.Player.GetGodmode())
			//	ev.Handcuffed = false;
			//else if (ev.Player.TeamRole.Role == Role.TUTORIAL && !ConfigManager.Manager.Config.GetBoolValue("admintoolbox_tutorial_canbehandcuffed", false))
			//	ev.Handcuffed = false;
		}

		public void OnBan(BanEvent ev)
		{
			if (ev.Player != null && ev.Player is Player)
				AdminToolbox.AddMissingPlayerVariables(ev.Player);

			if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.SteamId) && ev.Duration > 1)
			{
				AdminToolbox.ATPlayerDict[ev.Player.SteamId].banCount++;
				AdminToolbox.atfileManager.PlayerStatsFileManager(ev.Player.SteamId);
			}
		}
	}
}
