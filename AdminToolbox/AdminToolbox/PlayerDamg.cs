using Smod2;
using Smod2.API;
using Smod2.Events;
using System.Linq;

namespace AdminToolbox
{
    class LastAttacked
    {
        public static Player lastAttacker = null;
        public static Player lastVictim = null;
        public static DamageType lastDamageType = DamageType.NONE;
    }
    class DamageDetect : IEventPlayerHurt
    {
        private Plugin plugin;
        public DamageDetect(Plugin plugin)
        {
            this.plugin = plugin;
        }
        public void OnPlayerHurt(Player player, Player attacker, float damage, out float damageOutput, DamageType type, out DamageType typeOutput)
        {
            if (AdminToolbox.playerdict.ContainsKey(player.SteamId)) { if (AdminToolbox.playerdict[player.SteamId][1]) { damageOutput = 0f; typeOutput = DamageType.NONE; ; return; }; }
            if (AdminToolbox.playerdict.ContainsKey(attacker.SteamId)) { if (AdminToolbox.playerdict[attacker.SteamId][2]) { damageOutput = 0f; typeOutput = DamageType.NONE; ; return; }; }
            //if (AdminToolbox.adminSteamID.Contains(player.SteamId) && AdminToolbox.adminMode == true) { damageOutput = 0f; typeOutput = DamageType.NONE; ; return; }
            damageOutput = damage;
            typeOutput = type;
            int[] allowedDmg = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_tutorial_dmg_allowed",new int[] { -1 }, false);
            int[] DebugDmg = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_debug_damagetypes", new int[] { 5, 13, 14, 15, 16, 17 },false);
            int[] scpDamagesTypes = { 2, 6, 7, 9, 10 };

            if (AdminToolbox.isRoundFinished)
            {
                int damageMultiplier = ConfigManager.Manager.Config.GetIntValue("admintoolbox_endedRound_damageMultiplier", 1, false);
                damageOutput = damage * damageMultiplier;
                typeOutput = type;
                if((int)player.Class.ClassType!=14)
                    return;
            }
            switch ((int)player.Class.ClassType)
            {
                case -1:
                    break;
                case 2:
                    break;
                case 14:
                    if (!allowedDmg.Contains((int)type))
                    {
                        if (DebugDmg.Contains((int)type) && (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_tutorial", false,false))) plugin.Info(player.Class.Name + " " + player.Name + " not allowed damagetype: " + type);
                        typeOutput = DamageType.NONE;
                        damageOutput = 0f;
                        return;
                    }
                    else
                        goto default;
                default:
                    if ((attacker.Name == "Server" && !(ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_server", false,false))) || (attacker.Name == "Spectator" && !(ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_spectator",false, false)))) return;
                    if (AdminToolbox.nineTailsTeam.Contains((int)player.Class.Team) && AdminToolbox.nineTailsTeam.Contains((int)attacker.Class.Team))
                    {
                        if (type == DamageType.TESLA)
                        {
                            LastAttacked.lastAttacker = attacker;
                            LastAttacked.lastVictim = player;
                            LastAttacked.lastDamageType = type;
                            //string x = LastAttacked.lastAttacker.Name + " " + LastAttacked.lastVictim.Name + " " + LastAttacked.lastDamageType;
                            //plugin.Info(x);
                        }
                        if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_damage",false, false))
                        {
                            if (!DebugDmg.Contains((int)type)) return;
                            plugin.Warn(attacker.Class.Name + " " + attacker.Name + " attacked fellow " + player.Class.Name + " " + player.Name + /*" for " + damage +^*/ " with " + type);
                        }
                    }
                    else if(AdminToolbox.chaosTeam.Contains((int)player.Class.Team) && AdminToolbox.chaosTeam.Contains((int)attacker.Class.Team))
                    {
                        if (type == DamageType.TESLA)
                        {
                            LastAttacked.lastAttacker = attacker;
                            LastAttacked.lastVictim = player;
                            LastAttacked.lastDamageType = type;
                            //string x = LastAttacked.lastAttacker.Name + " " + LastAttacked.lastVictim.Name + " " + LastAttacked.lastDamageType;
                            //plugin.Info(x);
                        }
                        if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_damage",false, false))
                        {
                            if (!DebugDmg.Contains((int)type)) return;
                            plugin.Warn(attacker.Class.Name + " " + attacker.Name + " attacked fellow " + player.Class.Name + " " + player.Name + /*" for " + damage +^*/ " with " + type);
                        }
                    }
                    else if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_player_damage",false, false))
                    {
                        if (!DebugDmg.Contains((int)type)) return;
                        plugin.Info(attacker.Class.Name + " " + attacker.Name + " attacked " + player.Class.Name + " " + player.Name + " for " + damage + " damage" + " with: " + type);
                    }
                    break;
            }
        }
    }
    class DieDetect : IEventPlayerDie
    {
        private Plugin plugin;
        public DieDetect(Plugin plugin)
        {
            this.plugin = plugin;
        }
        public void OnPlayerDie(Player player, Player killer, out bool spawnRagdoll)
        {
            spawnRagdoll = true;
            if (AdminToolbox.isRoundFinished) return;
            if (player.Name == "Server" || killer.Name == "Server") { spawnRagdoll = false; return; }
            switch ((int)player.Class.ClassType)
            {
                case -1:
                    return;
                case 2:
                    return;
                case 3:
                    spawnRagdoll = false;
                    goto default;
                default:
                    //plugin.Info("OnPlayerDie: \n" + LastAttacked.lastAttacker.Name + " " + LastAttacked.lastVictim.Name + " " + LastAttacked.lastDamageType);

                    if (!(ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_scp_and_self_killed",false, false)) && player.Name == killer.Name) return;
                    if (AdminToolbox.nineTailsTeam.Contains((int)player.Class.Team) && AdminToolbox.nineTailsTeam.Contains((int)killer.Class.Team))
                    {
                        if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_kill",true, false))
                            if ((LastAttacked.lastDamageType == DamageType.TESLA) && (player.SteamId==LastAttacked.lastVictim.SteamId && killer.SteamId==LastAttacked.lastAttacker.SteamId))
                            {
                                plugin.Warn(killer.Class.Name + " " + killer.Name + " granaded fellow " + player.Class.Name + " " + player.Name);
                                LastAttacked.lastAttacker = null;
                                LastAttacked.lastVictim = null;
                                LastAttacked.lastDamageType = DamageType.NONE;
                            }
                            else
                                plugin.Warn(killer.Class.Name + " " + killer.Name + " killed fellow " + player.Class.Name + " " + player.Name);
                    }
                    else if (AdminToolbox.chaosTeam.Contains((int)player.Class.Team) && AdminToolbox.chaosTeam.Contains((int)killer.Class.Team))
                    {
                        if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_kill",true, false))
                            if ((LastAttacked.lastDamageType == DamageType.TESLA) && (player.SteamId == LastAttacked.lastVictim.SteamId && killer.SteamId == LastAttacked.lastAttacker.SteamId))
                            {
                                plugin.Warn(killer.Class.Name + " " + killer.Name + " granaded fellow " + player.Class.Name + " " + player.Name);
                                LastAttacked.lastAttacker = null;
                                LastAttacked.lastVictim = null;
                                LastAttacked.lastDamageType = DamageType.NONE;
                            }
                            else
                                plugin.Warn(killer.Class.Name + " " + killer.Name + " killed fellow " + player.Class.Name + " " + player.Name);
                    }
                    else if ((ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_player_kill",false, false)))
                    {
                        plugin.Info(killer.Name + " killed: " + player.Name);
                    }
                    break;
            }
        }
    }
}