using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AdminToolbox
{
    #region PlayerDamage

    class LastAttacked
    {
        //Just a place to store the last attacked player 
        public static Player lastAttacker = null, lastVictim = null;
        public static DamageType lastDamageType = DamageType.NONE, last106Damage;
    }
    class DamageDetect : IEventHandlerPlayerHurt
    {
        private Plugin plugin;
        public DamageDetect(Plugin plugin)
        {
            this.plugin = plugin;
        }
        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            int[] humanDamageTypes = { (int)DamageType.COM15, (int)DamageType.E11_STANDARD_RIFLE, (int)DamageType.P90, (int)DamageType.MP7, (int)DamageType.LOGICER, (int)DamageType.FRAG };
            int[] scpDamagesTypes = { (int)DamageType.SCP_049, (int)DamageType.SCP_049_2, (int)DamageType.SCP_096, (int)DamageType.SCP_106, (int)DamageType.SCP_173, (int)DamageType.SCP_939 };
            int[] nineTailsTeam = { (int)Team.MTF, (int)Team.RSC }, chaosTeam = { (int)Team.CHI, (int)Team.CDP };


        AdminToolbox.AddSpesificPlayer(ev.Player);
            AdminToolbox.AddSpesificPlayer(ev.Attacker);
            if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId)) { if (AdminToolbox.playerdict[ev.Player.SteamId][1]) { ev.Damage = 0f; ev.DamageType = DamageType.NONE; ; return; }; }
            if (AdminToolbox.playerdict.ContainsKey(ev.Attacker.SteamId)) { if (AdminToolbox.playerdict[ev.Attacker.SteamId][2]) { ev.Damage = 0f; ev.DamageType = DamageType.NONE; ; return; }; }
            int[] allowedDmg = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_tutorial_dmg_allowed", new int[] { -1 }, false);
            int[] DebugDmg = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_debug_damagetypes", humanDamageTypes, false);

            if(AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId))
                if (AdminToolbox.playerdict[ev.Attacker.SteamId][6])
                    if (ev.DamageType != DamageType.FRAG)
                        ev.Damage = ev.Player.GetHealth() + 1;

            string[] roleDamages = ConfigManager.Manager.Config.GetListValue("admintoolbox_block_role_damage", new string[] { "14:14" }, false);
            if (roleDamages.Length > 0)
            {
                bool foundPlayer = false;
                foreach (var item in roleDamages)
                {
                    string[] myStringKey = item.Replace(" ", "").Split(':');
                    if (!Int32.TryParse(myStringKey[0], out int z)) { plugin.Info("Not a valid config at \"admintoolbox_block_role_damage\"  Value: " + myStringKey[0] + ":" + myStringKey[1]); continue; }
                    string[] myString = myStringKey[1].Split('.', '-', '#', '_');
                    if (myString.Length >= 1)
                    {
                        foreach (var item2 in myString)
                        {
                            if (Int32.TryParse(item2, out int x))
                            {
                                if (z == (int)ev.Attacker.TeamRole.Role && x == (int)ev.Player.TeamRole.Role)
                                {
                                    ev.Damage = 0f;
                                    ev.DamageType = DamageType.NONE;
                                    foundPlayer = true;
                                    break;
                                }
                            }
                            else
                            {
                                plugin.Info("Invalid config value at \"admintoolbox_block_role_damage:\"  Value: " + myStringKey[0] + ":" + myStringKey[1] + "\nSkipping entry...");
                                continue;
                            }
                        }
                        if (foundPlayer) break;
                    }
                }
                if (foundPlayer) return;
            }

            if (AdminToolbox.isRoundFinished)
            {
                int damageMultiplier = ConfigManager.Manager.Config.GetIntValue("admintoolbox_endedRound_damageMultiplier", 1, true);
                ev.Damage = ev.Damage * damageMultiplier;
                ev.DamageType = ev.DamageType;
                if ((int)ev.Player.TeamRole.Role != 14)
                    return;
            }
            switch ((int)ev.Player.TeamRole.Role)
            {
                case 14:
                    if (!allowedDmg.Contains((int)ev.DamageType))
                    {
                        if (DebugDmg.Contains((int)ev.DamageType) && (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_tutorial", false, false))) plugin.Info(ev.Player.TeamRole.Name + " " + ev.Player.Name + " not allowed damagetype: " + ev.DamageType);
                        ev.DamageType = DamageType.NONE;
                        ev.Damage = 0f;
                        return;
                    }
                    else
                        goto default;
                default:
                    if (ev.Player.TeamRole.Role == Role.SCP_106) { LastAttacked.last106Damage = ev.DamageType; };
                    if ((ev.Attacker.Name == "Server" && !(ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_server", false, false))) || (ev.Attacker.Name == "Spectator" && !(ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_spectator", false, false)))) return;
                    if (nineTailsTeam.Contains((int)ev.Player.TeamRole.Team) && nineTailsTeam.Contains((int)ev.Attacker.TeamRole.Team))
                    {
                        if (ev.DamageType == DamageType.FRAG)
                        {
                            LastAttacked.lastAttacker = ev.Attacker;
                            LastAttacked.lastVictim = ev.Player;
                            LastAttacked.lastDamageType = ev.DamageType;
                            //string x = LastAttacked.lastAttacker.Name + " " + LastAttacked.lastVictim.Name + " " + LastAttacked.lastDamageType;
                            //plugin.Info(x);
                        }
                        if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_damage", false, false))
                        {
                            if (DebugDmg.Contains((int)ev.DamageType))
                                plugin.Warn(ev.Attacker.TeamRole.Name + " " + ev.Attacker.Name + " attacked fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name + /*" for " + damage +^*/ " with " + ev.DamageType);
                        }
                    }
                    else if (chaosTeam.Contains((int)ev.Player.TeamRole.Team) && chaosTeam.Contains((int)ev.Attacker.TeamRole.Team))
                    {
                        if (ev.DamageType == DamageType.FRAG)
                        {
                            LastAttacked.lastAttacker = ev.Attacker;
                            LastAttacked.lastVictim = ev.Player;
                            LastAttacked.lastDamageType = ev.DamageType;
                        }
                        if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_damage", false, false))
                        {
                            if (DebugDmg.Contains((int)ev.DamageType))
                                plugin.Warn(ev.Attacker.TeamRole.Name + " " + ev.Attacker.Name + " attacked fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name + /*" for " + damage +^*/ " with " + ev.DamageType);
                        }
                    }
                    else if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_player_damage", false, false))
                    {
                        if (DebugDmg.Contains((int)ev.DamageType))
                            plugin.Info(ev.Attacker.TeamRole.Name + " " + ev.Attacker.Name + " attacked " + ev.Player.TeamRole.Name + " " + ev.Player.Name + /*" for " + ev.Damage + " damage" +*/ " with: " + ev.DamageType);
                    }
                    break;
            }
        }
    }

    #endregion
    #region PlayerDeath

    class DieDetect : IEventHandlerPlayerDie
    {
        private Plugin plugin;
        public DieDetect(Plugin plugin)
        {
            this.plugin = plugin;
        }
        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            int[] nineTailsTeam = { (int)Team.MTF, (int)Team.RSC }, chaosTeam = { (int)Team.CHI, (int)Team.CDP };

            AdminToolbox.AddSpesificPlayer(ev.Player);
            AdminToolbox.AddSpesificPlayer(ev.Killer);
            if (ev.Player.Name == "Server" || ev.Killer.Name == "Server") { ev.SpawnRagdoll = false; return; }
            if (AdminToolbox.playerStats.ContainsKey(ev.Player.SteamId)) AdminToolbox.playerStats[ev.Player.SteamId][2]++;
            switch ((int)ev.Player.TeamRole.Role)
            {
                case 3:
                    if (LastAttacked.last106Damage == DamageType.LURE || LastAttacked.last106Damage == DamageType.CONTAIN)
                        ev.SpawnRagdoll = false;
                    goto default;
                default:
                    if (AdminToolbox.isRoundFinished) return;

                    if (!(ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_scp_and_self_killed", false, false)) && ev.Player.Name == ev.Killer.Name) return;
                    if (nineTailsTeam.Contains((int)ev.Player.TeamRole.Team) && nineTailsTeam.Contains((int)ev.Killer.TeamRole.Team))
                    {
                        if (AdminToolbox.playerStats.ContainsKey(ev.Killer.SteamId)) AdminToolbox.playerStats[ev.Killer.SteamId][1]++;
                        if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_kill", true, false))
                            if ((LastAttacked.lastDamageType == DamageType.FRAG) && (ev.Player.SteamId == LastAttacked.lastVictim.SteamId && ev.Killer.SteamId == LastAttacked.lastAttacker.SteamId))
                            {
                                if (AdminToolbox.isColored)
                                    plugin.Warn(ev.Killer.TeamRole.Name + " @#fg=Yellow;" + ev.Killer.Name + "@#fg=Red; granaded @#fg=Default;fellow @#fg=Blue;" + ev.Player.TeamRole.Name + "@#fg=Yellow; " + ev.Player.Name+ "@#fg=Default;");
                                else
                                    plugin.Warn(ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " granaded fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name);
                                LogHandler.WriteToLog(ev.Killer.TeamRole.Name.ToString() + " " + ev.Killer.Name.ToString() + " granaded fellow " + ev.Player.TeamRole.Name.ToString() + " " + ev.Player.Name.ToString());
                                LastAttacked.lastAttacker = null;
                                LastAttacked.lastVictim = null;
                                LastAttacked.lastDamageType = DamageType.NONE;
                            }
                            else
                            {
                                if (AdminToolbox.isColored)
                                    plugin.Warn(ev.Killer.TeamRole.Name + " @#fg=Yellow;" + ev.Killer.Name + "@#fg=Red; killed fellow @#fg=Blue;" + ev.Player.TeamRole.Name + " @#fg=Yellow;" + ev.Player.Name+ "@#fg=Default;");
                                else
                                    plugin.Warn(ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " killed fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name);
                                LogHandler.WriteToLog(ev.Killer.TeamRole.Name.ToString() + " " + ev.Killer.Name.ToString() + " killed fellow " + ev.Player.TeamRole.Name.ToString() + " " + ev.Player.Name.ToString());
                            }
                    }
                    else if (chaosTeam.Contains((int)ev.Player.TeamRole.Team) && chaosTeam.Contains((int)ev.Killer.TeamRole.Team))
                    {
                        if (AdminToolbox.playerStats.ContainsKey(ev.Killer.SteamId)) AdminToolbox.playerStats[ev.Killer.SteamId][1]++;
                        if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_kill", true, false))
                            if ((LastAttacked.lastDamageType == DamageType.FRAG) && (ev.Player.SteamId == LastAttacked.lastVictim.SteamId && ev.Killer.SteamId == LastAttacked.lastAttacker.SteamId))
                            {
                                if (AdminToolbox.isColored)
                                    plugin.Warn(ev.Killer.TeamRole.Name + " @#fg=Yellow;" + ev.Killer.Name + "@#fg=Red; granaded @#fg=Default;fellow @#fg=Green;" + ev.Player.TeamRole.Name + " @#fg=Yellow;" + ev.Player.Name+ "@#fg=Default;");
                                else
                                    plugin.Warn(ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " granaded fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name);
                                LogHandler.WriteToLog(ev.Killer.TeamRole.Name.ToString() + " " + ev.Killer.Name.ToString() + " granaded fellow " + ev.Player.TeamRole.Name.ToString() + " " + ev.Player.Name.ToString());
                                LastAttacked.lastAttacker = null;
                                LastAttacked.lastVictim = null;
                                LastAttacked.lastDamageType = DamageType.NONE;
                            }
                            else
                            {
                                if (AdminToolbox.isColored)
                                    plugin.Warn(ev.Killer.TeamRole.Name + " @#fg=Yellow;" + ev.Killer.Name + "@#fg=Red; killed fellow @#fg=Green;|" + ev.Player.TeamRole.Name + " @#fg=Yellow;" + ev.Player.Name+ "@#fg=Default;");
                                else
                                    plugin.Warn(ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " killed fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name);
                                LogHandler.WriteToLog(ev.Killer.TeamRole.Name.ToString() + " " + ev.Killer.Name.ToString() + " killed fellow " + ev.Player.TeamRole.Name.ToString() + " " + ev.Player.Name.ToString());
                            }
                    }
                    else if ((ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_player_kill", false, false)))
                    {
                        plugin.Info(ev.Killer.Name + " killed: " + ev.Player.Name);
                        if (AdminToolbox.playerStats.ContainsKey(ev.Killer.SteamId)) AdminToolbox.playerStats[ev.Killer.SteamId][0]++;
                    }
                    else
                        if (AdminToolbox.playerStats.ContainsKey(ev.Killer.SteamId)) AdminToolbox.playerStats[ev.Killer.SteamId][0]++;
                    break;
            }
        }
    }
    #endregion
}