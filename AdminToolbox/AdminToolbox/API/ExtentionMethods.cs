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
	static internal class ExtentionMethods
	{
		static Server Server => PluginManager.Manager.Server;

		internal static bool GetIsJailed(this Player player)
		{
			return AdminToolbox.ATPlayerDict.ContainsKey(player.SteamId) && AdminToolbox.ATPlayerDict[player.SteamId].isJailed;
		}

		internal static string[] CurrentOnlineIDs(this List<Player> players)
		{
			string[] newArray = new string[players.Count];
			for(int i = 0; i < players.Count; i++)
			{
				newArray[i] = players[i].SteamId;
			}
			return newArray;
		}

		internal static bool ContainsPlayer(this Dictionary<string,PlayerSettings> dict, Player player)
		{
			return AdminToolbox.ATPlayerDict?.ContainsKey(player.SteamId) ?? false;
		}

		/// <summary>
		/// Cleans up any <see cref="PlayerSettings"/> that does not have a player attached, and is older than "2" minutes old
		/// </summary>
		internal static void Cleanup(this Dictionary<string,PlayerSettings> dict)
		{
			string[] currentPlayers = PluginManager.Manager.Server.GetPlayers().CurrentOnlineIDs();
			Dictionary<string, PlayerSettings> newDict = new Dictionary<string, PlayerSettings>(dict);
			foreach(KeyValuePair<string,PlayerSettings> kp in newDict)
			{
				if (!currentPlayers.Any(s => s == kp.Key) && !kp.Value.keepSettings && Math.Abs((DateTime.Now - kp.Value.JoinTime).TotalMinutes - Server.Round.Duration) > 2)
				{
					dict.Remove(kp.Key);
				}
			}
		}
	}
}
