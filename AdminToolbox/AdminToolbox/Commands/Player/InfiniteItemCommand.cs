using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API; //This is for accessing AT's API namespace
	using API.Extentions;
	public class InfiniteItemCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		private static IConfigFile Config => ConfigManager.Manager.Config;

		private Server Server => PluginManager.Manager.Server;

		public InfiniteItemCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "This is a description";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";
		public static readonly string[] CommandAliases = new string[] { "INFI", "INFITEM", "INFINITEI", "INFINITEITEM" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length > 0)
					switch (args[0].ToUpper())
					{
						case "HELP":
							return new string[] { GetUsage() + " <Player> <Item>" };
						case "LIST":
						case "PLAYERS":
							List<string> plNames = new List<string> { "Players with INFINITEITEM enabled:" };
							foreach (Player p in Server.GetPlayers())
								if (AdminToolbox.ATPlayerDict.TryGetValue(p.UserId, out PlayerSettings ps) && ps.InfiniteItem != Smod2.API.ItemType.NULL)
									plNames.Add(" - " + p.Name + " -> + " + ps.InfiniteItem.ToString());
							return plNames.ToArray();
					}

				Player[] players = new Player[0];
				if (args.Length > 0 && Utility.AllAliasWords.Contains(args[0].ToUpper()))
				{
					players = Server.GetPlayers().ToArray();
					if (players.Length < 1)
						return new string[] { "Server is empty!", GetUsage() };
				}
				else
				{
					Player p = (args.Length > 0) ? GetPlayerFromString.GetPlayer(args[0]) : sender as Player;
					if (p == null)
						return new string[] { "Couldn't get player: " + args[0] };
					players = new Player[] { p };
				}
				if (players.Length > 0)
				{
					Smod2.API.ItemType item = args.Length > 1 && Utility.TryParseItem(args[1], out Smod2.API.ItemType i) ? i : Smod2.API.ItemType.NULL;
					foreach (Player pl in players)
					{
						if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings ps))
						{
							ps.InfiniteItem = item;
						}
					}
					return new string[] { $"Set {players.Length.ToString()} player's INFINITEITEM to {item.ToString()}" };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
