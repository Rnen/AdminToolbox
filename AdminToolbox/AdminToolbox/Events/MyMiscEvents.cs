using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;

namespace AdminToolbox
{
	using API;
	using API.Extentions;
	using API.Webhook;
	using Managers;

	internal class MyMiscEvents : IEventHandlerIntercom, IEventHandlerDoorAccess, IEventHandlerSpawn,
		IEventHandlerWaitingForPlayers, IEventHandlerAdminQuery, IEventHandlerLure, IEventHandlerContain106,
		IEventHandlerPlayerJoin, IEventHandlerUpdate, IEventHandlerWarheadStartCountdown, IEventHandlerSetServerName,
		IEventHandlerHandcuffed, IEventHandlerBan, IEventHandlerTeamRespawn, IEventHandlerThrowGrenade,
		IEventHandlerPlayerDropItem, IEventHandlerReload
	{
		private readonly AdminToolbox plugin;
		private static IConfigFile Config => ConfigManager.Manager.Config;

		private Dictionary<string, PlayerSettings> Dict => AdminToolbox.ATPlayerDict;

		public MyMiscEvents(AdminToolbox plugin) => this.plugin = plugin;

		private void Debug(string str) => plugin.Debug("[MiscEvents]: " + str);
		private void Info(string str) => plugin.Info("[MiscEvents]: " + str);

		public void OnIntercom(PlayerIntercomEvent ev)
		{
			if (AdminToolbox.intercomLock)
			{
				Debug("IntercomLock active, denied use.");
				ev.SpeechTime = 0f;
			}
			if (ev == null || ev.Player == null)
			{
				Debug("Intercom event or event-player was null!");
				return;
			}
			#region Blacklist
			try
			{
				string[] blackListedUserIDS = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_UserID_blacklist", new string[0], false);
				if (blackListedUserIDS.Length > 0)
					foreach (string item in blackListedUserIDS)
						if (item == ev.Player.UserID)
						{
							Debug($"Player \"{ev.Player.Name}\" found in intercom blacklist, denied use.");
							ev.SpeechTime = 0f;
							break;
						}
			} 
			catch (Exception e)
			{
				plugin.Info($"Exception during Intercom Blacklist: " + e);
			}
			#endregion
			#region IntercomWhitelist
			try
			{
				string[] whitelistRanks = Config.GetListValue("admintoolbox_intercom_whitelist", new string[0], false);
				if (whitelistRanks.Length > 0)
				{
					foreach (string item in whitelistRanks)
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
									else plugin.Info(myKeyString[2] + " is not a cool-down number in: " + myKeyString[0]);
								else if (myKeyString.Length > 3)
									plugin.Error("Unknown values at \"admintoolbox_intercom_whitelist: " + item + "\", skipping...");
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				plugin.Info($"Exception during Intercom White-list: " + e);
			}
			#endregion
			try
			{
				string intercomTransmit = Config.GetStringValue("admintoolbox_intercomtransmit_text", string.Empty);
				if (!string.IsNullOrEmpty(intercomTransmit) && ev.SpeechTime > 0f)
				{
					if (ev.Player.GetRankName() != null && !ev.Player.GetUserGroup().Cover)
						intercomTransmit = intercomTransmit.Replace("$playerrank", ev.Player.GetRankName());
					if (ev.Player.GetUserGroup().BadgeText != null && !ev.Player.GetUserGroup().Cover)
						intercomTransmit = intercomTransmit.Replace("$playerbadge", ev.Player.GetUserGroup().BadgeText);
					intercomTransmit = intercomTransmit
						.Replace("$playerid", ev.Player.PlayerID.ToString())
						.Replace("$playerrole", ev.Player.PlayerRole.RoleID.ToString())
						.Replace("$playerteam", ev.Player.PlayerRole.Team.ToString())
						.Replace("$playerhp", ev.Player.Health.ToString())
						.Replace("$playerhealth", ev.Player.Health.ToString())
						.Replace("$player", ev.Player.Name)
						.Replace("\n", Environment.NewLine);
					plugin.Server.Map.SetIntercomContent(IntercomStatus.TRANSMITTING, intercomTransmit);
				}
			}
			catch (Exception e)
			{
				plugin.Info($"Exception during Intercom Transmit Text: " + e);
			}
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			if (ev.Player != null && ev.Player is Player player)
			{
				ATFile.AddMissingPlayerVariables(player);

				if (AdminToolbox.ATPlayerDict.TryGetValue(player.UserID, out PlayerSettings playerSetting))
				{
					if (playerSetting.destroyDoor)
					{
						Debug($"Player \"{ev.Player.Name}\" destroy-doors active, breaking door...");
						ev.Destroy = true;
					}

					if (playerSetting.lockDown)
					{
						Debug($"Player \"{ev.Player.Name}\" lock-down active, denying door use.");
						ev.Allow = false;
					}

					if (playerSetting.lockDoors)
					{
						Debug($"Player \"{ev.Player.Name}\" lock-doors active, {(ev.Door.IsLocked ? "unlocking" : "locking")} {(!string.IsNullOrEmpty(ev.Door.Name) ? ev.Door.Name : "door")}...");
						ev.Door.IsLocked = !ev.Door.IsLocked;
					}
				}
			}
		}

		public void OnSpawn(PlayerSpawnEvent ev)
		{
			ev.Player.SetGhostMode(false); //Temp fix for default *True* ghost-mode

			if (ev.Player != null && ev.Player is Player)
			{
				ATFile.AddMissingPlayerVariables(ev.Player);
			}

			if (AdminToolbox.ATPlayerDict.TryGetValue(ev.Player.UserID, out PlayerSettings pSettings))
			{
				if (pSettings.overwatchMode)
				{
					pSettings.DeathPos = ev.SpawnPos;
					ev.Player.OverwatchMode = true;
				}
				else if (ev.Player.PlayerRole.RoleID != Smod2.API.RoleType.TUTORIAL
					&& pSettings.isJailed && !ev.Player.IsInsideJail())
				{
					JailHandler.SendToJail(ev.Player, pSettings.JailedToTime);
				}
			}
		}

		private int checkNewVersion = 8;
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			Debug($"Entered {System.Reflection.MethodBase.GetCurrentMethod().Name} method");


			ATFile.RenameOldFilesToNewUserID();

			AdminToolbox.lockRound = false;
			if (AdminToolbox.isStarting)
			{
				AdminToolbox.isStarting = false;
			}

			if (!Config.GetBoolValue("admintoolbox_enable", true, false))
			{
				PluginManager.Manager.DisablePlugin(plugin);
			}

#if !DEBUG
			AdminToolbox.DebugMode = Config.GetBoolValue("admintoolbox_debug", false);
#endif

			if (!AdminToolbox.isColoredCommand)
			{
				AdminToolbox.isColored = Config.GetBoolValue("admintoolbox_colors", false);
			}

			if (!AdminToolbox.intercomLockChanged)
			{
				AdminToolbox.intercomLock = Config.GetBoolValue("admintoolbox_intercomlock", false);
			}
			if (checkNewVersion >= 15)
			{
				checkNewVersion = 0;
				if (ATWeb.NewerVersionAvailable())
				{
					plugin.Info($"\n\nNew Version of \"{AdminToolbox.singleton.Details.name}\" available for download! [CURRENT:{AdminToolbox.AT_Version}][NEW:{ATWeb.LatestRelease.Version}]\n" +
						$"Either update via \"AT_AutoUpdate.bat\" or use the command: \"AT DOWNLOAD\"\n\n");
				}
			}
			else
			{
				checkNewVersion++;
			}
			AdminToolbox.WarpManager.RefreshWarps();
			AdminToolbox.LogManager.ManageDatedATLogs();
		}

		public void OnAdminQuery(AdminQueryEvent ev)
		{
			if (ev.Query != "REQUEST_DATA PLAYER_LIST SILENT")
			{
				AdminToolbox.LogManager.WriteToLog(new string[] { ev.Admin.Name + " used command: \"" + ev.Query + "\"" }, Managers.LogManager.ServerLogType.RemoteAdminActivity);
			}
		}

		public void OnLure(PlayerLureEvent ev)
		{
			int[] TUTallowedDmg = Config.GetIntListValue("admintoolbox_tutorial_dmg_allowed", new int[] { -1 }, false);
			if ((AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.UserID) && AdminToolbox.ATPlayerDict[ev.Player.UserID].godMode) || (ev.Player.PlayerRole.Team == Smod2.API.TeamType.TUTORIAL && !TUTallowedDmg.Contains((int)DamageType.FEMUR_BREAKER)))
			{
				ev.AllowContain = false;
			}
		}

		public void OnContain106(PlayerContain106Event ev)
		{
			foreach (Player scp106 in ev.SCP106s)
			{
				if (AdminToolbox.ATPlayerDict.ContainsKey(scp106.UserID) && (AdminToolbox.ATPlayerDict[scp106.UserID].godMode || AdminToolbox.ATPlayerDict[ev.Player.UserID].dmgOff))
				{
					ev.ActivateContainment = false;
					break;
				}
			}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			ev.Player.SetGhostMode(false); //Temp fix for default *True* ghost-mode
			if (!AdminToolbox.isStarting && ev.Player != null && ev.Player is Player player)
			{
				ATFile.AddMissingPlayerVariables(player);
				AdminToolbox.FileManager.PlayerStatsFileManager(player, Managers.ATFile.PlayerFile.Read);

				if (Config.GetBoolValue("admintoolbox_player_join_info_extended", true, false))
				{
					int bancount = AdminToolbox.ATPlayerDict.ContainsKey(player.UserID) ? AdminToolbox.ATPlayerDict[player.UserID].PlayerStats.BanCount : 0;
					string str = Environment.NewLine +
						ev.Player.Name + " joined as player (" + player.PlayerID + ")" + Environment.NewLine +
						"From IP: " + player.IPAddress.Replace("::ffff:", string.Empty) + Environment.NewLine +
						"Using UserID: " + player.UserID + Environment.NewLine;
					if (bancount > 0) 
						str += "Player has: \"" + bancount + "\" ban(s) on record" + Environment.NewLine;
					plugin.Info(str);
				}
				else if (Config.GetBoolValue("admintoolbox_player_join_info", true, false))
				{
					plugin.Info($"\"{player.Name}\" joined the server!");
				}
				if (AdminToolbox.ATPlayerDict.ContainsKey(player.UserID))
				{
					if (AdminToolbox.ATPlayerDict[player.UserID].overwatchMode)
					{
						ev.Player.OverwatchMode = true;
					}
					AdminToolbox.ATPlayerDict[player.UserID].JoinTime = DateTime.UtcNow;
				}
			}
		}

		private static readonly int
			JailCheckInterval = Config.GetIntValue("admintoolbox_jailcheck_interval", 5),
			WritePlayerFileInterval = Config.GetIntValue("admintoolbox_writeplayerfile_interval", 180);
			//DictCleanupInterval = Config.GetIntValue("admintoolbox_dictcleanup_interval", 300);

		private DateTime oneSecTimer = DateTime.UtcNow,
			fiveSecTimer = DateTime.UtcNow.AddSeconds(5),
			oneMinuteTimer = DateTime.UtcNow.AddSeconds(30),
			threeMinTimer = DateTime.UtcNow.AddMinutes(1)/*, 
			fiveMinTimer = DateTime.UtcNow.AddMinutes(2)*/;

		public void OnUpdate(UpdateEvent ev)
		{
			if (oneSecTimer < DateTime.UtcNow)
			{
				if (AdminToolbox.waitForTeleports.Count > 0)
				{
					WaitForTeleport[] waitFors = AdminToolbox.waitForTeleports.ToArray();
					foreach (WaitForTeleport wft in waitFors)
					{
						if (DateTime.UtcNow > wft.DateTime)
						{
							wft.Player.Teleport(wft.Pos);
							wft.Done = true;
						}
					}
					AdminToolbox.waitForTeleports.RemoveAll(s => s.Done);
				}

				oneSecTimer = DateTime.UtcNow.AddSeconds(1);
			}
			if (fiveSecTimer <= DateTime.UtcNow)
			{
				if (plugin.Server.Round.Duration > 0)
				{
					JailHandler.CheckJailedPlayers();
				}
				//if(plugin.scheduledCommands.Count > 0)
				//plugin.scheduledCommands.RemoveAll(sch => sch.hasExecuted);
				fiveSecTimer = DateTime.UtcNow.AddSeconds(JailCheckInterval);
			}
			if (oneMinuteTimer <= DateTime.UtcNow)
			{
				AdminToolbox.ATPlayerDict.Cleanup();
				oneMinuteTimer = DateTime.UtcNow.AddMinutes(1);
			}
			if (threeMinTimer <= DateTime.UtcNow)
			{
				string[] keys = AdminToolbox.ATPlayerDict.Keys.ToArray();
				if (keys?.Length > 0)
				{
					AdminToolbox.FileManager.PlayerStatsFileManager(keys, Managers.ATFile.PlayerFile.Write);
				}
				threeMinTimer = DateTime.UtcNow.AddSeconds(WritePlayerFileInterval);
			}
			//if (fiveMinTimer <= DateTime.UtcNow)
			//{

			//}
		}

		public void OnStartCountdown(WarheadStartEvent ev)
		{
			if (Config.GetBoolValue("admintoolbox_custom_nuke_cards", false))
			{
				int[] allowedCards = Config.GetIntListValue("admintoolbox_nuke_card_list", new int[] { 
					(int)ItemType.KeycardContainmentEngineer, 
					(int)ItemType.KeycardFacilityManager, 
					(int)ItemType.KeycardO5}, false);
				ev.Cancel = !allowedCards.Contains((int)ev.Activator.GetCurrentItem().ItemType);
			}
		}

		public void OnSetServerName(SetServerNameEvent ev)
		{
			ev.ServerName = ev.ServerName.Replace("$atversion", "AT:" + plugin.Details.version);
			if (Config.GetBoolValue("admintoolbox_tracking", true) && !ev.ServerName.Contains("AT:" + plugin.Details.version))
				ev.ServerName += "<color=#ffffff00><size=1>AT:" + plugin.Details.version + "</size></color>";
		}

		public void OnHandcuffed(PlayerHandcuffedEvent ev)
		{
			PlayerSettings playerSetting = Dict.ContainsKey(ev.Player.UserID) ? Dict[ev.Player.UserID] : null;

			if (ev.Player.GodMode || (playerSetting?.godMode ?? false))
			{
				ev.Allow = false;
			}
			else if (ev.Player.PlayerRole.RoleID == Smod2.API.RoleType.TUTORIAL && !Config.GetBoolValue("admintoolbox_tutorial_canbehandcuffed", false))
			{
				ev.Allow = false;
			}
		}

		public void OnBan(BanEvent ev)
		{
			if (Config.GetBoolValue("admintoolbox_ban_console_info", true))
				Info($"\nPlayer \"{ev.Player.Name}\" banned.\n" +
					$"ID: {ev.Player.UserID}" +
					$"Duration: {ev.Duration / 60} minutes\n" +
					$"Reason: {(string.IsNullOrEmpty(ev.Reason) ? "Unspecified" : ev.Reason)}\n" +
					$"Issuer: {(string.IsNullOrEmpty(ev.Issuer) ? "Unspecified" : ev.Issuer)}\n");
			

			string[] banWebhookUrls = Config.GetListValue("admintoolbox_ban_webhooks", new string[0]);
			if (banWebhookUrls.Length > 0 && (ev.Duration > 0 || Config.GetBoolValue("admintoolbox_ban_webhook_onkick", false)))
			{	
				foreach (string url in banWebhookUrls)
					if (!string.IsNullOrEmpty(url))
						plugin.Debug(ATWeb.SendWebhook(Utility.BuildBanWebhook(ev.Player, (int)ev.Duration, ev.Reason, ev.Issuer), url));
				Debug($"Player \"{ev.Player.Name}\" banned, Web-hook posted.");
			}

			if (ev.Player != null && ev.Player is Player)
			{
				ATFile.AddMissingPlayerVariables(ev.Player);
			}

			if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.UserID) && ev.Duration > 1)
			{
				AdminToolbox.ATPlayerDict[ev.Player.UserID].PlayerStats.BanCount++;
				AdminToolbox.FileManager.PlayerStatsFileManager(ev.Player.UserID);
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (AdminToolbox.respawnLock)
				ev.PlayerList.Clear();
		}

		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(ev.Player.UserID, out PlayerSettings ps))
			{
				if (ps.isJailed || ps.lockDown)
					ev.Allow = false;
				else if (ps.grenadeMode || ps.InfiniteItem == Smod2.API.ItemType.GRENADE_HE || ps.InfiniteItem == Smod2.API.ItemType.GRENADE_FLASH)
					ev.Player.GiveItem((Smod2.API.ItemType)(int)ev.GrenadeType);
			}
		}

		public void OnPlayerDropItem(PlayerDropItemEvent ev)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(ev.Player.UserID, out PlayerSettings ps))
			{
				if (ps.isJailed || ps.lockDown)
					ev.Allow = false;
				else if (ps.InfiniteItem != Smod2.API.ItemType.NONE && ev.Item.ItemType == ps.InfiniteItem)
					ev.Player.GiveItem(ps.InfiniteItem);
				else if (ps.grenadeMode && ev.Item.ItemType == Smod2.API.ItemType.GRENADE_HE)
					ev.Player.GiveItem(Smod2.API.ItemType.GRENADE_HE);
			}
		}

		public void OnReload(PlayerReloadEvent ev)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(ev.Player.UserID, out PlayerSettings ps) && ps.InfiniteItem != Smod2.API.ItemType.NONE)
				if (ps.InfiniteItem.ToString().Contains("DROPPED"))
					foreach (AmmoType ammo in Enum.GetValues(typeof(AmmoType)))
						if (ammo.ToString() == ps.InfiniteItem.ToString())
						{
							ev.Player.SetAmmo(ammo, ev.CurrentAmmoTotal + ev.NormalMaxClipSize);
							break;
						}
		}
	}


	internal class LateEscapeEventCheck : IEventHandlerCheckEscape
	{
		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (ev.AllowEscape && AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.UserID))
			{
				AdminToolbox.ATPlayerDict[ev.Player.UserID].PlayerStats.EscapeCount++;
			}
		}
	}
}
