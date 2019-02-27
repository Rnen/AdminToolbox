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
	/// Contains all Log-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public class LogManager
	{
		static IConfigFile Config => ConfigManager.Manager.Config;

		public LogManager()
		{
			Awake();
		}

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
			AppFolder => ATFileManager.GetFolder(ATFileManager.Folder.AppData);
		private static string
			AdminToolboxFolder => ATFileManager.AdminToolboxFolder;
		private static string
			AdminToolboxLogs => ATFileManager.AdminToolboxLogs;

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
			if(CheckExistingFolders())
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
					streamWriter.Close();
				}
			}
		}

		internal void SetLogStartTime()
		{
			_logStartTime = DateTime.Now.Year.ToString() + "-" + ((DateTime.Now.Month >= 10) ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString())) + "-" + ((DateTime.Now.Day >= 10) ? DateTime.Now.Day.ToString() : ("0" + DateTime.Now.Day.ToString())) + " " + ((DateTime.Now.Hour >= 10) ? DateTime.Now.Hour.ToString() : ("0" + DateTime.Now.Hour.ToString())) + "." + ((DateTime.Now.Minute >= 10) ? DateTime.Now.Minute.ToString() : ("0" + DateTime.Now.Minute.ToString())) + "." + ((DateTime.Now.Second >= 10) ? DateTime.Now.Second.ToString() : ("0" + DateTime.Now.Second.ToString()));
		}

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
		/// Manages <see cref ="AdminToolbox"/> logfiles. <para>Set <see cref="bool"/> 
		/// <paramref name="force"/> to true to forcefully delete all </para>
		/// </summary>
		public void ManageDatedATLogs(bool force = false)
		{
			int configInt = Config.GetIntValue("admintoolbox_logremover_hours_old", 0);

			if (configInt > 0 || force)
			{
				string[] files = Directory.GetFiles(AdminToolboxLogs + Port);
				AdminToolbox.plugin.Info("Cnfig = " + configInt);
				if (files.Length > 0)
					foreach (string path in files)
					{
						int num = (int)(DateTime.Now - File.GetCreationTime(path)).TotalHours;
						AdminToolbox.plugin.Debug(path + "\nHours Old: " + num);
						if (num > configInt)
						{
							AdminToolbox.plugin.Debug("-----------Deleted--------------\n");
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
		/// Appends <see cref="string"/> to the <see cref="AdminToolbox"/> log.
		/// </summary>
		internal void WriteToLog(string str, ServerLogType logType = ServerLogType.Misc)
		{
			WriteToLog(new string[] { str }, logType);
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
			switch (logType)
			{
				case ServerLogType.TeamKill:
					if (Config.GetBoolValue("admintoolbox_log_teamkills", false, false))
						AddLog(str2, logType);
					break;
				case ServerLogType.KillLog:
					if (Config.GetBoolValue("admintoolbox_log_kills", false, false))
						AddLog(str2, logType);
					break;
				case ServerLogType.RemoteAdminActivity:
					if (Config.GetBoolValue("admintoolbox_log_commands", false, false))
						AddLog(str2, logType);
					break;
				case ServerLogType.PlayerDamage:
					if (Config.GetBoolValue("admintoolbox_log_damage", false, false))
						AddLog(str2, logType);
					break;
				default:
					AddLog(str2, logType);
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
					AdminToolbox.plugin.Info(infoString + "\n\n New Path: " + AdminToolboxLogs.Replace(AppFolder, string.Empty));
				Directory.Delete(AppFolder + "ATServerLogs");
			}
		}
	}

}
