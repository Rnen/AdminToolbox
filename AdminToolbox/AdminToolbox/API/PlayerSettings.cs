using System;
using System.Collections.Generic;
using Smod2.API;

namespace AdminToolbox.API
{
	public class PlayerInfo
	{
		/// <summary>
		/// Last known nickname of the player
		/// </summary>
		public string LastNickname { get; internal set; } = "Unknown";
		/// <summary>
		/// The player's UserId
		/// </summary>
		public string UserId { get; internal set; } = "00000000000000000";

		/// <summary>
		/// The last known instance of the player's DNT setting
		/// </summary>
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
	}
	/// <summary>
	/// <see cref ="PlayerSettings"/> is <see cref ="AdminToolbox"/>'s settings
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

		internal float
			previousHealth = 100;
		internal int 
			prevAmmo5 = 0,
			prevAmmo7 = 0,
			prevAmmo9 = 0;
		public Vector DeathPos = Vector.Zero;
		internal Vector originalPos = Vector.Zero;
		internal Smod2.API.RoleType previousRole = Smod2.API.RoleType.CLASSD;
		internal List<Smod2.API.Item> playerPrevInv = new List<Smod2.API.Item>();

		internal Smod2.API.ItemType InfiniteItem = Smod2.API.ItemType.NULL;

		public DateTime JailedToTime { get; internal set; } = DateTime.UtcNow;
		public DateTime JoinTime { get; internal set; } = DateTime.UtcNow;


		public PlayerSettings(string UserId) => this.PlayerInfo.UserId = UserId;
	}
}
