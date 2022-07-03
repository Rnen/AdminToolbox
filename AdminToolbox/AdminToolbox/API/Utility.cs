using System;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Globalization;

namespace AdminToolbox.API
{
	using API.Extentions;
	using Webhook;
	/// <summary>
	/// The plugin's utility class, storing different tools
	/// </summary>
	public static class Utility
	{
		private static IConfigFile Config => ConfigManager.Manager.Config;

		/// <summary>
		/// Safely gets a <see cref="Smod2.API.Role"/> from an <see cref="int"/>. 
		/// </summary>
		/// <param name="roleID"></param>
		/// <param name="role"></param>
		/// <returns>Success. Invalid <see cref="int"/> parameter returns <see cref="Smod2.API.RoleType.NONE"/></returns>
		public static bool TryParseRole(int roleID, out Smod2.API.RoleType role)
		{
			try
			{
				role = (Smod2.API.RoleType)roleID;
			}
			catch
			{
				role = Smod2.API.RoleType.NONE;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Safely gets a <see cref="Smod2.API.ItemType"/> from an <see cref="int"/>. 
		/// </summary>
		/// <param name="itemID"></param>
		/// <param name="itemType"></param>
		/// <returns>Success. Invalid <see cref="int"/> parameter returns <see cref="Smod2.API.ItemType.NONE"/></returns>
		public static bool TryParseItem(int itemID, out Smod2.API.ItemType itemType)
		{
			try
			{
				itemType = (Smod2.API.ItemType)itemID;
			}
			catch
			{
				itemType = Smod2.API.ItemType.NONE;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Safely gets a <see cref="Smod2.API.ItemType"/> from a <see cref="string"/>. 
		/// </summary>
		/// <remarks>Tries to cast to <see cref="int"/> first, then compares names</remarks>
		/// <param name="item">Item either by name or <see cref="int"/> ID</param>
		/// <param name="itemType"><see cref="Smod2.API.ItemType"/></param>
		/// <returns>Success. Invalid <paramref name="item"/> returns <see cref="Smod2.API.ItemType.NONE"/></returns>
		public static bool TryParseItem(string item, out Smod2.API.ItemType itemType)
		{
			itemType = Smod2.API.ItemType.NONE;
			try
			{
				if (int.TryParse(item, out int x))
					return TryParseItem(x, out itemType);

				foreach (Smod2.API.ItemType i in Enum.GetValues(typeof(Smod2.API.ItemType)))
				{
					if (i.ToString().ToUpper().Contains(item.ToUpper()))
					{
						itemType = i;
						return true;
					}
				}
				return true;
			}
			catch
			{
				itemType = Smod2.API.ItemType.NONE;
				return false;
			}
		}

		/// <summary>
		/// Words: <c>*, ALL, EVERY</c>
		/// <para>Used by selectors</para>
		/// </summary>
		/// <remarks>Strings are in all-caps</remarks>
		public static readonly string[] AllAliasWords = { "*", "ALL", "EVERY" };

		/// <summary>
		/// <see cref="DamageType"/> grouped by Damage and Team
		/// </summary>
		public static readonly int[]
			HumanDamageTypes = {
				(int)DamageType.COM15,
				(int)DamageType.AK,
				(int)DamageType.COM18,
				(int)DamageType.CROSSVEC,
				(int)DamageType.E11_SR,
				(int)DamageType.FSP9,
				(int)DamageType.LOGICER,
				(int)DamageType.EXPLOSION, 
				(int)DamageType.MICRO_HID,
				(int)DamageType.REVOLVER,
				(int)DamageType.UNKNOWN_FIREARM,
				(int)DamageType.SHOTGUN
			},
			ScpDamagesTypes = {
				(int)DamageType.SCP_049,
				(int)DamageType.SCP_049_2,
				(int)DamageType.SCP_096,
				//(int)DamageType.SCP_106,
				(int)DamageType.SCP_173,
				(int)DamageType.SCP_939
			},
			NineTailsTeam = {
				(int)TeamType.MTF,
				(int)TeamType.SCIENTIST
			},
			ChaosTeam = {
				(int)TeamType.CHAOS_INSURGENCY,
				(int)TeamType.D_CLASS
			};

		/// <summary>
		/// <see cref="DamageType"/> flags grouped by weapon type
		/// </summary>
		public static readonly DamageType Human = DamageType.COM15 |
				DamageType.COM15 |
				DamageType.AK |
				DamageType.COM18 |
				DamageType.CROSSVEC |
				DamageType.E11_SR |
				DamageType.FSP9 |
				DamageType.LOGICER |
				DamageType.EXPLOSION |
				DamageType.MICRO_HID |
				DamageType.REVOLVER |
				DamageType.UNKNOWN_FIREARM |
				DamageType.SHOTGUN;
		/// <summary>
		/// <see cref="DamageType"/> flags grouped by role type
		/// </summary>
		public static readonly DamageType SCP = DamageType.SCP_049 | DamageType.SCP_049_2 | DamageType.SCP_096 |/* DamageType.SCP_106 |*/ DamageType.SCP_173 | DamageType.SCP_939;

		/// <summary>
		/// Checks if <paramref name="player1"/> and <paramref name="player2"/> are on the same team
		/// </summary>
		/// <param name="player1"><seealso cref="Player"/></param>
		/// <param name="player2"><seealso cref="Player"/></param>
		/// <returns>True if players are on the same team. False when both are the same player or one or more player is null</returns>
		public static bool IsTeam(Player player1, Player player2)
		{
			if (player1 == null || player2 == null || player1.PlayerID == player2.PlayerID)
				return false;
			if (NineTailsTeam.Contains((int)player1.PlayerRole.Team) && NineTailsTeam.Contains((int)player2.PlayerRole.Team))
				return true;
			else if (ChaosTeam.Contains((int)player1.PlayerRole.Team) && ChaosTeam.Contains((int)player2.PlayerRole.Team))
				return true;
			else
				return false;
		}


		internal static DiscordWebhook BuildBanWebhook(Player player, int duration, string reason = "", string issuer = "")
		{
			DiscordWebhook webH;
			List<Field> listOfFields = new List<Field>();
			CultureInfo timeFormat = new CultureInfo("ja-JP"); //Don't ask why Japanese, I just liked the formatting.
			listOfFields.AddField("Player-name: ", player?.Name ?? "TEST", true);
			listOfFields.AddField("Duration: ", (duration / 60).ToString("0", CultureInfo.InvariantCulture) + " minutes", true);
			listOfFields.AddField("Timespan: ", $"[{DateTime.UtcNow.ToString(timeFormat)}] -> [{DateTime.UtcNow.AddSeconds(duration).ToString(timeFormat)}]");
			if (!string.IsNullOrEmpty(reason))
				listOfFields.AddField("Reason: ", reason, true);
			if (Config.GetBoolValue("admintoolbox_ban_webhook_include_admin", false))
				listOfFields.AddField("Issued By: ", string.IsNullOrEmpty(issuer) ? "Server" : issuer, true);

			webH = new DiscordWebhook { embeds = new EmbedData[] { new EmbedData { author = new Author { name = "User Banned: " }, title = "", fields = listOfFields.ToArray() } } };

			return webH;
		}
	}
}
