using System;
using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace AdminToolbox
{
	using API;
	using API.Extentions;

	internal class PlayerDamageEvent : IEventHandlerPlayerHurt
	{
		private IConfigFile Config => ConfigManager.Manager.Config;

		private readonly AdminToolbox plugin;

		private Dictionary<string, PlayerSettings> Dict => AdminToolbox.ATPlayerDict;

		public PlayerDamageEvent(AdminToolbox plugin) => this.plugin = plugin;

		private int[] CalculateTutorialDamage()
		{
			List<int> _tutDamageList = new List<int>();
			string _tutDefaultDmgAllowed = PluginManager.Manager.Plugins
				.Any(p => p.Details.id.ToLower().Contains("serpents.hand")) ? "*" : ((int)DamageType.NONE).ToString();
			string[] _allowedDmgConfig =
				Config.GetListValue(
					"admintoolbox_tutorial_dmg_allowed",
					new string[] { _tutDefaultDmgAllowed.ToString() },
					false
				);

			if (_allowedDmgConfig.Count() > 0)
			{
				if (_allowedDmgConfig.Any(item => Utility.AllAliasWords.Contains(item.ToUpper())))
				{
					_tutDamageList.Clear();
					for (int dmgIndex = 0; dmgIndex < Enum.GetNames(typeof(DamageType)).Length; dmgIndex++)
						_tutDamageList.Add(dmgIndex);
				}
				else
					foreach (string str in _allowedDmgConfig)
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

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			Managers.ATFile.AddMissingPlayerVariables(new Player[] { ev.Attacker, ev.Player });

			Dict.TryGetValue(ev.Player.UserID, out PlayerSettings playerSetting);
			Dict.TryGetValue(ev.Attacker.UserID, out PlayerSettings attackerSetting);

			float originalDamage = ev.Damage;
			DamageType originalType = ev.DamageType;

			float damageMultiplier = Config.GetFloatValue("admintoolbox_round_damageMultiplier", 1f);
			ev.Damage = originalDamage * damageMultiplier;

			if ((playerSetting?.isJailed ?? false) || (playerSetting?.godMode ?? false))
				ev.Damage = 0f;
			else if ((attackerSetting?.isJailed ?? false) || (attackerSetting?.dmgOff ?? false))
				ev.Damage = 0f;
			//else if (ev.DamageType == DamageType.FLYING_DETECTION && Config.GetBoolValue("admintoolbox_antifly_disable", false))
				//ev.Damage = 0f;


			int[] allowedTutDmg = new int[] { -1 };
			if (ev.Player.PlayerRole.RoleID == Smod2.API.RoleType.TUTORIAL)
				allowedTutDmg = CalculateTutorialDamage();

			int[] DebugDmg = Config.GetIntListValue("admintoolbox_debug_damagetypes", Utility.HumanDamageTypes, false);

			string _roleDamagesDefault = PluginManager.Manager.EnabledPlugins.Any(p => p.Details.id.ToLower() == "cyan.serpents.hand") ? "2:2" : "14:14";
			string[] roleDamages = Config.GetListValue("admintoolbox_block_role_damage", new string[] { _roleDamagesDefault }, false);
			if (roleDamages.Length == 1 && roleDamages[0].ToUpper() == "DEFAULT")
				roleDamages[0] = _roleDamagesDefault;


			if (ev.DamageType != DamageType.EXPLOSION && (attackerSetting?.instantKill ?? false))
				ev.Damage = ev.Player.Health + ev.Player.ArtificialHealth + 1;

			if (ev.Player.IsHandcuffed() && Utility.HumanDamageTypes.Contains((int)ev.DamageType) && Config.GetBoolValue("admintoolbox_nokill_captured", false))
			{
				if (!attackerSetting?.instantKill ?? true)
				{
					ev.Damage = 0f;
					return;
				}
			}

			if (AdminToolbox.isRoundFinished && !Config.GetBoolValue("admintoolbox_roledamageblock_onroundend", true)) goto RoundEnd;
			if (roleDamages.Length > 0 && ev.Attacker.PlayerID != ev.Player.PlayerID)
			{
				bool foundPlayer = false;
				foreach (string item in roleDamages)
				{
					string[] myStringKey = item.Trim().Split(':');
					if (int.TryParse(myStringKey[0], out int attackerIntRole) && Utility.TryParseRole(attackerIntRole, out Smod2.API.RoleType attackerRole))
					{
						string[] myString = myStringKey[1].Split('.', '-', '#', '_', ',', '+', '@', '>', '<', ';');
						if (myString.Length >= 1)
						{
							foreach (string item2 in myString)
							{
								if (int.TryParse(item2, out int victimIntRole) && Utility.TryParseRole(victimIntRole, out Smod2.API.RoleType victimRole))
								{
									if (attackerRole == ev.Attacker.PlayerRole.RoleID && victimRole == ev.Player.PlayerRole.RoleID)
									{
										if (attackerSetting?.instantKill ?? false) continue;
										ev.Damage = 0f;
										//ev.DamageType = DamageType.NONE;
										plugin.Debug(ev.Attacker.PlayerRole.Name + " " + ev.Attacker.Name + "was blocked from attacking " + ev.Player.PlayerRole.Name + " " + ev.Player + " with " + ev.DamageType);
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
					else
					{
						plugin.Info("Not a valid config at \"admintoolbox_block_role_damage\"  Value: " + myStringKey[0] + ":" + myStringKey[1] + "\nSkipping entry...");
						continue;
					}
				}
			}
RoundEnd:;
			if (AdminToolbox.isRoundFinished)
			{
				float enddamageMultiplier = Config.GetFloatValue("admintoolbox_endedRound_damageMultiplier", 1f, true);
				if (!(attackerSetting?.instantKill ?? false))
					ev.Damage = originalDamage * enddamageMultiplier;
			}
			switch (ev.Player.PlayerRole.RoleID)
			{
				case Smod2.API.RoleType.TUTORIAL:
					if (allowedTutDmg.Contains((int)ev.DamageType) || allowedTutDmg.Contains(-2))
						goto default;
					if (DebugDmg.Contains((int)ev.DamageType) && Config.GetBoolValue("admintoolbox_debug_tutorial", false, false))
						plugin.Info(ev.Player.PlayerRole.Name + " " + ev.Player.Name + " not allowed damagetype: " + ev.DamageType);
					if ((attackerSetting?.instantKill ?? false) && Config.GetBoolValue("admintoolbox_instantkill_affects_tutorials", true))
						goto default;

					//ev.DamageType = DamageType.NONE;
					ev.Damage = 0f;
					break;
				default:
					if (AdminToolbox.isRoundFinished) break;
					ev.Damage = (ev.DamageType == DamageType.DECONTAMINATION) ? originalDamage * Config.GetFloatValue("admintoolbox_decontamination_damagemultiplier", 1f, true) : ev.Damage;
					if ((ev.Attacker.Name == "Server" && !Config.GetBoolValue("admintoolbox_debug_server", false, false)) || (ev.Attacker.Name == "Spectator" &&
						!Config.GetBoolValue("admintoolbox_debug_spectator", false, false))) return;
					if (Utility.IsTeam(ev.Player, ev.Attacker))
					{
						if (Config.GetBoolValue("admintoolbox_debug_friendly_damage", false, false))
						{
							ev.Damage = (ev.Damage >= 1) ? Config.GetFloatValue("admintoolbox_friendlyfire_damagemultiplier", 1f) * originalDamage : ev.Damage;
							if (DebugDmg.Contains((int)ev.DamageType) && !AdminToolbox.isRoundFinished)
								plugin.Info(ev.Attacker.PlayerRole.Name + " " + ev.Attacker.Name + " attacked fellow " + ev.Player.PlayerRole.Name + " " + ev.Player.Name + /*" for " + damage +^*/ " with " + ev.DamageType);
						}
					}
					else if (Config.GetBoolValue("admintoolbox_debug_player_damage", false, false))
					{
						if (DebugDmg.Contains((int)ev.DamageType) && !AdminToolbox.isRoundFinished)
							plugin.Info(ev.Attacker.PlayerRole.Name + " " + ev.Attacker.Name + " attacked " + ev.Player.PlayerRole.Name + " " + ev.Player.Name + /*" for " + ev.Damage + " damage" +*/ " with: " + ev.DamageType);
					}
					break;
			}
			if (ev.Damage >= (ev.Player.Health + ev.Player.ArtificialHealth) && playerSetting != null)
			{
				playerSetting.DeathPos = ev.Player.GetPosition();
				if (playerSetting.grenadeMode)
					ev.Player.ThrowGrenade(GrenadeType.FRAG_GRENADE, Vector.Zero, throwForce: 0f, slowThrow: true);
			}
			AdminToolbox.LogManager.WriteToLog(ev.Attacker.PlayerRole.Name + " " + ev.Attacker.Name + " attacked " + ev.Player.PlayerRole.Name + " " + ev.Player.Name + " for " + ev.Damage + " damage" + " with: " + ev.DamageType, Managers.LogManager.ServerLogType.PlayerDamage);
		}
	}

	public class PlayerDieEvent : IEventHandlerPlayerDie
	{
		private readonly Plugin plugin;

		private IConfigFile Config => ConfigManager.Manager.Config;

		private Dictionary<string, PlayerSettings> Dict => AdminToolbox.ATPlayerDict;

		public PlayerDieEvent(Plugin plugin) => this.plugin = plugin;

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			Managers.ATFile.AddMissingPlayerVariables(new Player[] { ev.Player, ev.Killer });
			Dict.TryGetValue(ev.Player.UserID, out PlayerSettings playerSetting);
			Dict.TryGetValue(ev.Killer.UserID, out PlayerSettings killerSetting);

			switch ((int)ev.Player.PlayerRole.RoleID)
			{
				case 3:
					if (ev.DamageTypeVar == DamageType.RECONTAINED)
					{
						ev.SpawnRagdoll = false;
						//ev.DamageTypeVar = DamageType.RAGDOLLLESS;
					}
					goto default;
				default:
					if (AdminToolbox.isRoundFinished)
						break;
					if (playerSetting != null && ev.Killer?.PlayerID != ev.Player?.PlayerID)
						playerSetting.PlayerStats.Deaths++;
					if (ev.Player?.PlayerID == ev.Killer?.PlayerID && !Config.GetBoolValue("admintoolbox_debug_scp_and_self_killed", false, false))
						return;
					if (Utility.IsTeam(ev.Player, ev.Killer))
					{
						string keyWord = (ev.DamageTypeVar == DamageType.EXPLOSION) ? "granaded" : "killed";
						if (killerSetting != null && ev.Killer.PlayerID != ev.Player.PlayerID) 
							killerSetting.PlayerStats.TeamKills++;
						if (Config.GetBoolValue("admintoolbox_debug_friendly_kill", true, false))
							if (AdminToolbox.isColored)
								plugin.Info(ev.Killer.ToColoredMultiAdminTeam() + " @#fg=Yellow;" + ev.Killer.Name + "@#fg=DarkRed; " + keyWord + " fellow @#fg=Default;" + ev.Player.ToColoredMultiAdminTeam() + "@#fg=Yellow; " + ev.Player.Name + "@#fg=Default;");
							else
								plugin.Info(ev.Killer.PlayerRole.Name + " " + ev.Killer.Name + " " + keyWord + " fellow " + ev.Player.PlayerRole.Name + " " + ev.Player.Name);
						AdminToolbox.LogManager.WriteToLog(ev.Killer.PlayerRole.Name + " " + ev.Killer.Name + " " + keyWord + " fellow " + ev.Player.PlayerRole.Name + " " + ev.Player.Name, Managers.LogManager.ServerLogType.TeamKill);
					}
					else
					{
						if (Config.GetBoolValue("admintoolbox_debug_player_kill", false, false))
							plugin.Info(ev.Killer.Name + " killed: " + ev.Player.Name);
						if (killerSetting != null && ev.Killer.PlayerID != ev.Player.PlayerID)
							killerSetting.PlayerStats.Kills++;
						AdminToolbox.LogManager.WriteToLog(ev.Killer.PlayerRole.Name + " " + ev.Killer.Name + " killed " + ev.Player.PlayerRole.Name + " " + ev.Player.Name, Managers.LogManager.ServerLogType.KillLog);
					}
					break;
			}
			if (ev.Player.PlayerID == ev.Killer.PlayerID && playerSetting != null)
				playerSetting.PlayerStats.SuicideCount++;
		}
	}
}
