using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Managers
{
	/// <summary>
	/// Contains all <see cref="File"/>-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public partial class ATFileManager
	{
		static IConfigFile Config => ConfigManager.Manager.Config;

		private static readonly bool unifiedStats = Config.GetBoolValue("admintoolbox_stats_unified", true);
		private static readonly string admintoolboxFolderPath = Config.GetStringValue("admintoolbox_folder_path", string.Empty);
		private static readonly string appFolder = FileManager.GetAppFolder();

		internal static readonly string
			AdminToolboxFolder = (admintoolboxFolderPath != string.Empty) ? ((admintoolboxFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString())) ? "AdminToolbox" : Path.DirectorySeparatorChar + "AdminToolbox") : appFolder + "AdminToolbox",
			AdminToolboxPlayerStats = (unifiedStats) ? AdminToolboxFolder + Path.DirectorySeparatorChar + "PlayerStats" + Path.DirectorySeparatorChar + "Global" : AdminToolboxFolder + Path.DirectorySeparatorChar + "PlayerStats" + Path.DirectorySeparatorChar + PluginManager.Manager.Server.Port,
			AdminToolboxLogs = AdminToolboxFolder + Path.DirectorySeparatorChar + "ServerLogs";

		/// <summary>
		/// Read/Writes <see cref ="Player"/> stats to/from <see cref="File"/>
		/// </summary>
		public void PlayerStatsFileManager(PlayerFile FileOperation = PlayerFile.Read)
			=> PlayerStatsFileManager(new List<string>() { }, FileOperation);

		/// <summary>
		/// Read/Writes the <paramref name="steamID"/>'s stats to/from <see cref="File"/>
		/// </summary>
		public void PlayerStatsFileManager(string steamID, PlayerFile FileOperation = PlayerFile.Write)
			=> PlayerStatsFileManager(new List<string>() { steamID }, FileOperation);

		/// <summary>
		/// Read/Writes <see cref ="Player"/> stats to/from <see cref="File"/>
		/// </summary>
		public void PlayerStatsFileManager(Player player, PlayerFile FileOperation = PlayerFile.Write)
			=> PlayerStatsFileManager(new List<string>() { player.SteamId }, FileOperation);

		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each <see cref="Player"/> in the <see cref="List{T}"/>
		/// </summary>
		public void PlayerStatsFileManager(List<Player> playerList, PlayerFile FileOperation = PlayerFile.Read)
		{
			if (playerList != null && playerList.Count > 0)
				PlayerStatsFileManager(playerList.Select(p => p.SteamId).ToList(), FileOperation);
			else
				PlayerStatsFileManager(FileOperation);
		}
		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each steamID in the <see cref="List{T}"/>
		/// </summary>
		public void PlayerStatsFileManager(List<string> steamIdList, PlayerFile FileOperation = PlayerFile.Read)
		{
			if (Directory.Exists(FileManager.GetAppFolder()))
			{
				char splitChar = ';';
				if (!Directory.Exists(AdminToolboxFolder))
					Directory.CreateDirectory(AdminToolboxFolder);
				if (!Directory.Exists(AdminToolboxPlayerStats))
					Directory.CreateDirectory(AdminToolboxPlayerStats);

				if (steamIdList != null && steamIdList.Count > 0)
					foreach (string sid in steamIdList)
						ReadWriteHandler(sid, FileOperation);
				else
					foreach (string steamID in AdminToolbox.ATPlayerDict.Keys)
						ReadWriteHandler(steamID, FileOperation);

				void ReadWriteHandler(string steamID, PlayerFile Operation)
				{
					if (string.IsNullOrEmpty(steamID)) return;
					if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID))
					{
						AdminToolbox.AddMissingPlayerVariables(PluginManager.Manager.Server.GetPlayers(steamID));
						return;
					};
					switch (Operation)
					{
						case PlayerFile.Read:
							ReadFromFile(steamID);
							break;
						case PlayerFile.Write:
							WriteToFile(steamID);
							break;
						default:
							ReadFromFile(steamID);
							break;
					}
				}
				void WriteToFile(string steamID)
				{
					if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID))
					{
						if (PluginManager.Manager.Server.GetPlayers(steamID).Count < 1) return;
						AdminToolbox.AddMissingPlayerVariables(PluginManager.Manager.Server.GetPlayers(steamID).FirstOrDefault());
					}
					if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return;
					string playerFilePath = (AdminToolbox.ATPlayerDict.ContainsKey(steamID)) ? AdminToolboxPlayerStats + Path.DirectorySeparatorChar + steamID + ".txt" : AdminToolboxPlayerStats + Path.DirectorySeparatorChar + "server" + ".txt";
					if (!File.Exists(playerFilePath))
						File.Create(playerFilePath).Dispose();

					API.PlayerSettings setting = (AdminToolbox.ATPlayerDict.ContainsKey(steamID)) ? AdminToolbox.ATPlayerDict[steamID] : new API.PlayerSettings(steamID);
					int Kills = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].Kills > 0) ? AdminToolbox.ATPlayerDict[steamID].Kills : 0;
					int TeamKills = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].TeamKills > 0) ? AdminToolbox.ATPlayerDict[steamID].TeamKills : 0;
					int Deaths = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].Deaths > 0) ? AdminToolbox.ATPlayerDict[steamID].Deaths : 0;
					double minutesPlayed = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].MinutesPlayed > 0) ? DateTime.Now.Subtract(AdminToolbox.ATPlayerDict[steamID].JoinTime).TotalMinutes + AdminToolbox.ATPlayerDict[steamID].MinutesPlayed : 0;
					int BanCount = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].banCount > 0) ? AdminToolbox.ATPlayerDict[steamID].banCount : 0;
					if (AdminToolbox.ATPlayerDict.ContainsKey(steamID)) AdminToolbox.ATPlayerDict[steamID].JoinTime = DateTime.Now;
					string str = string.Empty + Kills + splitChar + TeamKills + splitChar + Deaths + splitChar + minutesPlayed + splitChar + BanCount;
					using (StreamWriter streamWriter = new StreamWriter(playerFilePath, false))
					{
						streamWriter.Write(str);
						streamWriter.Close();
					}
					ReadFromFile(steamID);
				}
				void ReadFromFile(string steamID)
				{
					string playerFilePath = (AdminToolbox.ATPlayerDict.ContainsKey(steamID)) ? AdminToolboxPlayerStats + Path.DirectorySeparatorChar + steamID + ".txt" : AdminToolboxPlayerStats + Path.DirectorySeparatorChar + "server" + ".txt";
					if (!File.Exists(playerFilePath))
						PlayerStatsFileManager(new List<string> { steamID }, PlayerFile.Write);
					string[] fileStrings = (File.ReadAllLines(playerFilePath).Length > 0) ? File.ReadAllLines(playerFilePath) : new string[] { "0;0;0;0;0" };
					string[] playerStats = fileStrings.FirstOrDefault().Split(splitChar);
					if (AdminToolbox.ATPlayerDict.ContainsKey(steamID))
					{
						API.PlayerSettings setting = AdminToolbox.ATPlayerDict[steamID];
						setting.Kills = (playerStats.Length > 0 && int.TryParse(playerStats[0], out int x0) && x0 > setting.Kills) ? x0 : setting.Kills;
						setting.TeamKills = (playerStats.Length > 1 && int.TryParse(playerStats[1], out int x1) && x1 > setting.TeamKills) ? x1 : setting.TeamKills;
						setting.Deaths = (playerStats.Length > 2 && int.TryParse(playerStats[2], out int x2) && x2 > setting.Deaths) ? x2 : setting.Deaths;
						setting.MinutesPlayed = (playerStats.Length > 3 && double.TryParse(playerStats[3], out double x3) && x3 > setting.MinutesPlayed) ? x3 : setting.MinutesPlayed;
						setting.banCount = (playerStats.Length > 4 && int.TryParse(playerStats[4], out int x4) && x4 > setting.banCount) ? x4 : setting.banCount;
						AdminToolbox.ATPlayerDict[steamID] = setting;
					}
				}
			}
		}

		/// <summary>
		/// Writes current <see cref="AdminToolbox"/> version to <see cref ="File"/>
		/// </summary>
		public static void WriteVersionToFile()
		{
			if (Directory.Exists(FileManager.GetAppFolder()))
			{
				string text = "at_version=" + AdminToolbox.plugin.Details.version.Split('-').FirstOrDefault();
				using (StreamWriter streamWriter = new StreamWriter(FileManager.GetAppFolder() + "at_version.md", false))
				{
					streamWriter.Write(text);
					streamWriter.Close();
				}
				if (File.Exists(FileManager.GetAppFolder() + "n_at_version.md"))
					File.Delete(FileManager.GetAppFolder() + "n_at_version.md");
			}
			else
				AdminToolbox.plugin.Info("Could not find SCP Secret Lab folder!");
		}
	}
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
			if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return false;
			API.PlayerSettings setting = AdminToolbox.ATPlayerDict[steamID];
			setting.overwatchMode =  spectatorOnly ?? setting.overwatchMode;
			setting.godMode = godMode ?? setting.godMode;
			setting.dmgOff = dmgOff ?? setting.dmgOff;
			setting.destroyDoor = destroyDoor ?? setting.destroyDoor;
			setting.lockDown = lockDown ?? setting.lockDown;
			setting.instantKill = instantKill ?? setting.instantKill;
			setting.isJailed = isJailed ?? setting.isJailed;
			AdminToolbox.ATPlayerDict[steamID] = setting;
			return true;
		}
		/// <summary>
		/// For setting <see cref="API.PlayerSettings"/> bools on a <see cref="Player"/>
		/// <para>Returns false if <paramref name="player"/>'s steamID is not in <see cref="AdminToolbox.ATPlayerDict"/></para>
		/// </summary>
		public static bool SetPlayerBools(Player player, bool? spectatorOnly = null, bool? godMode = null, bool? dmgOff = null, bool? destroyDoor = null, bool? keepSettings = null, bool? lockDown = null, bool? instantKill = null, bool? isJailed = null)
		{
			return SetPlayerBools(player.SteamId, spectatorOnly, godMode, dmgOff, destroyDoor, keepSettings, lockDown, instantKill, isJailed);
		}
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
		/// For setting <see cref="API.PlayerSettings"/> stats
		/// <para>Returns false if <paramref name="steamID"/> is not in <see cref="AdminToolbox.ATPlayerDict"/></para>
		/// </summary>
		public static bool SetPlayerStats(string steamID, int? Kills = null, int? TeamKills = null, int? Deaths = null, int? RoundsPlayed = null, int? BanCount = null)
		{
			if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return false;
			API.PlayerSettings setting = AdminToolbox.ATPlayerDict[steamID];
			setting.Kills = Kills ?? setting.Kills;
			setting.TeamKills = TeamKills ?? setting.TeamKills;
			setting.Deaths = Deaths ?? setting.Deaths;
			setting.RoundsPlayed = RoundsPlayed ?? setting.RoundsPlayed;
			setting.banCount = BanCount ?? setting.banCount;
			AdminToolbox.ATPlayerDict[steamID] = setting;
			return true;
		}
	}
}
