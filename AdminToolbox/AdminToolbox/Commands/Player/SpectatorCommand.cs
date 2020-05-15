using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class SpectatorCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;
		public string GetCommandDescription() => "Switch on/off always spectator for player";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] <BOOL>";

		public static readonly string[] CommandAliases = new string[] { "SPECTATOR", "SPEC", "ATOVERWATCH" };

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
							if (bool.TryParse(args[1], out bool j))
							{
								int playerNum = 0;
								foreach (Player player in Server.GetPlayers())
								{
									AdminToolbox.AddMissingPlayerVariables(player);
									if (AdminToolbox.ATPlayerDict.TryGetValue(player.UserId, out PlayerSettings ps))
									{
										ps.overwatchMode = j;
										player.OverwatchMode = j;
										playerNum++;
									}
								}
								if (playerNum > 1)
									return new string[] { playerNum + " player's Overwatch status set to: " + j };
								else
									return new string[] { playerNum + " player Overwatch status set to: " + j };
							}
							else
								return new string[] { "Not a valid bool!" };
						}
						else
						{
							int playerNum = 0;
							foreach (Player pl in Server.GetPlayers())
							{
								if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings plsett))
								{
									plsett.overwatchMode = !plsett.overwatchMode;
									pl.OverwatchMode = !pl.OverwatchMode;
									playerNum++;
								}
							}
							return new string[] { "Toggled " + playerNum + " player's \"OverwatchMode\"" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						string str = "\nPlayers with \"Overwatch\" enabled: \n";
						List<string> myPlayerList = new List<string>();
						foreach (Player pl in Server.GetPlayers())
						{
							if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings plsett) && plsett.overwatchMode)
								myPlayerList.Add(pl.Name);
						}
						if (myPlayerList.Count > 0)
						{
							myPlayerList.Sort();
							foreach (string item in myPlayerList)
								str += "\n - " + item;
						}
						else str = "\nNo players with \"Overwatch\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
					AdminToolbox.AddMissingPlayerVariables(myPlayer);
					if (AdminToolbox.ATPlayerDict.TryGetValue(myPlayer.UserId, out PlayerSettings psetting))
						if (args.Length > 1)
						{
							if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { psetting.overwatchMode = true; myPlayer.OverwatchMode = true; }
							else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { psetting.overwatchMode = false; myPlayer.OverwatchMode = false; }
							return new string[] { myPlayer.Name + " Overwatch: " + psetting.overwatchMode };
						}
						else
						{
							psetting.overwatchMode = !psetting.overwatchMode;
							myPlayer.OverwatchMode = !myPlayer.OverwatchMode;
							return new string[] { myPlayer.Name + " Overwatch: " + psetting.overwatchMode };
						}
					else
						return new string[] { myPlayer.Name + " is not in the dictionary" };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
