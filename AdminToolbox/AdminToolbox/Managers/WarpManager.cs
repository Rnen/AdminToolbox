using System.Collections.Generic;
using System.IO;
using System.Linq;
using Smod2;

namespace AdminToolbox.Managers
{
	using System;
	using API;

	/// <summary>
	/// Contains all Warp-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public class WarpManager
	{
		private static AdminToolbox Plugin => AdminToolbox.singleton;

		private static int Port => PluginManager.Manager.Server.Port;

		private static string WarpPointsFolder => ATFile.GetFolderPath(Folder.Warps);

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

		private void Debug(string message) => Plugin.Debug("[WARPMANAGER]: " + message);


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
			Debug("Removing underground warps");
			List<string> keysToRemove = new List<string>();
			foreach(KeyValuePair<string,WarpPoint> kp in AdminToolbox.WarpVectorDict)
			{
				if (kp.Value.Vector.Y < 900f && kp.Value.Vector.Y > -1900f)
				{
					keysToRemove.Add(kp.Key);
				}
			}
			if (keysToRemove.Count > 0)
			{
				foreach (string key in keysToRemove)
					AdminToolbox.WarpVectorDict.Remove(key);
				Debug($"{string.Join(" ,", keysToRemove)} removed from warps due to being in the deletion zone.");
			}
		}

		/// <summary>
		/// Writes the current <see cref="WarpPoint"/>s in the <see cref="AdminToolbox.WarpVectorDict"/> dictionary to file
		/// </summary>
		/// <param name="warpPoints">the array of WarpPoints to write, null writes the AT WarpVectorDict</param>
		public bool WriteWarpsToFile(WarpPoint[] warpPoints = null)
		{
			Debug("Entered WriteToFile");
			if (!ConfigManager.Manager.Config.GetBoolValue("admintoolbox_warpfiles", true))
				return false;
			try
			{
				
				WarpPoint[] warparray = (warpPoints != null && warpPoints.Length > 0) ? warpPoints : AdminToolbox.WarpVectorDict.Values.ToArray();
				if (warparray == null || warparray.Length < 1)
				{
					Debug("Warparray was null or empty, returning");
					return false;
				}
				else
				{
					string jsonData = "";
					Debug("Attempting JSON Serialize " + warparray.Length + " array items!");
					foreach (WarpPoint w in warparray)
						Debug(w.Name);
					jsonData = Utf8Json.JsonSerializer.PrettyPrint(Utf8Json.JsonSerializer.Serialize(warparray)); 
					//JsonConvert.SerializeObject(warparray, Formatting.Indented);
					//jsonData = JsonUtility.ToJson(warparray, true);
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
			catch (Exception e)
			{
				Plugin.Info("Failed during writing of warpfile!");
				Debug("Error occured during writing to file: " + e.Message);
				return false;
			}
		}

		/// <summary>
		/// Reads from file and returns <see cref ="Dictionary{String, WarpPoint}"/> of (<see cref="string"/>,<see cref="WarpPoint"/>)
		/// </summary>
		public Dictionary<string, WarpPoint> ReadWarpsFromFile()
		{
			Debug("Entered ReadFromFile");
			if (!ConfigManager.Manager.Config.GetBoolValue("admintoolbox_warpfiles", true))
				return presetWarps;
			try
			{
				Dictionary<string, WarpPoint> newDict = new Dictionary<string, WarpPoint>();
				string jsonData = "";

				if (!File.Exists(WarpFilePath))
				{
					Debug("File path not found, writing new file");
					WriteWarpsToFile();
					return presetWarps;
				}
				Debug("Reading file...");
				using (StreamReader streamReader = new StreamReader(WarpFilePath))
				{
					jsonData = streamReader.ReadToEnd();
				}
				if (string.IsNullOrEmpty(jsonData) || (!jsonData.StartsWith("[")))
				{
					Debug("File data empty or not JSON");
					return presetWarps;
				}
				Debug("Converting JSON to array");

				WarpPoint[] warpArray = Utf8Json.JsonSerializer.Deserialize<WarpPoint[]>(jsonData);
				//WarpPoint[] warpArray = JsonConvert.DeserializeObject<WarpPoint[]>(jsonData);
				//WarpPoint[] warpArray = UnityEngine.JsonUtility.FromJson<WarpPoint[]>(jsonData);
				if (warpArray.Length > 0)
				{
					Debug("Populating dict with json array");
					foreach (WarpPoint wp in warpArray)
						newDict.Add(wp.Name.ToLower(), wp);
				}
				else
				{
					Debug("Array empty, returning preset");
					return presetWarps;
				}

				if (!newDict.Any(p => p.Key.ToLower() == "jail"))
				{
					Debug("Jail Warp not found, adding");
					newDict.Add("jail", presetWarps["jail"]);
				}

				if (!newDict.Any(p => p.Key.ToLower() == "pocket"))
				{
					Debug("Pocket warp not found, adding");
					newDict.Add("pocket", presetWarps["pocket"]);
				}

				return newDict;
			}
			catch (Exception e)
			{
				Plugin.Info("Failed during reading of warpfiles!");
				Debug("Error occured during reading of file: " + e.Message);
				return presetWarps;
			}
		}

	}
}
