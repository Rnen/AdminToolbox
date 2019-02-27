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
using System.IO.Compression;

namespace AdminToolbox.Managers
{
	using API;
	/// <summary>
	/// Contains all <see cref="File"/>-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public partial class ATFileManager
	{
		private static IConfigFile Config => ConfigManager.Manager.Config;
		private static int Port => PluginManager.Manager.Server.Port;

		internal const char splitChar = ';';

		private static string AdmintoolboxFolderPath => Config.GetStringValue("admintoolbox_folder_path", string.Empty);

		internal enum Folder
		{
			AppData,
			ATFolder,
			PlayerFiles,
			Logs,
			Warps
		}

		private static string GetATFolderPath()
		{
			string cnfgpath = Config.GetStringValue("admintoolbox_folder_path", string.Empty);
			if (!string.IsNullOrEmpty(cnfgpath))
			{
				if (!cnfgpath.EndsWith(Path.DirectorySeparatorChar.ToString()))
					cnfgpath += Path.DirectorySeparatorChar;
				cnfgpath += "AdminToolbox" + Path.DirectorySeparatorChar;
			}
			else
			{
				cnfgpath = FileManager.GetAppFolder(shared: true, addseparator: true) + "AdminToolbox" + Path.DirectorySeparatorChar;
			}
			if (!Directory.Exists(cnfgpath))
				Directory.CreateDirectory(cnfgpath);
			return cnfgpath;
		}

		internal static string GetFolder(Folder folder = Folder.ATFolder)
		{
			
			string ret = GetATFolderPath();
			switch (folder)
			{
				case Folder.PlayerFiles:
					ret += "PlayerStats" + Path.DirectorySeparatorChar;
					if (!Directory.Exists(ret))
						Directory.CreateDirectory(ret);
					if (Config.GetBoolValue("admintoolbox_stats_unified", true))
						ret += "Global" + Path.DirectorySeparatorChar;
					else
						ret += Port + Path.DirectorySeparatorChar; 
					break;

				case Folder.Logs:
					ret += "ServerLogs" + Path.DirectorySeparatorChar;
					break;

				case Folder.Warps:
					ret +=  "WarpPoints" + Path.DirectorySeparatorChar;
					break;

				case Folder.AppData:
					return FileManager.GetAppFolder(shared: true, addseparator: true);

				case Folder.ATFolder:
				default:
					break;
			}
			if (!Directory.Exists(ret))
				Directory.CreateDirectory(ret);
			return ret;
		}

		internal static string AdminToolboxFolder =>
			GetATFolderPath();

		internal static string AdminToolboxPlayerStats => 
			GetFolder(Folder.PlayerFiles);

		internal static string AdminToolboxLogs =>
			GetFolder(Folder.Logs);

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
			if (Directory.Exists(AdminToolboxFolder))
			{
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

				#region NewFilesStuff
				/*
				void NewWriteToFile(string steamID)
				{
					if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return;
					string playerFilePath = AdminToolboxPlayerStats + steamID + ".txt";
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

					NewReadFromFile(steamID);
				}
				void NewReadFromFile(string steamID)
				{
					if (!AdminToolbox.ATPlayerDict.ContainsKey(steamID)) return;
					string playerFilePath = AdminToolboxPlayerStats + steamID + ".txt";
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
				}
				*/
				#endregion

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

					PlayerSettings setting = (AdminToolbox.ATPlayerDict.ContainsKey(steamID)) ? AdminToolbox.ATPlayerDict[steamID] : new API.PlayerSettings(steamID);
					int Kills = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].PlayerStats.Kills > 0) ? AdminToolbox.ATPlayerDict[steamID].PlayerStats.Kills : 0;
					int TeamKills = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].PlayerStats.TeamKills > 0) ? AdminToolbox.ATPlayerDict[steamID].PlayerStats.TeamKills : 0;
					int Deaths = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].PlayerStats.Deaths > 0) ? AdminToolbox.ATPlayerDict[steamID].PlayerStats.Deaths : 0;
					double minutesPlayed = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].PlayerStats.MinutesPlayed > 0) ? DateTime.Now.Subtract(AdminToolbox.ATPlayerDict[steamID].JoinTime).TotalMinutes + AdminToolbox.ATPlayerDict[steamID].PlayerStats.MinutesPlayed : 0;
					int BanCount = (AdminToolbox.ATPlayerDict.ContainsKey(steamID) && AdminToolbox.ATPlayerDict[steamID].PlayerStats.BanCount > 0) ? AdminToolbox.ATPlayerDict[steamID].PlayerStats.BanCount : 0;
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
						PlayerSettings setting = AdminToolbox.ATPlayerDict[steamID];
						setting.PlayerStats.Kills = (playerStats.Length > 0 && int.TryParse(playerStats[0], out int x0) && x0 > setting.PlayerStats.Kills) ? x0 : setting.PlayerStats.Kills;
						setting.PlayerStats.TeamKills = (playerStats.Length > 1 && int.TryParse(playerStats[1], out int x1) && x1 > setting.PlayerStats.TeamKills) ? x1 : setting.PlayerStats.TeamKills;
						setting.PlayerStats.Deaths = (playerStats.Length > 2 && int.TryParse(playerStats[2], out int x2) && x2 > setting.PlayerStats.Deaths) ? x2 : setting.PlayerStats.Deaths;
						setting.PlayerStats.MinutesPlayed = (playerStats.Length > 3 && double.TryParse(playerStats[3], out double x3) && x3 > setting.PlayerStats.MinutesPlayed) ? x3 : setting.PlayerStats.MinutesPlayed;
						setting.PlayerStats.BanCount = (playerStats.Length > 4 && int.TryParse(playerStats[4], out int x4) && x4 > setting.PlayerStats.BanCount) ? x4 : setting.PlayerStats.BanCount;
						AdminToolbox.ATPlayerDict[steamID] = setting;
					}
				}
			}
		}

		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each steamID in the <see cref="List{T}"/>
		/// </summary>
		public static void ConvertOldFilesToJSON(string file = "")
		{
			return;
			int x = 0;
			string[] files = (string.IsNullOrEmpty(file)) ? Directory.GetFiles(AdminToolboxPlayerStats) : new string[] { file };
			if (files.Where(f => !File.ReadAllText(f).StartsWith("{")).Count() >= 100)
			{
				AdminToolbox.plugin.Info("Warning: The plugin will be converting old playerfiles to a new format." + "\n" + "Beware that this might take some time");
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
			string path = GetFolder(Folder.AppData);
			if (Directory.Exists(path))
			{
				string text = "at_version=" + AdminToolbox.plugin.Details.version.Split('-').FirstOrDefault();
				using (StreamWriter streamWriter = new StreamWriter(path + "at_version.md", false))
				{
					streamWriter.Write(text);
					streamWriter.Close();
				}
				if (File.Exists(path + "n_at_version.md"))
					File.Delete(path + "n_at_version.md");
			}
			else
				AdminToolbox.plugin.Info("Could not find SCP Secret Lab folder!");
		}
	}
}
