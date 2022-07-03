using System.Linq;
using System.Collections.Generic;
using Smod2;
using Smod2.API;

namespace AdminToolbox.API
{
	/// <summary>
	/// Class containing the <see cref="GetPlayer(string)"/> constructor
	/// </summary>
	public static class GetFromString
	{
		private static Server Server => PluginManager.Manager.Server;

		/// <summary>
		/// Returns the first found <see cref ="Player"/> from the searched string
		/// </summary>
		/// <param name="searchString">The sting used to search. Can be name, PlayerID or SteamID</param>
		/// <returns><see cref="Player"/> or null if player is not found.</returns>
		/// <remarks>It is recommended to use <see cref="TryGetPlayer(string, out Player)"/> instead</remarks>
		public static Player GetPlayer(string searchString)
		{
			Player playerOut = null;
			if (string.IsNullOrEmpty(searchString))
				return null;
			try
			{
				if (byte.TryParse(searchString, out byte pID))
				{
					foreach (Player pl in Server.GetPlayers())
						if (pl.PlayerID == pID)
						{
							playerOut = pl;
							break;
						}
				}
				else if (long.TryParse(searchString, out long ID))
				{
					foreach (Player pl in Server.GetPlayers())
						if (pl.UserID.Contains(ID.ToString()))
						{
							playerOut = pl;
							break;
						}
				}
				else
				{
					playerOut = Server.GetPlayers(searchString.ToLower()).OrderBy(s => s.Name.Length).FirstOrDefault();
				}
			}
			catch (System.Exception e)
			{
				AdminToolbox.singleton.Debug($"[GetPlayer Exception]: " + e);
			}
			return playerOut;
		}

		/// <summary>
		/// Tries getting the supplied player from <see cref="GetPlayer(string)"/>. 
		/// </summary>
		/// <param name="searchString">The sting used to search. Can be name, PlayerID or SteamID</param>
		/// <param name="player">The first <see cref="Player"/> found from the search</param>
		/// <returns>Success</returns>
		public static bool TryGetPlayer(string searchString, out Player player)
		{
			player = GetPlayer(searchString);
			return player != null;
		}

		/// <summary>
		/// Gets a list of players that has the supplied UserGroup, RankName or BadgeText
		/// </summary>
		/// <param name="name">Name of <see cref="UserGroup"/>, badge-text or rank-name</param>
		/// <returns><see cref="Player"/>[] or null if players with supplied group could not be found.</returns>
		public static Player[] GetGroup(string name)
		{
			if (!name.StartsWith("#"))
				name = name.TrimStart(new char[] { '#' }).ToLower();
			if (!string.IsNullOrEmpty(name))
			{
				return Server.GetPlayers().Where(n => n.GetUserGroup().Name.ToLower().Contains(name) || n.GetRankName().ToLower().Contains(name) || n.GetUserGroup().BadgeText.ToLower().Contains(name)).ToArray();
			}
			return null;
		}

		/// <summary>
		/// Gets a list of players that has the supplied role
		/// </summary>
		/// <param name="name">Name of <see cref="RoleType"/></param>
		/// <returns><see cref="Player"/>[] or null if players with supplied role could not be found.</returns>
		public static Player[] GetRole(string name)
		{
			if (!name.StartsWith("$"))
				name = name.TrimStart(new char[] { '$' }).ToLower();
			if (!string.IsNullOrEmpty(name))
			{
				return Server.GetPlayers().Where(n => n.PlayerRole.RoleID.ToString().ToLower().Contains(name)).ToArray();
			}
			return null;
		}

		/// <summary>
		/// Gets a list of players that are on the supplied team
		/// </summary>
		/// <param name="name">Name of <see cref="TeamType"/></param>
		/// <returns><see cref="Player"/>[] or null if players with supplied team could not be found.</returns>
		public static Player[] GetTeam(string name)
		{
			if (!name.StartsWith("$"))
				name = name.TrimStart(new char[] { '$' }).ToLower();
			if (!string.IsNullOrEmpty(name))
			{
				return Server.GetPlayers().Where(n => n.PlayerRole.Team.ToString().ToLower().Contains(name)).ToArray();
			}
			return null;
		}


	}
}
