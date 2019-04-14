using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Linq;
using System.Collections.Generic;
using System;
using AdminToolbox.API;


namespace AdminToolbox.API
{
	public static class Utility
	{
		public static bool TryParseRole(int roleID, out Role role)
		{
			role = Role.UNASSIGNED;
			int[] validRoles = Enum.GetValues(typeof(Role)).Cast<int>().ToArray();
			if (!validRoles.Contains(roleID))
				return false;
			else
			{
				role = (Role)roleID;
				return true;
			}
		}

		public static bool TryParseItem(int itemID, out ItemType itemType)
		{
			itemType = ItemType.NULL;
			int[] validItems = Enum.GetValues(typeof(ItemType)).Cast<int>().ToArray();
			if (!validItems.Contains(itemID))
				return false;
			else
			{
				itemType = (ItemType)itemID;
				return true;
			}
		}

		public static string[] AllAliasWords = { "*", "ALL", "EVERY" };

		public static readonly int[]
			HumanDamageTypes = {
				(int)DamageType.COM15,
				(int)DamageType.E11_STANDARD_RIFLE,
				(int)DamageType.P90,
				(int)DamageType.MP7,
				(int)DamageType.LOGICER,
				(int)DamageType.FRAG
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

		public static DamageType Human = DamageType.COM15 | DamageType.E11_STANDARD_RIFLE | DamageType.P90 | DamageType.MP7 | DamageType.LOGICER | DamageType.FRAG;
		public static DamageType SCP = DamageType.SCP_049 | DamageType.SCP_049_2 | DamageType.SCP_096 | DamageType.SCP_106 | DamageType.SCP_173 | DamageType.SCP_939;

		public static bool IsTeam(Player victim, Player attacker)
		{
			if (victim == null || attacker == null || victim.PlayerId == attacker.PlayerId)
				return false;
			if (NineTailsTeam.Contains((int)victim.TeamRole.Team) && NineTailsTeam.Contains((int)attacker.TeamRole.Team))
				return true;
			else if (ChaosTeam.Contains((int)victim.TeamRole.Team) && ChaosTeam.Contains((int)attacker.TeamRole.Team))
				return true;
			else
				return false;
		}

	}
}
