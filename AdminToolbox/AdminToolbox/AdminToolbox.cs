using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox
{
	[PluginDetails(
		author = "Evan (AKA Rnen)",
		name = "Admin Toolbox",
		description = "Plugin for advanced admin tools",
		id = "rnen.admin.toolbox",
		version = ATversion,
		SmodMajor = 3,
		SmodMinor = 1,
		SmodRevision = 21
		)]
	public class AdminToolbox : Plugin
	{
		public const string ATversion = "1.3.7";

		public readonly static LogManager LogManager = new LogManager();
		public readonly static WarpManager.WarpManager WarpManager = new WarpManager.WarpManager();
		public readonly static JailManager JailManager = new JailManager();

		internal static bool 
			isRoundFinished = false, 
			isColored = false,
			isColoredCommand = false, 
			intercomLockChanged = false, 
			isStarting = true;
		public static bool lockRound = false, intercomLock = false;
		public static Dictionary<string, AdminToolboxPlayerSettings> ATPlayerDict { get; internal set; } = new Dictionary<string, AdminToolboxPlayerSettings>();
		public static Dictionary<string, Vector> warpVectors = new Dictionary<string, Vector>(WarpManager.ReadWarpsFromFile());

		public static Vector JailPos = (warpVectors.ContainsKey("jail")) ? warpVectors["jail"] : new Vector(53, 1020, -44);

		public static int RoundCount { get; internal set; } = 1;
		internal static string _roundStartTime;

		internal static AdminToolbox plugin;

		public class AdminToolboxPlayerSettings
		{
			public string SteamID { get; private set; } = "";
			public bool
				overwatchMode = false,
				godMode = false,
				dmgOff = false,
				destroyDoor = false,
				keepSettings = false,
				lockDown = false,
				instantKill = false,
				isJailed = false;
			public bool IsInsideJail
			{
				get
				{
					if (string.IsNullOrEmpty(this.SteamID))
						foreach (Player ply in plugin.Server.GetPlayers())
							if (ATPlayerDict.ContainsKey(ply.SteamId))
								ATPlayerDict[ply.SteamId].SteamID = ply.SteamId;
					Player player = plugin.Server.GetPlayers().Where(p => p.SteamId == SteamID).FirstOrDefault();
					if (player == null) return false;
					Vector jail = JailPos,
						pPos = player.GetPosition();
					float x = Math.Abs(pPos.x - jail.x),
						y = Math.Abs(pPos.y - jail.y),
						z = Math.Abs(pPos.z - jail.z);
					if (x > 7 || y > 5 || z > 7)
						return false;
					else
						return true;
				}
			}
			public int
				Kills = 0,
				TeamKills = 0,
				Deaths = 0,
				RoundsPlayed = 0,
				banCount = 0;
			internal int
				previousHealth = 100,
				prevAmmo5 = 0,
				prevAmmo7 = 0,
				prevAmmo9 = 0;
			public Vector DeathPos = Vector.Zero,
				originalPos = Vector.Zero;
			internal Role previousRole = Role.CLASSD;
			internal List<Smod2.API.Item> playerPrevInv = new List<Smod2.API.Item>();
			public DateTime JailedToTime { get; internal set; } = DateTime.Now;
			public DateTime JoinTime { get; internal set; } = DateTime.Now;
			public double MinutesPlayed { get; internal set; } = 1;

			public AdminToolboxPlayerSettings(string steamID)
			{
				this.SteamID = steamID;
			}
		}

		public override void OnDisable()
		{
			if (isColored)
				this.Info(this.Details.name + " v." + this.Details.version + " - @#fg=Red;Disabled@#fg=Default;");
			else
				this.Info(this.Details.name + " v." + this.Details.version + " - Disabled");
		}

		public override void OnEnable()
		{
			plugin = this;
			WriteVersionToFile();
			//CheckCurrVersion(this, this.Details.version);
			if (isColored)
				this.Info(this.Details.name + " v." + this.Details.version + " - @#fg=Green;Enabled@#fg=Default;");
			else
				this.Info(this.Details.name + " v." + this.Details.version + " - Enabled");
			_roundStartTime = DateTime.Now.Year.ToString() + "-" + ((DateTime.Now.Month >= 10) ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString())) + "-" + ((DateTime.Now.Day >= 10) ? DateTime.Now.Day.ToString() : ("0" + DateTime.Now.Day.ToString())) + " " + ((DateTime.Now.Hour >= 10) ? DateTime.Now.Hour.ToString() : ("0" + DateTime.Now.Hour.ToString())) + "." + ((DateTime.Now.Minute >= 10) ? DateTime.Now.Minute.ToString() : ("0" + DateTime.Now.Minute.ToString())) + "." + ((DateTime.Now.Second >= 10) ? DateTime.Now.Second.ToString() : ("0" + DateTime.Now.Second.ToString()));
			warpVectors = new Dictionary<string, Vector>(AdminToolbox.WarpManager.ReadWarpsFromFile());
			LogManager.WriteToLog(new string[] { "\"Plugin Started\"" }, LogManager.ServerLogType.Misc);
		}

		public override void Register()
		{
			#region EventHandlers Registering Eventhandlers
			// Register Events
			this.AddEventHandlers(new RoundEventHandler(this), Priority.Normal);
			this.AddEventHandler(typeof(IEventHandlerPlayerHurt), new DamageDetect(this), Priority.Normal);
			this.AddEventHandler(typeof(IEventHandlerPlayerDie), new DieDetect(this), Priority.Normal);
			this.AddEventHandlers(new MyMiscEvents(this), Priority.Normal);
			#endregion
			#region Commands Registering Commands
			// Register Commands
			this.AddCommands(new string[] { "spec", "spectator", "atoverwatch" }, new Command.SpectatorCommand());
			this.AddCommands(new string[] { "p", "player", "playerinfo", "pinfo" }, new Command.PlayerCommand());
			this.AddCommands(new string[] { "players", "playerlist", "plist" }, new Command.PlayerListCommand());
			this.AddCommands(new string[] { "atheal", "at-heal" }, new Command.HealCommand());
			this.AddCommands(new string[] { "atgod", "atgodmode", "at-god", "at-godmode" }, new Command.GodModeCommand());
			this.AddCommand("nodmg", new Command.NoDmgCommand());
			this.AddCommands(new string[] { "tut", "tutorial" }, new Command.TutorialCommand());
			this.AddCommand("role", new Command.RoleCommand());
			this.AddCommands(new string[] { "keep", "keepsettings" }, new Command.KeepSettingsCommand());
			this.AddCommands(new string[] { "athp", "atsethp", "at-hp", "at-sethp" }, new Command.SetHpCommand());
			this.AddCommand("pos", new Command.PosCommand());
			this.AddCommand("tpx", new Command.TeleportCommand());
			this.AddCommand("warp", new Command.WarpCommmand());
			this.AddCommand("warps", new Command.WarpsCommmand());
			this.AddCommands(new string[] { "roundlock", "lockround", "rlock", "lockr" }, new Command.RoundLockCommand(this));
			this.AddCommands(new string[] { "breakdoor", "bd", "breakdoors" }, new Command.BreakDoorsCommand());
			this.AddCommands(new string[] { "pl", "playerlockdown", "plock", "playerlock" }, new Command.LockdownCommand());
			this.AddCommand("atcolor", new Command.ATColorCommand(this));
			this.AddCommand("atdisable", new Command.ATDisableCommand(this));
			this.AddCommands(new string[] { "ik", "instakill", "instantkill" }, new Command.InstantKillCommand());
			this.AddCommands(new string[] { "j", "jail" }, new Command.JailCommand());
			this.AddCommands(new string[] { "il", "ilock", "INTERLOCK", "intercomlock" }, new Command.IntercomLockCommand(this));
			this.AddCommands(new string[] { "s", "server", "serverinfo" }, new Command.ServerCommand());
			this.AddCommands(new string[] { "e", "empty" }, new Command.EmptyCommand());
			this.AddCommands(new string[] { "atban","offlineban","oban" }, new Command.ATBanCommand(this));
			this.AddCommands(new string[] { "kill", "slay" }, new Command.KillCommand(this));
			this.AddCommands(new string[] { "speak" }, new Command.SpeakCommand());
			this.AddCommands(new string[] { "ghost", "ghostmode", "ghostm", "invisible", "gh" }, new Command.GhostCommand(this));

			#endregion
			#region Config Registering Config Entries
			// Register config settings
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_enable", true, Smod2.Config.SettingType.BOOL, true, "Enable/Disable AdminToolbox"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_colors", false, Smod2.Config.SettingType.BOOL, true, "Enable/Disable AdminToolbox colors in server window"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_tracking", true, Smod2.Config.SettingType.BOOL, true, "Appends the AdminToolbox version to your server name, this is for tracking how many servers are running the plugin"));

			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_tutorial_dmg_allowed", new int[] { -1 }, Smod2.Config.SettingType.NUMERIC_LIST, true, "What (int)damagetypes TUTORIAL is allowed to take"));

			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_damagetypes", new int[] { 5, 13, 14, 15, 16, 17 }, Smod2.Config.SettingType.NUMERIC_LIST, true, "What (int)damagetypes to debug"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_server", false, Smod2.Config.SettingType.BOOL, true, "Debugs damage dealt by server"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_spectator", false, Smod2.Config.SettingType.BOOL, true, "Debugs damage done to/by spectators"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_tutorial", false, Smod2.Config.SettingType.BOOL, true, "Debugs damage done to tutorial"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_player_damage", false, Smod2.Config.SettingType.BOOL, true, "Debugs damage to all players except teammates"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_friendly_damage", false, Smod2.Config.SettingType.BOOL, true, "Debugs damage to teammates"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_player_kill", false, Smod2.Config.SettingType.BOOL, true, "Debugs player kills except teamkills"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_friendly_kill", true, Smod2.Config.SettingType.BOOL, true, "Debugs team-kills"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_scp_and_self_killed", false, Smod2.Config.SettingType.BOOL, true, "Debug suicides and SCP kills"));

			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_endedRound_damagemultiplier", 1f, Smod2.Config.SettingType.FLOAT, true, "Damage multiplier after end of round"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_round_damagemultiplier", 1f, Smod2.Config.SettingType.FLOAT, true, "Damage multiplier"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_decontamination_damagemultiplier", 1f, Smod2.Config.SettingType.FLOAT, true, "Damage multiplier for the decontamination of LCZ"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_friendlyfire_damagemultiplier", 1f, Smod2.Config.SettingType.FLOAT, true, "Damage multiplier for friendly fire"));

			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_custom_nuke_cards", false, Smod2.Config.SettingType.BOOL, true, "Enables the use of custom keycards for the activation of the nuke"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_nuke_card_list", new int[] { 6, 9, 11 }, Smod2.Config.SettingType.NUMERIC_LIST, true, "List of all cards that can enable the nuke"));

			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_log_teamkills", false, Smod2.Config.SettingType.BOOL, true, "Writing logfiles for teamkills"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_log_kills", false, Smod2.Config.SettingType.BOOL, true, "Writing logfiles for regular kills"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_log_commands", false, Smod2.Config.SettingType.BOOL, true, "Writing logfiles for all AT command usage"));

			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_round_info", true, Smod2.Config.SettingType.BOOL, true, "Prints round count and player number on round start & end"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_player_join_info", true, Smod2.Config.SettingType.BOOL, true, "Writes player name in console on players joining"));

			//this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_intercom_whitelist", new string[] { string.Empty }, Smod2.Config.SettingType.LIST, true, "What ServerRank can use the Intercom to your specified settings"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_intercom_steamid_blacklist", new string[] { string.Empty }, Smod2.Config.SettingType.LIST, true, "Blacklist of steamID's that cannot use the intercom"));
			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_intercomlock", false, Smod2.Config.SettingType.BOOL, true, "If set to true, locks the command for all non-whitelist players"));

			this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_block_role_damage", new string[] { string.Empty }, Smod2.Config.SettingType.LIST, true, "What roles cannot attack other roles"));
			#endregion
		}

		internal static void AddMissingPlayerVariables(List<Player> players = null)
		{
			if (PluginManager.Manager.Server.GetPlayers().Count == 0) return;
			else if ( players == null || players.Count < 1) players = PluginManager.Manager.Server.GetPlayers();
			if (players.Count > 0)
				players.ForEach(p => { if(p != null) AddToPlayerDict(p); });
		}
		private static void AddToPlayerDict(Player player)
		{
			if (player != null && player is Player && !string.IsNullOrEmpty(player.SteamId) && !ATPlayerDict.ContainsKey(player.SteamId))
			{
				ATPlayerDict.Add(player.SteamId, new AdminToolboxPlayerSettings(player.SteamId));
			}
		}

		public static void WriteVersionToFile()
		{
			if (Directory.Exists(FileManager.GetAppFolder()))
			{
				string text = "at_version=" + plugin.Details.version;
				using (StreamWriter streamWriter = new StreamWriter(FileManager.GetAppFolder() + "at_version.md", false))
				{
					streamWriter.Write(text);
					streamWriter.Close();
				}
				if (File.Exists(FileManager.GetAppFolder() + "n_at_version.md"))
					File.Delete(FileManager.GetAppFolder() + "n_at_version.md");
			}
			else
				plugin.Info("Could not find SCP Secret Lab folder!");
		}
		internal static void CheckCurrVersion(AdminToolbox plugin, string version)
		{
			try
			{
				string host = "http://raw.githubusercontent.com/Rnen/AdminToolbox/master/version.md";
				if (!int.TryParse(version.Replace(".", string.Empty), out int currentVersion))
					plugin.Info("Coult not get Int16 from currentVersion");
				if (int.TryParse(new System.Net.WebClient().DownloadString(host).Replace(".", string.Empty).Replace("at_version=", string.Empty), out int onlineVersion))
				{
					if (onlineVersion > currentVersion)
						plugin.Info("Your version is out of date, please run the \"AT_AutoUpdate.bat\" or visit the AdminToolbox GitHub");
				}
				else
					plugin.Info("Could not get Int16 from onlineVersion");

			}
			catch (System.Exception e)
			{
				plugin.Error("Could not fetch latest version: " + e.Message);
			}
		}
	}

	internal static class LevenshteinDistance
	{
		/// <summary>
		/// Compute the distance between two strings.
		/// </summary>
		internal static int Compute(string s, string t)
		{
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			// Step 1
			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			// Step 2
			for (int i = 0; i <= n; d[i, 0] = i++)
			{
			}

			for (int j = 0; j <= m; d[0, j] = j++)
			{
			}

			// Step 3
			for (int i = 1; i <= n; i++)
			{
				//Step 4
				for (int j = 1; j <= m; j++)
				{
					// Step 5
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

					// Step 6
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
			// Step 7
			return d[n, m];
		}
	}
	public class GetPlayerFromString
	{
		public static Player GetPlayer(string args)
		{
			Player playerOut = null;
			if (short.TryParse(args, out short pID))
			{
				foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
					if (pl.PlayerId == pID)
						return pl;
			}
			else if (long.TryParse(args, out long sID))
			{
				foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
					if (pl.SteamId == sID.ToString())
						return pl;
			}
			else
			{
				//Takes a string and finds the closest player from the playerlist
				int maxNameLength = 31, LastnameDifference = 31;
				string str1 = args.ToLower();
				foreach (Player pl in PluginManager.Manager.Server.GetPlayers(str1))
				{
					if (!pl.Name.ToLower().Contains(args.ToLower()))
						continue;
					if (str1.Length < maxNameLength)
					{
						int x = maxNameLength - str1.Length;
						int y = maxNameLength - pl.Name.Length;
						string str2 = pl.Name;
						for (int i = 0; i < x; i++)
						{
							str1 += "z";
						}
						for (int i = 0; i < y; i++)
						{
							str2 += "z";
						}
						int nameDifference = LevenshteinDistance.Compute(str1, str2);
						if (nameDifference < LastnameDifference)
						{
							LastnameDifference = nameDifference;
							playerOut = pl;
						}
					}
				}
			}
			return playerOut;
		}
	}

	public class SetPlayerVariables
	{
		public static void SetPlayerBools(string steamID, bool? spectatorOnly = null, bool? godMode = null, bool? dmgOff = null, bool? destroyDoor = null, bool? keepSettings = null, bool? lockDown = null, bool? instantKill = null, bool? isJailed = null)
		{
			if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return;
			AdminToolbox.ATPlayerDict[steamID].overwatchMode = (spectatorOnly.HasValue) ? (bool)spectatorOnly : AdminToolbox.ATPlayerDict[steamID].overwatchMode;
			AdminToolbox.ATPlayerDict[steamID].godMode = (godMode.HasValue) ? (bool)godMode : AdminToolbox.ATPlayerDict[steamID].godMode;
			AdminToolbox.ATPlayerDict[steamID].dmgOff = (dmgOff.HasValue) ? (bool)dmgOff : AdminToolbox.ATPlayerDict[steamID].dmgOff;
			AdminToolbox.ATPlayerDict[steamID].destroyDoor = (destroyDoor.HasValue) ? (bool)destroyDoor : AdminToolbox.ATPlayerDict[steamID].destroyDoor;
			AdminToolbox.ATPlayerDict[steamID].lockDown = (lockDown.HasValue) ? (bool)lockDown : AdminToolbox.ATPlayerDict[steamID].lockDown;
			AdminToolbox.ATPlayerDict[steamID].instantKill = (instantKill.HasValue) ? (bool)instantKill : AdminToolbox.ATPlayerDict[steamID].instantKill;
			AdminToolbox.ATPlayerDict[steamID].isJailed = (isJailed.HasValue) ? (bool)isJailed : AdminToolbox.ATPlayerDict[steamID].isJailed;
		}
		public static void SetPlayerStats(string steamID, int? Kills = null, int? TeamKills = null, int? Deaths = null, int? RoundsPlayed = null, int? BanCount = null)
		{
			if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return;
			AdminToolbox.ATPlayerDict[steamID].Kills = (Kills.HasValue) ? (int)Kills : AdminToolbox.ATPlayerDict[steamID].Kills;
			AdminToolbox.ATPlayerDict[steamID].TeamKills = (TeamKills.HasValue) ? (int)TeamKills : AdminToolbox.ATPlayerDict[steamID].TeamKills; ;
			AdminToolbox.ATPlayerDict[steamID].Deaths = (Deaths.HasValue) ? (int)Deaths : AdminToolbox.ATPlayerDict[steamID].Deaths;
			AdminToolbox.ATPlayerDict[steamID].RoundsPlayed = (RoundsPlayed.HasValue) ? (int)RoundsPlayed : AdminToolbox.ATPlayerDict[steamID].RoundsPlayed;
			AdminToolbox.ATPlayerDict[steamID].banCount = (BanCount.HasValue) ? (int)BanCount : AdminToolbox.ATPlayerDict[steamID].banCount;
		}
	}
}