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
	/// <see cref ="PlayerSettings"/> is <see cref ="AdminToolbox"/>'s settings <see cref="Class"/>
	/// <para>Used in <see cref="AdminToolbox.ATPlayerDict"/></para>
	/// </summary>
	public class PlayerSettings
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		/// <summary>
		/// Checks to see if <see cref ="Player"/> is currently inside radius of <see cref="Vector"/> <see cref ="AdminToolbox.JailPos"/>
		/// </summary>
		public bool IsInsideJail => GetIsInsideJail();
		private bool GetIsInsideJail()
		{
			if (string.IsNullOrEmpty(this.SteamID))
				foreach (KeyValuePair<string, PlayerSettings> kp in AdminToolbox.ATPlayerDict)
					if (string.IsNullOrEmpty(kp.Value.SteamID)) kp.Value.SteamID = kp.Key;
			Player player = AdminToolbox.plugin?.Server.GetPlayers(this.SteamID).FirstOrDefault();
			if (player == null || player.SteamId != this.SteamID) return false;
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

		public int
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
		public Vector DeathPos = Vector.Zero;
		internal Vector originalPos = Vector.Zero;
		internal Role previousRole = Role.CLASSD;
		internal List<Smod2.API.Item> playerPrevInv = new List<Smod2.API.Item>();
		public DateTime JailedToTime { get; internal set; } = DateTime.Now;
		public DateTime JoinTime { get; internal set; } = DateTime.Now;
		public double MinutesPlayed { get; internal set; } = 1;

		public PlayerSettings(string steamID) => this.SteamID = steamID;
		public PlayerSettings(PlayerSettings playerSettings)
		{
			this.SteamID = playerSettings.SteamID;
			this.banCount = playerSettings.banCount;
			this.DeathPos = playerSettings.DeathPos;
			this.Deaths = playerSettings.Deaths;
			this.destroyDoor = playerSettings.destroyDoor;
			this.dmgOff = playerSettings.dmgOff;
			this.godMode = playerSettings.godMode;
			this.instantKill = playerSettings.instantKill;
			this.isJailed = playerSettings.isJailed;
			this.JailedToTime = playerSettings.JailedToTime;
			this.JoinTime = playerSettings.JoinTime;
			this.keepSettings = playerSettings.keepSettings;
			this.Kills = playerSettings.Kills;
			this.lockDown = playerSettings.lockDown;
			this.MinutesPlayed = playerSettings.MinutesPlayed;
			this.originalPos = playerSettings.originalPos;
			this.overwatchMode = playerSettings.overwatchMode;
			this.playerPrevInv = playerSettings.playerPrevInv;
			this.prevAmmo5 = playerSettings.prevAmmo5;
			this.prevAmmo7 = playerSettings.prevAmmo7;
			this.prevAmmo9 = playerSettings.prevAmmo9;
			this.previousHealth = playerSettings.previousHealth;
			this.previousRole = playerSettings.previousRole;
			this.RoundsPlayed = playerSettings.RoundsPlayed;
			this.TeamKills = playerSettings.TeamKills;
		}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
	}
}
