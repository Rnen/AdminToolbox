using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace AdminToolbox.Managers
{
	using API;
	/// <summary>
	/// Contains all <see cref="File"/>-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public partial class ATFileManager
	{
		static IConfigFile Config => ConfigManager.Manager.Config;

		private static readonly bool unifiedStats = Config.GetBoolValue("admintoolbox_stats_unified", true);
		private static readonly string admintoolboxFolderPath = Config.GetStringValue("admintoolbox_folder_path", string.Empty);
		internal static readonly string appFolder = FileManager.GetAppFolder(shared: true);

		internal static readonly string
			AdminToolboxFolder = (admintoolboxFolderPath != string.Empty) ? ((admintoolboxFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString())) ? "AdminToolbox" : Path.DirectorySeparatorChar + "AdminToolbox") : appFolder + "AdminToolbox",
			AdminToolboxPlayerStats = (unifiedStats) ? AdminToolboxFolder + Path.DirectorySeparatorChar + "PlayerStats" + Path.DirectorySeparatorChar + "Global" : AdminToolboxFolder + Path.DirectorySeparatorChar + "PlayerStats" + Path.DirectorySeparatorChar + PluginManager.Manager.Server.Port,
			AdminToolboxLogs = AdminToolboxFolder + Path.DirectorySeparatorChar + "ServerLogs";

		/// <summary>
		/// Read/Writes <see cref ="Player"/> stats to/from <see cref="File"/>
		/// </summary>
		public void PlayerStatsFileManager(PlayerFile FileOperation = PlayerFile.Read)
			=> PlayerStatsFileManager(new string[] { }, FileOperation);

		/// <summary>
		/// Read/Writes the <paramref name="steamID"/>'s stats to/from <see cref="File"/>
		/// </summary>
		public void PlayerStatsFileManager(string steamID, PlayerFile FileOperation = PlayerFile.Write)
			=> PlayerStatsFileManager(new string[] { steamID }, FileOperation);

		/// <summary>
		/// Read/Writes <see cref ="Player"/> stats to/from <see cref="File"/>
		/// </summary>
		public void PlayerStatsFileManager(Player player, PlayerFile FileOperation = PlayerFile.Write)
			=> PlayerStatsFileManager(new string[] { player.SteamId }, FileOperation);

		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each <see cref="Player"/> in the <see cref="List{T}"/>
		/// </summary>
		public void PlayerStatsFileManager(List<Player> playerList, PlayerFile FileOperation = PlayerFile.Read)
		{
			if (playerList != null && playerList.Count > 0)
				PlayerStatsFileManager(playerList.Select(p => p.SteamId).ToArray(), FileOperation);
			else
				PlayerStatsFileManager(FileOperation);
		}
		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each steamID in the <see cref="List{T}"/>
		/// </summary>
		public void PlayerStatsFileManager(List<string> steamIdList, PlayerFile FileOperation = PlayerFile.Read)
		{
			if(steamIdList != null && steamIdList.Count > 0)
				PlayerStatsFileManager(steamIdList.ToArray(), FileOperation);
			else
				PlayerStatsFileManager(new string[] { }, FileOperation);
		}

		public void PlayerStatsFileManager(string[] steamIdArray, PlayerFile FileOperation = PlayerFile.Read)
		{
			if (Directory.Exists(appFolder))
			{
				char splitChar = ';';
				if (!Directory.Exists(AdminToolboxFolder))
					Directory.CreateDirectory(AdminToolboxFolder);
				if (!Directory.Exists(AdminToolboxPlayerStats))
					Directory.CreateDirectory(AdminToolboxPlayerStats);

				if (steamIdArray != null && steamIdArray.Length > 0)
					foreach (string sid in steamIdArray)
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
						case PlayerFile.Write:
							WriteToFile(steamID);
							break;
						case PlayerFile.Read:
						default:
							ReadFromFile(steamID);
							break;
					}
				}
				void WriteToFile(string steamID)
				{
					if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return;
					string playerFilePath = AdminToolboxPlayerStats + Path.DirectorySeparatorChar + steamID + ".txt";
					if (!File.Exists(playerFilePath))
						File.Create(playerFilePath).Dispose();

					if (File.ReadAllLines(playerFilePath).Length <= 2)
						ConvertOldFilesToJSON(playerFilePath);
					SerilizablePlayerClass pl = new SerilizablePlayerClass(AdminToolbox.ATPlayerDict[steamID]);

					if (string.IsNullOrEmpty(pl.PlayerInfo.FirstJoin))
						pl.PlayerInfo.FirstJoin = DateTime.Now.AddMinutes(-pl.PlayerStats.MinutesPlayed).ToString(CultureInfo.InvariantCulture);

					using (StreamWriter sw = new StreamWriter(playerFilePath, false))
					{
						sw.WriteLine(JsonConvert.SerializeObject(pl, Formatting.Indented));
					}

					ReadFromFile(steamID);
				}
				void ReadFromFile(string steamID)
				{
					if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return;
					string playerFilePath = AdminToolboxPlayerStats + Path.DirectorySeparatorChar + steamID + ".txt";
					if (!File.Exists(playerFilePath))
						WriteToFile(steamID);

					if (File.ReadAllLines(playerFilePath).FirstOrDefault().Split(splitChar).Length > 3)
						ConvertOldFilesToJSON(playerFilePath);

					string rawJSON = File.ReadAllText(playerFilePath);
					SerilizablePlayerClass pl = JsonConvert.DeserializeObject<SerilizablePlayerClass>(rawJSON);

					PlayerSettings playersetting = AdminToolbox.ATPlayerDict[steamID];

					if (string.IsNullOrEmpty(pl.PlayerInfo.FirstJoin))
						playersetting.PlayerInfo.FirstJoin = DateTime.Now.AddMinutes(-pl.PlayerStats.MinutesPlayed).ToString(CultureInfo.InvariantCulture);
					playersetting.PlayerStats = pl.PlayerStats;

					/*
					string[] fileStrings = (File.ReadAllLines(playerFilePath).Length > 0) ? File.ReadAllLines(playerFilePath) : new string[] { "0;0;0;0;0" };
					string[] playerStats = fileStrings.FirstOrDefault().Split(splitChar);
					if (AdminToolbox.ATPlayerDict.ContainsKey(steamID))
					{
						PlayerStats stats = AdminToolbox.ATPlayerDict[steamID].PlayerStats;
						stats.Kills = (playerStats.Length > 0 && int.TryParse(playerStats[0], out int x0) && x0 > stats.Kills) ? x0 : stats.Kills;
						stats.TeamKills = (playerStats.Length > 1 && int.TryParse(playerStats[1], out int x1) && x1 > stats.TeamKills) ? x1 : stats.TeamKills;
						stats.Deaths = (playerStats.Length > 2 && int.TryParse(playerStats[2], out int x2) && x2 > stats.Deaths) ? x2 : stats.Deaths;
						stats.MinutesPlayed = (playerStats.Length > 3 && double.TryParse(playerStats[3], out double x3) && x3 > stats.MinutesPlayed) ? x3 : stats.MinutesPlayed;
						stats.BanCount = (playerStats.Length > 4 && int.TryParse(playerStats[4], out int x4) && x4 > stats.BanCount) ? x4 : stats.BanCount;
						AdminToolbox.ATPlayerDict[steamID].PlayerStats = stats;
					}
					*/
				}
			}
		}

		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each steamID in the <see cref="List{T}"/>
		/// </summary>
		public static void ConvertOldFilesToJSON(string file = "")
		{
			int x = 0;
			string[] files = (string.IsNullOrEmpty(file)) ? Directory.GetFiles(AdminToolboxPlayerStats) : new string[] { file };
			if (files.Where(f => !File.ReadAllText(f).StartsWith("{")).Count() >= 100)
			{
				AdminToolbox.plugin.Debug("Warning: The plugin will be converting old playerfiles to a new format." + "\n" + "Beware that this might take some time");
			}
			if (files.Length > 0)
				foreach (string path in files)
				{
					if (!File.Exists(path) || File.ReadAllLines(path).FirstOrDefault().StartsWith("{")) continue;
					try
					{
						string[] rawData = File.ReadAllText(path).Split(';');

						PlayerStats ps = new PlayerStats();
						ps.Kills = (rawData.Length > 0 && int.TryParse(rawData[0], out int x0) && x0 > ps.Kills) ? x0 : ps.Kills;
						ps.TeamKills = (rawData.Length > 1 && int.TryParse(rawData[1], out int x1) && x1 > ps.TeamKills) ? x1 : ps.TeamKills;
						ps.Deaths = (rawData.Length > 2 && int.TryParse(rawData[2], out int x2) && x2 > ps.Deaths) ? x2 : ps.Deaths;
						ps.MinutesPlayed = (rawData.Length > 3 && double.TryParse(rawData[3].Replace(",","."), NumberStyles.Number, CultureInfo.InvariantCulture, out double x3) && x3 > ps.MinutesPlayed && x3 < int.MaxValue) ? x3 : ps.MinutesPlayed;
						ps.BanCount = (rawData.Length > 4 && int.TryParse(rawData[4], out int x4) && x4 > ps.BanCount) ? x4 : ps.BanCount;
						
						int subtractedMinutes = (ps.MinutesPlayed < 0) ? (int)ps.MinutesPlayed : (int)-ps.MinutesPlayed;

						SerilizablePlayerClass playerClass = new SerilizablePlayerClass(ps);
						playerClass.PlayerInfo.FirstJoin = DateTime.Now.Add(TimeSpan.FromMinutes(subtractedMinutes)).ToString(CultureInfo.InvariantCulture);

						using (StreamWriter sw = new StreamWriter(path, false))
						{
							sw.WriteLine(JsonConvert.SerializeObject(playerClass, Formatting.Indented));
						}
						x++;
					}
					catch(Exception e)
					{
						AdminToolbox.plugin.Debug("Failed during convertion of: " + path);
						AdminToolbox.plugin.Debug(e.StackTrace);
						continue;
					}
					// Kills + TeamKills  + Deaths  + minutesPlayed  + BanCount;
				}
			if (x > 0)
				AdminToolbox.plugin.Debug(x + " files converted to new JSON format!");
		}

		/// <summary>
		/// The class <see cref="AdminToolbox"/> uses for JSON Serialize/Deserialize operations
		/// </summary>
		public class SerilizablePlayerClass
		{
			public PlayerInfo PlayerInfo = new PlayerInfo();
			public PlayerStats PlayerStats = new PlayerStats();

			public SerilizablePlayerClass() { }
			public SerilizablePlayerClass(PlayerStats stats)
			{
				this.PlayerStats = stats;
			}
			public SerilizablePlayerClass(PlayerSettings setting)
			{
				this.PlayerInfo = setting.PlayerInfo;
				this.PlayerStats = setting.PlayerStats;
			}
		}

		/// <summary>
		/// Writes current <see cref="AdminToolbox"/> version to <see cref ="File"/>
		/// </summary>
		public static void WriteVersionToFile()
		{
			if (Directory.Exists(appFolder))
			{
				string text = "at_version=" + AdminToolbox.plugin.Details.version.Split('-').FirstOrDefault();
				using (StreamWriter streamWriter = new StreamWriter(appFolder + "at_version.md", false))
				{
					streamWriter.Write(text);
					streamWriter.Close();
				}
				if (File.Exists(appFolder + "n_at_version.md"))
					File.Delete(appFolder + "n_at_version.md");
			}
			else
				AdminToolbox.plugin.Info("Could not find SCP Secret Lab folder!");
		}
	}
}
