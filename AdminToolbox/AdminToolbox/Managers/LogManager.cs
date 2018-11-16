using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox
{
	public class LogManager
	{
		internal class LogData
		{
			internal string Content;

			internal string Type;

			internal string Time;

			internal bool Saved;
		}
		private readonly List<LogData> logs = new List<LogData>();

		private int _port;

		private int _maxlen;

		private static readonly bool unifiedStats = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_stats_unified", true);
		private static readonly string admintoolboxFolderPath = ConfigManager.Manager.Config.GetStringValue("admintoolbox_folder_path", string.Empty);
		private static readonly string appFolder = FileManager.GetAppFolder();

		internal static readonly string
			AdminToolboxFolder = (admintoolboxFolderPath != string.Empty) ? ((admintoolboxFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString())) ? "AdminToolbox" : Path.DirectorySeparatorChar + "AdminToolbox") : appFolder + "AdminToolbox",
			AdminToolboxPlayerStats = (unifiedStats) ? AdminToolboxFolder + Path.DirectorySeparatorChar + "PlayerStats" + Path.DirectorySeparatorChar + "Global" : AdminToolboxFolder + Path.DirectorySeparatorChar + "PlayerStats" + Path.DirectorySeparatorChar + PluginManager.Manager.Server.Port,
			AdminToolboxLogs = AdminToolboxFolder + Path.DirectorySeparatorChar + "ServerLogs";

		public string GetFolderLocation()
		{
			return AdminToolboxFolder;
		}

		public enum ServerLogType
		{
			RemoteAdminActivity,
			KillLog,
			TeamKill,
			Suicice,
			GameEvent,
			Misc
		}
		private static readonly string[] Txt = new string[]
		{
		"Remote Admin",
		"Kill",
		"TeamKill",
		"Suicide",
		"Game Event",
		"Misc"
		};
		private void Awake()
		{
			Txt.ToList().ForEach(delegate (string txt)
			{
				_maxlen = Math.Max(_maxlen, txt.Length);
			});
		}
		void Start()
		{
			_port = PluginManager.Manager.Server.Port;
		}

		internal void AddLog(string msg, ServerLogType type)
		{
			_port = PluginManager.Manager.Server.Port;
			string time = TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm:ss zzz");
			logs.Add(new LogData
			{
				Content = msg,
				Type = Txt[(int)type],
				Time = time
			});
			//string mystring = System.Reflection.Assembly.GetAssembly(this.GetType()).Location;
			if (Directory.Exists(FileManager.GetAppFolder()))
			{
				if (!Directory.Exists(AdminToolboxFolder))
					Directory.CreateDirectory(AdminToolboxFolder);
				if (!Directory.Exists(AdminToolboxLogs))
					Directory.CreateDirectory(AdminToolboxLogs);
				if (Directory.Exists(FileManager.GetAppFolder() + "ATServerLogs"))
					MoveOldFiles();
				if (!Directory.Exists(AdminToolboxLogs + Path.DirectorySeparatorChar + _port))
					Directory.CreateDirectory(AdminToolboxLogs + Path.DirectorySeparatorChar + _port);

				string text = string.Empty;
				foreach (LogData log in logs)
				{
					if (!log.Saved)
					{
						log.Saved = true;
						string text2 = text;
						text = text2 + log.Time + " | " + ToMax(log.Type, _maxlen) + " | " + log.Content + Environment.NewLine;
					}
				}
				using (StreamWriter streamWriter = new StreamWriter(AdminToolboxLogs + Path.DirectorySeparatorChar + _port + Path.DirectorySeparatorChar + AdminToolbox._roundStartTime + "_Round-" + AdminToolbox.RoundCount + ".txt", true))
				{
					streamWriter.Write(text);
					streamWriter.Close();
				}
			}
		}

		public void ManageDatedATLogs(bool force = false)
		{
			int configInt = ConfigManager.Manager.Config.GetIntValue("admintoolbox_logremover_hours_old", 0);

			if (configInt > 0 || force)
			{
				Directory.GetFiles(AdminToolboxLogs).ToList().ForEach(path => { if ((DateTime.Now - File.GetCreationTime(path)).TotalHours > configInt) File.Delete(path); });
			}
		}

		private void MoveOldFiles()
		{
			string infoString = (Directory.GetDirectories(FileManager.GetAppFolder() + "ATServerLogs").Length > 0) ? "\n\n Relocated folders: " : string.Empty;
			string appdataPath = FileManager.GetAppFolder();
			foreach (string path in Directory.GetDirectories(FileManager.GetAppFolder() + "ATServerLogs"))
			{
				if (!Directory.Exists(Path.DirectorySeparatorChar + path.Replace(FileManager.GetAppFolder() + "ATServerLogs" + Path.DirectorySeparatorChar, string.Empty)))
				{
					Directory.Move(path, AdminToolboxLogs + Path.DirectorySeparatorChar + path.Replace(FileManager.GetAppFolder() + "ATServerLogs" + Path.DirectorySeparatorChar, string.Empty));
					infoString += "\n" + " - " + path.Replace(appdataPath, string.Empty);
				}
			}
			if (infoString != string.Empty) AdminToolbox.plugin.Info(infoString + "\n\n New Path: " + AdminToolboxLogs.Replace(appdataPath, string.Empty));
			Directory.Delete(FileManager.GetAppFolder() + "ATServerLogs");
		}

		private static string ToMax(string text, int max)
		{
			while (text.Length < max)
			{
				text += " ";
			}
			return text;
		}

		public enum PlayerFile
		{
			Read = 0,
			Write = 1
		}
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
					if (pl.SteamId == string.Empty || pl.SteamId == null || pl.Name == "Server" || pl.Name == string.Empty) return;
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
						PlayerStatsFileManager(new List<Player> { pl }, LogManager.PlayerFile.Write);
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
		public void WriteToLog(string[] str, LogManager.ServerLogType logType = LogManager.ServerLogType.Misc)
		{
			string str2 = string.Empty;
			if (str.Length != 0)
				foreach (string st in str)
					str2 += st;
			switch (logType)
			{
				case LogManager.ServerLogType.TeamKill:
					if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_log_teamkills", false, false))
						AddLog(str2, logType);
					break;
				case LogManager.ServerLogType.KillLog:
					if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_log_kills", false, false))
						AddLog(str2, logType);
					break;
				case LogManager.ServerLogType.RemoteAdminActivity:
					if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_log_commands", false, false))
						AddLog(str2, logType);
					break;
				default:
					AddLog(str2, logType);
					break;
			}
		}
	}

}
