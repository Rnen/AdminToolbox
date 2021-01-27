using System;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Globalization;

using SMRoleType = Smod2.API.RoleType;
using SMItemType = Smod2.API.ItemType;

namespace AdminToolbox.API
{
	using API.Extentions;
	using Webhook;
	public static class Utility
	{
		private static IConfigFile Config => ConfigManager.Manager.Config;

		/// <summary>
		/// Safely getting a <see cref="Smod2.API.Role"/> from an <see cref="int"/>. 
		/// <returns>Returns <see cref="bool"/> based on success</returns>
		/// <para>Invalid <see cref="int"/> parameters returns <see cref="Smod2.API.Role.UNASSIGNED"/></para>
		/// </summary>
		public static bool TryParseRole(int roleID, out SMRoleType role)
		{
			try
			{
				role = (SMRoleType)roleID;
			}
			catch
			{
				role = SMRoleType.UNASSIGNED;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Safely getting a <see cref="Smod2.API.ItemType"/> from an <see cref="int"/>. 
		/// <returns>Returns <see cref="bool"/> based on success</returns>
		/// <para>Invalid <see cref="int"/> parameters returns <see cref="Smod2.API.ItemType.NULL"/></para>
		/// </summary>
		public static bool TryParseItem(int itemID, out SMItemType itemType)
		{
			try
			{
				itemType = (SMItemType)itemID;
			}
			catch
			{
				itemType = SMItemType.NULL;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Safely getting a <see cref="Smod2.API.ItemType"/> from a <see cref="string"/>. 
		/// <returns>Returns <see cref="bool"/> based on success</returns>
		/// <para>Tries to cast to <see cref="int"/> first, then compares names</para>
		/// </summary>
		public static bool TryParseItem(string item, out SMItemType itemType)
		{
			itemType = SMItemType.NULL;

			try
			{
				if (int.TryParse(item, out int x))
					return TryParseItem(x, out itemType);

				foreach (SMItemType i in Enum.GetValues(typeof(SMItemType)))
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
				itemType = SMItemType.NULL;
				return false;
			}
		}

		/// <summary>
		/// <c>*, ALL, EVERY</c>
		/// <para>Uses all-caps</para>
		/// </summary>
		public static string[] AllAliasWords = { "*", "ALL", "EVERY" };

		public static readonly int[]
			HumanDamageTypes = {
				(int)DamageType.COM15,
				(int)DamageType.E11_STANDARD_RIFLE,
				(int)DamageType.P90,
				(int)DamageType.MP7,
				(int)DamageType.LOGICIER,
				(int)DamageType.GRENADE, 
				(int)DamageType.MICROHID
			},
			ScpDamagesTypes = {
				(int)DamageType.SCP_049,
				(int)DamageType.SCP_049_2,
				(int)DamageType.SCP_096,
				(int)DamageType.SCP_106,
				(int)DamageType.SCP_173,
				(int)DamageType.SCP_939
			},
			NineTailsTeam = {
				(int)Team.MTF,
				(int)Team.RSC
			},
			ChaosTeam = {
				(int)Team.CHI,
				(int)Team.CDP
			};

		public static readonly DamageType Human = DamageType.COM15 | DamageType.E11_STANDARD_RIFLE | DamageType.P90 | DamageType.MP7 | DamageType.LOGICIER | DamageType.GRENADE | DamageType.MICROHID;
		public static readonly DamageType SCP = DamageType.SCP_049 | DamageType.SCP_049_2 | DamageType.SCP_096 | DamageType.SCP_106 | DamageType.SCP_173 | DamageType.SCP_939;


		/// <summary>
		/// Checks if two <see cref="Player"/>s are on the same team
		/// <para>Returns False when both are the same player</para>
		/// </summary>
		public static bool IsTeam(Player player1, Player player2)
		{
			if (player1 == null || player2 == null || player1.PlayerId == player2.PlayerId)
				return false;
			if (NineTailsTeam.Contains((int)player1.TeamRole.Team) && NineTailsTeam.Contains((int)player2.TeamRole.Team))
				return true;
			else if (ChaosTeam.Contains((int)player1.TeamRole.Team) && ChaosTeam.Contains((int)player2.TeamRole.Team))
				return true;
			else
				return false;
		}


		internal static DiscordWebhook BuildBanWebhook(Player player, int duration, string reason = "", string issuer = "")
		{
			DiscordWebhook webH;
			List<Field> listOfFields = new List<Field>();
			CultureInfo timeFormat = new CultureInfo("ja-JP");
			listOfFields.AddField("Playername: ", player?.Name ?? "TEST", true);
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
