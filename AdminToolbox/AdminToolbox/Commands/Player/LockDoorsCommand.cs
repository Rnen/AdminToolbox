using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;

	public class LockDoorsCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") <Player>";
		public string GetCommandDescription() => "Makes the user able to lock doors interacted with";

		public static readonly string[] CommandAliases = new string[] { "LOCKDOORS", "ATDL", "ATLD", "ATLOCK", "DOORLOCK" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length > 0 && (args[0].ToLower() == "list" || args[0].ToLower() == "get"))
				{
					string str = "Players with \"LockDoors\" enabled: \n";
					List<string> myPlayerList = new List<string>();
					foreach (Player pl in Server.GetPlayers())
					{
						if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserID, out PlayerSettings psetting) && psetting.lockDoors)
							myPlayerList.Add(pl.Name);
					}
					if (myPlayerList.Count > 0)
					{
						myPlayerList.Sort();
						foreach (string item in myPlayerList)
							str += "\n - " + item;
					}
					else str = "No players with \"LockDoors\" enabled!";
					return new string[] { str };
				}

#pragma warning disable IDE0059 // Unnecessary assignment of a value
				Player[] players = new Player[0];
#pragma warning restore IDE0059 // Unnecessary assignment of a value
				bool? enabled = null;

				if (args.Length > 0 && bool.TryParse(args[0], out bool b1))
					enabled = b1;
				else if (args.Length > 1 && bool.TryParse(args[1], out bool b2))
					enabled = b2;

				if (args.Length > 0 && Utility.AllAliasWords.Contains(args[0].ToUpper()))
				{
					players = Server.GetPlayers().ToArray();
					if (players.Length < 1)
						return new string[] { "Server is empty!", GetUsage() };
				}
				else
				{
					Player p = (args.Length > 0) ? GetFromString.GetPlayer(args[0]) : sender as Player;
					if (p == null)
						return new string[] { "Couldn't get player: " + args[0] };
					players = new Player[] { p };
				}
				if (players.Length > 0)
				{
					int pcount = 0;
					foreach (Player pl in players)
					{
						if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserID, out PlayerSettings ps))
						{
							pcount++;
							if (enabled.HasValue)
								ps.lockDoors = (bool)enabled;
							else
								ps.lockDoors = !ps.lockDoors;
						}
					}
					if (enabled.HasValue)
						return new string[] { $"Set {pcount} players \"LockDoors\" state to {(bool)enabled}" };
					else return new string[] { $"Toogled {pcount} players \"LockDoors\" state." };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
