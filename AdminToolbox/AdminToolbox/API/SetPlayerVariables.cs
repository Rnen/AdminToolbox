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
		/// For setting <see cref="PlayerSettings"/> booleans
		/// </summary>
		/// <param name="UserID">The ID to look for in the dictionary</param>
		/// <param name="spectatorOnly"><see cref="PlayerSettings.overwatchMode"/></param>
		/// <param name="godMode"><see cref="PlayerSettings.godMode"/></param>
		/// <param name="dmgOff"><see cref="PlayerSettings.dmgOff"/></param>
		/// <param name="destroyDoor"><see cref="PlayerSettings.destroyDoor"/></param>
		/// <param name="keepSettings"><see cref="PlayerSettings.keepSettings"/></param>
		/// <param name="lockDown"><see cref="PlayerSettings.lockDown"/></param>
		/// <param name="instantKill"><see cref="PlayerSettings.instantKill"/></param>
		/// <param name="isJailed"><see cref="PlayerSettings.isJailed"/></param>
		/// <returns>Returns false if <paramref name="UserID"/> is not in <see cref="AdminToolbox.ATPlayerDict"/></returns>
		public static bool SetPlayerBools(string UserID, bool? spectatorOnly = null, bool? godMode = null, bool? dmgOff = null, bool? destroyDoor = null, bool? keepSettings = null, bool? lockDown = null, bool? instantKill = null, bool? isJailed = null)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(UserID, out PlayerSettings setting))
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
		/// <inheritdoc cref="SetPlayerBools(string, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?)"/>
		/// </summary>
		/// <param name="player">The <see cref="Player"/> to look for in the dictionary</param>
		/// <param name="spectatorOnly"><see cref="PlayerSettings.overwatchMode"/></param>
		/// <param name="godMode"><see cref="PlayerSettings.godMode"/></param>
		/// <param name="dmgOff"><see cref="PlayerSettings.dmgOff"/></param>
		/// <param name="destroyDoor"><see cref="PlayerSettings.destroyDoor"/></param>
		/// <param name="keepSettings"><see cref="PlayerSettings.keepSettings"/></param>
		/// <param name="lockDown"><see cref="PlayerSettings.lockDown"/></param>
		/// <param name="instantKill"><see cref="PlayerSettings.instantKill"/></param>
		/// <param name="isJailed"><see cref="PlayerSettings.isJailed"/></param>
		/// <returns>Returns false if <paramref name="player"/> is not in <see cref="AdminToolbox.ATPlayerDict"/></returns>
		
		public static bool SetPlayerBools(Player player, bool? spectatorOnly = null, bool? godMode = null, bool? dmgOff = null, bool? destroyDoor = null, bool? keepSettings = null, bool? lockDown = null, bool? instantKill = null, bool? isJailed = null)
			=> SetPlayerBools(player.UserID, spectatorOnly, godMode, dmgOff, destroyDoor, keepSettings, lockDown, instantKill, isJailed);
		/// <summary>
		/// <inheritdoc cref="SetPlayerBools(string, bool?, bool?, bool?, bool?, bool?, bool?, bool?, bool?)"/>
		/// </summary>
		/// <param name="players">The list of <see cref="Player"/> to look for in the dictionary</param>
		/// <param name="spectatorOnly"><see cref="PlayerSettings.overwatchMode"/></param>
		/// <param name="godMode"><see cref="PlayerSettings.godMode"/></param>
		/// <param name="dmgOff"><see cref="PlayerSettings.dmgOff"/></param>
		/// <param name="destroyDoor"><see cref="PlayerSettings.destroyDoor"/></param>
		/// <param name="keepSettings"><see cref="PlayerSettings.keepSettings"/></param>
		/// <param name="lockDown"><see cref="PlayerSettings.lockDown"/></param>
		/// <param name="instantKill"><see cref="PlayerSettings.instantKill"/></param>
		/// <param name="isJailed"><see cref="PlayerSettings.isJailed"/></param>
		/// <returns>Returns false if one or more <see cref="Player"/> is not in <see cref="AdminToolbox.ATPlayerDict"/></returns>

		public static bool SetPlayerBools(List<Player> players, bool? spectatorOnly = null, bool? godMode = null, bool? dmgOff = null, bool? destroyDoor = null, bool? keepSettings = null, bool? lockDown = null, bool? instantKill = null, bool? isJailed = null)
		{
			int failiures = 0;
			foreach (Player player in players)
				if (!SetPlayerBools(player.UserID, spectatorOnly, godMode, dmgOff, destroyDoor, keepSettings, lockDown, instantKill, isJailed))
					failiures++;
			return !(failiures > 0);
		}

		/// <summary>
		/// For setting <see cref="PlayerStats"/>
		/// </summary>
		/// <param name="UserID">ID of the player to look for</param>
		/// <param name="Kills"><see cref="PlayerStats.Kills"/></param>
		/// <param name="TeamKills"><see cref="PlayerStats.TeamKills"/></param>
		/// <param name="Deaths"><see cref="PlayerStats.Deaths"/></param>
		/// <param name="RoundsPlayed"><see cref="PlayerStats.RoundsPlayed"/></param>
		/// <param name="BanCount"><see cref="PlayerStats.BanCount"/></param>
		/// <returns>Returns false if <paramref name="UserID"/> is not in <see cref="AdminToolbox.ATPlayerDict"/></returns>
		public static bool SetPlayerStats(string UserID, int? Kills = null, int? TeamKills = null, int? Deaths = null, int? RoundsPlayed = null, int? BanCount = null)
		{
			if (AdminToolbox.ATPlayerDict.TryGetValue(UserID, out PlayerSettings settings))
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
