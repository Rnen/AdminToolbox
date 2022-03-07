using System;
using System.Collections.Generic;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.EventHandlers;
using Smod2.Events;

namespace AdminToolbox
{
	using API;
	using Command;
	using Managers;

	/// <summary>
	/// The <see cref="AdminToolbox"/> <see cref="Plugin"/> main class
	/// </summary>
	[PluginDetails(
		author = "Evan (AKA Rnen)",
		name = "Admin Toolbox",
		description = "Plugin for advanced admin tools",
		id = "rnen.admin.toolbox",
		version = AT_Version + "-" + AT_Revision, 
		SmodMajor = 3,
		SmodMinor = 10,
		SmodRevision = 0
		)]
	public class AdminToolbox : Plugin
	{
		internal const string AT_Version = "1.3.8";
		internal const string AT_Revision = "24";
		internal const string SModLetter = "";

		internal static List<WaitForTeleport> waitForTeleports = new List<WaitForTeleport>();

		/// <summary>
		/// The plugin's instance of <see cref="Managers.LogManager"/>
		/// </summary>
		public static readonly LogManager LogManager = new LogManager();

		/// <summary>
		/// The plugin's instance instance of <see cref="Managers.WarpManager"/>
		/// </summary>
		public static readonly WarpManager WarpManager = new WarpManager();

		/// <summary>
		/// The plugin's instance instance of <see cref="Managers.ATFile"/>
		/// </summary>
		public static readonly ATFile FileManager = new ATFile();

		internal static bool roundStatsRecorded = false;
		internal static readonly RoundStats RoundStats = new RoundStats();

		internal static bool
			isRoundFinished = false,
			isColored = false,
			isColoredCommand = false,
			intercomLockChanged = false,
			isStarting = true;
		public static bool
			lockRound = false,
			intercomLock = false,
			respawnLock = false;

		/// <summary>
		/// <see cref="AdminToolbox"/> debug mode
		/// </summary>
		public static bool DebugMode { get; internal set; }
#if DEBUG
			= true;
#else
			= false;
#endif

		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/> of <see cref ="PlayerSettings"/> containing the plugin's settings on all players. Uses <see cref="Player.UserID"/> as KEY
		/// </summary>
		public static Dictionary<string, PlayerSettings> ATPlayerDict { get; internal set; } = new Dictionary<string, PlayerSettings>();

		/// <summary>
		/// <see cref ="Dictionary{TKey, TValue}"/> of all current warp vectors
		/// </summary>
		public static Dictionary<string, WarpPoint> WarpVectorDict = new Dictionary<string, WarpPoint>(WarpManager.presetWarps);

		/// <summary>
		/// <see cref="AdminToolbox"/> round count
		/// </summary>
		public static int RoundCount { get; internal set; } = 0;

		internal static AdminToolbox singleton;

		/// <summary>
		/// Called when <see cref="AdminToolbox"/> gets disabled
		/// </summary>
		public override void OnDisable()
		{
			UnRegisterCommands();
			UnRegisterEvents();
			Debug(this.Details.name + " v." + this.Details.version + (isColored ? " - @#fg=Red;Disabled@#fg=Default;" : " - Disabled"));
		}

		/// <summary>
		/// Called when <see cref="AdminToolbox"/> gets enabled
		/// </summary>
		public override void OnEnable()
		{
			singleton = this;
			ATFile.WriteVersionToFile();
			Debug(this.Details.name + " v." + this.Details.version + (isColored ? " - @#fg=Green;Enabled@#fg=Default;" : " - Enabled"));
		}

		/// <summary>
		/// Called when <see cref="AdminToolbox"/> registers its configs, commands and events
		/// </summary>
		public override void Register()
		{
			this.RegisterEvents();
			this.RegisterCommands();
			this.RegisterConfigs();
		}
		internal void RegisterEvents()
		{
			this.AddEventHandlers(new RoundEventHandler(this));
			this.AddEventHandler(typeof(IEventHandlerPlayerHurt), new PlayerDamageEvent(this));
			this.AddEventHandler(typeof(IEventHandlerPlayerDie), new PlayerDieEvent(this));
			this.AddEventHandlers(new MyMiscEvents(this));
			this.AddEventHandler(typeof(IEventHandlerCheckRoundEnd), new LateOnCheckRoundEndEvent(), Priority.LAST);
			this.AddEventHandler(typeof(IEventHandlerCheckEscape), new LateEscapeEventCheck(), Priority.LAST);
		}
		internal void UnRegisterEvents() => EventManager.Manager.RemoveEventHandlers(this);
		internal void RegisterCommands()
		{
			this.AddCommands(SpectatorCommand.CommandAliases, new SpectatorCommand());
			this.AddCommands(PlayerCommand.CommandAliases, new PlayerCommand());
			this.AddCommands(PlayerListCommand.CommandAliases, new PlayerListCommand());
			this.AddCommands(HealCommand.CommandAliases, new HealCommand());
			this.AddCommands(GodModeCommand.CommandAliases, new GodModeCommand());
			this.AddCommands(NoDmgCommand.CommandAliases, new NoDmgCommand());
			this.AddCommands(TutorialCommand.CommandAliases, new TutorialCommand());
			this.AddCommands(RoleCommand.CommandAliases, new RoleCommand());
			this.AddCommands(KeepSettingsCommand.CommandAliases, new KeepSettingsCommand());
			this.AddCommands(SetHpCommand.CommandAliases, new SetHpCommand());
			this.AddCommands(PosCommand.CommandAliases, new PosCommand());
			this.AddCommands(TeleportCommand.CommandAliases, new TeleportCommand());
			this.AddCommands(WarpCommmand.CommandAliases, new WarpCommmand());
			this.AddCommands(WarpsCommmand.CommandAliases, new WarpsCommmand());
			this.AddCommands(RoundLockCommand.CommandAliases, new RoundLockCommand(this));
			this.AddCommands(BreakDoorsCommand.CommandAliases, new BreakDoorsCommand());
			this.AddCommands(LockdownCommand.CommandAliases, new LockdownCommand());
			this.AddCommands(ATColorCommand.CommandAliases, new ATColorCommand(this));
			this.AddCommands(ATDisableCommand.CommandAliases, new ATDisableCommand(this));
			this.AddCommands(InstantKillCommand.CommandAliases, new InstantKillCommand());
			this.AddCommands(JailCommand.CommandAliases, new JailCommand());
			this.AddCommands(IntercomLockCommand.CommandAliases, new IntercomLockCommand(this));
			this.AddCommands(ServerCommand.CommandAliases, new ServerCommand());
			this.AddCommands(EmptyCommand.CommandAliases, new EmptyCommand());
			this.AddCommands(ATBanCommand.CommandAliases, new ATBanCommand(this));
			this.AddCommands(KillCommand.CommandAliases, new KillCommand(this));
			this.AddCommands(SpeakCommand.CommandAliases, new SpeakCommand());
			this.AddCommands(GhostCommand.CommandAliases, new GhostCommand());
			this.AddCommands(AT_HelpCommand.CommandAliases, new AT_HelpCommand());
			this.AddCommands(ATCommand.CommandAliases, new ATCommand(this));
			this.AddCommands(ServerStatsCommand.CommandAliases, new ServerStatsCommand());
			this.AddCommands(LockDoorsCommand.CommandAliases, new LockDoorsCommand());
			this.AddCommands(RespawnLockCommand.CommandAliases, new RespawnLockCommand());
			this.AddCommands(ClosestDoorCommand.CommandAliases, new ClosestDoorCommand());
			this.AddCommands(GrenadeModeCommand.CommandAliases, new GrenadeModeCommand());
			this.AddCommands(InfiniteItemCommand.CommandAliases, new InfiniteItemCommand());
		}
		internal void UnRegisterCommands() => PluginManager.CommandManager.UnregisterCommands(this);
		internal void RegisterConfigs()
		{
			#region Core-configs
			this.AddConfig(new ConfigSetting("admintoolbox_enable", true, true, "Enable/Disable AdminToolbox"));
			this.AddConfig(new ConfigSetting("admintoolbox_colors", false, true, "Enable/Disable AdminToolbox colors in server window"));
			this.AddConfig(new ConfigSetting("admintoolbox_tracking", true, true, "Appends the AdminToolbox version to your server name, this is for tracking how many servers are running the plugin"));
			#endregion

			this.AddConfig(new ConfigSetting("admintoolbox_tutorial_dmg_allowed", new int[] { -1 }, true, "What (int)damagetypes TUTORIAL is allowed to take"));

			#region Debug
			this.AddConfig(new ConfigSetting("admintoolbox_debug_damagetypes", Utility.HumanDamageTypes, true, "What (int)damagetypes to debug"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_server_damage", false, true, "Debugs damage dealt by server"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_spectator_damage", false, true, "Debugs damage done to/by spectators"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_tutorial_damage", false, true, "Debugs damage done to tutorial"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_player_damage", false, true, "Debugs damage to all players except teammates"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_friendly_damage", false, true, "Debugs damage to teammates"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_player_kill", false, true, "Debugs player kills except teamkills"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_friendly_kill", true, true, "Debugs team-kills"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_scp_and_self_killed", false, true, "Debug suicides and SCP kills"));
			#endregion

			#region DamageMultipliers
			this.AddConfig(new ConfigSetting("admintoolbox_endedRound_damagemultiplier", 1f, true, "Damage multiplier after end of round"));
			this.AddConfig(new ConfigSetting("admintoolbox_round_damagemultiplier", 1f, true, "Damage multiplier"));
			this.AddConfig(new ConfigSetting("admintoolbox_decontamination_damagemultiplier", 1f, true, "Damage multiplier for the decontamination of LCZ"));
			this.AddConfig(new ConfigSetting("admintoolbox_friendlyfire_damagemultiplier", 1f, true, "Damage multiplier for friendly fire"));
			this.AddConfig(new ConfigSetting("admintoolbox_pocketdimention_damagemultiplier", 1f, true, "Damage multiplier for pocket dimention damage"));
			#endregion
			#region Cards
			this.AddConfig(new ConfigSetting("admintoolbox_custom_nuke_cards", false, true, "Enables the use of custom keycards for the activation of the nuke"));
			this.AddConfig(new ConfigSetting("admintoolbox_nuke_card_list", new int[] { (int)ItemType.KeycardContainmentEngineer, (int)ItemType.KeycardFacilityManager, (int)ItemType.KeycardO5 }, true, "List of all cards that can enable the nuke"));
			#endregion
			#region Log-Stuff
			this.AddConfig(new ConfigSetting("admintoolbox_log_teamkills", false, true, "Writing logfiles for teamkills"));
			this.AddConfig(new ConfigSetting("admintoolbox_log_kills", false, true, "Writing logfiles for regular kills"));
			this.AddConfig(new ConfigSetting("admintoolbox_log_commands", false, true, "Writing logfiles for all AT command usage"));

			this.AddConfig(new ConfigSetting("admintoolbox_round_info", true, true, "Prints round count and player number on round start & end"));
			this.AddConfig(new ConfigSetting("admintoolbox_player_join_info", true, true, "Writes player name in console on players joining"));
			#endregion
			#region Intercom
			//this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_intercom_whitelist", new string[] { string.Empty }, Smod2.Config.SettingType.LIST, true, "What ServerRank can use the Intercom to your specified settings"));
			this.AddConfig(new ConfigSetting("admintoolbox_intercom_UserID_blacklist", new string[0], true, "Blacklist of UserID's that cannot use the intercom"));
			this.AddConfig(new ConfigSetting("admintoolbox_intercomlock", false, true, "If set to true, locks the command for all non-whitelist players"));
			#endregion

			this.AddConfig(new ConfigSetting("admintoolbox_block_role_damage", new string[0], true, "What roles cannot attack other roles"));

			this.AddConfig(new ConfigSetting("admintoolbox_ban_webhooks", new string[0], true, "Links to channel webhooks for bans"));
		}

		/// <summary>
		/// Debugs messages when <see cref="DebugMode"/> is enabled
		/// </summary>
		public new void Debug(string message)
		{
			if (DebugMode)
				this.Info(message);
		}

	}

}
