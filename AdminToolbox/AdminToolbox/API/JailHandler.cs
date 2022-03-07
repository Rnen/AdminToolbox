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
		public static Vector JailPos
		{
			get
			{
				if (AdminToolbox.WarpVectorDict.TryGetValue("jail", out WarpPoint wp))
					return wp.Vector.ToSMVector();
				else
					return new Vector(53, 1020, -44);
			}
		}

		private static Server Server => PluginManager.Manager.Server;

		private static void Debug(string message) => AdminToolbox.singleton.Debug("[JailHandler]: " + message);
		private static void Info(string message) => AdminToolbox.singleton.Info("[JailHandler]: " + message);

		/// <summary>
		/// Checks the players marked as "Jailed" to see if they are at where they're supposed to be
		/// <para> Gets run in the <see cref="MyMiscEvents"/>.cs Update event</para>
		/// </summary>
		internal static void CheckJailedPlayers()
		{
			Player[] jailedPlayers = Server.GetPlayers().ToArray().JailedPlayers();
			if (jailedPlayers.Length > 0)
				foreach (Player pl in jailedPlayers)
					if (AdminToolbox.ATPlayerDict.ContainsKey(pl.UserID))
						if (!pl.IsInsideJail()) SendToJail(pl);
						else if (AdminToolbox.ATPlayerDict[pl.UserID].JailedToTime <= DateTime.UtcNow) 
							ReturnFromJail(pl);
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
			if (player == null || player.PlayerRole.RoleID == Smod2.API.RoleType.SPECTATOR || player.OverwatchMode) return false;
			Debug($"Attempting to jail {player.Name}");
			if (AdminToolbox.ATPlayerDict.TryGetValue(player.UserID, out PlayerSettings psetting))
			{
				if (!jailedToTime.HasValue || jailedToTime < DateTime.UtcNow)
					Debug($"Jail time for \"{player.Name}\" not specified, jailing for a year.");
				psetting.JailedToTime = jailedToTime ?? ((psetting.JailedToTime > DateTime.UtcNow) ? psetting.JailedToTime : DateTime.UtcNow.AddYears(1));
				//Saves original variables
				psetting.originalPos = player.GetPosition();
				if (!psetting.isJailed)
				{
					psetting.previousRole = player.PlayerRole.RoleID;
					psetting.playerPrevInv = player.GetInventory();
					psetting.previousHealth = player.Health;

					foreach (AmmoType a in typeof(AmmoType).GetEnumValues())
						if (a != AmmoType.NONE)
							psetting.Ammo[a] = player.GetAmmo(a);
				}
				//Changes role to Tutorial, teleports to jail, removes inv.
				Debug($"Variables stored, sending \"{player.Name}\" to jail");
				player.ChangeRole(Smod2.API.RoleType.TUTORIAL, true, false);
				player.Teleport(JailPos, true);
				player.ClearInventory();
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
			if (player == null || string.IsNullOrEmpty(player.UserID.Trim()))
			{
				Debug("Return: Player or UserID null/empty");
				return false;
			}
			Debug($"Attempting to unjail \"{player.Name}\"");
			if (AdminToolbox.ATPlayerDict.TryGetValue(player.UserID, out PlayerSettings psetting))
			{
				psetting.isJailed = false;
				psetting.JailedToTime = DateTime.UtcNow;
				player.ChangeRole(psetting.previousRole, true, false);
				player.Teleport(psetting.originalPos, true);
				player.Health = psetting.previousHealth;
				if (psetting.playerPrevInv != null)
				{
					player.ClearInventory();
					foreach (SMItem item in psetting.playerPrevInv)
						player.GiveItem(item.ItemType);
				}
				foreach (AmmoType a in typeof(AmmoType).GetEnumValues())
					if (a != AmmoType.NONE)
						player.SetAmmo(a, psetting.Ammo[a]); 
				AdminToolbox.ATPlayerDict[player.UserID].playerPrevInv = null;
				return true;
			}
			else
			{
				Info("Could not return player from jail! Player not in PlayerDict!");
				return false;
			}
		}
	}
}
