using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using ServerMod2.API;
using System;
using System.IO;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using System.Linq;

namespace AdminToolbox
{
    [PluginDetails(
        author = "Evan (AKA Rnen)",
        name = "Admin Toolbox",
        description = "Plugin for advanced admin tools",
        id = "rnen.admin.toolbox",
        version = "1.3.2",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 8
        )]
    class AdminToolbox : Plugin
    {
        public static bool isRoundFinished = false, lockRound = false, isColored = false, isColoredCommand = false;

        public static Dictionary<string, List<bool>> playerdict = new Dictionary<string, List<bool>>();
        public static Dictionary<string, List<int>> playerStats = new Dictionary<string, List<int>>();
        public static Dictionary<string, Vector> warpVectors = new Dictionary<string, Vector>();

        public static int roundCount = 0;
        public static LogHandlers AdminToolboxLogger = new LogHandlers();
        public static string _roundStartTime;

        public override void OnDisable()
        {
            if(isColored)
                this.Info(this.Details.name + " v." + this.Details.version + " - @#fg=Red;Disabled@#fg=Default;");
            else
                this.Info(this.Details.name + " v." + this.Details.version + " - Disabled");
        }
        public static void SetPlayerBools(Player player, bool spectatorOnly, bool godMode, bool dmgOff, bool destroyDoor, bool lockDown, bool instantKill)
        {
            //This is actually never used, its just for keeping track, might become an all on/off switch at some point
            playerdict[player.SteamId][0] = spectatorOnly;
            playerdict[player.SteamId][1] = godMode;
            playerdict[player.SteamId][2] = dmgOff;
            playerdict[player.SteamId][3] = destroyDoor;
            //playerdict[player.SteamId[4] = keepSettings;
            playerdict[player.SteamId][5] = lockDown;
            playerdict[player.SteamId][6] = instantKill;
        }
        public static void SetPlayerStats(Player player, int Kills, int TeamKills, int Deaths, int RoundsPlayed)
        {
            playerStats[player.SteamId][0] = Kills;
            playerStats[player.SteamId][1] = TeamKills;
            playerStats[player.SteamId][2] = Deaths;
            playerStats[player.SteamId][3] = RoundsPlayed;
        }

        public override void OnEnable()
        {
            if(isColored)
                this.Info(this.Details.name + " v." + this.Details.version + " - @#fg=Green;Enabled@#fg=Default;");
            else
                this.Info(this.Details.name + " v." + this.Details.version + " - Enabled");
            _roundStartTime = DateTime.Now.Year.ToString() + "-" + ((DateTime.Now.Month >= 10) ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString())) + "-" + ((DateTime.Now.Day >= 10) ? DateTime.Now.Day.ToString() : ("0" + DateTime.Now.Day.ToString())) + " " + ((DateTime.Now.Hour >= 10) ? DateTime.Now.Hour.ToString() : ("0" + DateTime.Now.Hour.ToString())) + "." + ((DateTime.Now.Minute >= 10) ? DateTime.Now.Minute.ToString() : ("0" + DateTime.Now.Minute.ToString())) + "." + ((DateTime.Now.Second >= 10) ? DateTime.Now.Second.ToString() : ("0" + DateTime.Now.Second.ToString()));
        }

        public override void Register()
        {
            // Register Events
            this.AddEventHandlers(new RoundEventHandler(this), Priority.High);
            this.AddEventHandler(typeof(IEventHandlerPlayerHurt), new DamageDetect(this), Priority.High);
            this.AddEventHandler(typeof(IEventHandlerPlayerDie), new DieDetect(this), Priority.High);
            this.AddEventHandler(typeof(IEventHandlerPlayerJoin), new PlayerJoinHandler(this), Priority.Highest);
            this.AddEventHandlers(new MyMiscEvents(this));
            // Register Commands
            this.AddCommand("spectator", new Command.SpectatorCommand(this));
            this.AddCommand("spec", new Command.SpectatorCommand(this));

            this.AddCommand("player", new Command.PlayerCommand(this));
            this.AddCommand("p", new Command.PlayerCommand(this));

            this.AddCommand("players", new Command.PlayerListCommand(this));
            
            this.AddCommand("heal", new Command.HealCommand(this));

            this.AddCommand("god", new Command.GodModeCommand(this));
            this.AddCommand("godmode", new Command.GodModeCommand(this));

            this.AddCommand("nodmg", new Command.NoDmgCommand(this));

            this.AddCommand("tut", new Command.TutorialCommand(this));
            this.AddCommand("tutorial", new Command.TutorialCommand(this));

            this.AddCommand("role", new Command.RoleCommand(this));

            this.AddCommand("keep", new Command.KeepSettingsCommand(this));
            this.AddCommand("keepsettings", new Command.KeepSettingsCommand(this));

            this.AddCommand("hp", new Command.SetHpCommand(this));
            this.AddCommand("sethp", new Command.SetHpCommand(this));


            this.AddCommand("pos", new Command.PosCommand(this));
            this.AddCommand("tpx", new Command.TeleportCommand(this));

            this.AddCommand("warp", new Command.WarpCommmand(this));

            this.AddCommand("roundlock", new Command.RoundLockCommand(this));
            this.AddCommand("lockround", new Command.RoundLockCommand(this));
            this.AddCommand("rlock", new Command.RoundLockCommand(this));
            this.AddCommand("lockr", new Command.RoundLockCommand(this));

            this.AddCommand("breakdoors", new Command.BreakDoorsCommand(this));
            this.AddCommand("breakdoor", new Command.BreakDoorsCommand(this));
            this.AddCommand("bd", new Command.BreakDoorsCommand(this));

            this.AddCommand("playerlockdown", new Command.LockdownCommand(this));
            this.AddCommand("pl", new Command.LockdownCommand(this));
            this.AddCommand("playerlock", new Command.LockdownCommand(this));
            this.AddCommand("plock", new Command.LockdownCommand(this));

            this.AddCommand("atcolor", new Command.ATColorCommand(this));
            this.AddCommand("atdisable", new Command.ATDisableCommand(this));

            this.AddCommand("instantkill", new Command.InstantKillCommand(this));
            this.AddCommand("instakill", new Command.InstantKillCommand(this));
            this.AddCommand("ik", new Command.InstantKillCommand(this));

            // Register config settings
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_enable", true, Smod2.Config.SettingType.BOOL, true, "Enable/Disable AdminToolbox"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_colors", false, Smod2.Config.SettingType.BOOL, true, "Enable/Disable AdminToolbox colors in server window"));


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

            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_endedRound_damageMultiplier", 1, Smod2.Config.SettingType.NUMERIC, true, "Damage multiplier after end of round"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_round_damageMultiplier", 1, Smod2.Config.SettingType.NUMERIC, true, "Damage multiplier"));


            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_log_teamkills", false, Smod2.Config.SettingType.BOOL, true, "Writing logfiles for teamkills"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_log_kills", false, Smod2.Config.SettingType.BOOL, true, "Writing logfiles for regular kills"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_log_commands", false, Smod2.Config.SettingType.BOOL, true, "Writing logfiles for all AT command usage"));

            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_round_info", true, Smod2.Config.SettingType.BOOL, true, "Prints round count and player number on round start & end"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_player_joinANDleave", false, Smod2.Config.SettingType.BOOL, true, "Writes player name in console on players joining"));

            //this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_intercom_whitelist", new string[] { string.Empty }, Smod2.Config.SettingType.LIST, true, "What ServerRank can use the Intercom to your specified settings"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_intercom_steamid_blacklist", new string[] { string.Empty }, Smod2.Config.SettingType.LIST, true, "Blacklist of steamID's that cannot use the intercom"));

            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_block_role_damage", new string[] {  string.Empty }, Smod2.Config.SettingType.LIST, true, "What roles cannot attack other roles"));
        }
        public static void AddMissingPlayerVariables()
        {
            if (PluginManager.Manager.Server.GetPlayers().Count > 0)
                foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
                {
                    AddSpesificPlayer(pl);
                }
        }
        public static void AddSpesificPlayer(Player playerToAdd)
        {
            if (playerToAdd.SteamId != null || playerToAdd.SteamId != "")
            {
                if (!playerdict.ContainsKey(playerToAdd.SteamId))
                    playerdict.Add(playerToAdd.SteamId, new List<bool>(new bool[] { false, false, false, false, false, false, false }));
                if (!playerStats.ContainsKey(playerToAdd.SteamId))
                    playerStats.Add(playerToAdd.SteamId, new List<int>(new int[] { 0, 0, 0, 0 }));
            }
        }
        public static void WriteToLog(string[] str, LogHandlers.ServerLogType logType)
        {
            string str2 = string.Empty;
            if (str.Length != 0)
                foreach (string st in str)
                    str2 += st;
            switch (logType)
            {
                case LogHandlers.ServerLogType.TeamKill:
                    if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_log_teamkills", false, false))
                        AdminToolboxLogger.AddLog(str2, logType);
                    break;
                case LogHandlers.ServerLogType.KillLog:
                    if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_log_kills", false, false))
                        AdminToolboxLogger.AddLog(str2, logType);
                    break;
                case LogHandlers.ServerLogType.RemoteAdminActivity:
                    if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_log_commands", false, false))
                        AdminToolboxLogger.AddLog(str2, logType);
                        break;
                default:
                    break;
            }
        }
    }

    public static class LevenshteinDistance
    {
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
            //Takes a string and finds the closest player from the playerlist
            int maxNameLength = 31, LastnameDifference =31/*, lastNameLength = 31*/;
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
    public class LogHandlers
    {
        public class LogHandler
        {
            public string Content;

            public string Type;

            public string Time;

            public bool Saved;
        }
        private readonly List<LogHandler> logs = new List<LogHandler>();

        public static LogHandlers singleton;

        private int _port;

        private int _ready;

        private int _maxlen;

        public enum ServerLogType
        {
            RemoteAdminActivity,
            KillLog,
            TeamKill,
            GameEvent,
            Misc
        }
        public static readonly string[] Txt = new string[]
        {
        "Remote Admin",
        "Kill",
        "TeamKill",
        "Game Event",
        "Misc"
        };
        private void Awake()
        {
            Txt.ToList().ForEach(delegate (string txt)
            {
                _maxlen = Math.Max(_maxlen, txt.Length);
            });
            _ready++;
            AddLog("Started logging.", ServerLogType.Misc);
        }
        void Start()
        {
            //AdminToolbox._roundStartTime = DateTime.Now.Year.ToString() + "-" + ((DateTime.Now.Month >= 10) ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString())) + "-" + ((DateTime.Now.Day >= 10) ? DateTime.Now.Day.ToString() : ("0" + DateTime.Now.Day.ToString())) + " " + ((DateTime.Now.Hour >= 10) ? DateTime.Now.Hour.ToString() : ("0" + DateTime.Now.Hour.ToString())) + "." + ((DateTime.Now.Minute >= 10) ? DateTime.Now.Minute.ToString() : ("0" + DateTime.Now.Minute.ToString())) + "." + ((DateTime.Now.Second >= 10) ? DateTime.Now.Second.ToString() : ("0" + DateTime.Now.Second.ToString()));
            _port = PluginManager.Manager.Server.Port;
        }

        public void AddLog(string msg, ServerLogType type)
        {
            _port = PluginManager.Manager.Server.Port;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
            logs.Add(new LogHandler
            {
                Content = msg,
                Type = Txt[(int)type],
                Time = time
            });
            if (Directory.Exists(FileManager.AppFolder))
            {
                if (!Directory.Exists(FileManager.AppFolder + "ATServerLogs"))
                {
                    Directory.CreateDirectory(FileManager.AppFolder + "ATServerLogs");
                }
                if (!Directory.Exists(FileManager.AppFolder + "ATServerLogs/" + _port))
                {
                    Directory.CreateDirectory(FileManager.AppFolder + "ATServerLogs/" + _port);
                }
                StreamWriter streamWriter = new StreamWriter(FileManager.AppFolder + "ATServerLogs" + "/" + _port  + "/" + AdminToolbox._roundStartTime + " Round " + AdminToolbox.roundCount + ".txt", true);
                string text = string.Empty;
                foreach (LogHandler log in logs)
                {
                    if (!log.Saved)
                    {
                        log.Saved = true;
                        string text2 = text;
                        text = text2 + log.Time + " | " + ToMax(log.Type, _maxlen) + " | " + log.Content + Environment.NewLine;
                    }
                }
                streamWriter.Write(text);
                streamWriter.Close();
            }
        }
        private static string ToMax(string text, int max)
        {
            while (text.Length < max)
            {
                text += " ";
            }
            return text;
        }
    }
}