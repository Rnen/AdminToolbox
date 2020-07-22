using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Managers
{
	using API;

	internal enum Folder
	{
		AppData,
		ATRoot,
		PlayerFiles,
		Logs,
		Warps
	}
	/// <summary>
	/// Contains all <see cref="File"/>-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	/// 
	public class ATFileManager
	{
		private static IConfigFile Config => ConfigManager.Manager.Config;
		private static int Port => PluginManager.Manager.Server.Port;

		internal const char SplitChar = ';';

		internal bool ProcessingCollection = false;

		/// <summary>
		/// <see cref="PlayerFile"/> <see cref="System.Enum"/> for <see cref="ATFileManager"/>
		/// </summary>
		public enum PlayerFile
		{
			Read = 0,
			Write = 1
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
				cnfgpath = FileManager.GetAppFolder(addSeparator: true) + "AdminToolbox" + Path.DirectorySeparatorChar;
			}
			if (!Directory.Exists(cnfgpath))
				Directory.CreateDirectory(cnfgpath);
			return cnfgpath;
		}

		internal static string GetFolderPath(Folder folder = Folder.ATRoot)
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
					ret += "WarpPoints" + Path.DirectorySeparatorChar;
					break;

				case Folder.AppData:
					return FileManager.GetAppFolder(addSeparator: true);

				case Folder.ATRoot:
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
			GetFolderPath(Folder.PlayerFiles);

		internal static string AdminToolboxLogs =>
			GetFolderPath(Folder.Logs);

		/// <summary>
		/// Read/Writes <see cref ="Player"/> stats to/from <see cref="File"/>
		/// </summary>
		public void PlayerStatsFileManager(PlayerFile FileOperation = PlayerFile.Read) => PlayerStatsFileManager(new string[] { }, FileOperation);

		/// <summary>
		/// Read/Writes the <paramref name="UserId"/>'s stats to/from <see cref="File"/>
		/// </summary>
		public void PlayerStatsFileManager(string UserId, PlayerFile FileOperation = PlayerFile.Write) => PlayerStatsFileManager(new string[] { UserId }, FileOperation);

		/// <summary>
		/// Read/Writes <see cref ="Player"/> stats to/from <see cref="File"/>
		/// </summary>
		public void PlayerStatsFileManager(Player player, PlayerFile FileOperation = PlayerFile.Write) => PlayerStatsFileManager(new string[] { player.UserId }, FileOperation);

		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each <see cref="Player"/> in the <see cref="List{T}"/>
		/// </summary>
		public void PlayerStatsFileManager(List<Player> playerList, PlayerFile FileOperation = PlayerFile.Read)
		{
			if (playerList != null && playerList.Count > 0)
				PlayerStatsFileManager(playerList.Select(p => p.UserId).ToArray(), FileOperation);
			else
				PlayerStatsFileManager(FileOperation);
		}

		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each UserId in the <see cref="List{T}"/>
		/// </summary>
		public void PlayerStatsFileManager(List<string> UserIdList, PlayerFile FileOperation = PlayerFile.Read)
		{
			if (UserIdList != null && UserIdList.Count > 0)
				PlayerStatsFileManager(UserIdList.ToArray(), FileOperation);
			else
				PlayerStatsFileManager(new string[0], FileOperation);
		}

		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each UserId in the <see cref="Array"/>
		/// </summary>
		public void PlayerStatsFileManager(string[] UserIdArray, PlayerFile FileOperation = PlayerFile.Read)
		{
			if (!Config.GetBoolValue("admintoolbox_playerfiles", false))
				return;
			if (Directory.Exists(AdminToolboxFolder))
			{
				if (!Directory.Exists(AdminToolboxFolder))
					Directory.CreateDirectory(AdminToolboxFolder);
				if (!Directory.Exists(AdminToolboxPlayerStats))
					Directory.CreateDirectory(AdminToolboxPlayerStats);

				ProcessingCollection = true;

				if (UserIdArray != null && UserIdArray.Length > 0)
					foreach (string sid in UserIdArray)
						ReadWriteHandler(sid, FileOperation);
				else
				{
					string[] keys = AdminToolbox.ATPlayerDict.Keys.ToArray();
					foreach (string UserId in keys)
						ReadWriteHandler(UserId, FileOperation);
				}

				ProcessingCollection = false;


				void ReadWriteHandler(string UserId, PlayerFile Operation)
				{
					if (string.IsNullOrEmpty(UserId)) return;
					if (!AdminToolbox.ATPlayerDict.ContainsKey(UserId))
					{
						AdminToolbox.AddMissingPlayerVariables(PluginManager.Manager.Server.GetPlayers(UserId));
						return;
					}
					switch (Operation)
					{
						case PlayerFile.Write:
							WriteToFile(UserId);
							break;
						case PlayerFile.Read:
						default:
							ReadFromFile(UserId);
							break;
					}
				}

				#region NewFilesStuff
				/*
				void NewWriteToFile(string UserId)
				{
					if (!AdminToolbox.ATPlayerDict.ContainsKey(UserId)) return;
					string playerFilePath = AdminToolboxPlayerStats + UserId + ".txt";
					if (!File.Exists(playerFilePath))
						File.Create(playerFilePath).Dispose();

					if (File.ReadAllLines(playerFilePath).Length <= 2)
						ConvertOldFilesToJSON(playerFilePath);
					SerilizablePlayerClass pl = new SerilizablePlayerClass(AdminToolbox.ATPlayerDict[UserId]);

					if (string.IsNullOrEmpty(pl.PlayerInfo.FirstJoin))
						pl.PlayerInfo.FirstJoin = DateTime.Now.AddMinutes(-pl.PlayerStats.MinutesPlayed).ToString(CultureInfo.InvariantCulture);

					using (StreamWriter sw = new StreamWriter(playerFilePath, false))
					{
						sw.WriteLine(JsonConvert.SerializeObject(pl, Formatting.Indented));
					}

					NewReadFromFile(UserId);
				}
				void NewReadFromFile(string UserId)
				{
					if (!AdminToolbox.ATPlayerDict.ContainsKey(UserId)) return;
					string playerFilePath = AdminToolboxPlayerStats + UserId + ".txt";
					if (!File.Exists(playerFilePath))
						WriteToFile(UserId);

					if (File.ReadAllLines(playerFilePath).FirstOrDefault().Split(splitChar).Length > 3)
						ConvertOldFilesToJSON(playerFilePath);

					string rawJSON = File.ReadAllText(playerFilePath);
					SerilizablePlayerClass pl = JsonConvert.DeserializeObject<SerilizablePlayerClass>(rawJSON);

					PlayerSettings playersetting = AdminToolbox.ATPlayerDict[UserId];

					if (string.IsNullOrEmpty(pl.PlayerInfo.FirstJoin))
						playersetting.PlayerInfo.FirstJoin = DateTime.Now.AddMinutes(-pl.PlayerStats.MinutesPlayed).ToString(CultureInfo.InvariantCulture);
					playersetting.PlayerStats = pl.PlayerStats;
				}
				*/
				#endregion

				void WriteToFile(string UserId)
				{
					if (!AdminToolbox.ATPlayerDict.ContainsKey(UserId))
					{
						if (PluginManager.Manager.Server.GetPlayers(UserId).Count < 1) return;
						AdminToolbox.AddMissingPlayerVariables(PluginManager.Manager.Server.GetPlayers(UserId).FirstOrDefault());
					}
					if (!AdminToolbox.ATPlayerDict.ContainsKey(UserId)) return;
					string playerFilePath = AdminToolbox.ATPlayerDict.ContainsKey(UserId) ? AdminToolboxPlayerStats + Path.DirectorySeparatorChar + UserId + ".txt" : AdminToolboxPlayerStats + Path.DirectorySeparatorChar + "server" + ".txt";
					if (!File.Exists(playerFilePath))
						File.Create(playerFilePath).Dispose();
					AdminToolbox.singleton.Debug("Writing: " + playerFilePath);

					PlayerSettings setting = AdminToolbox.ATPlayerDict.ContainsKey(UserId) ? AdminToolbox.ATPlayerDict[UserId] : new API.PlayerSettings(UserId);
					int Kills = (AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.Kills > 0) ? AdminToolbox.ATPlayerDict[UserId].PlayerStats.Kills : 0;
					int TeamKills = (AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.TeamKills > 0) ? AdminToolbox.ATPlayerDict[UserId].PlayerStats.TeamKills : 0;
					int Deaths = (AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.Deaths > 0) ? AdminToolbox.ATPlayerDict[UserId].PlayerStats.Deaths : 0;
					double minutesPlayed = (AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.MinutesPlayed > 0) ? DateTime.Now.Subtract(AdminToolbox.ATPlayerDict[UserId].JoinTime).TotalMinutes + AdminToolbox.ATPlayerDict[UserId].PlayerStats.MinutesPlayed : 0;
					int BanCount = (AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.BanCount > 0) ? AdminToolbox.ATPlayerDict[UserId].PlayerStats.BanCount : 0;
					if (AdminToolbox.ATPlayerDict.ContainsKey(UserId)) AdminToolbox.ATPlayerDict[UserId].JoinTime = DateTime.Now;
					string str = string.Empty + Kills + SplitChar + TeamKills + SplitChar + Deaths + SplitChar + minutesPlayed + SplitChar + BanCount;
					using (StreamWriter streamWriter = new StreamWriter(playerFilePath, false))
					{
						streamWriter.Write(str);
						streamWriter.Close();
					}
					ReadFromFile(UserId);
				}
				void ReadFromFile(string UserId)
				{
					string playerFilePath = AdminToolbox.ATPlayerDict.ContainsKey(UserId) ? AdminToolboxPlayerStats + Path.DirectorySeparatorChar + UserId + ".txt" : AdminToolboxPlayerStats + Path.DirectorySeparatorChar + "server" + ".txt";
					if (!File.Exists(playerFilePath))
						PlayerStatsFileManager(new List<string> { UserId }, PlayerFile.Write);
					AdminToolbox.singleton.Debug("Reading: " + playerFilePath);
					string[] fileStrings = (File.ReadAllLines(playerFilePath).Length > 0) ? File.ReadAllLines(playerFilePath) : new string[] { "0;0;0;0;0" };
					string[] playerStats = fileStrings.FirstOrDefault().Split(SplitChar);
					if (AdminToolbox.ATPlayerDict.ContainsKey(UserId))
					{
						PlayerSettings setting = AdminToolbox.ATPlayerDict[UserId];
						setting.PlayerStats.Kills = (playerStats.Length > 0 && int.TryParse(playerStats[0], out int x0) && x0 > setting.PlayerStats.Kills) ? x0 : setting.PlayerStats.Kills;
						setting.PlayerStats.TeamKills = (playerStats.Length > 1 && int.TryParse(playerStats[1], out int x1) && x1 > setting.PlayerStats.TeamKills) ? x1 : setting.PlayerStats.TeamKills;
						setting.PlayerStats.Deaths = (playerStats.Length > 2 && int.TryParse(playerStats[2], out int x2) && x2 > setting.PlayerStats.Deaths) ? x2 : setting.PlayerStats.Deaths;
						setting.PlayerStats.MinutesPlayed = (playerStats.Length > 3 && double.TryParse(playerStats[3], out double x3) && x3 > setting.PlayerStats.MinutesPlayed) ? x3 : setting.PlayerStats.MinutesPlayed;
						setting.PlayerStats.BanCount = (playerStats.Length > 4 && int.TryParse(playerStats[4], out int x4) && x4 > setting.PlayerStats.BanCount) ? x4 : setting.PlayerStats.BanCount;
						AdminToolbox.ATPlayerDict[UserId] = setting;
					}
				}
			}
		}

		public static void ConvertOldFilesToNewUserID()
		{
			if (!Config.GetBoolValue("admintoolbox_playerfiles", false))
				return;
			if (Config.GetBoolValue("admintoolbox_userfiles_convert", true))
			{
				string[] files = Directory.GetFiles(AdminToolboxPlayerStats);
				if (files.Any(s => !s.Contains('@')))
				{
					int x = 0;
					AdminToolbox.singleton.Info("(File Manager) - Converting old files to new UserID format");
					if (files.Length > 5000)
					{
						AdminToolbox.singleton.Info("(File Manager) - Large amount of files detected, this may take a moment...");
					}
					for (int i = files.Length - 1; i > -1; i--)
					{
						if (!Path.GetFileName(files[i]).Contains('@'))
						{
							x++;
							AdminToolbox.singleton.Debug(files[i].Substring(0, files[i].Length - 4) + "@steam.txt");
							File.Move(files[i], files[i].Substring(0, files[i].Length - 4) + "@steam.txt");
						}
					}
					AdminToolbox.singleton.Info($"(File Manager) - {x} files converted");
				}
			}
			else if (Config.GetBoolValue("admintoolbox_userfiles_revert", false))
			{
				string[] files = Directory.GetFiles(AdminToolboxPlayerStats);
				if (files.Any(s => s.Contains('@')))
				{
					int x = 0;
					AdminToolbox.singleton.Info("(File Manager) - Reverting new files to old SteamID format");

					for (int i = files.Length - 1; i > -1; i--)
					{
						if (Path.GetFileName(files[i]).Contains('@'))
						{
							x++;
							AdminToolbox.singleton.Debug(files[i].Substring(0, files[i].Length - 4) + "@steam.txt");
							File.Move(files[i], files[i].Substring(0, files[i].Length - 10) + ".txt");
						}
					}
					AdminToolbox.singleton.Info($"(File Manager) - {x} files reverted");
				}
			}
		}

		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each UserId in the <see cref="List{T}"/>
		/// </summary>
		public static void ConvertOldFilesToJSON(string file = "")
		{
			return;
#pragma warning disable CS0162 // Unreachable code detected
			int x = 0;

			string[] files = string.IsNullOrEmpty(file) ? Directory.GetFiles(AdminToolboxPlayerStats) : new string[] { file };
			if (files.Where(f => !File.ReadAllText(f).StartsWith("{")).Count() >= 100)
			{
				AdminToolbox.singleton.Info("Warning: The plugin will be converting old playerfiles to a new format." + "\n" + "Beware that this might take some time");
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
						ps.MinutesPlayed = (rawData.Length > 3 && double.TryParse(rawData[3].Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out double x3) && x3 > ps.MinutesPlayed && x3 < int.MaxValue) ? x3 : ps.MinutesPlayed;
						ps.BanCount = (rawData.Length > 4 && int.TryParse(rawData[4], out int x4) && x4 > ps.BanCount) ? x4 : ps.BanCount;

						int subtractedMinutes = (ps.MinutesPlayed < 0) ? (int)ps.MinutesPlayed : (int)-ps.MinutesPlayed;

						SerilizablePlayerClass playerClass = new SerilizablePlayerClass(ps);
						playerClass.PlayerInfo.FirstJoin = DateTime.Now.Add(TimeSpan.FromMinutes(subtractedMinutes)).ToString(CultureInfo.InvariantCulture);

						using (StreamWriter sw = new StreamWriter(path, false))
						{
							//sw.WriteLine(UnityEngine.JsonUtility.ToJson(playerClass, true));
						}
						x++;
					}
					catch (Exception e)
					{
						AdminToolbox.singleton.Debug("Failed during convertion of: " + path);
						AdminToolbox.singleton.Debug(e.StackTrace);
						continue;
					}
					// Kills + TeamKills  + Deaths  + minutesPlayed  + BanCount;
				}
			if (x > 0)
				AdminToolbox.singleton.Debug(x + " files converted to new JSON format!");
#pragma warning restore CS0162 // Unreachable code detected
		}

		/// <summary>
		/// The class <see cref="AdminToolbox"/> uses for JSON Serialize/Deserialize operations
		/// </summary>
		public class SerilizablePlayerClass
		{
			public PlayerInfo PlayerInfo = new PlayerInfo();
			public PlayerStats PlayerStats = new PlayerStats();

			public SerilizablePlayerClass() { }
			public SerilizablePlayerClass(PlayerStats stats) => this.PlayerStats = stats;
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
			string path = GetFolderPath(Folder.AppData);
			if (Directory.Exists(path))
			{
				string text = "at_version=" + AdminToolbox.singleton.Details.version.Split('-').FirstOrDefault();
				using (StreamWriter streamWriter = new StreamWriter(path + "at_version.md", false))
				{
					streamWriter.Write(text);
				}
				if (File.Exists(path + "n_at_version.md"))
					File.Delete(path + "n_at_version.md");
			}
			else
				AdminToolbox.singleton.Info("Could not find SCP Secret Lab folder!");
		}
	}
}
