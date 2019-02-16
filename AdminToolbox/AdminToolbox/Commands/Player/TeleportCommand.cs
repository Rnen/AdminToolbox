using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	class TeleportCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Teleports player to player2";
		public string GetUsage() => "TPX [PLAYER] [PLAYER2]";

		public static readonly string[] CommandAliases = new string[] { "TPX" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				Server server = PluginManager.Manager.Server;
				if (args.Length > 1)
				{
					if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
					{
						int playerNum = -1;
						Player myTpPlayer = GetPlayerFromString.GetPlayer(args[1]);
						if (myTpPlayer == null) { return new string[] { "Couldn't find player: " + args[1] }; ; }
						Vector pos = myTpPlayer.GetPosition();
						foreach (Player pl in server.GetPlayers())
						{
							if (pl.PlayerId != myTpPlayer.PlayerId)
							{
								pl.Teleport(pos, true);
								playerNum++;
							}
						}
						return new string[] { "Teleported " + playerNum + " players to " + myTpPlayer.Name };
					}
					Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; ; }
					Player myPlayer2 = API.GetPlayerFromString.GetPlayer(args[1]);
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