using System;
using System.Collections.Generic;
using Smod2.API;
using SMItem = Smod2.API.Item;

namespace AdminToolbox.API
{
	public class PlayerInfo
	{
		public string LastNickname { get; internal set; } = "Unknown";
		public string SteamID { get; internal set; } = "00000000000000000";
		public bool DNT { get; internal set; } = false;

		public string FirstJoin { get; set; } = "";

		public PlayerInfo() { }
	}
	/// <summary>
	/// <see cref ="PlayerSettings"/> is a class containing all the player's stats for <see cref="AdminToolbox"/>
	/// <para>Used in <see cref="PlayerSettings"/></para>
	/// </summary>
	public class PlayerStats
	{
		public int Kills { get; set; } = 0;
		public int TeamKills { get; set; } = 0;
		public int SuicideCount { get; set; } = 0;
		public int Deaths { get; set; } = 0;
		public int RoundsPlayed { get; set; } = 0;
		public int BanCount { get; set; } = 0;
		public int EscapeCount { get; set; } = 0;

		public double MinutesPlayed { get; set; } = 0.1;

		public PlayerStats() { }
	}
	/// <summary>
	/// <see cref ="PlayerSettings"/> is <see cref ="AdminToolbox"/>'s settings <see cref="Class"/>
	/// <para>Used in <see cref="AdminToolbox.ATPlayerDict"/></para>
	/// </summary>
	public class PlayerSettings
	{
		public bool
			overwatchMode = false,
			godMode = false,
			dmgOff = false,
			destroyDoor = false,
			keepSettings = false,
			lockDown = false,
			instantKill = false,
			isJailed = false,
			lockDoors = false,
			grenadeMode = false;

		public PlayerStats PlayerStats = new PlayerStats();
		public PlayerInfo PlayerInfo = new PlayerInfo();

		internal int
			previousHealth = 100,
			prevAmmo5 = 0,
			prevAmmo7 = 0,
			prevAmmo9 = 0;
		public Vector DeathPos = Vector.Zero;
		internal Vector originalPos = Vector.Zero;
		internal Role previousRole = Role.CLASSD;
		internal List<SMItem> playerPrevInv = new List<SMItem>();

		internal ItemType InfiniteItem = ItemType.NULL;

		public DateTime JailedToTime { get; internal set; } = DateTime.Now;
		public DateTime JoinTime { get; internal set; } = DateTime.Now;


		public PlayerSettings(string steamID) => this.PlayerInfo.SteamID = steamID;
	}
}
