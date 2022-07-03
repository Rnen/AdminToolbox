using System;
using System.Collections.Generic;
using Smod2.API;

namespace AdminToolbox.API
{
	/// <summary>
	/// Class storing basic data of a Player, used by <see cref="PlayerSettings"/>
	/// </summary>
	public class PlayerInfo
	{
		/// <summary>
		/// Last known nickname of the player
		/// </summary>
		public string LastNickname { get; internal set; } = "Unknown";
		/// <summary>
		/// The player's UserID
		/// </summary>
		public string UserID { get; internal set; } = "00000000000000000";

		/// <summary>
		/// The last known instance of the player's DNT setting
		/// </summary>
		public bool DNT { get; internal set; } = false;
		/// <summary>
		/// DateTime of first join in UNIX
		/// </summary>
		public string FirstJoin { get; set; } = "";

		internal PlayerInfo() { }
	}

	/// <summary>
	/// Class storing statistics of a player, used by <see cref="PlayerSettings"/>
	/// </summary>
	public class PlayerStats
	{
		/// <summary>
		/// Count of times killed members of other teams
		/// </summary>
		public int Kills { get; set; } = 0;
		/// <summary>
		/// Count of times killed members of same team
		/// </summary>
		public int TeamKills { get; set; } = 0;
		/// <summary>
		/// Count of times killed by yourself
		/// </summary>
		public int SuicideCount { get; set; } = 0;
		/// <summary>
		/// Count of times killed by others
		/// </summary>
		public int Deaths { get; set; } = 0;
		/// <summary>
		/// Count of rounds played
		/// </summary>
		public int RoundsPlayed { get; set; } = 0;
		/// <summary>
		/// Count of times banned (more than 1 minute)
		/// </summary>
		public int BanCount { get; set; } = 0;
		/// <summary>
		/// Count of times escaped as a Scientist/Class-D
		/// </summary>
		public int EscapeCount { get; set; } = 0;
		/// <summary>
		/// Minutes played on the server
		/// </summary>
		public double MinutesPlayed { get; set; } = 0.1;

		internal PlayerStats() { }
	}

	/// <summary>
	/// Settings assigned to each player to keep track of states and info. Used by <see cref="AdminToolbox.ATPlayerDict"/>
	/// </summary>
	public class PlayerSettings
	{
		/// <summary>
		/// State used by commands and logic
		/// </summary>
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

		/// <summary>
		/// The statistics class. See <seealso cref="API.PlayerStats"/>
		/// </summary>
		public PlayerStats PlayerStats = new PlayerStats();
		/// <summary>
		/// The information class. See <seealso cref="API.PlayerInfo"/>
		/// </summary>
		public PlayerInfo PlayerInfo = new PlayerInfo();

		internal float
			previousHealth = 100;
		internal Dictionary<AmmoType, int> Ammo = 
			new Dictionary<AmmoType, int>() { [AmmoType.AMMO_44_CAL] = 0, [AmmoType.AMMO_12_GAUGE] = 0, 
				[AmmoType.AMMO_556_X45] = 0, [AmmoType.AMMO_762_X39] = 0, [AmmoType.AMMO_9_X19] = 0};
		/// <summary>
		/// The last recorded death position of the player
		/// </summary>
		public Vector DeathPos = Vector.Zero;
		internal Vector originalPos = Vector.Zero;
		internal Smod2.API.RoleType previousRole = Smod2.API.RoleType.D_CLASS;
		internal List<Smod2.API.Item> playerPrevInv = new List<Smod2.API.Item>();

		internal Smod2.API.ItemType InfiniteItem = Smod2.API.ItemType.NONE;

		/// <summary>
		/// At what time the player should be released from Jail
		/// </summary>
		public DateTime JailReleaseTime { get; internal set; } = DateTime.UtcNow;
		/// <summary>
		/// At what time the player joined the server
		/// </summary>
		public DateTime JoinTime { get; internal set; } = DateTime.UtcNow;

		/// <summary>
		/// Constructor that requires a ID to set up
		/// </summary>
		/// <param name="UserID">The ID that will be assigned to the stored class. 
		/// This is what will be used to look this up again later</param>
		public PlayerSettings(string UserID) => this.PlayerInfo.UserID = UserID;
	}
}
