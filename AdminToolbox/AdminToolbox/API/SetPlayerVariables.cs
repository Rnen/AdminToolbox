using System.Collections.Generic;
using Smod2.API;

namespace AdminToolbox.API
{
	/// <summary>
	/// Class containing methods for modifying <see cref="API.PlayerSettings"/> on <see cref="Player"/>s
	/// </summary>
	public static class SetPlayerVariables
	{
		/// <summary>
		/// For setting <see cref="API.PlayerSettings"/> bools by <paramref name="steamID"/>
		/// <para>Returns false if <paramref name="steamID"/> is not in <see cref="AdminToolbox.ATPlayerDict"/></para>
		/// </summary>
		public static bool SetPlayerBools(string steamID, bool? spectatorOnly = null, bool? godMode = null, bool? dmgOff = null, bool? destroyDoor = null, bool? keepSettings = null, bool? lockDown = null, bool? instantKill = null, bool? isJailed = null)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(steamID, out PlayerSettings setting))
			{
				setting.overwatchMode = spectatorOnly ?? setting.overwatchMode;
				setting.godMode = godMode ?? setting.godMode;
				setting.dmgOff = dmgOff ?? setting.dmgOff;
				setting.destroyDoor = destroyDoor ?? setting.destroyDoor;
				setting.lockDown = lockDown ?? setting.lockDown;
				setting.instantKill = instantKill ?? setting.instantKill;
				setting.isJailed = isJailed ?? setting.isJailed;
				return true;
			}
			else
				return false;

		}
		/// <summary>
		/// For setting <see cref="API.PlayerSettings"/> bools on a <see cref="Player"/>
		/// <para>Returns false if <paramref name="player"/>'s steamID is not in <see cref="AdminToolbox.ATPlayerDict"/></para>
		/// </summary>
		public static bool SetPlayerBools(Player player, bool? spectatorOnly = null, bool? godMode = null, bool? dmgOff = null, bool? destroyDoor = null, bool? keepSettings = null, bool? lockDown = null, bool? instantKill = null, bool? isJailed = null)
			=> SetPlayerBools(player.SteamId, spectatorOnly, godMode, dmgOff, destroyDoor, keepSettings, lockDown, instantKill, isJailed);
		/// <summary>
		/// For setting <see cref="API.PlayerSettings"/> bools on a list of <see cref="Player"/>s
		/// <para>Returns false if one or more of <paramref name="players"/> steamid's is not in <see cref="AdminToolbox.ATPlayerDict"/></para>
		/// </summary>
		public static bool SetPlayerBools(List<Player> players, bool? spectatorOnly = null, bool? godMode = null, bool? dmgOff = null, bool? destroyDoor = null, bool? keepSettings = null, bool? lockDown = null, bool? instantKill = null, bool? isJailed = null)
		{
			int failiures = 0;
			foreach (Player player in players)
				if (!SetPlayerBools(player.SteamId, spectatorOnly, godMode, dmgOff, destroyDoor, keepSettings, lockDown, instantKill, isJailed))
					failiures++;
			return !(failiures > 0);
		}
		/// <summary>
		/// For setting <see cref="PlayerStats"/>
		/// <para>Returns false if <paramref name="steamID"/> is not in <see cref="AdminToolbox.ATPlayerDict"/></para>
		/// </summary>
		public static bool SetPlayerStats(string steamID, int? Kills = null, int? TeamKills = null, int? Deaths = null, int? RoundsPlayed = null, int? BanCount = null)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(steamID, out PlayerSettings settings))
			{
				PlayerStats stats = settings.PlayerStats;
				stats.Kills = Kills ?? stats.Kills;
				stats.TeamKills = TeamKills ?? stats.TeamKills;
				stats.Deaths = Deaths ?? stats.Deaths;
				stats.RoundsPlayed = RoundsPlayed ?? stats.RoundsPlayed;
				stats.BanCount = BanCount ?? stats.BanCount;
				settings.PlayerStats = stats;
				return true;
			}
			else
				return false;
		}
	}
}
