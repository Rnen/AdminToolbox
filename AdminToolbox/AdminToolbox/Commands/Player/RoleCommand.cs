using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;

	public class RoleCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Sets player to specified (ROLE-ID)";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] [ROLE-ID]";

		public static readonly string[] CommandAliases = new string[] { "ROLE", "ROLECHANGE" };

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
							if (int.TryParse(args[1], out int j) && Utility.TryParseRole(j, out Role spesifiedRole))
							{
								int playerNum = 0;
								foreach (Player pl in server.GetPlayers())
								{
									Vector originalPos = pl.GetPosition();
									if (pl.TeamRole.Role == Role.UNASSIGNED || pl.TeamRole.Role == Role.SPECTATOR)
										pl.ChangeRole(spesifiedRole, true, true);
									else
									{
										pl.ChangeRole(spesifiedRole, true, false);
										pl.Teleport(originalPos, true);
									}
									pl.SetHealth(pl.TeamRole.MaxHP);
									playerNum++;
								}
								return new string[] { playerNum + " " + (playerNum > 1 ? "roles" : "role") + " set to " + spesifiedRole };
							}
							else
							{
								return new string[] { $"\"{args[1]}\" is not a valid role-ID number!" };
							}
						}
						else
						{
							return new string[] { GetUsage() };
						}
					}
					Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; }
					if (args.Length > 1)
					{
						if (int.TryParse(args[1], out int j) && Utility.TryParseRole(j, out Role spesifiedRole))
						{
							TeamRole oldRole = myPlayer.TeamRole;
							Vector originalPos = myPlayer.GetPosition();
							bool tele = myPlayer.TeamRole.Role == Role.UNASSIGNED || myPlayer.TeamRole.Role == Role.SPECTATOR;
							myPlayer.ChangeRole(spesifiedRole, true, tele);
							if(tele)
								myPlayer.Teleport(originalPos, true);
							myPlayer.SetHealth(myPlayer.TeamRole.MaxHP);
							return new string[] { "Changed " + myPlayer.Name + " from " + oldRole.Name + " to " + spesifiedRole };
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
			else
				return deniedReply;
		}
	}
}
