using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API; //This is for accessing AT's API namespace
	using API.Extentions;
	public class GrenadeModeCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Enableds/Disables grenademode";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";
		public static readonly string[] CommandAliases = new string[] { "GMODE", "GRENADEM", "GRENADEMODE", "ATGRENADE" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length > 0)
					switch (args[0].ToUpper())
					{
						case "HELP":
							return new string[] { GetUsage() + " <Player/Bool> <Bool>" };
						case "LIST":
						case "PLAYERS":
							List<string> plNames = new List<string> { "Players with GRENADEMODE enabled:" };
							foreach (Player p in Server.GetPlayers())
								if (AdminToolbox.ATPlayerDict.TryGetValue(p.UserID, out PlayerSettings ps) && ps.grenadeMode)
									plNames.Add(" - " + p.Name);
							return plNames.ToArray();
					}

#pragma warning disable IDE0059 // Unnecessary assignment of a value
				Player[] players = new Player[0];
#pragma warning restore IDE0059 // Unnecessary assignment of a value
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
					bool? s = (args.Length > 1 && bool.TryParse(args[1], out bool b)) ? (bool?)b : null;
					foreach (Player pl in players)
					{
						if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserID, out PlayerSettings ps))
						{
							ps.grenadeMode = s.HasValue ? (bool)s : !ps.grenadeMode;
						}
					}
					return new string[] { $"{(s.HasValue ? $"Set" : "")} {players.Length.ToString()} player's GRENADEMODE " + (s.HasValue ? $"to {s}" : "toggled") };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
