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
		public void PlayerStatsFileManager(List<Player> players = null, PlayerFile FileOperation = PlayerFile.Read)
		{
			if (Directory.Exists(FileManager.GetAppFolder()))
			{
				char splitChar = ';';
				if (!Directory.Exists(AdminToolboxFolder))
					Directory.CreateDirectory(AdminToolboxFolder);
				if (!Directory.Exists(AdminToolboxPlayerStats))
					Directory.CreateDirectory(AdminToolboxPlayerStats);

				if (players != null && players.Count > 0)
					foreach (Player player in players)
						ReadWriteHandler(player, FileOperation);
				else
					foreach (Player player in PluginManager.Manager.Server.GetPlayers())
						ReadWriteHandler(player, FileOperation);

				void ReadWriteHandler(Player pl, PlayerFile Operation)
				{
					if (string.IsNullOrEmpty(pl.SteamId) || pl.Name == "Server" || string.IsNullOrEmpty(pl.Name)) return;
					if (!AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId)) AdminToolbox.AddMissingPlayerVariables(new List<Player> { pl });
					switch (Operation)
					{
						case PlayerFile.Read:
							ReadFromFile(pl);
							break;
						case PlayerFile.Write:
							WriteToFile(pl);
							break;
					}
				}
				void WriteToFile(Player pl)
				{
					string playerFilePath = (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId)) ? AdminToolboxPlayerStats + Path.DirectorySeparatorChar + pl.SteamId + ".txt" : AdminToolboxPlayerStats + Path.DirectorySeparatorChar + "server" + ".txt";
					if (!File.Exists(playerFilePath))
						File.Create(playerFilePath).Dispose();

					//AdminToolbox.AdminToolboxPlayerSettings playerSettings = (AdminToolbox.playerdict.ContainsKey(pl.SteamId)) ? AdminToolbox.playerdict[pl.SteamId] : new AdminToolbox.AdminToolboxPlayerSettings();
					int Kills = (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && AdminToolbox.ATPlayerDict[pl.SteamId].Kills > 0) ? AdminToolbox.ATPlayerDict[pl.SteamId].Kills : 0;
					int TeamKills = (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && AdminToolbox.ATPlayerDict[pl.SteamId].TeamKills > 0) ? AdminToolbox.ATPlayerDict[pl.SteamId].TeamKills : 0;
					int Deaths = (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && AdminToolbox.ATPlayerDict[pl.SteamId].Deaths > 0) ? AdminToolbox.ATPlayerDict[pl.SteamId].Deaths : 0;
					double minutesPlayed = (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && AdminToolbox.ATPlayerDict[pl.SteamId].MinutesPlayed > 0) ? DateTime.Now.Subtract(AdminToolbox.ATPlayerDict[pl.SteamId].JoinTime).TotalMinutes + AdminToolbox.ATPlayerDict[pl.SteamId].MinutesPlayed : 0;
					int BanCount = (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && AdminToolbox.ATPlayerDict[pl.SteamId].banCount > 0) ? AdminToolbox.ATPlayerDict[pl.SteamId].banCount : 0;
					if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId)) AdminToolbox.ATPlayerDict[pl.SteamId].JoinTime = DateTime.Now;
					string str = string.Empty + Kills + splitChar + TeamKills + splitChar + Deaths + splitChar + minutesPlayed + splitChar + BanCount;
					using (StreamWriter streamWriter = new StreamWriter(playerFilePath, false))
					{
						streamWriter.Write(str);
						streamWriter.Close();
					}
					ReadFromFile(pl);
				}
				void ReadFromFile(Player pl)
				{
					string playerFilePath = (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId)) ? AdminToolboxPlayerStats + Path.DirectorySeparatorChar + pl.SteamId + ".txt" : AdminToolboxPlayerStats + Path.DirectorySeparatorChar + "server" + ".txt";
					if (!File.Exists(playerFilePath))
						PlayerStatsFileManager(new List<Player> { pl }, PlayerFile.Write);
					string[] fileStrings = (File.ReadAllLines(playerFilePath).Length > 0) ? File.ReadAllLines(playerFilePath) : new string[] { "0;0;0;0;0" };
					string[] playerStats = fileStrings.First().Split(splitChar);
					if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId))
					{
						//AdminToolbox.AdminToolboxPlayerSettings myPlayer = AdminToolbox.playerdict[pl.SteamId];
						AdminToolbox.ATPlayerDict[pl.SteamId].Kills = (playerStats.Length > 0 && int.TryParse(playerStats[0], out int x0) && x0 > AdminToolbox.ATPlayerDict[pl.SteamId].Kills) ? x0 : AdminToolbox.ATPlayerDict[pl.SteamId].Kills;
						AdminToolbox.ATPlayerDict[pl.SteamId].TeamKills = (playerStats.Length > 1 && int.TryParse(playerStats[1], out int x1) && x1 > AdminToolbox.ATPlayerDict[pl.SteamId].TeamKills) ? x1 : AdminToolbox.ATPlayerDict[pl.SteamId].TeamKills;
						AdminToolbox.ATPlayerDict[pl.SteamId].Deaths = (playerStats.Length > 2 && int.TryParse(playerStats[2], out int x2) && x2 > AdminToolbox.ATPlayerDict[pl.SteamId].Deaths) ? x2 : AdminToolbox.ATPlayerDict[pl.SteamId].Deaths;
						AdminToolbox.ATPlayerDict[pl.SteamId].MinutesPlayed = (playerStats.Length > 3 && double.TryParse(playerStats[3], out double x3) && x3 > AdminToolbox.ATPlayerDict[pl.SteamId].MinutesPlayed) ? x3 : AdminToolbox.ATPlayerDict[pl.SteamId].MinutesPlayed;
						AdminToolbox.ATPlayerDict[pl.SteamId].banCount = (playerStats.Length > 4 && int.TryParse(playerStats[4], out int x4) && x4 > AdminToolbox.ATPlayerDict[pl.SteamId].banCount) ? x4 : AdminToolbox.ATPlayerDict[pl.SteamId].banCount;
						//AdminToolbox.playerdict[pl.SteamId] = myPlayer;
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
				string text = "at_version=" + AdminToolbox.plugin.Details.version;
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
	/// Functions for modifying <see cref="Managers.PlayerSettings"/> on <see cref="Player"/>s
	/// </summary>
	public static class SetPlayerVariables
	{
		/// <summary>
		/// For setting <see cref="Managers.PlayerSettings"/> bools
		/// <para>Returns false if <paramref name="steamID"/> is not in <see cref="AdminToolbox.ATPlayerDict"/></para>
		/// </summary>
		public static bool SetPlayerBools(string steamID, bool? spectatorOnly = null, bool? godMode = null, bool? dmgOff = null, bool? destroyDoor = null, bool? keepSettings = null, bool? lockDown = null, bool? instantKill = null, bool? isJailed = null)
		{
			if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return false;
			AdminToolbox.ATPlayerDict[steamID].overwatchMode = (spectatorOnly.HasValue) ? (bool)spectatorOnly : AdminToolbox.ATPlayerDict[steamID].overwatchMode;
			AdminToolbox.ATPlayerDict[steamID].godMode = (godMode.HasValue) ? (bool)godMode : AdminToolbox.ATPlayerDict[steamID].godMode;
			AdminToolbox.ATPlayerDict[steamID].dmgOff = (dmgOff.HasValue) ? (bool)dmgOff : AdminToolbox.ATPlayerDict[steamID].dmgOff;
			AdminToolbox.ATPlayerDict[steamID].destroyDoor = (destroyDoor.HasValue) ? (bool)destroyDoor : AdminToolbox.ATPlayerDict[steamID].destroyDoor;
			AdminToolbox.ATPlayerDict[steamID].lockDown = (lockDown.HasValue) ? (bool)lockDown : AdminToolbox.ATPlayerDict[steamID].lockDown;
			AdminToolbox.ATPlayerDict[steamID].instantKill = (instantKill.HasValue) ? (bool)instantKill : AdminToolbox.ATPlayerDict[steamID].instantKill;
			AdminToolbox.ATPlayerDict[steamID].isJailed = (isJailed.HasValue) ? (bool)isJailed : AdminToolbox.ATPlayerDict[steamID].isJailed;
			return true;
		}
		/// <summary>
		/// For setting <see cref="Managers.PlayerSettings"/> stats
		/// <para>Returns false if <paramref name="steamID"/> is not in <see cref="AdminToolbox.ATPlayerDict"/></para>
		/// </summary>
		public static bool SetPlayerStats(string steamID, int? Kills = null, int? TeamKills = null, int? Deaths = null, int? RoundsPlayed = null, int? BanCount = null)
		{
			if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return false;
			AdminToolbox.ATPlayerDict[steamID].Kills = (Kills.HasValue) ? (int)Kills : AdminToolbox.ATPlayerDict[steamID].Kills;
			AdminToolbox.ATPlayerDict[steamID].TeamKills = (TeamKills.HasValue) ? (int)TeamKills : AdminToolbox.ATPlayerDict[steamID].TeamKills; ;
			AdminToolbox.ATPlayerDict[steamID].Deaths = (Deaths.HasValue) ? (int)Deaths : AdminToolbox.ATPlayerDict[steamID].Deaths;
			AdminToolbox.ATPlayerDict[steamID].RoundsPlayed = (RoundsPlayed.HasValue) ? (int)RoundsPlayed : AdminToolbox.ATPlayerDict[steamID].RoundsPlayed;
			AdminToolbox.ATPlayerDict[steamID].banCount = (BanCount.HasValue) ? (int)BanCount : AdminToolbox.ATPlayerDict[steamID].banCount;
			return true;
		}
	}
}
