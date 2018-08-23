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
	class DamageDetect : IEventHandlerPlayerHurt
	{
		private Plugin plugin;
		public DamageDetect(Plugin plugin)
		{
			this.plugin = plugin;
		}
		public void OnPlayerHurt(PlayerHurtEvent ev)
		{

			int[] humanDamageTypes = {
				(int)DamageType.COM15,
				(int)DamageType.E11_STANDARD_RIFLE,
				(int)DamageType.P90,
				(int)DamageType.MP7,
				(int)DamageType.LOGICER,
				(int)DamageType.FRAG
			},
				scpDamagesTypes = {
				(int)DamageType.SCP_049,
				(int)DamageType.SCP_049_2,
				(int)DamageType.SCP_096,
				(int)DamageType.SCP_106,
				(int)DamageType.SCP_173,
				(int)DamageType.SCP_939
			},
				nineTailsTeam = {
				(int)Team.MTF,
				(int)Team.RSC
			},
				chaosTeam = {
				(int)Team.CHI,
				(int)Team.CDP
			};

			AdminToolbox.AddMissingPlayerVariables(new List<Player> { ev.Attacker, ev.Player });

			float originalDamage = ev.Damage;
			DamageType originalType = ev.DamageType;
			float damageMultiplier = ConfigManager.Manager.Config.GetFloatValue("admintoolbox_round_damageMultiplier", 1f, true);
			ev.Damage = originalDamage * damageMultiplier;

			if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId) && AdminToolbox.playerdict[ev.Player.SteamId].isJailed) { ev.Damage = 0f; return; }
			if (AdminToolbox.playerdict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.playerdict[ev.Attacker.SteamId].isJailed) { ev.Damage = 0f; return; }

			if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId)) { if (AdminToolbox.playerdict[ev.Player.SteamId].godMode) { ev.Damage = 0f; ev.DamageType = DamageType.NONE; ; return; }; }
			if (AdminToolbox.playerdict.ContainsKey(ev.Attacker.SteamId)) { if (AdminToolbox.playerdict[ev.Attacker.SteamId].dmgOff) { ev.Damage = 0f; ev.DamageType = DamageType.NONE; ; return; }; }
			int[] allowedDmg = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_tutorial_dmg_allowed", new int[] { (int)ItemType.NULL }, false);
			int[] DebugDmg = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_debug_damagetypes", humanDamageTypes, false);

			if (ev.DamageType != DamageType.FRAG && AdminToolbox.playerdict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.playerdict[ev.Attacker.SteamId].instantKill)
				ev.Damage = ev.Player.GetHealth() + 1;
			string[] roleDamages = ConfigManager.Manager.Config.GetListValue("admintoolbox_block_role_damage", new string[] { "14:14" }, false);

			bool isFriendlyDmg()
			{
				if (nineTailsTeam.Contains((int)ev.Player.TeamRole.Team) && nineTailsTeam.Contains((int)ev.Attacker.TeamRole.Team)) return true;
				else if (chaosTeam.Contains((int)ev.Player.TeamRole.Team) && chaosTeam.Contains((int)ev.Attacker.TeamRole.Team))
					return true;
				else
					return false;
			}

			if (roleDamages.Length > 0)
			{
				bool foundPlayer = false;
				foreach (var item in roleDamages)
				{
					string[] myStringKey = item.Replace(" ", "").Split(':');
					if (!int.TryParse(myStringKey[0], out int attackRole)) { plugin.Info("Not a valid config at \"admintoolbox_block_role_damage\"  Value: " + myStringKey[0] + ":" + myStringKey[1]); continue; }
					string[] myString = myStringKey[1].Split('.', '-', '#', '_', ',', '+', '@', '>', '<', ';');
					if (myString.Length >= 1)
					{
						foreach (var item2 in myString)
						{
							if (int.TryParse(item2, out int victimRole))
							{
								if (attackRole == (int)ev.Attacker.TeamRole.Role && victimRole == (int)ev.Player.TeamRole.Role)
								{
									if (AdminToolbox.playerdict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.playerdict[ev.Attacker.SteamId].instantKill) continue;
									ev.Damage = 0f;
									ev.DamageType = DamageType.NONE;
									//plugin.Info(ev.Attacker.TeamRole.Name + " " + ev.Attacker.Name + "was blocked from attacking " + ev.Player.TeamRole.Name + " " + ev.Player + " with " + ev.DamageType);
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
				//if (foundPlayer) return;
			}
			if (AdminToolbox.isRoundFinished)
			{
				if (AdminToolbox.playerdict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.playerdict[ev.Attacker.SteamId].instantKill)
					goto SkipMultiplier;
				else
				{
					float enddamageMultiplier = ConfigManager.Manager.Config.GetFloatValue("admintoolbox_endedRound_damageMultiplier", 1f, true);
					ev.Damage = originalDamage * enddamageMultiplier;
				}
				SkipMultiplier:;
			}
			switch ((int)ev.Player.TeamRole.Role)
			{
				case 14:
					if (allowedDmg.Contains((int)ev.DamageType)) goto default;
					if (DebugDmg.Contains((int)ev.DamageType) && (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_tutorial", false, false))) plugin.Info(ev.Player.TeamRole.Name + " " + ev.Player.Name + " not allowed damagetype: " + ev.DamageType);
					if (AdminToolbox.playerdict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.playerdict[ev.Attacker.SteamId].instantKill) goto default;
					ev.DamageType = DamageType.NONE;
					ev.Damage = 0f;
					break;
				default:
					if (AdminToolbox.isRoundFinished) break;
					ev.Damage = (ev.DamageType == DamageType.DECONT) ? originalDamage * ConfigManager.Manager.Config.GetFloatValue("admintoolbox_decontamination_damagemultiplier", 1f, true) : ev.Damage;
					if ((ev.Attacker.Name == "Server" && !(ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_server", false, false))) || (ev.Attacker.Name == "Spectator" && !ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_spectator", false, false))) return;
					if (isFriendlyDmg())
					{
						if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_damage", false, false))
						{
							ev.Damage = (ev.Damage >= 1) ? ConfigManager.Manager.Config.GetFloatValue("admintoolbox_friendlyfire_damagemultiplier", 1f) * originalDamage : ev.Damage;
							if (DebugDmg.Contains((int)ev.DamageType) && !AdminToolbox.isRoundFinished)
								plugin.Info(ev.Attacker.TeamRole.Name + " " + ev.Attacker.Name + " attacked fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name + /*" for " + damage +^*/ " with " + ev.DamageType);
						}
					}
					else if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_player_damage", false, false))
					{
						if (DebugDmg.Contains((int)ev.DamageType) && !AdminToolbox.isRoundFinished)
							plugin.Info(ev.Attacker.TeamRole.Name + " " + ev.Attacker.Name + " attacked " + ev.Player.TeamRole.Name + " " + ev.Player.Name + /*" for " + ev.Damage + " damage" +*/ " with: " + ev.DamageType);
					}
					break;
			}
			if (ev.Damage >= ev.Player.GetHealth() && AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId)) AdminToolbox.playerdict[ev.Player.SteamId].DeathPos = ev.Player.GetPosition();

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

			AdminToolbox.AddMissingPlayerVariables(new List<Player> { ev.Player, ev.Killer });

			if (ev.Player.Name == "Server" || ev.Killer.Name == "Server" || ev.Player.Name == string.Empty) { ev.SpawnRagdoll = false; return; }

			bool isFriendlyKill()
			{
				if (nineTailsTeam.Contains((int)ev.Player.TeamRole.Team) && nineTailsTeam.Contains((int)ev.Killer.TeamRole.Team))
					return true;
				else if (chaosTeam.Contains((int)ev.Player.TeamRole.Team) && chaosTeam.Contains((int)ev.Killer.TeamRole.Team))
					return true;
				else
					return false;
			}

			switch ((int)ev.Player.TeamRole.Role)
			{
				case 3:
					if (ev.DamageTypeVar == DamageType.LURE || ev.DamageTypeVar == DamageType.CONTAIN)
						ev.SpawnRagdoll = false;
					goto default;
				default:
					if (AdminToolbox.isRoundFinished) break;
					if (AdminToolbox.playerdict.ContainsKey(ev.Player.SteamId)) AdminToolbox.playerdict[ev.Player.SteamId].Deaths++;
					if (!(ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_scp_and_self_killed", false, false)) && ev.Player.Name == ev.Killer.Name) return;
					if (isFriendlyKill())
					{
						string keyWord = (ev.DamageTypeVar == DamageType.FRAG) ? "granaded" : "killed";
						if (AdminToolbox.playerdict.ContainsKey(ev.Killer.SteamId)) AdminToolbox.playerdict[ev.Killer.SteamId].TeamKills++;
						if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_friendly_kill", true, false))
							if (AdminToolbox.isColored)
								plugin.Info(Colors.ColoredRole(ev.Killer) + " @#fg=Yellow;" + ev.Killer.Name + "@#fg=DarkRed; " + keyWord + " fellow @#fg=Default;" + Colors.ColoredRole(ev.Player) + "@#fg=Yellow; " + ev.Player.Name + "@#fg=Default;");
							else
								plugin.Info(ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " " + keyWord + " fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name);
						AdminToolbox.WriteToLog(new string[] { ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " " + keyWord + " fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name }, LogHandlers.ServerLogType.TeamKill);
					} // Colors.ColoredRole()
					else
					{
						if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_player_kill", false, false))
							plugin.Info(ev.Killer.Name + " killed: " + ev.Player.Name);
						if (AdminToolbox.playerdict.ContainsKey(ev.Killer.SteamId)) AdminToolbox.playerdict[ev.Killer.SteamId].Kills++;
						AdminToolbox.WriteToLog(new string[] { ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " killed " + ev.Player.TeamRole.Name + " " + ev.Player.Name }, LogHandlers.ServerLogType.KillLog);
					}
					break;
			}
		}
	}
	#endregion
	public class Colors
	{
		public static string ColoredRole(Player player)
		{
			if (!AdminToolbox.isColored) return player.TeamRole.Name;
			switch ((Team)player.TeamRole.Team)
			{
				case Team.SCP:
					return "@#fg=Red;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.MTF:
					return "@#fg=Blue;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.CHI:
					return "@#fg=Green;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.RSC:
					return "@#fg=Yellow;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.CDP:
					return "@#fg=Orange;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.TUT:
					return "@#fg=Green;" + player.TeamRole.Name + "@#fg=Default;";
				default:
					return player.TeamRole.Name;
			}
		}
	}
}