using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Smod2;
using Smod2.API;
using System.Text.RegularExpressions;

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
	public class ATFile
	{
		private static IConfigFile Config => ConfigManager.Manager.Config;
		private static Server Server => PluginManager.Manager.Server;
		private static int Port => PluginManager.Manager.Server.Port;
		private static AdminToolbox Plugin => AdminToolbox.singleton;
		private static void Debug(string str) => Plugin.Debug($"[{typeof(ATFile).Name}]: " + str);
		private static void Info(string str) => Plugin.Info($"[{typeof(ATFile).Name}]: " + str);

		internal const char SplitChar = ';';

		internal bool ProcessingCollection = false;

		/// <summary>
		/// <see cref="PlayerFile"/> <see cref="System.Enum"/> for <see cref="ATFile"/>
		/// </summary>
		public enum PlayerFile
		{
			Read = 0,
			Write = 1
		}


		private static string GetFolderPath()
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

			string ret = GetFolderPath();
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

		[Smod2.Piping.PipeProperty(true, false)]
		public static string MainPath =>
			GetFolderPath();

		[Smod2.Piping.PipeProperty(true, false)]
		internal static string PlayerStatsPath =>
			GetFolderPath(Folder.PlayerFiles);

		[Smod2.Piping.PipeProperty(true, false)]
		internal static string LogPath =>
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
			if (!Config.GetBoolValue("admintoolbox_playerfiles", true))
				return;
			if (Directory.Exists(MainPath))
			{
				if (!Directory.Exists(MainPath))
					Directory.CreateDirectory(MainPath);
				if (!Directory.Exists(PlayerStatsPath))
					Directory.CreateDirectory(PlayerStatsPath);

				ProcessingCollection = true;

				if (UserIdArray == null || UserIdArray.Length < 1)
					UserIdArray = AdminToolbox.ATPlayerDict.Keys.ToArray();
				foreach (string id in UserIdArray) 
				{
					if (string.IsNullOrEmpty(id)) 
						return;
					if (!AdminToolbox.ATPlayerDict.ContainsKey(id))
					{
						AddMissingPlayerVariables(Server.GetPlayers(id));
						continue;
					}
					switch (FileOperation)
					{
						case PlayerFile.Write:
							WriteToFile(id);
							continue;
						case PlayerFile.Read:
						default:
							ReadFromFile(id);
							continue;
					}
				}

				ProcessingCollection = false;
			}
		}

		#region NewFilesStuff -The new JSON stuff
#pragma warning disable IDE0051 // Remove unused private members
		private void NewWriteToFile(string UserId)
		{
			if (!AdminToolbox.ATPlayerDict.ContainsKey(UserId)) 
				return;
			string playerFilePath = PlayerStatsPath + UserId + ".txt";
			if (!File.Exists(playerFilePath))
				File.Create(playerFilePath).Dispose();

			if (File.ReadAllLines(playerFilePath).Length <= 2)
				ConvertOldFilesToJSON(playerFilePath);

			SerializablePlayerClass pl = new SerializablePlayerClass(AdminToolbox.ATPlayerDict[UserId]);

			if (string.IsNullOrEmpty(pl.PlayerInfo.FirstJoin))
				pl.PlayerInfo.FirstJoin = DateTime.UtcNow.AddMinutes(-pl.PlayerStats.MinutesPlayed).ToString(CultureInfo.InvariantCulture);

			using (StreamWriter sw = new StreamWriter(playerFilePath, false))
			{
				sw.WriteLine(Utf8Json.JsonSerializer.PrettyPrint(Utf8Json.JsonSerializer.Serialize(pl)));
			}
		}
		private void NewReadFromFile(string UserId)
		{
			if (!AdminToolbox.ATPlayerDict.ContainsKey(UserId)) 
				return;
			string playerFilePath = PlayerStatsPath + UserId + ".txt";
			if (!File.Exists(playerFilePath))
				WriteToFile(UserId);

			if (File.ReadAllLines(playerFilePath).Length <= 2)
				ConvertOldFilesToJSON(playerFilePath);

			string rawJSON = File.ReadAllText(playerFilePath);
			SerializablePlayerClass pl = Utf8Json.JsonSerializer.Deserialize<SerializablePlayerClass>(rawJSON);

			PlayerSettings playersetting = AdminToolbox.ATPlayerDict[UserId];

			if (string.IsNullOrEmpty(pl.PlayerInfo.FirstJoin))
				playersetting.PlayerInfo.FirstJoin = DateTime.UtcNow.AddMinutes(-pl.PlayerStats.MinutesPlayed).ToString(CultureInfo.InvariantCulture);
			playersetting.PlayerStats = pl.PlayerStats;
		}
#pragma warning restore IDE0051 // Remove unused private members
		#endregion

		private void WriteToFile(string UserId)
		{
			if (!AdminToolbox.ATPlayerDict.ContainsKey(UserId))
			{
				if (PluginManager.Manager.Server.GetPlayers(UserId).Count < 1) 
					return;
				AddMissingPlayerVariables(PluginManager.Manager.Server.GetPlayers(UserId));
			}
			if (!AdminToolbox.ATPlayerDict.ContainsKey(UserId)) 
				return;
			string playerFilePath = AdminToolbox.ATPlayerDict.ContainsKey(UserId) ? PlayerStatsPath + Path.DirectorySeparatorChar + UserId + ".txt" : PlayerStatsPath + Path.DirectorySeparatorChar + "server" + ".txt";
			if (!File.Exists(playerFilePath))
				File.Create(playerFilePath).Dispose();
			Debug("Writing: " + playerFilePath);

			string Kills = ((AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.Kills > 0) ? AdminToolbox.ATPlayerDict[UserId].PlayerStats.Kills : 0).ToString(CultureInfo.InvariantCulture);
			string TeamKills = ((AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.TeamKills > 0) ? AdminToolbox.ATPlayerDict[UserId].PlayerStats.TeamKills : 0).ToString(CultureInfo.InvariantCulture);
			string Deaths = ((AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.Deaths > 0) ? AdminToolbox.ATPlayerDict[UserId].PlayerStats.Deaths : 0).ToString(CultureInfo.InvariantCulture);
			string minutesPlayed = ((AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.MinutesPlayed > 0) ? DateTime.UtcNow.Subtract(AdminToolbox.ATPlayerDict[UserId].JoinTime).TotalMinutes + AdminToolbox.ATPlayerDict[UserId].PlayerStats.MinutesPlayed : 0).ToString(CultureInfo.InvariantCulture);
			string BanCount = ((AdminToolbox.ATPlayerDict.ContainsKey(UserId) && AdminToolbox.ATPlayerDict[UserId].PlayerStats.BanCount > 0) ? AdminToolbox.ATPlayerDict[UserId].PlayerStats.BanCount : 0).ToString(CultureInfo.InvariantCulture);
			if (AdminToolbox.ATPlayerDict.ContainsKey(UserId)) 
				AdminToolbox.ATPlayerDict[UserId].JoinTime = DateTime.UtcNow;
			string str = string.Join(SplitChar.ToString(), new string[] { Kills, TeamKills, Deaths, minutesPlayed, BanCount });
			using (StreamWriter streamWriter = new StreamWriter(playerFilePath, false))
			{
				streamWriter.Write(str);
				streamWriter.Close();
			}
		}
		private void ReadFromFile(string UserId)
		{
			string playerFilePath = AdminToolbox.ATPlayerDict.ContainsKey(UserId) ? PlayerStatsPath + Path.DirectorySeparatorChar + UserId + ".txt" : PlayerStatsPath + Path.DirectorySeparatorChar + "server" + ".txt";
			if (!File.Exists(playerFilePath))
				PlayerStatsFileManager(UserId, PlayerFile.Write);
			Debug("Reading: " + playerFilePath);
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

		public static void RenameOldFilesToNewUserID()
		{
			Info($"Entered {System.Reflection.MethodBase.GetCurrentMethod().Name} method");

			if (!Config.GetBoolValue("admintoolbox_playerfiles", true))
			{
				Info("PlayerfilesConfig was " + Config.GetBoolValue("admintoolbox_playerfiles", true) + ", returning");
				return;
			}
			if (Config.GetBoolValue("admintoolbox_userfiles_convert", true))
			{
				Info("Checking path: " + PlayerStatsPath);
				
				string[] files = Directory.GetFiles(PlayerStatsPath, "*.txt").Where(n => !n.Contains("@")).ToArray();

				Info("PlayerFiles count: " + files.Length);
				
				if (files.Length > 0)
				{
					Info($"Files to convert: {files.Length}\n - Converting old file(s) to new UserID format");
					if (files.Length > 2000)
					{
						Info(" - Large amount of files detected, this may take a moment...");
					}
					int fileCount = 0;
					for (int i = files.Length - 1; i > -1; i--)
					{
						if (!Path.GetFileName(files[i]).Contains('@'))
						{
							fileCount++;
							string _newpath = files[i].Substring(0, files[i].Length - 4) + "@steam.txt";
							Debug(_newpath);
							if (File.Exists(_newpath)) //At rare occations this file already exists
								File.Delete(_newpath);
							File.Move(files[i], _newpath);
						}
					}
					Info($"- {fileCount} file(s) converted");
				}
				else
				{
					Debug("No files to convert!");
				}
			}
			else if (Config.GetBoolValue("admintoolbox_userfiles_revert", false))
			{
				string[] files = Directory.GetFiles(PlayerStatsPath);
				if (files.Any(s => s.Contains('@')))
				{
					int fileCount = 0;
					Info("- Reverting new file(s) to old SteamID format");

					for (int i = files.Length - 1; i > -1; i--)
					{
						if (Path.GetFileName(files[i]).Contains('@'))
						{
							fileCount++;
							AdminToolbox.singleton.Debug(files[i].Substring(0, files[i].Length - 4) + "@steam.txt");
							File.Move(files[i], files[i].Substring(0, files[i].Length - 10) + ".txt");
						}
					}
					Info($"- {fileCount} file(s) reverted");
				}
			}
		}

		/// <summary>
		/// Read/Writes stats to/from <see cref="File"/> for each UserId in the <see cref="List{T}"/>
		/// </summary>
		public static void ConvertOldFilesToJSON(string filePath = "")
		{
			int fileCount = 0;

			string[] statsFiles = string.IsNullOrEmpty(filePath) ? Directory.GetFiles(PlayerStatsPath) : new string[] { filePath };
			if (statsFiles.Where(f => !File.ReadAllText(f).StartsWith("{")).Count() >= 100)
			{
				Info("!Warning! The plugin will be converting old playerfiles to a new format, this might take some time");
			}
			if (statsFiles.Length > 0)
				foreach (string path in statsFiles)
				{
					if (!File.Exists(path) || File.ReadAllLines(path).FirstOrDefault().StartsWith("{")) 
						continue;
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

						SerializablePlayerClass playerClass = new SerializablePlayerClass(ps);
						playerClass.PlayerInfo.FirstJoin = DateTime.UtcNow.Add(TimeSpan.FromMinutes(subtractedMinutes)).ToString(CultureInfo.InvariantCulture);

						using (StreamWriter sw = new StreamWriter(path, false))
						{
							sw.WriteLine(Utf8Json.JsonSerializer.PrettyPrint(Utf8Json.JsonSerializer.Serialize(playerClass)));
						}
						fileCount++;
					}
					catch (Exception e)
					{
						Debug("Failed during convertion of: " + path + "\n" + e);
						continue;
					}
					// Kills + TeamKills  + Deaths  + minutesPlayed  + BanCount;
				}
			if (fileCount > 0)
				Debug(fileCount + " file(s) converted to new JSON format!");
		}

		/// <summary>
		/// The class <see cref="AdminToolbox"/> uses for JSON Serialize/Deserialize operations
		/// </summary>
		public class SerializablePlayerClass
		{
			public PlayerInfo PlayerInfo = new PlayerInfo();
			public PlayerStats PlayerStats = new PlayerStats();

			public SerializablePlayerClass() { }
			public SerializablePlayerClass(PlayerStats stats) => this.PlayerStats = stats;
			public SerializablePlayerClass(PlayerSettings setting)
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
				try
				{
					string text = "at_version=" + AdminToolbox.singleton.Details.version.Split('-').FirstOrDefault();
					using (StreamWriter streamWriter = new StreamWriter(path + "at_version.md", false))
					{
						streamWriter.Write(text);
					}
				}
				catch(Exception e)
				{
					Debug("Failed writing version to file!\n" + e);
				}
				try
				{
					if (File.Exists(path + "n_at_version.md"))
						File.Delete(path + "n_at_version.md");
				}
				catch (Exception e)
				{
					Debug("Failed deleting temp version file!\n" + e);
				}
			}
			else
				Info("Could not find SCP Secret Lab folder!");
		}

		internal static void AddMissingPlayerVariables()
		{
			if (PluginManager.Manager.Server.GetPlayers().Count == 0) 
				return;
			AddMissingPlayerVariables(PluginManager.Manager.Server.GetPlayers());
		}
		internal static void AddMissingPlayerVariables(Player player)
			=> AddMissingPlayerVariables(new List<Player>() { player });
		internal static void AddMissingPlayerVariables(List<Player> players)
			=> AddMissingPlayerVariables(players.ToArray());
		internal static void AddMissingPlayerVariables(Player[] players)
		{
			Player[] allPlayers = PluginManager.Manager.Server.GetPlayers().ToArray();
			if (allPlayers.Length == 0)
			{
				return;
			}
			else if (players == null || players.Length < 1)
			{
				players = allPlayers;
			}
			if (players.Length > 0)
			{
				foreach (Player player in players)
				{
					if (player != null && !string.IsNullOrEmpty(player.UserId))
					{
						AddToPlayerDict(player);
					}
				}
			}
		}
		private static void AddToPlayerDict(Player player)
		{
			if (player != null && player is Player p &&
				!string.IsNullOrEmpty(p.UserId) && !AdminToolbox.ATPlayerDict.ContainsKey(p.UserId))
			{
				AdminToolbox.ATPlayerDict.Add(p.UserId, new PlayerSettings(p.UserId));
			}
		}
	}
}
