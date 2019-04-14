using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class SetHpCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Sets player HP. Use int for amount";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] (AMOUNT)";

		public static readonly string[] CommandAliases = new string[] { "ATHP", "ATSETHP", "AT-HP", "AT-SETHP" };

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
							if (int.TryParse(args[1], out int j))
							{
								int playerNum = 0;
								foreach (Player pl in server.GetPlayers())
								{
									pl.SetHealth(j);
									playerNum++;
								}
								if (playerNum > 1)
									return new string[] { "Set " + playerNum + " players HP to " + j + "HP" };
								else
									return new string[] { "Set " + playerNum + " players HP to " + j + "HP" };
							}
							else
							{
								return new string[] { "Not a valid number!" };
							}
						}
						else
						{
							foreach (Player pl in server.GetPlayers()) { pl.SetHealth(pl.TeamRole.MaxHP); }
							return new string[] { "Set all players to their default max HP" };
						}
					}
					Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
					if (args.Length > 1)
					{
						if (int.TryParse(args[1], out int j))
						{
							myPlayer.SetHealth(j);
							return new string[] { "Set " + myPlayer.Name + "'s HP to " + j + "HP" };
						}
						else
							return new string[] { "Not a valid number!" };
					}
					else
					{
						myPlayer.SetHealth(myPlayer.TeamRole.MaxHP);
						return new string[] { "Set " + myPlayer.Name + " to default (" + myPlayer.TeamRole.MaxHP + ") HP" };
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
