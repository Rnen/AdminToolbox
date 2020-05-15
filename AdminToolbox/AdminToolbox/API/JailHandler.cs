using System;
using Smod2;
using Smod2.API;
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
		public static Vector JailPos => AdminToolbox.WarpVectorDict?["jail"]?.Vector.ToSMVector() ?? new Vector(53, 1020, -44);

		private static Server Server => PluginManager.Manager.Server;

		private static void Debug(string message) => AdminToolbox.singleton.Debug("[JailHandler]: " + message);

		/// <summary>
		/// Checks the players marked as "Jailed" to see if they are at where they're supposed to be
		/// <para> Gets run in the <see cref="MyMiscEvents"/>.cs Update event</para>
		/// </summary>
		internal static void CheckJailedPlayers()
		{
			Player[] jailedPlayers = Server.GetPlayers().ToArray().JailedPlayers();
			if (jailedPlayers.Length > 0)
				foreach (Player pl in jailedPlayers)
					if (AdminToolbox.ATPlayerDict.ContainsKey(pl.UserId))
						if (!pl.IsInsideJail()) SendToJail(pl);
						else if (AdminToolbox.ATPlayerDict[pl.UserId].JailedToTime <= DateTime.Now) ReturnFromJail(pl);
		}

		/// <summary>
		/// Sends <see cref="Player"/> to jail
		/// </summary>
		public static bool SendToJail(Player player) => SendToJail(player, null);
		/// <summary>
		/// Sends <see cref="Player"/> to jail, with time overload. <returns> Returns bool of operation success </returns>
		/// </summary>
		/// <param name="player">the <see cref="Player"/> to send into jail</param>
		/// <param name="jailedToTime">the time to jail the player. Null sets the time to remaining time, or if thats null, one year</param>
		public static bool SendToJail(Smod2.API.Player player, DateTime? jailedToTime)
		{
			if (player == null || player.TeamRole.Role == Smod2.API.RoleType.SPECTATOR || player.OverwatchMode) return false;
			Debug($"Attempting to jail {player.Name}");
			if (AdminToolbox.ATPlayerDict.TryGetValue(player.UserId, out PlayerSettings psetting))
			{
				if (!jailedToTime.HasValue || jailedToTime < DateTime.Now)
					Debug($"Jail time for \"{player.Name}\" not specified, jailing for a year.");
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
				Debug($"Variables stored, sending \"{player.Name}\" to jail");
				player.ChangeRole(Smod2.API.RoleType.TUTORIAL, true, false);
				player.Teleport(JailPos, true);
				foreach (SMItem item in player.GetInventory())
					item.Remove();
				psetting.isJailed = true;
				return true;
			}
			else
			{
				Debug($"Could not send player to jail! Player \"{player.Name}\" not in AT-PlayerDict!");
				return false;
			}
		}

		/// <summary>
		/// Removes <see cref="Player"/> from jail and restored original values/position. 
		/// <returns> Returns bool of operation success </returns>
		/// </summary>
		/// <param name="player">the player to return</param>
		public static bool ReturnFromJail(Player player)
		{
			if (player == null || string.IsNullOrEmpty(player.UserId.Trim()))
			{
				Debug("Return: Player or UserId null/empty");
				return false;
			}
			Debug($"Attempting to unjail \"{player.Name}\"");
			if (AdminToolbox.ATPlayerDict.TryGetValue(player.UserId, out PlayerSettings psetting))
			{
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
				AdminToolbox.ATPlayerDict[player.UserId].playerPrevInv = null;
				return true;
			}
			else
			{
				AdminToolbox.singleton.Info("Could not return player from jail! Player not in PlayerDict!");
				return false;
			}
		}
	}
}
