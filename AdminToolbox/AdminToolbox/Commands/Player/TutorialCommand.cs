using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	class TutorialCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Sets player to TUTORIAL";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER / *]";

		public static readonly string[] CommandAliases = new string[] { "TUT", "TUTORIAL" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
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
									pl.ChangeRole(Role.TUTORIAL, spawnTeleport: false, removeHandcuffs: true);
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
							foreach (Player pl in server.GetPlayers()) { pl.ChangeRole(Role.TUTORIAL, spawnTeleport: false, removeHandcuffs: true); }
							return new string[] { "Changed all players to " + Role.TUTORIAL };
						}
					}
					Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
					Vector originalPos = myPlayer.GetPosition();
					myPlayer.ChangeRole(Role.TUTORIAL, spawnTeleport: false, removeHandcuffs: true);
					myPlayer.Teleport(originalPos, true);
					return new string[] { "Set " + myPlayer.Name + " to " + Role.TUTORIAL };
				}
				else if (sender is Player p)
				{
					Vector originalPos = p.GetPosition();
					p.ChangeRole(Role.TUTORIAL, spawnTeleport: false, removeHandcuffs: true);
					p.Teleport(originalPos, true);
					return new string[] { "Set " + p.Name + " to " + Role.TUTORIAL };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}