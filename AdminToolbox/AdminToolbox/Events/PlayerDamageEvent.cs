using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AdminToolbox
{
	
	class DamageDetect : IEventHandlerPlayerHurt
	{
		private AdminToolbox plugin;
		static IConfigFile Config => ConfigManager.Manager.Config;
		public DamageDetect(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		readonly int[] 
			humanDamageTypes = {
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

		int[] CalculateTutorialDamage()
		{
			List<int> _tutDamageList = new List<int>();
			string _tutDefaultDmgAllowed = (PluginManager.Manager.Plugins.Where(p => p.Details.id == "cyan.serpents.hand").Count() > 0) ?  "*" : ((int)DamageType.NONE).ToString();
			string[] _allowedDmgString = 
				ConfigManager.Manager.Config.GetListValue(
					"admintoolbox_tutorial_dmg_allowed",
					new string[] { (_tutDefaultDmgAllowed).ToString() },
					false
				);
			string[] _allAliasWords = { "*", "all", "every" };
			if (_allowedDmgString.Count() > 0)
			{
				if (_allowedDmgString.Any(item => _allAliasWords.Contains(item.ToLower())))
				{
					_tutDamageList.Clear();
					for (int dmgIndex = 0; dmgIndex < (Enum.GetNames(typeof(DamageType)).Length - 1); dmgIndex++)
						_tutDamageList.Add(dmgIndex);
				}
				else
					foreach (string str in _allowedDmgString)
						if (int.TryParse(str, out int number))
							_tutDamageList.Add(number);
			}
			else
				return new int[] { -1 };
			if (_tutDamageList.Count > 0)
				return _tutDamageList.ToArray();
			else
				return new int[] { -1 };
		}

		bool IsFriendlyFire(Player victim, Player attacker)
		{
			if (victim == null || attacker == null) return false;
			if ( (nineTailsTeam.Contains((int)victim.TeamRole.Team) && nineTailsTeam.Contains((int)attacker.TeamRole.Team)) 
				|| (chaosTeam.Contains((int)victim.TeamRole.Team) && chaosTeam.Contains((int)attacker.TeamRole.Team))
				|| (victim.TeamRole.Team == Smod2.API.Team.TUTORIAL && attacker.TeamRole.Team == Smod2.API.Team.TUTORIAL) )
				return true;
			else
				return false;
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{

			AdminToolbox.AddMissingPlayerVariables(new List<Player> { ev.Attacker, ev.Player });

			float originalDamage = ev.Damage;
			DamageType originalType = ev.DamageType;

			float damageMultiplier = ConfigManager.Manager.Config.GetFloatValue("admintoolbox_round_damageMultiplier", 1f, true);
			ev.Damage = originalDamage * damageMultiplier;

			if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.SteamId) && AdminToolbox.ATPlayerDict[ev.Player.SteamId].isJailed) { ev.Damage = 0f; return; }
			if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.ATPlayerDict[ev.Attacker.SteamId].isJailed) { ev.Damage = 0f; return; }

			if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.SteamId)) { if (AdminToolbox.ATPlayerDict[ev.Player.SteamId].godMode) { ev.Damage = 0f; ev.DamageType = DamageType.NONE; ; return; }; }
			if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Attacker.SteamId)) { if (AdminToolbox.ATPlayerDict[ev.Attacker.SteamId].dmgOff) { ev.Damage = 0f; ev.DamageType = DamageType.NONE; ; return; }; }

			int[] allowedDmg = new int[] { -1 };
			if (ev.Player.TeamRole.Role == Role.TUTORIAL)
				allowedDmg = CalculateTutorialDamage();

			int[] DebugDmg = ConfigManager.Manager.Config.GetIntListValue("admintoolbox_debug_damagetypes", humanDamageTypes, false);

			string _roleDamagesDefault = (PluginManager.Manager.Plugins.Where(p => p.Details.id == "cyan.serpents.hand").Count() > 0) ? "2:2" : "14:14";
			string[] roleDamages = ConfigManager.Manager.Config.GetListValue("admintoolbox_block_role_damage", new string[] { _roleDamagesDefault }, false);


			if (ev.DamageType != DamageType.FRAG && AdminToolbox.ATPlayerDict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.ATPlayerDict[ev.Attacker.SteamId].instantKill)
				ev.Damage = ev.Player.GetHealth() + 1;

			if(ev.Player.IsHandcuffed() && humanDamageTypes.Contains((int)ev.DamageType) && ConfigManager.Manager.Config.GetBoolValue("admintoolbox_nokill_captured", false))
			{
				if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.ATPlayerDict[ev.Attacker.SteamId].instantKill)
					ev.Damage = ev.Damage; //basically doing nothing
				else
				{
					ev.Damage = 0f;
					return;
				}
			}

			if (AdminToolbox.isRoundFinished && !ConfigManager.Manager.Config.GetBoolValue("admintoolbox_roledamageblock_onroundend", true)) goto RoundEnd;
			if (roleDamages.Length > 0 && ev.Attacker.PlayerId != ev.Player.PlayerId)
			{
				bool foundPlayer = false;
				foreach (var item in roleDamages)
				{
					string[] myStringKey = item.Trim().Split(':');
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
									if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.ATPlayerDict[ev.Attacker.SteamId].instantKill) continue;
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
			RoundEnd:;
			if (AdminToolbox.isRoundFinished)
			{
				if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.ATPlayerDict[ev.Attacker.SteamId].instantKill)
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
					if (allowedDmg.Contains((int)ev.DamageType) || allowedDmg.Contains(-2)) goto default;
					if (DebugDmg.Contains((int)ev.DamageType) && (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_tutorial", false, false))) plugin.Info(ev.Player.TeamRole.Name + " " + ev.Player.Name + " not allowed damagetype: " + ev.DamageType);
					if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Attacker.SteamId) && AdminToolbox.ATPlayerDict[ev.Attacker.SteamId].instantKill) goto default;
					ev.DamageType = DamageType.NONE;
					ev.Damage = 0f;
					break;
				default:
					if (AdminToolbox.isRoundFinished) break;
					ev.Damage = (ev.DamageType == DamageType.DECONT) ? originalDamage * ConfigManager.Manager.Config.GetFloatValue("admintoolbox_decontamination_damagemultiplier", 1f, true) : ev.Damage;
					if ((ev.Attacker.Name == "Server" && !(ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_server", false, false))) || (ev.Attacker.Name == "Spectator" && !ConfigManager.Manager.Config.GetBoolValue("admintoolbox_debug_spectator", false, false))) return;
					if (IsFriendlyFire(ev.Player, ev.Attacker))
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
			if (ev.Damage >= ev.Player.GetHealth() && AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.SteamId)) AdminToolbox.ATPlayerDict[ev.Player.SteamId].DeathPos = ev.Player.GetPosition();
			AdminToolbox.logManager.AddLog(ev.Attacker.TeamRole.Name + " " + ev.Attacker.Name + " attacked " + ev.Player.TeamRole.Name + " " + ev.Player.Name + " for " + ev.Damage + " damage" + " with: " + ev.DamageType, Managers.LogManager.ServerLogType.PlayerDamage);
		}
	}

	class DieDetect : IEventHandlerPlayerDie
	{
		private Plugin plugin;
		static IConfigFile Config => ConfigManager.Manager.Config;
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
					if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Player.SteamId)) AdminToolbox.ATPlayerDict[ev.Player.SteamId].Deaths++;
					if (!(Config.GetBoolValue("admintoolbox_debug_scp_and_self_killed", false, false)) && ev.Player.Name == ev.Killer.Name) return;
					if (isFriendlyKill())
					{
						string keyWord = (ev.DamageTypeVar == DamageType.FRAG) ? "granaded" : "killed";
						if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Killer.SteamId)) AdminToolbox.ATPlayerDict[ev.Killer.SteamId].TeamKills++;
						if (Config.GetBoolValue("admintoolbox_debug_friendly_kill", true, false))
							if (AdminToolbox.isColored)
								plugin.Info(Colors.ColoredMARole(ev.Killer) + " @#fg=Yellow;" + ev.Killer.Name + "@#fg=DarkRed; " + keyWord + " fellow @#fg=Default;" + Colors.ColoredMARole(ev.Player) + "@#fg=Yellow; " + ev.Player.Name + "@#fg=Default;");
							else
								plugin.Info(ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " " + keyWord + " fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name);
						AdminToolbox.logManager.WriteToLog(new string[] { ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " " + keyWord + " fellow " + ev.Player.TeamRole.Name + " " + ev.Player.Name }, Managers.LogManager.ServerLogType.TeamKill);
					} // Colors.ColoredRole()
					else
					{
						if (Config.GetBoolValue("admintoolbox_debug_player_kill", false, false))
							plugin.Info(ev.Killer.Name + " killed: " + ev.Player.Name);
						if (AdminToolbox.ATPlayerDict.ContainsKey(ev.Killer.SteamId)) AdminToolbox.ATPlayerDict[ev.Killer.SteamId].Kills++;
						AdminToolbox.logManager.WriteToLog(new string[] { ev.Killer.TeamRole.Name + " " + ev.Killer.Name + " killed " + ev.Player.TeamRole.Name + " " + ev.Player.Name }, Managers.LogManager.ServerLogType.KillLog);
					}
					break;
			}
		}
	}
	
	class Colors
	{
		public static string ColoredMARole(Player player)
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