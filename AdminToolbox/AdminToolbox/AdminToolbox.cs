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
	/// <summary>
	/// The <see cref="AdminToolbox"/> <see cref="Plugin"/>!
	/// </summary>
	[PluginDetails(
		author = "Evan (AKA Rnen)",
		name = "Admin Toolbox",
		description = "Plugin for advanced admin tools",
		id = "rnen.admin.toolbox",
		version = ATversion,
		SmodMajor = 3,
		SmodMinor = 1,
		SmodRevision = 22
		)]
	public class AdminToolbox : Plugin
	{
		/// <summary>
		/// <see cref="AdminToolbox"/> version
		/// </summary>
		public const string ATversion = "1.3.7";

		/// <summary>
		/// <see cref="AdminToolbox"/>s instance of <see cref="Managers.LogManager"/>
		/// </summary>
		public static readonly Managers.LogManager logManager = new Managers.LogManager();
		
		/// <summary>
		/// <see cref="AdminToolbox"/>s instance of <see cref="Managers.WarpManager"/>
		/// </summary>
		public static readonly Managers.WarpManager warpManager = new Managers.WarpManager();
		
		/// <summary>
		/// <see cref="AdminToolbox"/>s instance of <see cref="Managers.ATFileManager"/>
		/// </summary>
		public static readonly Managers.ATFileManager atfileManager = new Managers.ATFileManager();

		internal static bool 
			isRoundFinished = false, 
			isColored = false,
			isColoredCommand = false, 
			intercomLockChanged = false, 
			isStarting = true;
		public static bool 
			lockRound = false, 
			intercomLock = false;

		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/> of <see cref ="API.PlayerSettings"/> containing <see cref="AdminT"/> settings on all players. Uses <see cref="Player.SteamId"/> as KEY
		/// </summary>
		public static Dictionary<string, API.PlayerSettings> ATPlayerDict { get; internal set; } = new Dictionary<string, API.PlayerSettings>();
		
		/// <summary>
		/// <see cref ="Dictionary{TKey, TValue}"/> of all current warp vectors
		/// </summary>
		public static Dictionary<string, Vector> warpVectors = new Dictionary<string, Vector>(warpManager.ReadWarpsFromFile());

		internal static Vector JailPos => (warpVectors.ContainsKey("jail")) ? warpVectors["jail"] : new Vector(53, 1020, -44);

		/// <summary>
		/// <see cref="AdminToolbox"/> round count
		/// </summary>
		public static int RoundCount { get; internal set; } = 1;
		internal static string _roundStartTime;
		
		internal static AdminToolbox plugin;

		public override void OnDisable()
			=> this.Info(this.Details.name + " v." + this.Details.version + (isColored ? " - @#fg=Red;Disabled@#fg=Default;" : " - Disabled"));

		public override void OnEnable()
		{
			plugin = this;
			Managers.ATFileManager.WriteVersionToFile();
			//CheckCurrVersion(this, this.Details.version);
			this.Info(this.Details.name + " v." + this.Details.version + (isColored ? " - @#fg=Green;Enabled@#fg=Default;" : " - Enabled"));
			_roundStartTime = DateTime.Now.Year.ToString() + "-" + ((DateTime.Now.Month >= 10) ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString())) + "-" + ((DateTime.Now.Day >= 10) ? DateTime.Now.Day.ToString() : ("0" + DateTime.Now.Day.ToString())) + " " + ((DateTime.Now.Hour >= 10) ? DateTime.Now.Hour.ToString() : ("0" + DateTime.Now.Hour.ToString())) + "." + ((DateTime.Now.Minute >= 10) ? DateTime.Now.Minute.ToString() : ("0" + DateTime.Now.Minute.ToString())) + "." + ((DateTime.Now.Second >= 10) ? DateTime.Now.Second.ToString() : ("0" + DateTime.Now.Second.ToString()));
			AdminToolbox.warpManager.RefreshWarps();
			logManager.WriteToLog(new string[] { "\"Plugin Started\"" }, Managers.LogManager.ServerLogType.Misc);
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

		internal static void AddMissingPlayerVariables()
		{
			if (PluginManager.Manager.Server.GetPlayers().Count == 0) return;
			AddMissingPlayerVariables(PluginManager.Manager.Server.GetPlayers());
		}
		internal static void AddMissingPlayerVariables(List<Player> players)
		{
			if (players == null || players.Count < 1) players = PluginManager.Manager.Server.GetPlayers();
			if (players.Count > 0)
				players.ForEach(p => { if(p != null) AddToPlayerDict(p); });
		}
		private static void AddToPlayerDict(Player player)
		{
			if (player != null && player is Player && !string.IsNullOrEmpty(player.SteamId) && !ATPlayerDict.ContainsKey(player.SteamId))
			{
				ATPlayerDict.Add(player.SteamId, new API.PlayerSettings(player.SteamId));
			}
		}

		internal static void CheckCurrVersion()
		{
			//Un-used code
			try
			{
				string host = "http://raw.githubusercontent.com/Rnen/AdminToolbox/master/version.md";
				if (!int.TryParse(AdminToolbox.plugin.Details.version.Replace(".", string.Empty), out int currentVersion))
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
}