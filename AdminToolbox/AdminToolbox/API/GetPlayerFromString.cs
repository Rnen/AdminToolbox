using Smod2;
using Smod2.API;
using System.Linq;

namespace AdminToolbox.API
{
	/// <summary>
	/// Class containing the <see cref="GetPlayer(string)"/> constructor
	/// </summary>
	public class GetPlayerFromString
	{
		private static Server Server => PluginManager.Manager.Server;

		/// <summary>
		/// Returns <see cref ="Player"/> from <see cref ="string"/>
		/// </summary>
		public static Player GetPlayer(string args)
		{
			Player playerOut = null;
			if (short.TryParse(args, out short pID))
			{
				foreach (Player pl in Server.GetPlayers())
					if (pl.PlayerId == pID)
						return pl;
			}
			else if (long.TryParse(args, out long sID))
			{
				foreach (Player pl in Server.GetPlayers())
					if (pl.SteamId == sID.ToString())
						return pl;
			}
			else
			{
				return Server.GetPlayers(args.ToLower()).OrderBy(s => s.Name.Length).FirstOrDefault();
				//Takes a string and finds the closest player from the playerlist
				int maxNameLength = 31, LastnameDifference = 31;
				string str1 = args.ToLower();
				foreach (Player pl in Server.GetPlayers(str1))
				{
					if (!pl.Name.ToLower().Contains(args.ToLower()))
						continue;
					if (str1.Length < maxNameLength)
					{
						int x = maxNameLength - str1.Length;
						int y = maxNameLength - pl.Name.Length;
						string str2 = pl.Name;
						for (int i = 0; i < x; i++)
						{
							str1 += "z";
						}
						for (int i = 0; i < y; i++)
						{
							str2 += "z";
						}
						int nameDifference = LevenshteinDistance.Compute(str1, str2);
						if (nameDifference < LastnameDifference)
						{
							LastnameDifference = nameDifference;
							playerOut = pl;
						}
					}
				}
			}
			return playerOut;
		}
	}
}
