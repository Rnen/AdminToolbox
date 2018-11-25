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
		public static List<Player> GetJailedPlayers(string filter = "")
		{
			List<Player> myPlayers = new List<Player>();
			if (PluginManager.Manager.Server.GetPlayers().Count > 0)
				if (!string.IsNullOrEmpty(filter))
					foreach (Player pl in PluginManager.Manager.Server.GetPlayers(filter))
					{
						if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && AdminToolbox.ATPlayerDict[pl.SteamId].isJailed)
							myPlayers.Add(pl);
					}
				else
					foreach (Player pl in PluginManager.Manager.Server.GetPlayers())
						if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && AdminToolbox.ATPlayerDict[pl.SteamId].isJailed)
							myPlayers.Add(pl);
			return myPlayers;
		}
		internal static void CheckJailedPlayers()
		{
			if (GetJailedPlayers().Count > 0)
				GetJailedPlayers().ForEach(pl =>
				{
					if (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId))
					{
						if (!AdminToolbox.ATPlayerDict[pl.SteamId].IsInsideJail) SendToJail(pl);
						else if (AdminToolbox.ATPlayerDict[pl.SteamId].JailedToTime <= DateTime.Now) ReturnFromJail(pl);
					}
				});
		}

		/// <summary>
		/// Sends <see cref="Player"/> to jail
		/// </summary>
		public static void SendToJail(Player player, DateTime? jailedToTime = null)
		{
			if (AdminToolbox.ATPlayerDict.ContainsKey(player.SteamId))
			{
				AdminToolbox.ATPlayerDict[player.SteamId].JailedToTime = (jailedToTime.HasValue) ? (DateTime)jailedToTime : DateTime.Now.AddYears(1);
				//Saves original variables
				AdminToolbox.ATPlayerDict[player.SteamId].originalPos = player.GetPosition();
				if (!AdminToolbox.ATPlayerDict[player.SteamId].isJailed)
				{
					AdminToolbox.ATPlayerDict[player.SteamId].previousRole = player.TeamRole.Role;
					AdminToolbox.ATPlayerDict[player.SteamId].playerPrevInv = player.GetInventory();
					AdminToolbox.ATPlayerDict[player.SteamId].previousHealth = player.GetHealth();
					AdminToolbox.ATPlayerDict[player.SteamId].prevAmmo5 = player.GetAmmo(AmmoType.DROPPED_5);
					AdminToolbox.ATPlayerDict[player.SteamId].prevAmmo7 = player.GetAmmo(AmmoType.DROPPED_7);
					AdminToolbox.ATPlayerDict[player.SteamId].prevAmmo9 = player.GetAmmo(AmmoType.DROPPED_9);
				}
				//Changes role to Tutorial, teleports to jail, removes inv.
				player.ChangeRole(Role.TUTORIAL, true, false);
				player.Teleport(JailPos, true);
				foreach (Smod2.API.Item item in player.GetInventory())
					item.Remove();
				AdminToolbox.ATPlayerDict[player.SteamId].isJailed = true;
			}
			else
				AdminToolbox.plugin.Debug("Could not send player to jail! Player not in PlayerDict!");
		}

		/// <summary>
		/// Returns <see cref="Player"/> from jail
		/// </summary>
		public static void ReturnFromJail(Player ply)
		{
			if (AdminToolbox.ATPlayerDict.ContainsKey(ply.SteamId))
			{
				AdminToolbox.ATPlayerDict[ply.SteamId].isJailed = false;
				ply.ChangeRole(AdminToolbox.ATPlayerDict[ply.SteamId].previousRole, true, false);
				ply.Teleport(AdminToolbox.ATPlayerDict[ply.SteamId].originalPos, true);
				ply.SetHealth(AdminToolbox.ATPlayerDict[ply.SteamId].previousHealth);
				foreach (Smod2.API.Item item in ply.GetInventory())
					item.Remove();
				foreach (Smod2.API.Item item in AdminToolbox.ATPlayerDict[ply.SteamId].playerPrevInv)
					ply.GiveItem(item.ItemType);
				ply.SetAmmo(AmmoType.DROPPED_5, AdminToolbox.ATPlayerDict[ply.SteamId].prevAmmo5);
				ply.SetAmmo(AmmoType.DROPPED_7, AdminToolbox.ATPlayerDict[ply.SteamId].prevAmmo7);
				ply.SetAmmo(AmmoType.DROPPED_9, AdminToolbox.ATPlayerDict[ply.SteamId].prevAmmo9);
				AdminToolbox.ATPlayerDict[ply.SteamId].playerPrevInv = null;
			}
			else
				AdminToolbox.plugin.Info("Could not return player from jail! Player not in PlayerDict!");
		}
	}
}
