using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class TeleportCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;
		public string GetCommandDescription() => "Teleports player to player2";
		public string GetUsage() => "TPX [PLAYER] [PLAYER2]";

		public static readonly string[] CommandAliases = new string[] { "TPX" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length > 1)
				{
					if (Utility.AllAliasWords.Contains(args[0].ToUpper()))
					{
						int playerNum = -1;
						Player myTpPlayer = GetFromString.GetPlayer(args[1]);
						if (myTpPlayer == null) { return new string[] { "Couldn't find player: " + args[1] }; ; }
						Vector pos = myTpPlayer.GetPosition();
						foreach (Player pl in Server.GetPlayers())
						{
							if (pl.PlayerID != myTpPlayer.PlayerID)
							{
								pl.Teleport(pos, true);
								playerNum++;
							}
						}
						return new string[] { "Teleported " + playerNum + " players to " + myTpPlayer.Name };
					}
					Player myPlayer = GetFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; ; }
					Player myPlayer2 = GetFromString.GetPlayer(args[1]);
					if (myPlayer2 == null) { return new string[] { "Couldn't find player: " + args[1] }; ; }
					if (args[1] != null)
					{
						myPlayer.Teleport(myPlayer2.GetPosition(), true);
						return new string[] { "Teleported: " + myPlayer.Name + " to " + myPlayer2.Name };
					}
					else
						return new string[] { GetUsage() };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
