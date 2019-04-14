using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class TutorialCommand : ICommandHandler
	{
		private static Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Sets player to TUTORIAL";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER / *]";

		public static readonly string[] CommandAliases = new string[] { "TUT", "TUTORIAL" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length > 0 && (args[0].ToLower() == "all" || args[0].ToLower() == "*"))
				{
					Player[] players = Server.GetPlayers().ToArray();
					foreach (Player pl in players)
					{
						Vector originalPos2 = pl.GetPosition();
						Vector newPos2 = pl.TeamRole.Role == Role.SPECTATOR ? AdminToolbox.WarpVectorDict.TryGetVector("tutorial", out Vector vector2) ? vector2 : Server.Map.GetSpawnPoints(pl.TeamRole.Role)?[0] : originalPos2;
						pl.ChangeRole(Role.TUTORIAL, spawnTeleport: newPos2 == null, removeHandcuffs: true);
						if (newPos2 != null)
							AdminToolbox.waitForTeleports.Add(new WaitForTeleport(pl, newPos2));
					}
					return new string[] { $"Changed all ({players.Length}) players roles to " + Role.TUTORIAL };
				}
				Player myPlayer = (args.Length > 0) ? GetPlayerFromString.GetPlayer(args[0]) : sender as Player;
				if (myPlayer == null)
				{ return new string[] { "Couldn't get player: " + args[0] }; }
				Vector originalPos = myPlayer.GetPosition();
				Vector newPos = myPlayer.TeamRole.Role == Role.SPECTATOR ? AdminToolbox.WarpVectorDict.TryGetVector("tutorial", out Vector vector) ? vector : Server.Map.GetSpawnPoints(myPlayer.TeamRole.Role)?[0] : originalPos;
				myPlayer.ChangeRole(Role.TUTORIAL, spawnTeleport: newPos == null, removeHandcuffs: true);

				return new string[] { "Set " + myPlayer.Name + "'s role to " + Role.TUTORIAL };
			}
			else
				return deniedReply;
		}
	}
}
