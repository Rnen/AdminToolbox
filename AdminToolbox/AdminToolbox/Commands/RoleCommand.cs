using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
	class RoleCommand : ICommandHandler
	{
		public string GetCommandDescription()
		{
			return "Sets player to (ROLEID)";
		}

		public string GetUsage()
		{
			return "ROLE [PLAYER] [ROLEID]";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			AdminToolbox.AddMissingPlayerVariables();
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
								Vector originalPos = pl.GetPosition();
								if (pl.TeamRole.Role == Role.UNASSIGNED || pl.TeamRole.Role == Role.SPECTATOR)
									pl.ChangeRole((Role)j, true, true);
								else
								{
									pl.ChangeRole((Role)j, true, false);
									pl.Teleport(originalPos);
								}
								pl.SetHealth(pl.TeamRole.MaxHP);
								playerNum++;
							}
							if (playerNum > 1)
								return new string[] { playerNum + " roles set to " + (Role)j };
							else
								return new string[] { playerNum + " role set to " + (Role)j };
						}
						else
						{
							return new string[] { "Not a valid ID number!" };
						}
					}
					else
					{
						return new string[] { GetUsage() };
					}
				}
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
				if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
				if (args.Length > 1)
				{
					if (Int32.TryParse(args[1], out int j))
					{
						TeamRole oldRole = myPlayer.TeamRole;
						Vector originalPos = myPlayer.GetPosition();
						if (myPlayer.TeamRole.Role == Role.UNASSIGNED || myPlayer.TeamRole.Role == Role.SPECTATOR)
							myPlayer.ChangeRole((Role)j, true, true);
						else
						{
							myPlayer.ChangeRole((Role)j, true, false);
							myPlayer.Teleport(originalPos);
						}
						myPlayer.SetHealth(myPlayer.TeamRole.MaxHP);
						return new string[] { "Changed " + myPlayer.Name + " from " + oldRole.Name + " to " + (Role)j };
					}
					else
						return new string[] { "Not a valid ID number!" };
				}
				else
				{
					return new string[] { GetUsage() };
				}
			}
			else
				return new string[] { GetUsage() };
		}
	}
}
