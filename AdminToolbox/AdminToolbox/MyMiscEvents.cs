using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace AdminToolbox
{
	internal class MyMiscEvents : IEventHandlerIntercom, IEventHandlerDoorAccess, IEventHandlerSpawn, IEventHandlerWaitingForPlayers, IEventHandlerAdminQuery, IEventHandlerLure, IEventHandlerContain106, IEventHandlerPlayerJoin, IEventHandlerUpdate, IEventHandlerSetRole, IEventHandlerWarheadStartCountdown, IEventHandlerSetServerName
	{
		private Plugin plugin;

		public MyMiscEvents(Plugin plugin)
		{
			this.plugin = plugin;
		}
		public void OnIntercom(PlayerIntercomEvent ev)
		{
			if (AdminToolbox.intercomLock) ev.SpeechTime = 0f;
			#region Blacklis
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
					if (myKeyString[0].ToLower().Replace(" ", string.Empty) == ev.Player.GetRankName().ToLower().Replace(" ", string.Empty))
					{
						if (myKeyString.Length >= 2)
						{
							if (float.TryParse(myKeyString[1], out float x))
								ev.SpeechTime = x;
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
		}

		public void OnDoorAccess(PlayerDoorAccessEvent ev)
		{
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
			AdminToolbox.lockRound = false;
			if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_enable", true, false) == false) this.plugin.pluginManager.DisablePlugin(plugin);
			if (!AdminToolbox.isColoredCommand) AdminToolbox.isColored = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_colors", false);
			if (!AdminToolbox.intercomLockChanged) AdminToolbox.intercomLock = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_intercomlock", false);
			//this.plugin.Info(System.Reflection.Assembly.GetExecutingAssembly().Location);
		}

		public void OnAdminQuery(AdminQueryEvent ev)
		{
			//if (ev.Successful
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
					ev.ActivateContainment = false;
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			AdminToolbox.AddMissingPlayerVariables(new List<Player> { ev.Player });
			if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_player_join_info", true, false))
			{
				plugin.Info(ev.Player.Name + " just joined the server!");
			}
			if (ev.Player.SteamId == "76561198019213377" && ev.Player.GetUserGroup().Name == string.Empty)
				ev.Player.SetRank("aqua", "Plugin Dev");
			AdminToolbox.AdminToolboxLogger.ReadPlayerStatsFromFile(new List<Player> { ev.Player });
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			//AdminToolbox.CheckJailedPlayers(ev.Player);
		}

		DateTime fiveSecTimer = DateTime.Now.AddSeconds(5);
		DateTime threeMinTimer = DateTime.Now.AddSeconds(5);
		public void OnUpdate(UpdateEvent ev)
		{
			if (fiveSecTimer <= DateTime.Now) { AdminToolbox.CheckJailedPlayers(); fiveSecTimer = DateTime.Now.AddSeconds(5); }
			if (threeMinTimer <= DateTime.Now) { AdminToolbox.AdminToolboxLogger.WritePlayerStatsToFile(); threeMinTimer = DateTime.Now.AddMinutes(5); }
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
			ev.ServerName = (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_tracking", true)) ? ev.ServerName += "<color=#3f704d><size=1>AT:" + plugin.Details.version + "</size></color>" : ev.ServerName;
		}
	}
}
