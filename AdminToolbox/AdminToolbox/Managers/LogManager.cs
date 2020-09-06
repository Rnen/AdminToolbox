using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Smod2;

namespace AdminToolbox.Managers
{
	/// <summary>
	/// Contains all Log-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public class LogManager
	{
		private static IConfigFile Config => ConfigManager.Manager.Config;

		public LogManager() => Awake();

		internal class LogData
		{
			internal string Content;
			internal string Type;
			internal string Time;
			internal bool Saved;
		}

		private readonly List<LogData> logs = new List<LogData>();

		private static int Port => PluginManager.Manager.Server.Port;

		private int _maxlen;

		internal string _logStartTime;

		private static string
			AppFolder => ATFile.GetFolderPath(Folder.AppData);
		private static string
			AdminToolboxFolder => ATFile.AdminToolboxFolder;
		private static string
			AdminToolboxLogs => ATFile.AdminToolboxLogs;

		/// <summary>
		/// Enum used by <see cref="WriteToLog(string[], ServerLogType)"/>
		/// </summary>
		public enum ServerLogType
		{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
			RemoteAdminActivity,
			PlayerDamage,
			KillLog,
			TeamKill,
			Suicice,
			GameEvent,
			Misc
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
		}
		private static readonly string[] Txt = new string[]
		{
			"Remote Admin",
			"Player Damage",
			"Kill",
			"TeamKill",
			"Suicide",
			"Game Event",
			"Misc"
		};
		private void Awake()
		{
			SetLogStartTime();
			foreach (string txt in Txt.ToList())
			{
				_maxlen = Math.Max(_maxlen, txt.Length);
			}
			WriteToLog(new string[] { "\"Plugin Started\"" }, ServerLogType.Misc);
		}

		private void AddLog(string msg, ServerLogType type)
		{
			string time = TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm:ss zzz");
			logs.Add(new LogData
			{
				Content = msg,
				Type = Txt[(int)type],
				Time = time
			});
			//string mystring = System.Reflection.Assembly.GetAssembly(this.GetType()).Location;
			if (CheckExistingFolders())
			{
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
				using (StreamWriter streamWriter = new StreamWriter(AdminToolboxLogs + Port + Path.DirectorySeparatorChar + _logStartTime + "_Round-" + AdminToolbox.RoundCount + ".txt", true))
				{
					streamWriter.Write(text);
				}
			}
		}

		internal void SetLogStartTime() => _logStartTime = DateTime.Now.Year.ToString() + "-" + ((DateTime.Now.Month >= 10) ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString())) + "-" + ((DateTime.Now.Day >= 10) ? DateTime.Now.Day.ToString() : ("0" + DateTime.Now.Day.ToString())) + " " + ((DateTime.Now.Hour >= 10) ? DateTime.Now.Hour.ToString() : ("0" + DateTime.Now.Hour.ToString())) + "." + ((DateTime.Now.Minute >= 10) ? DateTime.Now.Minute.ToString() : ("0" + DateTime.Now.Minute.ToString())) + "." + ((DateTime.Now.Second >= 10) ? DateTime.Now.Second.ToString() : ("0" + DateTime.Now.Second.ToString()));

		private bool CheckExistingFolders()
		{
			try
			{
				if (!Directory.Exists(AppFolder)) return false;
				if (!Directory.Exists(AdminToolboxFolder))
					Directory.CreateDirectory(AdminToolboxFolder);
				if (!Directory.Exists(AdminToolboxLogs))
					Directory.CreateDirectory(AdminToolboxLogs);
				MoveOldFiles();
				if (!Directory.Exists(AdminToolboxLogs + Port))
					Directory.CreateDirectory(AdminToolboxLogs + Port);
			}
			catch (Exception)
			{
				return false;
			}
			return Directory.Exists(AppFolder) && Directory.Exists(AdminToolboxFolder) && Directory.Exists(AdminToolboxLogs) && Directory.Exists(AdminToolboxLogs + Port);
		}

		/// <summary>
		/// Removes <see cref ="AdminToolbox"/> logfiles older than set config time.
		/// </summary>
		public void ManageDatedATLogs() => ManageDatedATLogs(0);
		/// <summary>
		/// Removes <see cref ="AdminToolbox"/> logfiles older than set time.
		/// </summary>
		public void ManageDatedATLogs(uint hoursOld)
		{
			uint configInt = hoursOld > 0 ? hoursOld : (uint)Config.GetIntValue("admintoolbox_logremover_hours_old", 0);

			if (configInt > 0)
			{
				string[] files = Directory.GetFiles(AdminToolboxLogs + Port, "Round-*.txt", SearchOption.TopDirectoryOnly);
				AdminToolbox.singleton.Debug("LogManager file age to keep: " + configInt);
				if (files.Length > 0)
					foreach (string path in files)
					{
						int num = (int)(DateTime.Now - File.GetCreationTime(path)).TotalHours;
						AdminToolbox.singleton.Debug(path + "\nHours Old: " + num);
						if (num > configInt)
						{
							AdminToolbox.singleton.Debug("-----------Deleted--------------\n");
							File.Delete(path);
						}
					}
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

		/// <summary>
		/// Appends <see cref="string"/> <see cref="Array"/> to the <see cref="AdminToolbox"/> log.
		/// </summary>
		internal void WriteToLog(string[] str, ServerLogType logType = ServerLogType.Misc)
		{
			string str2 = string.Empty;
			if (str.Length != 0)
				foreach (string st in str)
					str2 += st;
			WriteToLog(str2, logType);
		}

		/// <summary>
		/// Appends <see cref="string"/> to the <see cref="AdminToolbox"/> log.
		/// </summary>
		internal void WriteToLog(string str, ServerLogType logType = ServerLogType.Misc)
		{
			switch (logType)
			{
				case ServerLogType.TeamKill:
					if (Config.GetBoolValue("admintoolbox_log_teamkills", false, false))
						AddLog(str, logType);
					break;
				case ServerLogType.KillLog:
					if (Config.GetBoolValue("admintoolbox_log_kills", false, false))
						AddLog(str, logType);
					break;
				case ServerLogType.RemoteAdminActivity:
					if (Config.GetBoolValue("admintoolbox_log_commands", false, false))
						AddLog(str, logType);
					break;
				case ServerLogType.PlayerDamage:
					if (Config.GetBoolValue("admintoolbox_log_damage", false, false))
						AddLog(str, logType);
					break;
				default:
					AddLog(str, logType);
					break;
			}
		}

		private void MoveOldFiles()
		{
			if (Directory.Exists(AppFolder + "ATServerLogs"))
			{
				string infoString = (Directory.GetDirectories(AppFolder + "ATServerLogs").Length > 0) ? "\n\n Relocated folders: " : string.Empty;
				foreach (string path in Directory.GetDirectories(AppFolder + "ATServerLogs"))
				{
					if (!Directory.Exists(Path.DirectorySeparatorChar + path.Replace(AppFolder + "ATServerLogs" + Path.DirectorySeparatorChar, string.Empty)))
					{
						Directory.Move(path, path.Replace(AdminToolboxLogs, string.Empty));
						infoString += "\n" + " - " + path.Replace(AppFolder, string.Empty);
					}
				}
				if (infoString != string.Empty)
					AdminToolbox.singleton.Info(infoString + "\n\n New Path: " + AdminToolboxLogs.Replace(AppFolder, string.Empty));
				Directory.Delete(AppFolder + "ATServerLogs");
			}
		}
	}

}
