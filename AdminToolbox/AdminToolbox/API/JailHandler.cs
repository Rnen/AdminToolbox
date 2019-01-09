using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.API
{
	/// <summary>
	/// Contains all Jail-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public class JailHandler
	{
		/// <summary>
		/// <see cref ="AdminToolbox"/> jail <see cref="Vector"/> position
		/// </summary>
		public static Vector JailPos => AdminToolbox.JailPos;

		/// <summary>
		/// Returns <see cref ="List{Player}"/> of jailed <see cref="Player"/>s
		/// </summary>
		/// <remarks> <paramref name="filter"/> is the same as the SMod GetPlayers filter</remarks>
		public static List<Player> GetJailedPlayers(string filter = "")
		{
			if (PluginManager.Manager.Server.GetPlayers().Count > 0 && PluginManager.Manager.Server.Round.Duration > 0)
				return PluginManager.Manager.Server.GetPlayers(filter)
					.Where(p => AdminToolbox.ATPlayerDict.ContainsKey(p.SteamId)
					&& p.TeamRole.Role != Role.UNASSIGNED
					&& p.TeamRole.Role != Role.SPECTATOR
					&& !p.OverwatchMode
					&& AdminToolbox.ATPlayerDict[p.SteamId].isJailed).ToList();
			else
				return new List<Player>();
		}
		internal static void CheckJailedPlayers()
		{
			if (GetJailedPlayers().Count > 0 && PluginManager.Manager.Server.Round.Duration > 0)
				foreach (Player pl in GetJailedPlayers())
					if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId))
						if (!AdminToolbox.ATPlayerDict[pl.SteamId].IsInsideJail) SendToJail(pl);
						else if (AdminToolbox.ATPlayerDict[pl.SteamId].JailedToTime <= DateTime.Now) ReturnFromJail(pl);
		}

		/// <summary>
		/// Sends <see cref="Player"/> to jail
		/// </summary>
		public static bool SendToJail(Player player) => SendToJail(player, null);
		/// <summary>
		/// Sends <see cref="Player"/> to jail
		/// </summary>
		public static bool SendToJail(Player player, DateTime? jailedToTime)
		{
			if (player.TeamRole.Role == Role.SPECTATOR) return false;
			if (AdminToolbox.ATPlayerDict.ContainsKey(player.SteamId))
			{
				API.PlayerSettings psetting = AdminToolbox.ATPlayerDict[player.SteamId];
				psetting.JailedToTime = jailedToTime ?? ((psetting.JailedToTime > DateTime.Now) ? psetting.JailedToTime : DateTime.Now.AddYears(1));
				//Saves original variables
				psetting.originalPos = player.GetPosition();
				if (!psetting.isJailed)
				{
					psetting.previousRole = player.TeamRole.Role;
					psetting.playerPrevInv = player.GetInventory();
					psetting.previousHealth = player.GetHealth();
					psetting.prevAmmo5 = player.GetAmmo(AmmoType.DROPPED_5);
					psetting.prevAmmo7 = player.GetAmmo(AmmoType.DROPPED_7);
					psetting.prevAmmo9 = player.GetAmmo(AmmoType.DROPPED_9);
				}
				//Changes role to Tutorial, teleports to jail, removes inv.
				player.ChangeRole(Role.TUTORIAL, true, false);
				player.Teleport(JailPos, true);
				foreach (Smod2.API.Item item in player.GetInventory())
					item.Remove();
				psetting.isJailed = true;
				return true;
			}
			else
			{
				AdminToolbox.plugin.Debug("Could not send player to jail! Player not in PlayerDict!");
				return false;
			}
		}

		/// <summary>
		/// Returns <see cref="Player"/> from jail
		/// </summary>
		public static void ReturnFromJail(Player player)
		{
			if (player == null || string.IsNullOrEmpty(player.SteamId.Trim())) return;
			if (AdminToolbox.ATPlayerDict.ContainsKey(player.SteamId))
			{
				API.PlayerSettings psetting = AdminToolbox.ATPlayerDict[player.SteamId];
				psetting.isJailed = false;
				psetting.JailedToTime = DateTime.Now;
				player.ChangeRole(psetting.previousRole, true, false);
				player.Teleport(psetting.originalPos, true);
				player.SetHealth(psetting.previousHealth);
				if (psetting.playerPrevInv != null)
				{
					foreach (Smod2.API.Item item in player.GetInventory())
						item.Remove();
					foreach (Smod2.API.Item item in psetting.playerPrevInv)
						player.GiveItem(item.ItemType);
				}
				player.SetAmmo(AmmoType.DROPPED_5, psetting.prevAmmo5);
				player.SetAmmo(AmmoType.DROPPED_7, psetting.prevAmmo7);
				player.SetAmmo(AmmoType.DROPPED_9, psetting.prevAmmo9);
				AdminToolbox.ATPlayerDict[player.SteamId].playerPrevInv = null;
			}
			else
				AdminToolbox.plugin.Info("Could not return player from jail! Player not in PlayerDict!");
		}
	}
}
