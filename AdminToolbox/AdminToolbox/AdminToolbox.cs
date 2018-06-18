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

namespace AdminToolbox
{
    [PluginDetails(
        author = "Evan (AKA Rnen)",
        name = "Admin Toolbox",
        description = "Plugin for advanced admin tools",
        id = "rnen.admin.toolbox",
        version = "1.3",
        SmodMajor = 3,
        SmodMinor = 3,
        SmodRevision = 0
        )]
    class AdminToolbox : Plugin
    {
        public static bool isRoundFinished = false, adminMode = false, lockRound = false, lockDown = false;

        public static int[] nineTailsTeam = { 1, 3 }, chaosTeam = { 2, 4 };

        public static string fileName;

        public static Dictionary<string, List<bool>> playerdict = new Dictionary<string, List<bool>>();
        public static Dictionary<string, List<int>> playerStats = new Dictionary<string, List<int>>();
        public static Dictionary<string, Vector> warpVectors = new Dictionary<string, Vector>();
        public static List<string> logText = new List<string>(), myRooms = new List<string>();
        public static int roundCount = 0;

        public override void OnDisable()
        {
            this.Info(this.Details.name + " v." + this.Details.version + " - Disabled");
        }
        public static void SetPlayerBools(Player player, bool keepSettings, bool godMode, bool dmgOff, bool destroyDoor)
        {
            //This is actually never used, its just for keeping track, might become an all on/off switch at some point
            playerdict[player.SteamId][0] = keepSettings;
            playerdict[player.SteamId][1] = godMode;
            playerdict[player.SteamId][2] = dmgOff;
            playerdict[player.SteamId][3] = destroyDoor;
        }
        public static void SetPlayerStats(Player player, int Kills, int TeamKills, int Deaths, int Something)
        {
            playerStats[player.SteamId][0] = Kills;
            playerStats[player.SteamId][1] = TeamKills;
            playerStats[player.SteamId][2] = Deaths;
            playerStats[player.SteamId][3] = Something;
        }

        public override void OnEnable()
        {
            if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_enable", true, false) == false) { pluginManager.DisablePlugin(this.Details.id); return; }
            this.Info(this.Details.name + " v." + this.Details.version + " - Enabled");
            fileName = DateTime.Today.Date + PluginManager.Manager.Server.Name + "_AdminToolbox_TKLog.txt";
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
            this.AddCommand("players", new Command.PlayerListCommand(this));
            this.AddCommand("tpx", new Command.TeleportCommand(this));
            this.AddCommand("heal", new Command.HealCommand(this));
            this.AddCommand("god", new Command.GodModeCommand(this));
            this.AddCommand("godmode", new Command.GodModeCommand(this));
            this.AddCommand("nodmg", new Command.NoDmgCommand(this));
            this.AddCommand("tut", new Command.TutorialCommand(this));
            this.AddCommand("tutorial", new Command.TutorialCommand(this));
            this.AddCommand("role", new Command.RoleCommand(this));
            //this.AddCommand("keep", new Command.KeepSettings(this));
            //this.AddCommand("keepsettings", new Command.KeepSettings(this));
            this.AddCommand("hp", new Command.SetHpCommand(this));
            this.AddCommand("sethp", new Command.SetHpCommand(this));
            this.AddCommand("player", new Command.PlayerCommand(this));
            this.AddCommand("pos", new Command.PosCommand(this));
            this.AddCommand("warp", new Command.WarpCommmand(this));
            this.AddCommand("roundlock", new Command.RoundLockCommand(this));
            this.AddCommand("rlock", new Command.RoundLockCommand(this));
            this.AddCommand("lockdown", new Command.LockdownCommand(this));
            this.AddCommand("breakdoors", new Command.BreakDoorsCommand(this));
            this.AddCommand("bd", new Command.BreakDoorsCommand(this));
            this.AddCommand("door", new Command.DoorCommand(this));
            // Register config settings
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_enable", true, Smod2.Config.SettingType.BOOL, true, "Enable/Disable AdminToolbox"));
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
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_writeTkToFile", false, Smod2.Config.SettingType.BOOL, true, "Unfinished"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_round_info", true, Smod2.Config.SettingType.BOOL, true, "Prints round count and player number on round start & end"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_debug_player_joinANDleave", false, Smod2.Config.SettingType.BOOL, true, "Writes player name in console on players joining"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_intercom_extended_IDs_whitelist", new string[] { }, Smod2.Config.SettingType.LIST, true, "What STEAMID's can use the Intercom freely"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_intercom_extended_duration", 1000f, Smod2.Config.SettingType.FLOAT, true, "How long people in the extended ID's list can talk"));
            this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_intercom_extended_cooldown", 0f, Smod2.Config.SettingType.FLOAT, true, "How long cooldown after whitelisted people have used it"));
            //this.AddConfig(new Smod2.Config.ConfigSetting("admintoolbox_block_role_damage", null, Smod2.Config.SettingType.NUMERIC_DICTIONARY, true, "What roles cannot attack other roles"));
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
    public class LogHandler
    {
        public static void WriteToLog(string str)
        {
            if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_writeTkToFile", false, false) == false) return;
            AdminToolbox.logText.Add(System.DateTime.Now.ToString() + ": " + str + "\n");
            string myLog = null;
            foreach (var item in AdminToolbox.logText)
            {
                myLog += item + Environment.NewLine;
            }
            Server server = PluginManager.Manager.Server;
            File.WriteAllText(AdminToolbox.fileName, myLog);
        }
    }
}