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
							if (int.TryParse(args[1], out int amount))
							{
								int playerNum = 0;
								foreach (Player pl in Server.GetPlayers())
								{
									pl.HP += amount;
									playerNum++;
								}
								return new string[] { "Added " + amount + " HP to " + playerNum + " player" + (playerNum > 1 ? "s" : "") };
							}
							else
							{
								return new string[] { args[1] + " : Not a valid number!" };
							}
						}
						else
						{
							int count = 0;
							foreach (Player pl in Server.GetPlayers())
							{
								pl.HP = pl.TeamRole.MaxHP;
								count++;
							}
							return new string[] { $"Set {count} player{(count > 1 ? "s" : "")} to their default max HP" };
						}
					}
					if (!GetPlayerFromString.TryGetPlayer(args[0], out Player myPlayer))
						return new string[] { "Couldn't find player: " + args[0] };
					else
					{
						if (args.Length > 1)
						{
							if (int.TryParse(args[1], out int amount))
							{
								myPlayer.HP += amount;
								return new string[] { "Added " + amount + " HP " + " to " + myPlayer.Name };
							}
							else
								return new string[] { "Not a valid number!" };
						}
						else
						{
							myPlayer.HP = myPlayer.TeamRole.MaxHP;
							return new string[] { "Set " + myPlayer.Name + " to full HP" };
						}
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
