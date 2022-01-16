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
		/// Returns <see cref ="Player"/> from <see cref="string"/> <paramref name="arg"/>
		/// </summary>
		/// <returns><see cref="Player"/></returns>
		public static Player GetPlayer(string arg)
		{
			Player playerOut = null;
			if (string.IsNullOrEmpty(arg))
				return null;
			try
			{
				if (byte.TryParse(arg, out byte pID))
				{
					foreach (Player pl in Server.GetPlayers())
						if (pl.PlayerID == pID)
						{
							playerOut = pl;
							break;
						}
				}
				else if (long.TryParse(arg, out long ID))
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
					playerOut = Server.GetPlayers(arg.ToLower()).OrderBy(s => s.Name.Length).FirstOrDefault();
				}
			}
			catch (System.Exception e)
			{
				AdminToolbox.singleton.Debug($"[GetPlayer Exception]: " + e);
			}
			return playerOut;
		}

		/// <summary>
		/// Gets list of players from <see cref="string"/> <paramref name="name"/> param
		/// </summary>
		/// <returns><see cref="System.Array"/> of <see cref="Player"/></returns>
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
		/// Gets list of players from <see cref="string"/> <paramref name="name"/> param
		/// </summary>
		/// <returns><see cref="System.Array"/> of <see cref="Player"/></returns>
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
		/// Gets list of players from <see cref="string"/> <paramref name="name"/> param
		/// </summary>
		/// <returns><see cref="System.Array"/> of <see cref="Player"/></returns>
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

		/// <summary>
		/// Attempts to get player from <see cref="GetPlayer(string)"/>. 
		/// </summary>
		/// <returns><see cref ="bool"/> based on success</returns> 
		public static bool TryGetPlayer(string arg, out Player player)
		{
			player = GetPlayer(arg);
			return player != null;
		}
	}
}
