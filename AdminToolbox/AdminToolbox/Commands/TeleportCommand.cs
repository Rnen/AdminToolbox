using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	class TeleportCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Teleports player to player2";
		}

		public string GetUsage()
		{
			return "TPX [PLAYER] [PLAYER2]";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			AdminToolbox.AddMissingPlayerVariables();
			Server server = PluginManager.Manager.Server;
			if (args.Length > 1)
			{
				if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
				{
					int playerNum = -1;
					Player myTpPlayer = GetPlayerFromString.GetPlayer(args[1], out myTpPlayer);
					if (myTpPlayer == null) { return new string[] { "Couldn't find player: " + args[1] }; ; }
					foreach (Player pl in server.GetPlayers())
					{
						pl.Teleport(myTpPlayer.GetPosition());
						playerNum++;
					}
					return new string[] { "Teleported " + playerNum + " players to " + myTpPlayer.Name };
				}
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
				if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; ; }
				Player myPlayer2 = GetPlayerFromString.GetPlayer(args[1], out myPlayer2);
				if (myPlayer2 == null) { return new string[] { "Couldn't find player: " + args[1] }; ; }
				if (args[1] != null)
				{
					myPlayer.Teleport(myPlayer2.GetPosition());
					return new string[] { "Teleported: " + myPlayer.Name + " to " + myPlayer2.Name };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return new string[] { GetUsage() };
		}
	}
}