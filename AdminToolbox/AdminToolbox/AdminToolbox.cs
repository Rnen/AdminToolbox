using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System;
using System.Collections.Generic;
using System.Linq;
using Smod2.Config;

namespace AdminToolbox
{
	using Command;
	using API;
	using Managers;
	using API.Extentions;

	/// <summary>
	/// The <see cref="AdminToolbox"/> <see cref="Plugin"/> main class
	/// </summary>
	[PluginDetails(
		author = "Evan (AKA Rnen)",
		name = "Admin Toolbox",
		description = "Plugin for advanced admin tools",
		id = "rnen.admin.toolbox",
		version = "1.3.8-6",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
		)]
	public class AdminToolbox : Plugin
	{
		internal const string AssemblyInfoVersion = "1.3.8.6";

		#region GitHub release info
		private DateTime LastOnlineCheck = DateTime.Now;
		private ATWeb.AT_LatestReleaseInfo LatestReleaseInfo;

		internal static List<WaitForTeleport> waitForTeleports = new List<WaitForTeleport>();

		internal ATWeb.AT_LatestReleaseInfo GetGitReleaseInfo()
		{
			if (LastOnlineCheck.AddMinutes(5) < DateTime.Now || LatestReleaseInfo == null)
			{
				LatestReleaseInfo = ATWeb.GetOnlineInfo(this);
				LastOnlineCheck = DateTime.Now;
			}
			return LatestReleaseInfo;
		}
		#endregion

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		public List<ScheduledCommandCall> scheduledCommands;
		public ScheduledRestart scheduledRestart;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		/// <summary>
		/// <see cref="AdminToolbox"/>s instance of <see cref="LogManager"/>
		/// </summary>
		public static readonly LogManager logManager = new LogManager();

		/// <summary>
		/// <see cref="AdminToolbox"/>s instance of <see cref="WarpManager"/>
		/// </summary>
		public static readonly WarpManager warpManager = new WarpManager();

		/// <summary>
		/// <see cref="AdminToolbox"/>s instance of <see cref="ATFileManager"/>
		/// </summary>
		public static readonly ATFileManager atfileManager = new ATFileManager();

		internal static bool roundStatsRecorded = false;
		internal static readonly ATRoundStats roundStats = new ATRoundStats();

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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

#if DEBUG
		public static bool DebugMode { get; internal set; } = true;
#else
		public static bool DebugMode { get; internal set; } = false;
#endif
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/> of <see cref ="API.PlayerSettings"/> containing <see cref="AdminToolbox"/> settings on all players. Uses <see cref="Player.SteamId"/> as KEY
		/// </summary>
		public static Dictionary<string, PlayerSettings> ATPlayerDict { get; internal set; } = new Dictionary<string, PlayerSettings>();

		/// <summary>
		/// <see cref ="Dictionary{TKey, TValue}"/> of all current warp vectors
		/// </summary>
		public static Dictionary<string, WarpPoint> WarpVectorDict = new Dictionary<string, WarpPoint>(warpManager.presetWarps);

		/// <summary>
		/// <see cref="AdminToolbox"/> round count
		/// </summary>
		public static int RoundCount { get; internal set; } = 0;

		internal static AdminToolbox plugin;

		/// <summary>
		/// Called when <see cref="AdminToolbox"/> gets disabled
		/// </summary>
		public override void OnDisable()
			=> this.Info(this.Details.name + " v." + this.Details.version + (isColored ? " - @#fg=Red;Disabled@#fg=Default;" : " - Disabled"));

		/// <summary>
		/// Called when <see cref="AdminToolbox"/> gets enabled
		/// </summary>
		public override void OnEnable()
		{
			plugin = this;
			ATFileManager.WriteVersionToFile();
			this.Info(this.Details.name + " v." + this.Details.version + (isColored ? " - @#fg=Green;Enabled@#fg=Default;" : " - Enabled"));
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
			this.AddEventHandlers(new RoundEventHandler(this), Priority.Normal);
			this.AddEventHandler(typeof(IEventHandlerPlayerHurt), new DamageDetect(this), Priority.Normal);
			this.AddEventHandler(typeof(IEventHandlerPlayerDie), new DieDetect(this), Priority.Normal);
			this.AddEventHandlers(new MyMiscEvents(this), Priority.Normal);
			this.AddEventHandler(typeof(IEventHandlerCheckRoundEnd), new LateOnCheckRoundEndEvent(this), Priority.Highest);
			this.AddEventHandler(typeof(IEventHandlerCheckEscape), new LateEscapeEventCheck(), Priority.Highest);
		}
		internal void UnRegisterEvents() => EventManager.RemoveEventHandlers(this);
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
			this.AddCommands(GhostCommand.CommandAliases, new GhostCommand(this));
			this.AddCommands(AT_HelpCommand.CommandAliases, new AT_HelpCommand());
			this.AddCommands(ATCommand.CommandAliases, new ATCommand(this));
			this.AddCommands(ServerStatsCommand.CommandAliases, new ServerStatsCommand(this));
			this.AddCommands(LockDoorsCommand.CommandAliases, new LockDoorsCommand(this));
			this.AddCommands(RespawnLockCommand.CommandAliases, new RespawnLockCommand());
			//this.AddCommands(new string[] { "timedrestart", "trestart" }, new Command.TimedCommand(this));
		}
		internal void UnRegisterCommands() => PluginManager.CommandManager.UnregisterCommands(this);//this.AddCommands(new string[] { "at", "admintoolbox", "atb", "a-t", "admin-toolbox", "admin_toolbox" }, new ATCommand(this));
		internal void RegisterConfigs()
		{
			#region Core-configs
			this.AddConfig(new ConfigSetting("admintoolbox_enable", true, true, "Enable/Disable AdminToolbox"));
			this.AddConfig(new ConfigSetting("admintoolbox_colors", false, true, "Enable/Disable AdminToolbox colors in server window"));
			this.AddConfig(new ConfigSetting("admintoolbox_tracking", true, true, "Appends the AdminToolbox version to your server name, this is for tracking how many servers are running the plugin"));
			#endregion

			this.AddConfig(new ConfigSetting("admintoolbox_tutorial_dmg_allowed", new int[] { -1 }, true, "What (int)damagetypes TUTORIAL is allowed to take"));

			#region Debug
			this.AddConfig(new ConfigSetting("admintoolbox_debug_damagetypes", new int[] { 5, 13, 14, 15, 16, 17 }, true, "What (int)damagetypes to debug"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_server", false, true, "Debugs damage dealt by server"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_spectator", false, true, "Debugs damage done to/by spectators"));
			this.AddConfig(new ConfigSetting("admintoolbox_debug_tutorial", false, true, "Debugs damage done to tutorial"));
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
			this.AddConfig(new ConfigSetting("admintoolbox_nuke_card_list", new int[] { 6, 9, 11 }, true, "List of all cards that can enable the nuke"));
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
			this.AddConfig(new ConfigSetting("admintoolbox_intercom_steamid_blacklist", new string[0], true, "Blacklist of steamID's that cannot use the intercom"));
			this.AddConfig(new ConfigSetting("admintoolbox_intercomlock", false, true, "If set to true, locks the command for all non-whitelist players"));
			#endregion

			this.AddConfig(new ConfigSetting("admintoolbox_block_role_damage", new string[0], true, "What roles cannot attack other roles"));

			this.AddConfig(new ConfigSetting("admintoolbox_ban_webhooks", new string[0], true, "Links to channel webhooks for bans"));
			//this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_timedrestart_automessages", new string[] { "" }, Smod2.Config.SettingType.LIST, true, ""));
			//this.AddConfig(new Smod2.Config.ConfigSetting("atb_timedrestart_automessages", new string[] { "" }, Smod2.Config.SettingType.LIST, true, ""));

		}
		internal void UnRegisterConfigs()
		{

		}


		internal static void AddMissingPlayerVariables()
		{
			if (PluginManager.Manager.Server.GetPlayers().Count == 0) return;
			AddMissingPlayerVariables(PluginManager.Manager.Server.GetPlayers());
		}
		internal static void AddMissingPlayerVariables(Player player) 
			=> AddMissingPlayerVariables(new List<Player>() { player });
		internal static void AddMissingPlayerVariables(List<Player> players) 
			=> AddMissingPlayerVariables(players.ToArray());
		internal static void AddMissingPlayerVariables(Player[] players)
		{
			Player[] allPlayers = PluginManager.Manager.Server.GetPlayers().ToArray();
			if (allPlayers.Length == 0)
			{
				return;
			}
			else if (players == null || players.Length < 1)
			{
				players = allPlayers;
			}
			if (players.Length > 0)
			{
				foreach (Player player in players)
				{
					if (player != null && !string.IsNullOrEmpty(player.SteamId))
					{
						AddToPlayerDict(player);
					}
				}
			}
		}
		private static void AddToPlayerDict(Player player)
		{
			if (player != null && player is Player p &&
				!string.IsNullOrEmpty(p.SteamId) && !ATPlayerDict.ContainsKey(p.SteamId))
			{
				ATPlayerDict.Add(p.SteamId, new PlayerSettings(p.SteamId));
			}
		}

		/// <summary>
		/// Debugs messages in <see cref="AdminToolbox"/> when DEBUG is defined
		/// </summary>
		public new void Debug(string message)
		{
			if (DebugMode)
				this.Info(message);
		}
	}

}
