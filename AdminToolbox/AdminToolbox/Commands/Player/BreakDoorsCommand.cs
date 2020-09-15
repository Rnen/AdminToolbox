using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class BreakDoorsCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Toggles that players break doors when interacting with them";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") <PLAYER> <BOOLEAN>";

		public static readonly string[] CommandAliases = new string[] { "BREAKDOORS", "BREAKDOOR", "BD" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				Managers.ATFile.AddMissingPlayerVariables();
				if (args.Length > 0)
				{
					if (Utility.AllAliasWords.Contains(args[0].ToUpper()))
					{
						if (args.Length > 1)
						{
							if (bool.TryParse(args[1], out bool j))
							{
								string outPut = null;
								int playerNum = 0;
								foreach (Player pl in Server.GetPlayers())
								{
									if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings ps))
									{
										ps.destroyDoor = j;
										playerNum++;
									}
								}
								outPut += "Set " + playerNum + " player's BreakDoors to " + j;
								return new string[] { "Set " + playerNum + " player's BreakDoors to " + j };
							}
							else
								return new string[] { "Not a valid bool!" };
						}
						else
						{
							foreach (Player pl in Server.GetPlayers())
							{
								if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings ps))
									ps.destroyDoor = !ps.destroyDoor;
							}
							return new string[] { "Toggled all players BreakDoors" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						string str = "Players with BreakDoors enabled: \n";
						List<string> myPlayerList = new List<string>();
						foreach (Player pl in Server.GetPlayers())
						{
							if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings ps) && ps.destroyDoor)
								myPlayerList.Add(pl.Name);
						}
						if (myPlayerList.Count > 0)
						{
							myPlayerList.Sort();
							foreach (string item in myPlayerList)
								str += "\n - " + item;
						}
						else str = "No players with \"BreakDoors\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null && sender is Player sendingPlayer)
						myPlayer = sendingPlayer;
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
					if (AdminToolbox.ATPlayerDict.TryGetValue(myPlayer.UserId, out PlayerSettings psetting))
						if (args.Length > 1)
						{
							if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { psetting.destroyDoor = true; }
							else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { psetting.destroyDoor = false; }
							return new string[] { myPlayer.Name + " BreakDoors: " + psetting.destroyDoor };
						}
						else
						{
							psetting.destroyDoor = !psetting.destroyDoor;
							return new string[] { myPlayer.Name + " BreakDoors: " + psetting.destroyDoor };
						}
					else
						return new string[] { myPlayer.Name + " not in dictionary" };

				}
				else if (sender is Player p && AdminToolbox.ATPlayerDict.TryGetValue(p.UserId, out PlayerSettings ps))
				{
					ps.destroyDoor = !ps.destroyDoor;
					return new string[] { "Toggled BreakDoors! Currently: " + ps.destroyDoor };
				}
				return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
