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
		IEventHandlerHandcuffed, IEventHandlerBan, IEventHandlerSetRole, IEventHandlerTeamRespawn, IEventHandlerThrowGrenade,
		IEventHandlerPlayerDropItem, IEventHandlerReload
	{
		private readonly AdminToolbox plugin;
		private static IConfigFile Config => ConfigManager.Manager.Config;
		private Server Server => PluginManager.Manager.Server;

		private Dictionary<string, PlayerSettings> Dict => AdminToolbox.ATPlayerDict;

		public MyMiscEvents(AdminToolbox plugin) => this.plugin = plugin;

		private void Debug(string str) => plugin.Debug("[MiscEvents]: " + str);

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
				string[] blackListedUserIdS = ConfigManager.Manager.Config.GetListValue("admintoolbox_intercom_UserId_blacklist", new string[0], false);
				if (blackListedUserIdS.Length > 0)
					foreach (string item in blackListedUserIdS)
						if (item == ev.Player.UserId)
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
									else plugin.Info(myKeyString[2] + " is not a cooldown number in: " + myKeyString[0]);
								else if (myKeyString.Length > 3)
									plugin.Error("Unknown values at \"admintoolbox_intercom_whitelist: " + item + "\", skipping...");
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				plugin.Info($"Exception during Intercom Whitelist: " + e);
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
						.Replace("$playerid", ev.Player.PlayerId.ToString())
						.Replace("$playerrole", ev.Player.TeamRole.Role.ToString())
						.Replace("$playerteam", ev.Player.TeamRole.Team.ToString())
						.Replace("$playerhp", ev.Player.GetHealth().ToString())
						.Replace("$playerhealth", ev.Player.GetHealth().ToString())
						.Replace("$player", ev.Player.Name)
						.Replace("\n", Environment.NewLine);
					plugin.Server.Map.SetIntercomContent(IntercomStatus.Transmitting, intercomTransmit);
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
				AdminToolbox.AddMissingPlayerVariables(player);
				AdminToolbox.ATPlayerDict.TryGetValue(player.UserId, out PlayerSettings playerSetting);

				if (playerSetting != null)
				{
					if (playerSetting.destroyDoor)
					{
						Debug($"Player \"{ev.Player.Name}\" destroy-doors active, breaking door...");
						ev.Destroy = true;
					}

					if (playerSetting.lockDown)
					{
						Debug($"Player \"{ev.Player.Name}\" lockdown active, denying door use.");
						ev.Allow = false;
					}

					if (playerSetting.lockDoors)
					{
						Debug($"Player \"{ev.Player.Name}\" lock-doors active, {(ev.Door.Locked ? "unlocking" : "locking")} {(!string.IsNullOrEmpty(ev.Door.Name) ? ev.Door.Name : "door")}...");
						ev.Door.Locked = !ev.Door.Locked;
					}
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
		} //Currently not used

		public void OnSpawn(PlayerSpawnEvent ev)
		{
			ev.Player.SetGhostMode(false); //Temp fix for default *True* ghostmode

			if (ev.Player != null && ev.Player is Player)
			{
				AdminToolbox.AddMissingPlayerVariables(ev.Player);
			}

			if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.UserId))
			{
				PlayerSettings pSettings = AdminToolbox.ATPlayerDict[ev.Player.UserId];
				if (pSettings.overwatchMode)
				{
					pSettings.DeathPos = ev.SpawnPos;
					ev.Player.OverwatchMode = true;
				}
				else if (ev.Player.TeamRole.Role != Smod2.API.RoleType.TUTORIAL
					&& pSettings.isJailed && !ev.Player.IsInsideJail())
				{
					JailHandler.SendToJail(ev.Player, pSettings.JailedToTime);
				}
			}
		}

		private int checkNewVersion = 8;
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			ATFile.ConvertOldFilesToNewUserID();


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
			//this.plugin.Info(System.Reflection.Assembly.GetExecutingAssembly().Location);
			if (checkNewVersion >= 8)
			{
				checkNewVersion = 0;
				if (ATWeb.NewerVersionAvailable())
				{
					plugin.Info("\n\n [New Version of AdminToolbox avaiable for download!] [V:" + this.plugin.GetGitReleaseInfo().Version + "]\n " + " Either update via \"AT_AutoUpdate.bat\" or write \"AT DOWNLOAD\"" + "\n\n");
				}
			}
			else
			{
				checkNewVersion++;
			}
			AdminToolbox.warpManager.RefreshWarps();
			AdminToolbox.logManager.ManageDatedATLogs();
		}

		public void OnAdminQuery(AdminQueryEvent ev)
		{
			if (ev.Query != "REQUEST_DATA PLAYER_LIST SILENT")
			{
				AdminToolbox.logManager.WriteToLog(new string[] { ev.Admin.Name + " used command: \"" + ev.Query + "\"" }, Managers.LogManager.ServerLogType.RemoteAdminActivity);
			}
		}

		public void OnLure(PlayerLureEvent ev)
		{
			int[] TUTallowedDmg = Config.GetIntListValue("admintoolbox_tutorial_dmg_allowed", new int[] { -1 }, false);
			if ((AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.UserId) && AdminToolbox.ATPlayerDict[ev.Player.UserId].godMode) || (ev.Player.TeamRole.Team == Smod2.API.TeamType.TUTORIAL && !TUTallowedDmg.Contains((int)DamageType.LURE)))
			{
				ev.AllowContain = false;
			}
		}

		public void OnContain106(PlayerContain106Event ev)
		{
			foreach (Player scp106 in ev.SCP106s)
			{
				if (AdminToolbox.ATPlayerDict.ContainsKey(scp106.UserId) && (AdminToolbox.ATPlayerDict[scp106.UserId].godMode || AdminToolbox.ATPlayerDict[ev.Player.UserId].dmgOff))
				{
					ev.ActivateContainment = false;
					break;
				}
			}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			ev.Player.SetGhostMode(false); //Temp fix for default *True* ghostmode
			if (!AdminToolbox.isStarting && ev.Player != null && ev.Player is Player player)
			{
				AdminToolbox.AddMissingPlayerVariables(player);
				AdminToolbox.atfileManager.PlayerStatsFileManager(player, Managers.ATFile.PlayerFile.Read);

				if (Config.GetBoolValue("admintoolbox_player_join_info_extended", true, false))
				{
					int bancount = AdminToolbox.ATPlayerDict.ContainsKey(player.UserId) ? AdminToolbox.ATPlayerDict[player.UserId].PlayerStats.BanCount : 0;
					string str = Environment.NewLine +
						ev.Player.Name + " joined as player (" + player.PlayerId + ")" + Environment.NewLine +
						"From IP: " + player.IpAddress.Replace("::ffff:", string.Empty) + Environment.NewLine +
						"Using UserId: " + player.UserId + Environment.NewLine;
					if (bancount > 0) str += "Player has: \"" + bancount + "\" ban(s) on record" + Environment.NewLine;
					plugin.Info(str);
				}
				else if (Config.GetBoolValue("admintoolbox_player_join_info", true, false))
				{
					plugin.Info(player.Name + " just joined the server!");
				}
				if (AdminToolbox.ATPlayerDict.ContainsKey(player.UserId))
				{
					if (AdminToolbox.ATPlayerDict[player.UserId].overwatchMode)
					{
						ev.Player.OverwatchMode = true;
					}
					AdminToolbox.ATPlayerDict[player.UserId].JoinTime = DateTime.Now;
				}
			}
		}

		private static readonly int
			JailCheckInterval = Config.GetIntValue("admintoolbox_jailcheck_interval", 5),
			WritePlayerFileInterval = Config.GetIntValue("admintoolbox_writeplayerfile_interval", 180),
			DictCleanupInterval = Config.GetIntValue("admintoolbox_dictcleanup_interval", 300);

		private DateTime oneSecTimer = DateTime.Now,
			fiveSecTimer = DateTime.Now.AddSeconds(5),
			oneMinuteTimer = DateTime.Now.AddSeconds(30),
			threeMinTimer = DateTime.Now.AddMinutes(1)/*, 
			fiveMinTimer = DateTime.Now.AddMinutes(2)*/;

		public void OnUpdate(UpdateEvent ev)
		{
			if (oneSecTimer < DateTime.Now)
			{
				if (AdminToolbox.waitForTeleports.Count > 0)
				{
					WaitForTeleport[] waitFors = AdminToolbox.waitForTeleports.ToArray();
					foreach (WaitForTeleport wft in waitFors)
					{
						if (DateTime.Now > wft.DateTime)
						{
							wft.Player.Teleport(wft.Pos);
							wft.Done = true;
						}
					}
					AdminToolbox.waitForTeleports.RemoveAll(s => s.Done);
				}

				oneSecTimer = DateTime.Now.AddSeconds(1);
			}
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
			if (oneMinuteTimer <= DateTime.Now)
			{
				AdminToolbox.ATPlayerDict.Cleanup();
				oneMinuteTimer = DateTime.Now.AddMinutes(1);
			}
			if (threeMinTimer <= DateTime.Now)
			{
				string[] keys = AdminToolbox.ATPlayerDict.Keys.ToArray();
				if (keys?.Length > 0)
				{
					AdminToolbox.atfileManager.PlayerStatsFileManager(keys, Managers.ATFile.PlayerFile.Write);
				}
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
				int[] allowedCards = Config.GetIntListValue("admintoolbox_nuke_card_list", new int[] { 6, 9, 11 }, false);
				ev.Cancel = !allowedCards.Contains((int)ev.Activator.GetCurrentItem().ItemType);
			}
		}

		public void OnSetServerName(SetServerNameEvent ev)
		{
			ev.ServerName = ev.ServerName.Replace("$atversion", "AT:" + plugin.Details.version);
			ev.ServerName = Config.GetBoolValue("admintoolbox_tracking", true) ? ev.ServerName += "<color=#ffffff00><size=1>AT:" + plugin.Details.version + "</size></color>" : ev.ServerName;
		}

		public void OnHandcuffed(PlayerHandcuffedEvent ev)
		{
			PlayerSettings playerSetting = Dict.ContainsKey(ev.Player.UserId) ? Dict[ev.Player.UserId] : null;

			if (ev.Player.GetGodmode() || (playerSetting?.godMode ?? false))
			{
				ev.Allow = false;
			}
			else if (ev.Player.TeamRole.Role == Smod2.API.RoleType.TUTORIAL && !Config.GetBoolValue("admintoolbox_tutorial_canbehandcuffed", false))
			{
				ev.Allow = false;
			}
		}

		public void OnBan(BanEvent ev)
		{
			string[] banWebhookUrls = Config.GetListValue("admintoolbox_ban_webhooks", new string[0]);
			if (banWebhookUrls.Length > 0 && (ev.Duration > 0 || Config.GetBoolValue("admintoolbox_ban_webhook_onkick", false)))
			{
				DiscordWebhook webH;
				List<Field> listOfFields = new List<Field>();

				listOfFields.AddField("Playername: ", ev.Player.Name);
				listOfFields.AddField("Duration: ", (ev.Duration / 60).ToString("0.0", CultureInfo.InvariantCulture) + " hours");
				if (!string.IsNullOrEmpty(ev.Reason))
					listOfFields.AddField("Reason: ", ev.Reason);
				if (Config.GetBoolValue("admintoolbox_ban_webhook_include_admin", false))
					listOfFields.AddField("Issued By: ", string.IsNullOrEmpty(ev.Issuer) ? "Server" : ev.Issuer);

				webH = new DiscordWebhook { embeds = new EmbedData[] { new EmbedData { author = new Author { name = "User Banned: " }, title = "", fields = listOfFields.ToArray() } } };

				foreach (string url in banWebhookUrls)
					if (!string.IsNullOrEmpty(url))
						plugin.Debug(ATWeb.SendWebhook(webH, url));
				plugin.Info("Ban webhooks posted!");
			}

			if (ev.Player != null && ev.Player is Player)
			{
				AdminToolbox.AddMissingPlayerVariables(ev.Player);
			}

			if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.UserId) && ev.Duration > 1)
			{
				AdminToolbox.ATPlayerDict[ev.Player.UserId].PlayerStats.BanCount++;
				AdminToolbox.atfileManager.PlayerStatsFileManager(ev.Player.UserId);
			}
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (AdminToolbox.respawnLock)
				ev.PlayerList.Clear();
		}

		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(ev.Player.UserId, out PlayerSettings ps))
			{
				if (ps.isJailed || ps.lockDown)
					ev.Allow = false;
				else if (ps.grenadeMode || ps.InfiniteItem == Smod2.API.ItemType.FRAG_GRENADE || ps.InfiniteItem == Smod2.API.ItemType.FLASHBANG)
					ev.Player.GiveItem((ev.GrenadeType == GrenadeType.FRAG_GRENADE) ? Smod2.API.ItemType.FRAG_GRENADE : Smod2.API.ItemType.FLASHBANG);
			}
		}

		public void OnPlayerDropItem(PlayerDropItemEvent ev)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(ev.Player.UserId, out PlayerSettings ps))
			{
				if (ps.isJailed || ps.lockDown)
					ev.Allow = false;
				else if (ps.InfiniteItem != Smod2.API.ItemType.NULL && ev.Item.ItemType == ps.InfiniteItem)
					ev.Player.GiveItem(ps.InfiniteItem);
				else if (ps.grenadeMode && ev.Item.ItemType == Smod2.API.ItemType.FRAG_GRENADE)
					ev.Player.GiveItem(Smod2.API.ItemType.FRAG_GRENADE);
			}
		}

		public void OnReload(PlayerReloadEvent ev)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(ev.Player.UserId, out PlayerSettings ps) && ps.InfiniteItem != Smod2.API.ItemType.NULL)
				if (ps.InfiniteItem.ToString().Contains("DROPPED"))
					foreach (AmmoType ammo in Enum.GetValues(typeof(AmmoType)))
						if (ammo.ToString() == ps.InfiniteItem.ToString())
						{
							ev.Player.SetAmmo(ammo, ev.CurrentAmmoTotal + ev.NormalMaxClipSize);
							break;
						}
		}
	}
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class LateEscapeEventCheck : IEventHandlerCheckEscape
	{
		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (ev.AllowEscape && AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.UserId))
			{
				AdminToolbox.ATPlayerDict[ev.Player.UserId].PlayerStats.EscapeCount++;
			}
		}
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
