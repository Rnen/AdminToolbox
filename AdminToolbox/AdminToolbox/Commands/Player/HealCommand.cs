using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class HealCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Heals player. Use int for spesific amount (optional)";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] (AMOUNT)";

		public static readonly string[] CommandAliases = new string[] { "ATHEAL", "AT-HEAL" };

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
							if (int.TryParse(args[1], out int j))
							{
								int playerNum = 0;
								foreach (Player pl in Server.GetPlayers())
								{
									pl.HP += j;
									playerNum++;
								}
								if (playerNum > 1)
									return new string[] { "Added " + j + " HP to " + playerNum + " player(s)" };
								else
									return new string[] { "Added " + j + " HP to " + playerNum + " player" };
							}
							else
							{
								return new string[] { "Not a valid number!" };
							}
						}
						else
						{
							foreach (Player pl in Server.GetPlayers()) { pl.SetHealth(pl.TeamRole.MaxHP); }
							return new string[] { "Set all players to their default max HP" };
						}
					}
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) return new string[] { "Couldn't find player: " + args[0] };
					if (args.Length > 1)
					{
						if (int.TryParse(args[1], out int j))
						{
							myPlayer.HP += j;
							return new string[] { "Added " + j + " HP " + " to " + myPlayer.Name };
						}
						else
							return new string[] { "Not a valid number!" };
					}
					else
					{
						myPlayer.SetHealth(myPlayer.TeamRole.MaxHP);
						return new string[] { "Set " + myPlayer.Name + " to full HP" };
					}
				}
				else
				{
					return new string[] { GetUsage() };
				}
			}
			else
				return deniedReply;
		}
	}
}
