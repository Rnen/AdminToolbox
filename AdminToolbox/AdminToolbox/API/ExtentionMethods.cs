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
using Smod2.Commands;


namespace AdminToolbox.API.Extentions
{
	using Managers;
	static internal class ExtentionMethods
	{
		static Server Server => PluginManager.Manager.Server;
		static AdminToolbox Plugin => AdminToolbox.plugin;

		internal static bool GetIsJailed(this Player player) 
			=> AdminToolbox.ATPlayerDict.ContainsKey(player.SteamId) && AdminToolbox.ATPlayerDict[player.SteamId].isJailed;

		public static bool IsInsideJail(this Player player)
		{
			Vector 
				jail = JailHandler.JailPos,
				playerPos = player.GetPosition();
			float
				x = Math.Abs(playerPos.x - jail.x),
				y = Math.Abs(playerPos.y - jail.y),
				z = Math.Abs(playerPos.z - jail.z);
			if (x > 7 || y > 5 || z > 7)
				return false;
			else
				return true;
		}

		internal static string[] SteamIDsToArray(this List<Player> players)
		{
			string[] newArray = new string[players.Count];
			for (int i = 0; i < players.Count; i++)
			{
				newArray[i] = players[i].SteamId;
			}
			return newArray;
		}

		internal static Player[] JailedPlayers(this List<Player> players)
		{
			if (players.Count > 0 && Server.Round.Duration > 0)
				return players
					.Where(p => p.TeamRole.Role != Role.UNASSIGNED
					&& p.TeamRole.Role != Role.SPECTATOR
					&& !p.OverwatchMode
					&& p.GetIsJailed()).ToArray();
			else
				return new Player[] { };
		}

		private static bool ContainsPlayer(this string[] array, Player player)
		{
			if (array != null && player != null && array.Length > 0)
			{
				foreach (string str in array.Select(s => s.Trim().ToUpper()))
				{
					if (string.IsNullOrEmpty(str))
						continue;
					if (str == player.SteamId)
						return true;
					else if (player.GetUserGroup().Name != null && str == player.GetUserGroup().Name.Trim().ToUpper())
						return true;
					else if (player.GetUserGroup().BadgeText != null && str == player.GetUserGroup().BadgeText.Trim().ToUpper())
						return true;
					else if (player.GetRankName() != null && str == player.GetRankName().Trim().ToUpper())
						return true;
				}
				return false;
			}
			else //if empty, let anyone use it
			{
				return false;
			}
		}

		internal static bool IsPermitted(this ICommandSender sender, string[] commandKey)
		{
			return sender.IsPermitted(commandKey, false, out string[] reply);
		}
		internal static bool IsPermitted(this ICommandSender sender, string[] commandKey, out string[] denied)
		{
			return sender.IsPermitted(commandKey, false, out denied);
		}
		internal static bool IsPermitted(this ICommandSender sender, string[] commandKey, bool mustBeListed, out string[] denied)
		{
			denied = new string[] { "Error during command whitelist calculation!" };
			if (sender is Player pl)
			{
				if (commandKey.Length < 1)
					return true;
				string[] masterWhitelist = ConfigManager.Manager.Config.GetListValue("admintoolbox_master_whitelist", new string[] { });
				if (masterWhitelist.ContainsPlayer(pl))
					return true;

				int validConfigs = 0;
				foreach (string command in commandKey)
				{
					command.Trim();
					if (string.IsNullOrEmpty(command)) continue;
					//Gets a array of whitelisted users (if any)
					string[] configList = ConfigManager.Manager.Config.GetListValue("admintoolbox_" + command.ToLower() + "_whitelist", new string[] { });
					if (configList.Length > 0)
						validConfigs++;
					if (configList.ContainsPlayer(pl))
						return true;
				}
				denied = new string[] { "You are not permitted to use the (" + string.Join(" / ", commandKey) + ")  command!" };
				if (mustBeListed && validConfigs < 1) return false;
				return !(validConfigs > 0);
			}
			else
				return true;
		}

		internal static bool ContainsPlayer(this Dictionary<string, PlayerSettings> dict, Player player) 
			=> AdminToolbox.ATPlayerDict?.ContainsKey(player?.SteamId) ?? false;

		internal static void ResetPlayerBools(this Dictionary<string, PlayerSettings>.KeyCollection dict)
		{
			string[] keys = dict.ToArray();
			if (keys.Length > 0)
			{
				foreach (string key in keys)
				{
					if (AdminToolbox.ATPlayerDict.ContainsKey(key) && !AdminToolbox.ATPlayerDict[key].keepSettings)
					{
						SetPlayerVariables.SetPlayerBools(key, godMode: false, dmgOff: false, destroyDoor: false, lockDown: false, instantKill: false);
					}
				}
			}
		}

		/// <summary>
		/// Cleans up any <see cref="PlayerSettings"/> that does not have a player attached, and is older than "2" minutes old
		/// </summary>
		internal static void Cleanup(this Dictionary<string, PlayerSettings> dict)
		{
			string[] currentPlayers = PluginManager.Manager.Server.GetPlayers().SteamIDsToArray();
			Dictionary<string, PlayerSettings> newDict = new Dictionary<string, PlayerSettings>(dict);
			if (newDict.Count > 0)
			{
				foreach (KeyValuePair<string, PlayerSettings> kp in newDict)
				{
					if (!currentPlayers.Any(s => s == kp.Key) && !kp.Value.keepSettings && Math.Abs((DateTime.Now - kp.Value.JoinTime).TotalMinutes - Server.Round.Duration) > 2)
					{
						AdminToolbox.atfileManager.PlayerStatsFileManager(kp.Key, Managers.ATFileManager.PlayerFile.Write);
						dict.Remove(kp.Key);
					}
				}
			}
		}
	}
}
