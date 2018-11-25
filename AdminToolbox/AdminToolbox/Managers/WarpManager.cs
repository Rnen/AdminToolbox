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
	/// Contains all Warp-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public class WarpManager
	{
		private readonly Dictionary<string, Vector> presetWarps = new Dictionary<string, Vector>()
			{
				{ "mtf",new Vector(181,994,-61) },
				{ "grass",new Vector(237,999,17) },
				{ "ci",new Vector(10,989,-60) },
				{ "jail",new Vector(53,1020,-44) },
				{ "flat",new Vector(250,980,110) },
				{ "heli",new Vector(293,977,-62) },
				{ "car",new Vector(-96,987,-59) },
				{ "topsitedoor",new Vector(89,989,-69)},
				{ "escape", new Vector(179,996,27) }
			};
		static readonly bool globalFiles = ConfigManager.Manager.Config.GetBoolValue("admintoolbox_warpfiles_global", true);

		static int _port => PluginManager.Manager.Server.Port;

		static string ATFolder => AdminToolbox.logManager.GetFolderLocation();
		static readonly string MiscFolder = ATFolder + Path.DirectorySeparatorChar + "Misc";
		static readonly string WarpPointsFolder = MiscFolder + Path.DirectorySeparatorChar + "WarpPoints";
		static readonly string WarpFilePath = WarpPointsFolder + Path.DirectorySeparatorChar + (globalFiles ? "Global"  : _port.ToString()) + ".txt";

		private const char splitChar = ';';

		public WarpManager()
		{

		}

		void CheckAndCreateFolders()
		{
			if (!Directory.Exists(WarpPointsFolder))
				Directory.CreateDirectory(WarpPointsFolder);
			if (!File.Exists(WarpFilePath))
				File.Create(WarpFilePath);
		}

		public bool WriteWarpsToFile()
		{
			return false;
			CheckAndCreateFolders();
			bool fileEmpty = File.ReadAllLines(WarpFilePath).Length <= 1 && File.ReadAllLines(WarpFilePath)[0].Length <= 1;
			
			if (fileEmpty)
			{
				string warps = "";
				foreach (KeyValuePair<string, Vector> kp in presetWarps)
					warps += kp.Key + splitChar + kp.Value.x + splitChar + kp.Value.y + splitChar + kp.Value.z + Environment.NewLine;
				using (StreamWriter st = new StreamWriter(WarpFilePath, false))
				{
					st.Write(warps);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Reads from file and returns <see cref ="Dictionary{String, Vector}"/>
		/// </summary>
		public Dictionary<string,Vector> ReadWarpsFromFile()
		{
			return presetWarps;
			Dictionary<string, Vector> fileWarps = new Dictionary<string, Vector>();
			int lineNmbr = 0;
			foreach (string line in File.ReadAllLines(WarpFilePath))
			{
				if (!line.StartsWith("#") && line.Split(splitChar).Length == 4)
				{
					string[] splitLine = line.Split(splitChar);
					string key = splitLine[0].Trim();

					if (int.TryParse(splitLine[1], out int x) && int.TryParse(splitLine[2], out int y) && int.TryParse(splitLine[3], out int z))
					{
						fileWarps.Add(key, new Vector(x, y, z));
					}
					else
					{
						AdminToolbox.plugin.Error("Line: " + lineNmbr + " is incorrect in: " + WarpFilePath.Replace(FileManager.GetAppFolder(),string.Empty));
					}
				}
				lineNmbr++;
			}
			if (fileWarps.Count > 0 && !fileWarps.ContainsKey("jail"))
				fileWarps.Add("jail",presetWarps["jail"]);
			return fileWarps;
		}

	}
}
