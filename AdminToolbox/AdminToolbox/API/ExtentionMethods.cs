using System;
using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.API.Extentions
{
	using API.Webhook;

	public static class ExtentionMethods
	{
		private static Server Server => PluginManager.Manager.Server;

		private static AdminToolbox Plugin => AdminToolbox.singleton;

		internal static bool GetIsJailed(this Player player)
			=> AdminToolbox.ATPlayerDict.ContainsKey(player.UserId) && AdminToolbox.ATPlayerDict[player.UserId].isJailed;

		public static bool IsInsideJail(this Player player)
		{
			Vector
				jail = JailHandler.JailPos,
				playerPos = player.GetPosition();
			float
				x = Math.Abs(playerPos.x - jail.x),
				y = Math.Abs(playerPos.y - jail.y),
				z = Math.Abs(playerPos.z - jail.z);
			return x > 7 || y > 5 || z > 7 ? false : true;
		}

		internal static string[] UserIdsToArray(this List<Player> players)
		{
			string[] newArray = new string[players.Count];
			for (int i = 0; i < players.Count; i++)
			{
				newArray[i] = players[i].UserId;
			}
			return newArray;
		}

		internal static Player[] JailedPlayers(this Player[] players)
		{
			if (players == null)
			{
				throw new ArgumentNullException(nameof(players));
			}

			return players.Length > 0 && Server.Round.Duration > 0
				? players
					.Where(p => p.TeamRole.Role != Smod2.API.RoleType.UNASSIGNED
					&& p.TeamRole.Role != Smod2.API.RoleType.SPECTATOR
					&& !p.OverwatchMode
					&& p.GetIsJailed()).ToArray()
				: (new Player[0]);
		}

		internal static List<Field> AddField(this List<Field> list, string title, string content, bool inline = false)
		{
			list.Add(new Field { name = title, value = content, inline = inline ? "true" : "false" });
			return list;
		}

		private static bool ContainsPlayer(this string[] array, Player player)
		{
			if (array != null && player != null && array.Length > 0)
			{
				foreach (string str in array.Select(s => s.Trim().ToUpper()))
				{
					if (string.IsNullOrEmpty(str))
						continue;
					if (str == player.UserId)
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

		internal static bool IsPlayer(this ICommandSender sender) => sender is Player p && !string.IsNullOrEmpty(p.UserId);

		internal static bool IsPermitted(this ICommandSender sender, string[] commandKey) => sender.IsPermitted(commandKey, false, out string[] reply);

		internal static bool IsPermitted(this ICommandSender sender, string[] commandKey, bool mustBeListed) => sender.IsPermitted(commandKey, mustBeListed, out string[] reply);

		internal static bool IsPermitted(this ICommandSender sender, string[] commandKey, out string[] denied) => sender.IsPermitted(commandKey, false, out denied);

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
					string[] configList = ConfigManager.Manager.Config.GetListValue("admintoolbox_" + command.ToLower() + "_whitelist", new string[0]);
					if (configList.Length > 0)
						validConfigs++;
					if (configList.ContainsPlayer(pl))
						return true;
				}
				string reply = "You are not permitted to use the (" + string.Join(" / ", commandKey) + ")  command!";
				denied = mustBeListed ? new string[] { reply, "You are required to be spesificly whitelisted to use this command." } : new string[] { reply };
				return (mustBeListed || ConfigManager.Manager.Config.GetBoolValue("admintoolbox_whitelist_required", false)) && validConfigs < 1
					? false
					: !(validConfigs > 0);
			}
			else
				return true;
		}

		internal static bool ContainsPlayer(this Dictionary<string, PlayerSettings> dict, Player player)
			=> AdminToolbox.ATPlayerDict?.ContainsKey(player?.UserId) ?? false;

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

		internal static bool TryGetVector(this Dictionary<string, WarpPoint> dict, string key, out Vector vector)
		{
			vector = Vector.Zero;
			if (dict.TryGetValue(key, out WarpPoint wp))
			{
				vector = wp.Vector.ToSMVector();
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Cleans up any <see cref="PlayerSettings"/> that does not have a player attached, and is older than "2" minutes old
		/// </summary>
		internal static void Cleanup(this Dictionary<string, PlayerSettings> dict)
		{
			string[] currentPlayers = PluginManager.Manager.Server.GetPlayers().UserIdsToArray();
			Dictionary<string, PlayerSettings> newDict = new Dictionary<string, PlayerSettings>(dict);
			if (newDict.Count > 0)
			{
				foreach (KeyValuePair<string, PlayerSettings> kp in newDict)
				{
					if (!currentPlayers.Any(s => s == kp.Key) && !kp.Value.keepSettings && Math.Abs((DateTime.Now - kp.Value.JoinTime).TotalMinutes - Server.Round.Duration) > 2)
					{
						AdminToolbox.FileManager.PlayerStatsFileManager(kp.Key, Managers.ATFile.PlayerFile.Write);
						dict.Remove(kp.Key);
					}
				}
			}
		}

		/// <summary>
		/// Colors the team for the MultiAdmin console window
		/// </summary>
		public static string ToColoredMultiAdminTeam(this Player player)
		{
			if (!AdminToolbox.isColored) return player.TeamRole.Name;
			switch ((Team)player.TeamRole.Team)
			{
				case Team.SCP:
					return "@#fg=Red;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.MTF:
					return "@#fg=Blue;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.CHI:
					return "@#fg=Green;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.RSC:
					return "@#fg=Yellow;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.CDP:
					return "@#fg=Orange;" + player.TeamRole.Name + "@#fg=Default;";
				case Team.TUT:
					return "@#fg=Green;" + player.TeamRole.Name + "@#fg=Default;";
				default:
					return player.TeamRole.Name;
			}
		}

		/// <summary>
		/// Colors the team for the rich text game windows
		/// </summary>
		public static string ToColoredRichTextRole(this Player player)
		{
			switch ((Team)player.TeamRole.Team)
			{
				case Team.SCP:
					return "<color=red>" + player.TeamRole.Name + "</color>";
				case Team.MTF:
					return "<color=blue>" + player.TeamRole.Name + "</color>";
				case Team.CHI:
					return "<color=green>" + player.TeamRole.Name + "</color>";
				case Team.RSC:
					return "<color=silver>" + player.TeamRole.Name + "</color>";
				case Team.CDP:
					return "<color=orange>" + player.TeamRole.Name + "</color>";
				case Team.TUT:
					return "<color=lime>" + player.TeamRole.Name + "</color>";
				default:
					return player.TeamRole.Name;
			}
		}
	}
}
