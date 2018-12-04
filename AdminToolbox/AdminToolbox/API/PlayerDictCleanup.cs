using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using Unity;

namespace AdminToolbox.API
{
	internal class PlayerDictCleanup
	{
		static Server Server => PluginManager.Manager.Server;

		internal static void Clean()
		{
			List<string> currentSteamIDs = Server.GetPlayers().Select(p => p.SteamId).Where(sid => !string.IsNullOrEmpty(sid)).ToList();

			if (AdminToolbox.ATPlayerDict.Count > 0 && currentSteamIDs.Count > 0)
				foreach (string s in AdminToolbox.ATPlayerDict.Where(kp => !currentSteamIDs.Contains(kp.Key) && !kp.Value.keepSettings)
					.Select(k => k.Key).ToList())
				{
					AdminToolbox.ATPlayerDict.Remove(s);
				}

			/* Old code
			List<string> keysToRemove = new List<string>();
			 if (Server.GetPlayers().Count > 0)
				Server.GetPlayers().ForEach(p => { if (!string.IsNullOrEmpty(p.SteamId)) playerSteamIds.Add(p.SteamId); });
			if (AdminToolbox.ATPlayerDict.Count > 0 && currentSteamIDs.Count > 0)
				foreach (KeyValuePair<string, API.PlayerSettings> kp in AdminToolbox.ATPlayerDict)
					if (!currentSteamIDs.Contains(kp.Key) && !kp.Value.keepSettings)
						keysToRemove.Add(kp.Key);
			if (keysToRemove.Count > 0)
				foreach (string key in keysToRemove)
					AdminToolbox.ATPlayerDict.Remove(key); */
		}
	}
}
