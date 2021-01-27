using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;

	public class RoleCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;
		public string GetCommandDescription() => "Sets player to specified (ROLE-ID)";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] [ROLE-ID]";

		public static readonly string[] CommandAliases = new string[] { "ROLE", "ROLECHANGE" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length > 0)
				{
					if (Utility.AllAliasWords.Contains(args[0].ToUpper()))
					{
						if (args.Length > 1)
						{
							if (int.TryParse(args[1], out int j) && Utility.TryParseRole(j, out Smod2.API.RoleType specifiedRole))
							{
								int playerNum = 0;
								foreach (Player pl in Server.GetPlayers())
								{
									Vector originalPos = pl.GetPosition();
									if (pl.TeamRole.Role == Smod2.API.RoleType.UNASSIGNED || pl.TeamRole.Role == Smod2.API.RoleType.SPECTATOR)
										pl.ChangeRole(specifiedRole, true, true);
									else
									{
										pl.ChangeRole(specifiedRole, true, false);
										pl.Teleport(originalPos, true);
									}
									pl.HP = pl.TeamRole.MaxHP;
									playerNum++;
								}
								return new string[] { playerNum + " " + (playerNum > 1 ? "roles" : "role") + " set to " + specifiedRole };
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
					if (!GetPlayerFromString.TryGetPlayer(args[0], out Player myPlayer))
						return new string[] { "Couldn't get player: " + args[0] };
					else
					{
						if (args.Length > 1)
						{
							if (int.TryParse(args[1], out int j) && Utility.TryParseRole(j, out Smod2.API.RoleType targetRole))
							{
								TeamRole oldRole = myPlayer.TeamRole;
								Vector originalPos = myPlayer.GetPosition();
								bool isDead = myPlayer.TeamRole.Role == Smod2.API.RoleType.UNASSIGNED || myPlayer.TeamRole.Role == Smod2.API.RoleType.SPECTATOR;
								myPlayer.ChangeRole(targetRole, true, spawnTeleport: isDead);
								if (!isDead)
									myPlayer.Teleport(originalPos, true);
								myPlayer.HP = myPlayer.TeamRole.MaxHP;
								return new string[] { "Changed " + myPlayer.Name + " from " + oldRole.Name + " to " + targetRole };
							}
							else
								return new string[] { "Not a valid ID number!" };
						}
						else
						{
							return new string[] { GetUsage() };
						}
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
