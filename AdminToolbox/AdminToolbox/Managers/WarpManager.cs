using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Unity;
using UnityEngine;
using Newtonsoft.Json;

namespace AdminToolbox.Managers
{
	/// <summary>
	/// Contains all Warp-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public class WarpManager
	{
		internal readonly Dictionary<string, WarpPoint> presetWarps = new Dictionary<string, WarpPoint>()
			{
				{ "mtf",new WarpPoint("mtf", "The MTF Spawn", new Vector(181,994,-61)) },
				{ "grass",new WarpPoint("grass", "Grasslands outside map", new Vector(237,999,17)) },
				{ "ci",new WarpPoint("ci", "The Chaos Spawn", new Vector(10,989,-60)) },
				{ "jail",new WarpPoint("jail", "The AdminToolbox Jail", new Vector(53,1020,-44)) },
				{ "flat",new WarpPoint("flat", "Unreachable grass flatlands", new Vector(250,980,110)) },
				{ "heli",new WarpPoint("heli", "MTF Heli outside map", new Vector(293,977,-62)) },
				{ "car",new WarpPoint("car", "Chaos Car outside map", new Vector(-96,987,-59)) },
				{ "escape", new WarpPoint("escape", "The Escape area", new Vector(179,996,27)) }
			};
		static readonly bool globalFiles = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_warpfiles_global", true);

		static int _port => PluginManager.Manager.Server.Port;

		static string ATFolder => AdminToolbox.logManager.GetFolderLocation();
		static readonly string WarpPointsFolder = ATFolder + Path.DirectorySeparatorChar + "WarpPoints";
		static readonly string WarpFilePath = WarpPointsFolder + Path.DirectorySeparatorChar + (globalFiles ? "Global"  : _port.ToString()) + ".txt";

		private const char splitChar = ';';

		void CheckAndCreateFolders()
		{
			if (!Directory.Exists(WarpPointsFolder))
				Directory.CreateDirectory(WarpPointsFolder);
			if (!File.Exists(WarpFilePath))
				File.Create(WarpFilePath);
		}

		/// <summary>
		/// Refreshing the <see cref="AdminToolbox.warpVectors"/> from <see cref="File"/>
		/// </summary>
		public bool RefreshWarps()
		{
			try
			{
				AdminToolbox.warpVectors = new Dictionary<string, WarpPoint>(this.ReadWarpsFromFile());
				return true;
			}
			catch
			{
				return false;
			}
		}


		public bool WriteWarpsToFile()
		{
			WarpPoint[] warparray = AdminToolbox.warpVectors.Select(s => s.Value).ToArray();
			if (warparray.Length == 0)
				warparray = presetWarps.Select(t => t.Value).ToArray();
			string jsonData = "";
			if (!Directory.Exists(ATFolder))
				Directory.CreateDirectory(ATFolder);
			if (!Directory.Exists(WarpPointsFolder))
				Directory.CreateDirectory(WarpPointsFolder);
			//AdminToolbox.plugin.Info("Warparray length: " + warparray.Length);
			if (warparray.ToList().Count > 0)
			{
				//AdminToolbox.plugin.Info("WarpFile path: " + WarpFilePath);
				jsonData = JsonConvert.SerializeObject(warparray, Formatting.Indented);
				//AdminToolbox.plugin.Info("JsonData: \n" + jsonData);
				bool b1 = File.Exists(WarpFilePath);
				using (StreamWriter streamWriter = new StreamWriter(WarpFilePath, false))
				{
					streamWriter.Write(jsonData);
				}
				if (!b1 && File.Exists(WarpFilePath))
					AdminToolbox.plugin.Info("Created a Warps savefile located at: \n" + WarpFilePath);
			}
			return File.Exists(WarpFilePath);
		}

		/// <summary>
		/// Reads from file and returns <see cref ="Dictionary{String, Vector}"/>
		/// </summary>
		public Dictionary<string, WarpPoint> ReadWarpsFromFile()
		{
			Dictionary<string, WarpPoint> newDict = new Dictionary<string, WarpPoint>();
			string jsonData = "";

			if ((!Directory.Exists(WarpPointsFolder) || !File.Exists(WarpFilePath)))
			{
				if (!WriteWarpsToFile())
				{
					AdminToolbox.plugin.Info("Failed finding warp file at " + WarpFilePath);
					return newDict;
				}
			}
			using (StreamReader streamReader = new StreamReader(WarpFilePath))
			{
				jsonData = streamReader.ReadToEnd();
			}
			WarpPoint[] warpArray = JsonConvert.DeserializeObject<WarpPoint[]>(jsonData);
			if (warpArray.Length > 0)
			{
				foreach (WarpPoint wp in warpArray)
					newDict.Add(wp.Name.ToLower(), wp);
			}
			if (!newDict.Any(p => p.Key.ToLower() == "jail"))
				newDict.Add("jail", new WarpPoint("jail", "AdminToolbox Jail", new Vector(53, 1020, -44)));

			return newDict;
		}

	}
	public class WarpPoint
	{
		public string Name { get; set; }
		public string Description { get; set; } = "";
		public Vector Vector { get; set; }
		public WarpPoint() { }
		public WarpPoint(string name, string desc, Vector vector)
		{
			this.Name = name; this.Description = desc; this.Vector = vector;
		}
		public WarpPoint(WarpPoint wp)
		{
			this.Name = wp.Name; this.Description = wp.Description; this.Vector = wp.Vector;
		}
		public override string ToString()
		{
			return Name + " - " + Description + " - (" + Vector + ")";
		}
	}
}
