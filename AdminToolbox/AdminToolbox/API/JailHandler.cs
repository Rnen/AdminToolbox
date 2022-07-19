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
		/// Jail <see cref="Vector"/> position
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

		private JailHandler() { }

		private static Server Server => PluginManager.Manager.Server;

		private static void Debug(string message) => AdminToolbox.singleton.Debug("[JailHandler]: " + message);
		private static void Info(string message) => AdminToolbox.singleton.Info("[JailHandler]: " + message);

		/// <summary>
		/// Checks the players marked as "Jailed" to see if they are at where they're supposed to be
		/// <para> Gets run in the <see cref="MyMiscEvents"/> update event on a timer</para>
		/// </summary>
		internal static void CheckJailedPlayers()
		{
			Player[] jailedPlayers = Server.GetPlayers().ToArray().JailedPlayers();
			if (jailedPlayers.Length > 0)
				foreach (Player pl in jailedPlayers)
					if (AdminToolbox.ATPlayerDict.ContainsKey(pl.UserID))
						if (!pl.IsInsideJail()) SendToJail(pl);
						else if (AdminToolbox.ATPlayerDict[pl.UserID].JailReleaseTime <= DateTime.UtcNow) 
							ReturnFromJail(pl);
		}

		/// <summary>
		/// Sends <see cref="Player"/> to Jail for the remaining previous time, or if that's null, one year
		/// </summary>
		/// <param name="player">The <see cref="Player"/> to put in Jail</param>
		/// <returns>Operation success</returns>
		public static bool SendToJail(Player player) => SendToJail(player, null);
		/// <summary>
		/// Puts <see cref="Player"/> in Jail
		/// </summary>
		/// <param name="player">The <see cref="Player"/> to put in Jail</param>
		/// <param name="releaseTime">The release-time of the player. Null sets the time to remaining previous time, or if that's null, one year</param>
		/// <returns>Operation success</returns>
		public static bool SendToJail(Player player, DateTime? releaseTime)
		{
			if (player == null || player.PlayerRole.RoleID == Smod2.API.RoleType.SPECTATOR || player.OverwatchMode) return false;
			Debug($"Attempting to jail {player.Name}");
			if (AdminToolbox.ATPlayerDict.TryGetValue(player.UserID, out PlayerSettings psetting))
			{
				if (!releaseTime.HasValue || releaseTime < DateTime.UtcNow)
					Debug($"Jail time for \"{player.Name}\" not specified, jailing for a year.");
				psetting.JailReleaseTime = releaseTime ?? ((psetting.JailReleaseTime > DateTime.UtcNow) ? psetting.JailReleaseTime : DateTime.UtcNow.AddYears(1));
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
				//Changes role to Tutorial, teleport to jail, removes inventory.
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
		/// </summary>
		/// <param name="player"> The <see cref="Player"/> to remove from Jail</param>
		/// <returns>Operation success</returns>
		public static bool ReturnFromJail(Player player)
		{
			if (player == null || string.IsNullOrEmpty(player.UserID.Trim()))
			{
				Debug("Return: Player or UserID null/empty");
				return false;
			}
			Debug($"Attempting to un-jail \"{player.Name}\"");
			if (AdminToolbox.ATPlayerDict.TryGetValue(player.UserID, out PlayerSettings psetting))
			{
				psetting.isJailed = false;
				psetting.JailReleaseTime = DateTime.UtcNow;
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
