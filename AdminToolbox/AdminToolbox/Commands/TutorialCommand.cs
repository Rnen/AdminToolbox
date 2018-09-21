using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
	class TutorialCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Sets player to TUTORIAL";
		}

		public string GetUsage()
		{
			return "(TUT / TUTORIAL) [PLAYER / *]";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			Server server = PluginManager.Manager.Server;
			if (args.Length > 0)
			{
				if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
				{
					if (args.Length > 1)
					{
						if (Int32.TryParse(args[1], out int j))
						{
							int playerNum = 0;
							foreach (Player pl in server.GetPlayers())
							{
								pl.ChangeRole(Role.TUTORIAL);
								playerNum++;
							}
							if (playerNum > 1)
								return new string[] { playerNum + " roles set to " + Role.TUTORIAL };
							else
								return new string[] { playerNum + " role set to " + Role.TUTORIAL };
						}
						else
							return new string[] { "Not a valid number!" };
					}
					else
					{
						foreach (Player pl in server.GetPlayers()) { pl.ChangeRole(Role.TUTORIAL); }
						return new string[] { "Changed all players to " + Role.TUTORIAL };
					}
				}
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
				Vector originalPos = myPlayer.GetPosition();
				myPlayer.ChangeRole(Role.TUTORIAL);
				myPlayer.Teleport(originalPos, true);
				return new string[] { "Set " + myPlayer.Name + " to " + Role.TUTORIAL };
			}
			else
				return new string[] { GetUsage() };
		}
	}
}