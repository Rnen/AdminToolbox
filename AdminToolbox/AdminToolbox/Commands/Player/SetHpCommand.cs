using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class SetHpCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;
		public string GetCommandDescription() => "Sets player HP. Use int for amount";
		public string GetUsage() => $"[{string.Join(" / ", CommandAliases)}] [PLAYER] (AMOUNT)";

		public static readonly string[] CommandAliases = new string[] { "ATHP", "ATSETHP", "AT-HP", "AT-SETHP" };

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
									pl.Health = amount;
									playerNum++;
								}
								return new string[] { $"Set {playerNum} player{(playerNum > 1 ? "s" : "")} HP to {amount}HP" };
							}
							else
							{
								return new string[] { $"{args[1]} : Not a valid number!" };
							}
						}
						else
						{
							int count = 0;
							foreach (Player pl in Server.GetPlayers())
							{
								pl.Health = pl.PlayerRole.MaxHP;
								count++;
							}
							return new string[] { $"Set {count} player{(count > 1 ? "s" : "")} to their default max HP" };
						}
					}
					if (!GetFromString.TryGetPlayer(args[0], out Player myPlayer)) 
						return new string[] { "Couldn't get player: " + args[0] }; 
					if (args.Length > 1)
					{
						if (int.TryParse(args[1], out int amount))
						{
							myPlayer.Health = amount;
							return new string[] { $"Set {myPlayer.Name}'s HP to {amount}HP" };
						}
						else
							return new string[] { $"{args[1]} : Not a valid number!" };
					}
					else
					{
						myPlayer.Health = myPlayer.PlayerRole.MaxHP;
						return new string[] { $"Set {myPlayer.Name} to default ({myPlayer.PlayerRole.MaxHP }) HP" };
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
