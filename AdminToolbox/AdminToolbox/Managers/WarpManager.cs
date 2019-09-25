using System.Collections.Generic;
using System.IO;
using System.Linq;
using Smod2;

namespace AdminToolbox.Managers
{
	using API;
	/// <summary>
	/// Contains all Warp-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public class WarpManager
	{
		private static AdminToolbox Plugin => AdminToolbox.plugin;

		private static int Port => PluginManager.Manager.Server.Port;

		private static string WarpPointsFolder => ATFileManager.GetFolderPath(Folder.Warps);

		private static string WarpFilePath => WarpPointsFolder + "Global.txt";

		internal readonly Dictionary<string, WarpPoint> presetWarps = new Dictionary<string, WarpPoint>()
			{
				{ "mtf",    new WarpPoint{ Name = "mtf", Description = "The MTF Spawn", Vector = new ATVector(181,994,-61) } },
				{ "grass",  new WarpPoint{ Name = "grass", Description = "Grasslands outside map", Vector = new ATVector(237,999,17) } },
				{ "ci",     new WarpPoint{ Name = "ci", Description = "The Chaos Spawn", Vector = new ATVector(10,989,-60) } },
				{ "jail",   new WarpPoint{ Name = "jail", Description = "The AdminToolbox Jail", Vector = new ATVector(53,1020,-44) } },
				{ "flat",   new WarpPoint{ Name = "flat", Description ="Unreachable grass flatlands", Vector = new ATVector(250,980,110) } },
				{ "heli",   new WarpPoint{ Name = "heli", Description = "MTF Heli outside map", Vector = new ATVector(293,977,-62) } },
				{ "car",    new WarpPoint{ Name = "car", Description = "Chaos Car outside map", Vector = new ATVector(-96,987,-59) } },
				{ "escape", new WarpPoint{ Name = "escape", Description = "The Escape area", Vector = new ATVector(179,996,27) } },
				{ "pocket", new WarpPoint{ Name = "pocket", Description = "The pocket dimention", Vector = new ATVector(0,-2000,0) } }
		};

		private void Debug(string message) => Plugin.Debug(message);


		/// <summary>
		/// Refreshing the <see cref="AdminToolbox.WarpVectorDict"/> from <see cref="File"/>
		/// </summary>
		public void RefreshWarps()
		{
			AdminToolbox.WarpVectorDict = this.ReadWarpsFromFile();
			if (ConfigManager.Manager.Config.GetBoolValue("admintoolbox_warps_remove_underground", true))
			{
				RemoveUndergroundWarps();
				WriteWarpsToFile();
			}
		}

		/// <summary>
		/// Removes any WarpPoint below surface level
		/// <para>Does not affect pocket dimention</para>
		/// </summary>
		public void RemoveUndergroundWarps()
		{
			if (AdminToolbox.WarpVectorDict.Count < 1)
				return;
			List<string> keysToRemove = new List<string>();
			foreach(KeyValuePair<string,WarpPoint> kp in AdminToolbox.WarpVectorDict)
			{
				if (kp.Value.Vector.Y < 900f && kp.Value.Vector.Y > -1900f)
					keysToRemove.Add(kp.Key);
			}
			if (keysToRemove.Count > 0)
				foreach (string key in keysToRemove)
					AdminToolbox.WarpVectorDict.Remove(key);
		}

		/// <summary>
		/// Writes the current <see cref="WarpPoint"/>s in the <see cref="AdminToolbox.WarpVectorDict"/> dictionary to file
		/// </summary>
		public bool WriteWarpsToFile(WarpPoint[] warpPoints = null)
		{
			try
			{
				Plugin.Info("Entered WriteWarpsToFile");
				WarpPoint[] warparray = (warpPoints != null && warpPoints.Length > 0) ? warpPoints : AdminToolbox.WarpVectorDict.Values.ToArray();
				if (warparray == null || warparray.Length < 1)
					return false;
				else
				{
					string jsonData = "";
					Debug("Attempting JSON Serialize " + warparray.Length + " array items!");
					jsonData = UnityEngine.JsonUtility.ToJson(warparray, true);
					Debug("Finished JSON Serialize");
					bool b1 = File.Exists(WarpFilePath);
					Debug("File exists: " + b1);
					Debug("Opening streamwriter");
					using (StreamWriter streamWriter = new StreamWriter(WarpFilePath, false))
					{
						Debug("Streamwriter open, writing");
						streamWriter.Write(jsonData);
						Debug("Streamwriter wrote!");
					}
					Debug("Closing streamwriter");
					if (!b1 && File.Exists(WarpFilePath))
						Plugin.Info("Created a Warps savefile located at: \n" + WarpFilePath);
				}
				return File.Exists(WarpFilePath);
			}
			catch
			{
				AdminToolbox.plugin.Info("Failed during writing of warpfile!");
				return false;
			}
		}

		/// <summary>
		/// Reads from file and returns <see cref ="Dictionary{String, Vector}"/>
		/// </summary>
		public Dictionary<string, WarpPoint> ReadWarpsFromFile()
		{
			try
			{
				Dictionary<string, WarpPoint> newDict = new Dictionary<string, WarpPoint>();
				string jsonData = "";

				if (!File.Exists(WarpFilePath))
				{
					WriteWarpsToFile();
					return presetWarps;
				}
				using (StreamReader streamReader = new StreamReader(WarpFilePath))
				{
					jsonData = streamReader.ReadToEnd();
				}
				if (string.IsNullOrEmpty(jsonData) || !jsonData.StartsWith("["))
					return presetWarps;
				WarpPoint[] warpArray = UnityEngine.JsonUtility.FromJson<WarpPoint[]>(jsonData);
				if (warpArray.Length > 0)
				{
					foreach (WarpPoint wp in warpArray)
						newDict.Add(wp.Name.ToLower(), wp);
				}
				else
					return presetWarps;

				if (!newDict.Any(p => p.Key.ToLower() == "jail"))
					newDict.Add("jail", presetWarps["jail"]);

				if (!newDict.Any(p => p.Key.ToLower() == "pocket"))
					newDict.Add("pocket",presetWarps["pocket"]);

				return newDict;
			}
			catch
			{
				Plugin.Info("Failed during reading of warpfiles!");
				return presetWarps;
			}
		}

	}
}
