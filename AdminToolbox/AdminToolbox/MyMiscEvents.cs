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

namespace AdminToolbox
{
	internal class MyMiscEvents : IEventHandlerIntercom, IEventHandlerDoorAccess, IEventHandlerSpawn, IEventHandlerWaitingForPlayers, IEventHandlerAdminQuery, IEventHandlerLure, IEventHandlerContain106, IEventHandlerPlayerJoin, IEventHandlerUpdate, IEventHandlerWarheadStartCountdown, IEventHandlerSetServerName, IEventHandlerHandcuffed, IEventHandlerBan
	{
		private Plugin plugin;

		public MyMiscEvents(Plugin plugin)
		{
			this.plugin = plugin;
		}
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
					.Replace("$player", ev.Player.Name));
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
			if (ev.Player != null && ev.Player is Player)
				AdminToolbox.AddMissingPlayerVariables(new List<Player> { ev.Player });
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
			if(ev.Player != null && ev.Player is Player)
				AdminToolbox.AddMissingPlayerVariables(new List<Player> { ev.Player });
			if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId))
			{
				AdminToolbox.playerdict[ev.Player.SteamId].DeathPos = ev.SpawnPos;
				if (AdminToolbox.playerdict[ev.Player.SteamId].spectatorOnly)
					ev.Player.ChangeRole(Role.SPECTATOR);
			}
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (AdminToolbox.isStarting) AdminToolbox.isStarting = false;
			AdminToolbox.lockRound = false;
			if (!ConfigManager.Manager.Config.GetBoolValue("admintoolbox_enable", true, false)) this.plugin.pluginManager.DisablePlugin(plugin);
			if (!AdminToolbox.isColoredCommand) AdminToolbox.isColored = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_colors", false);
			if (!AdminToolbox.intercomLockChanged) AdminToolbox.intercomLock = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_intercomlock", false);
			//this.plugin.Info(System.Reflection.Assembly.GetExecutingAssembly().Location);
		}

		public void OnAdminQuery(AdminQueryEvent ev)
		{
			if (ev.Query != "REQUEST_DATA PLAYER_LIST SILENT")
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
				{
					ev.ActivateContainment = false;
					break;
				}
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!AdminToolbox.isStarting && ev.Player != null && ev.Player is Player p)
			{
				AdminToolbox.AddMissingPlayerVariables(new List<Player> { p });
				AdminToolbox.AdminToolboxLogger.PlayerStatsFileManager(new List<Player> { p }, LogHandlers.PlayerFile.Read);

				if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_player_join_info_extended", true, false))
				{
					int bancount = (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId)) ? AdminToolbox.playerdict[ev.Player.SteamId].banCount : 0;
					string str = Environment.NewLine +
						ev.Player.Name + " joined as player (" + ev.Player.PlayerId + ")" + Environment.NewLine +
						"From IP: " + ev.Player.IpAddress + Environment.NewLine +
						"Using steamID: " + ev.Player.SteamId + Environment.NewLine;
					if (bancount > 0) str += "Player has: \"" + bancount + "\" ban(s) on record" + Environment.NewLine;
					plugin.Info(str);
				}
				else if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_player_join_info", true, false))
				{
					plugin.Info(ev.Player.Name + " just joined the server!");
				}
			}
		}

		DateTime fiveSecTimer = DateTime.Now.AddSeconds(5), threeMinTimer = DateTime.Now.AddMinutes(1), fiveMinTimer = DateTime.Now.AddMinutes(1);
		public void OnUpdate(UpdateEvent ev)
		{
			if (fiveSecTimer <= DateTime.Now) { AdminToolbox.CheckJailedPlayers(); fiveSecTimer = DateTime.Now.AddSeconds(5); }
			if (threeMinTimer <= DateTime.Now) { AdminToolbox.AdminToolboxLogger.PlayerStatsFileManager(null, LogHandlers.PlayerFile.Write); threeMinTimer = DateTime.Now.AddMinutes(3); }
			if (fiveMinTimer <= DateTime.Now)
			{
				fiveMinTimer = DateTime.Now.AddMinutes(5);
				List<string> playerSteamIds = new List<string>(), keysToRemove = new List<string>();

				if (PluginManager.Manager.Server.GetPlayers().Count > 0)
					PluginManager.Manager.Server.GetPlayers().ForEach(p => { if (!string.IsNullOrEmpty(p.SteamId)) playerSteamIds.Add(p.SteamId); });
				if (AdminToolbox.playerdict.Count > 0 && playerSteamIds.Count > 0)
					foreach (KeyValuePair<string, AdminToolbox.AdminToolboxPlayerSettings> kp in AdminToolbox.playerdict)
						if (!playerSteamIds.Contains(kp.Key) && !kp.Value.keepSettings)
							keysToRemove.Add(kp.Key);
				if (keysToRemove.Count > 0)
					foreach (string key in keysToRemove)
						AdminToolbox.playerdict.Remove(key);
			}
		}

		public void OnStartCountdown(WarheadStartEvent ev)
		{
			if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_custom_nuke_cards", false))
			{
				int[] allowedCards = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_nuke_card_list", new int[] { 6, 9, 11 }, false);
				ev.Cancel = !allowedCards.Contains((int)ev.Activator.GetCurrentItem().ItemType);
			}
		}

		public void OnSetServerName(SetServerNameEvent ev)
		{
			ev.ServerName = ev.ServerName.Replace("$atversion", "AT:" + plugin.Details.version);
			ev.ServerName = (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_tracking", true)) ? ev.ServerName += "<color=#ffffff00><size=1>AT:" + plugin.Details.version + "</size></color>" : ev.ServerName;
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
				AdminToolbox.AddMissingPlayerVariables(new List<Player> { ev.Player });

			if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId) && ev.Duration > 1)
				AdminToolbox.playerdict[ev.Player.SteamId].banCount++;
		}
	}
}
