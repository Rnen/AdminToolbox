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
	static class Utility
	{
		public static bool TryParseRole(int roleID, out Role role)
		{
			role = Role.UNASSIGNED;
			if (roleID > Enum.GetValues(typeof(Role)).Cast<int>().Max() || roleID < Enum.GetValues(typeof(Role)).Cast<int>().Min())
				return false;
			else
			{
				role = (Role)roleID;
				return true;
			}
		}

		public static readonly int[]
			humanDamageTypes = {
				(int)DamageType.COM15,
				(int)DamageType.E11_STANDARD_RIFLE,
				(int)DamageType.P90,
				(int)DamageType.MP7,
				(int)DamageType.LOGICER,
				(int)DamageType.FRAG
			},
			scpDamagesTypes = {
				(int)DamageType.SCP_049,
				(int)DamageType.SCP_049_2,
				(int)DamageType.SCP_096,
				(int)DamageType.SCP_106,
				(int)DamageType.SCP_173,
				(int)DamageType.SCP_939
			},
			nineTailsTeam = {
				(int)Team.MTF,
				(int)Team.RSC
			},
			chaosTeam = {
				(int)Team.CHI,
				(int)Team.CDP
			};
	}
}
