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
	public class LogManager
	{
		static IConfigFile Config => ConfigManager.Manager.Config;

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

		private static readonly string appFolder = FileManager.GetAppFolder();

		private static string
			AdminToolboxFolder => ATFileManager.AdminToolboxFolder;
		private static string
			AdminToolboxPlayerStats = ATFileManager.AdminToolboxPlayerStats;
		private static string
			AdminToolboxLogs = ATFileManager.AdminToolboxLogs;

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
			int configInt = Config.GetIntValue("admintoolbox_logremover_hours_old", 0);

			if (configInt > 0 || force)
			{
				Directory.GetFiles(AdminToolboxLogs).ToList().ForEach(path => { if ((DateTime.Now - File.GetCreationTime(path)).TotalHours > configInt) File.Delete(path); });
			}
		}

		private static string ToMax(string text, int max)
		{
			while (text.Length < max)
			{
				text += " ";
			}
			return text;
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
					if (Config.GetBoolValue("admintoolbox_log_teamkills", false, false))
						AddLog(str2, logType);
					break;
				case LogManager.ServerLogType.KillLog:
					if (Config.GetBoolValue("admintoolbox_log_kills", false, false))
						AddLog(str2, logType);
					break;
				case LogManager.ServerLogType.RemoteAdminActivity:
					if (Config.GetBoolValue("admintoolbox_log_commands", false, false))
						AddLog(str2, logType);
					break;
				default:
					AddLog(str2, logType);
					break;
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
	}

}
