using Smod2;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using Smod2.API;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SMItem = Smod2.API.Item;

namespace AdminToolbox.API
{
	using API.Extentions;
	/// <summary>
	/// Contains all Jail-related <see cref ="AdminToolbox"/> functionality
	/// </summary>
	public class JailHandler
	{
		/// <summary>
		/// <see cref ="AdminToolbox"/> jail <see cref="Vector"/> position
		/// </summary>
		public static Vector JailPos => AdminToolbox.WarpVectorDict?["jail"]?.Vector.ToVector() ?? new Vector(53, 1020, -44);

		private static Server Server => PluginManager.Manager.Server;

		/// <summary>
		/// Checks the players marked as "Jailed" to see if they are at where they're supposed to be
		/// <para> Gets run in the <see cref="MyMiscEvents"/>.cs Update event</para>
		/// </summary>
		internal static void CheckJailedPlayers()
		{
			Player[] jailedPlayers = Server.GetPlayers().JailedPlayers();
			if (jailedPlayers.Length > 0)
				foreach (Player pl in jailedPlayers)
					if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId))
						if (!pl.IsInsideJail()) SendToJail(pl);
						else if (AdminToolbox.ATPlayerDict[pl.SteamId].JailedToTime <= DateTime.Now) ReturnFromJail(pl);
		}

		/// <summary>
		/// Sends <see cref="Player"/> to jail
		/// </summary>
		public static bool SendToJail(Player player) => SendToJail(player, null);
		/// <summary>
		/// Sends <see cref="Player"/> to jail, with time overload
		/// </summary>
		/// <param name="player">The Player to send</param>
		/// <param name="jailedToTime">The time to jail the player. Null sets the time to remaining time, or if thats null, one year</param>
		/// <returns> Returns bool of operation success </returns>
		public static bool SendToJail(Player player, DateTime? jailedToTime)
		{
			if (player.TeamRole.Role == Role.SPECTATOR || player.OverwatchMode) return false;
			if (AdminToolbox.ATPlayerDict.ContainsKey(player.SteamId))
			{
				PlayerSettings psetting = AdminToolbox.ATPlayerDict[player.SteamId];
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
				foreach (SMItem item in player.GetInventory())
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
				PlayerSettings psetting = AdminToolbox.ATPlayerDict[player.SteamId];
				psetting.isJailed = false;
				psetting.JailedToTime = DateTime.Now;
				player.ChangeRole(psetting.previousRole, true, false);
				player.Teleport(psetting.originalPos, true);
				player.SetHealth(psetting.previousHealth);
				if (psetting.playerPrevInv != null)
				{
					foreach (SMItem item in player.GetInventory())
						item.Remove();
					foreach (SMItem item in psetting.playerPrevInv)
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
