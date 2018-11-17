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
	public class PlayerSettings
	{
		public string SteamID { get; private set; } = "";
		public bool
			overwatchMode = false,
			godMode = false,
			dmgOff = false,
			destroyDoor = false,
			keepSettings = false,
			lockDown = false,
			instantKill = false,
			isJailed = false;
		public bool IsInsideJail
		{
			get
			{
				if (string.IsNullOrEmpty(this.SteamID))
					foreach (Player ply in AdminToolbox.plugin.Server.GetPlayers())
						if (AdminToolbox.ATPlayerDict.ContainsKey(ply.SteamId))
							AdminToolbox.ATPlayerDict[ply.SteamId].SteamID = ply.SteamId;
				Player player = AdminToolbox.plugin.Server.GetPlayers().Where(p => p.SteamId == SteamID).FirstOrDefault();
				if (player == null) return false;
				Vector jail = AdminToolbox.JailPos,
					pPos = player.GetPosition();
				float x = Math.Abs(pPos.x - jail.x),
					y = Math.Abs(pPos.y - jail.y),
					z = Math.Abs(pPos.z - jail.z);
				if (x > 7 || y > 5 || z > 7)
					return false;
				else
					return true;
			}
		}
		public int
			Kills = 0,
			TeamKills = 0,
			Deaths = 0,
			RoundsPlayed = 0,
			banCount = 0;
		internal int
			previousHealth = 100,
			prevAmmo5 = 0,
			prevAmmo7 = 0,
			prevAmmo9 = 0;
		public Vector DeathPos = Vector.Zero,
			originalPos = Vector.Zero;
		internal Role previousRole = Role.CLASSD;
		internal List<Smod2.API.Item> playerPrevInv = new List<Smod2.API.Item>();
		public DateTime JailedToTime { get; internal set; } = DateTime.Now;
		public DateTime JoinTime { get; internal set; } = DateTime.Now;
		public double MinutesPlayed { get; internal set; } = 1;

		public PlayerSettings(string steamID)
		{
			this.SteamID = steamID;
		}
	}
}
