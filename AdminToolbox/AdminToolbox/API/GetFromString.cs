using System.Linq;
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
		public static Player GetPlayer(string arg)
		{
			Player playerOut = null;
			if (string.IsNullOrEmpty(arg))
				return null;
			try
			{
				if (short.TryParse(arg, out short pID))
				{
					foreach (Player pl in Server.GetPlayers())
						if (pl.PlayerId == pID)
						{
							playerOut = pl;
							break;
						}
				}
				else if (long.TryParse(arg, out long ID))
				{
					foreach (Player pl in Server.GetPlayers())
						if (pl.UserId.Contains(ID.ToString()))
						{
							playerOut = pl;
							break;
						}
				}
				else
				{
					playerOut =  Server.GetPlayers(arg.ToLower()).OrderBy(s => s.Name.Length).FirstOrDefault();
				}
			}
			catch (System.Exception e)
			{
				AdminToolbox.singleton.Debug($"[GetPlayer Exception]: " + e);
			}
			return playerOut;
		}

		/// <summary>
		/// Attempts to get player from <see cref="GetPlayer(string)"/>. 
		/// <returns>Returns <see cref ="bool"/> based on success</returns> 
		/// </summary>
		public static bool TryGetPlayer(string arg, out Player player)
		{
			player = GetPlayer(arg);
			return player != null;
		}
	}
}
