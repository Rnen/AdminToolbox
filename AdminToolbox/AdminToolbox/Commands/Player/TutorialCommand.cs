using System;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class TutorialCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Sets player to TUTORIAL";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") <PLAYER / *>";

		public static readonly string[] CommandAliases = new string[] { "TUT", "TUTORIAL" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				Player[] players = new Player[0];
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
					foreach (Player pl in players)
					{
						if (pl == null) continue;
						Vector originalPos2 = pl.GetPosition();
						Vector newPos2 = pl.PlayerRole.RoleID == Smod2.API.RoleType.SPECTATOR ? AdminToolbox.WarpVectorDict.TryGetVector("tutorial", out Vector vector2) ? vector2 : null : originalPos2;
						pl.ChangeRole(Smod2.API.RoleType.TUTORIAL, spawnTeleport: newPos2 == null, removeHandcuffs: true);
						if (newPos2 != null)
							AdminToolbox.waitForTeleports.Add(new WaitForTeleport { Player = pl, Pos = newPos2, DateTime = DateTime.UtcNow.AddSeconds(1) });
					}
					return new string[] { $"Set {(players.Length > 1 ? players.Length.ToString() + " players roles " : (players?[0]?.Name ?? "1 player") + "'s role ")}) to {Smod2.API.RoleType.TUTORIAL}" };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
