using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.API;
using System;
using System.Collections.Generic;

namespace AdminToolbox
{
	[PluginDetails(
		author = "Evan (AKA Rnen)",
		name = "Admin Toolbox",
		description = "Plugin for advanced admin tools",
		id = "rnen.admin.toolbox",
		version = "1.0",
		SmodMajor = 2,
		SmodMinor = 0,
		SmodRevision = 0
		)]
	class AdminToolbox : Plugin
	{
        public static bool isRoundFinished = false;
        public static bool evanSpectator_onRespawn = false;
        public static bool adminMode = false;

        public static bool debugFriendlyKill;
        public static bool debugPlayerKill;

        public static int[] nineTailsTeam = { 1, 3 };
        public static int[] chaosTeam = { 2, 4 };

        public static Dictionary<string, List<bool>> playerdict = new Dictionary<string, List<bool>>();


        //public static Vector[] positionVector = { ) };

        public static int roundCount = 0;

        public static string[] adminSteamID = { "76561198019213377", "76561198038462200" };

        //private float playedTime;

		public override void OnDisable()
		{
            
		}
        public static void SetPlayerBools(Player player, bool keepSettings, bool godMode, bool dmgOff)
        {
            playerdict[player.SteamId][0] = keepSettings;
            playerdict[player.SteamId][1] = godMode;
            playerdict[player.SteamId][2] = dmgOff;
        }

		public override void OnEnable()
		{
			this.Info(this.Details.name + " loaded sucessfully");

            debugFriendlyKill = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_kill", true);
            debugPlayerKill = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_player_kill", true);

        }
        void Update()
        {
        }

        public override void Register()
		{
            // Register Events
            this.AddEventHandler(typeof(IEventRoundStart), new RoundHandler(this), Priority.High);
            this.AddEventHandler(typeof(IEventRoundEnd), new RoundHandler(this), Priority.High);
            this.AddEventHandler(typeof(IEventPlayerHurt), new DamageDetect(this), Priority.High);
            this.AddEventHandler(typeof(IEventPlayerDie), new DieDetect(this), Priority.High);
            this.AddEventHandler(typeof(IEventAdminQuery), new AdminQuery(this), Priority.High);
            this.AddEventHandler(typeof(IEventAuthCheck), new AuthCheck(this), Priority.High);
            this.AddEventHandler(typeof(IEventPlayerJoin), new PlayerJoinHandler(), Priority.Highest);
            this.AddEventHandler(typeof(IEventPlayerLeave), new PlayerLeaveHandler(), Priority.Highest);
            //this.AddEventHandler(typeof(IEventAssignTeam), new TeamAssignHandler(this), Priority.High);
            //this.AddEventHandler(typeof(IEventTeamRespawn), new TeamRespawnHandler(this), Priority.High);

            // Register Commands
            //this.AddCommand("evan079", new MyCustomCommand(this));
            //this.AddCommand("spectator", new Command.SetToSpectatorCommand(this));
            this.AddCommand("players", new Command.PlayerList(this));
            this.AddCommand("tpx", new Command.TeleportCommand(this));
            this.AddCommand("heal", new Command.HealCommand(this));
            this.AddCommand("god", new Command.GodModeCommand(this));
            this.AddCommand("nodmg", new Command.NoDmgCommand(this));
            this.AddCommand("tut", new Command.SetTutorial(this));
            this.AddCommand("tutorial", new Command.SetTutorial(this));
            this.AddCommand("class", new Command.SetPlayerClass(this));
            this.AddCommand("keep", new Command.KeepSettings(this));
            this.AddCommand("keepsettings", new Command.KeepSettings(this));
            this.AddCommand("hp", new Command.SetHpCommand(this));
            this.AddCommand("sethp", new Command.SetHpCommand(this));
            // Register config settings
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_tutorial_dmg_allowed", new int[] { -1 }, Smod2.Config.SettingType.NUMERIC_LIST, true, "What (int)damagetypes TUTORIAL is allowed"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_damagetypes", new int[] { 5, 13, 14, 15, 16, 17 }, Smod2.Config.SettingType.NUMERIC_LIST, true, "What (int)damagetypes to debug"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_server", false, Smod2.Config.SettingType.BOOL, true, "true/false"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_spectator", false, Smod2.Config.SettingType.BOOL, true, "true/false"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_tutorial", false, Smod2.Config.SettingType.BOOL, true, "true/false"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_player_damage", false, Smod2.Config.SettingType.BOOL, true, "true/false"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_friendly_damage", false, Smod2.Config.SettingType.BOOL, true, "true/false"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_player_kill", false, Smod2.Config.SettingType.BOOL, true, "true/false"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_friendly_kill", true, Smod2.Config.SettingType.BOOL, true, "true/false"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_scp_and_self_killed", false, Smod2.Config.SettingType.BOOL, true, "true/false"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_endedRound_damageMultiplier", 1, Smod2.Config.SettingType.NUMERIC, true, "Damage multiplier after end of round"));
        }
    }
    public static class LevenshteinDistance
    {
        public static int intDifferenceTolerance = 2;
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
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
        public static Player GetPlayer(string args, out Player playerOut)
        {
            int maxNameLength = 31;
            int LastnameDifference = 31;
            Player plyer = null;
            string str1 = args.ToLower();
            foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
            {
                if (!pl.Name.ToLower().Contains(args.ToLower())) { goto NoPlayer; }
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
                        plyer = pl;
                    }
                }
                NoPlayer:;
            }
            playerOut = plyer;
            return playerOut;
        }
    }
}
