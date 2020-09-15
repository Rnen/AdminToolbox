using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class LockdownCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Locks all the doors for specified players";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] [BOOLEAN]";

		public static readonly string[] CommandAliases = new string[] { "PLAYERLOCKDOWN", "PL", "PLOCK", "PLAYERLOCK" };

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
								string outPut = null;
								int playerNum = 0;
								foreach (Player pl in Server.GetPlayers())
								{
									Managers.ATFile.AddMissingPlayerVariables(pl);
									if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings ps))
									{
										ps.lockDown = j;
										playerNum++;
									}
								}
								outPut += "\nSet " + playerNum + " player's Lockdown to " + j;
								return new string[] { outPut };
							}
							else
								return new string[] { "Not a valid bool!" };
						}
						else
						{
							foreach (Player pl in Server.GetPlayers())
							{
								Managers.ATFile.AddMissingPlayerVariables(pl);
								if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings ps))
									ps.lockDown = !ps.lockDown;
							}
							return new string[] { "Toggled all players Lockdown" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						string str = "\nPlayers with Lockdown enabled: \n";
						List<string> myPlayerList = new List<string>();
						foreach (Player pl in Server.GetPlayers())
						{
							if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings pls) && pls.lockDown)
							{
								myPlayerList.Add(pl.Name);
								//str += " - " +pl.Name + "\n";
							}
						}
						if (myPlayerList.Count > 0)
						{
							myPlayerList.Sort();
							foreach (string item in myPlayerList)
							{
								str += "\n - " + item;
							}
						}
						else str = "\nNo players with \"LockDown\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
					Managers.ATFile.AddMissingPlayerVariables(myPlayer);
					if (AdminToolbox.ATPlayerDict.TryGetValue(myPlayer.UserId, out PlayerSettings psetting))
						if (args.Length > 1)
						{
							if (bool.TryParse(args[1], out bool g)) psetting.lockDown = g;
							else if (args[1].ToLower() == "on") { psetting.lockDown = true; }
							else if (args[1].ToLower() == "off") { psetting.lockDown = false; }
							else return new string[] { GetUsage() };
							return new string[] { myPlayer.Name + " Lockdown: " + psetting.lockDown };
						}
						else
						{
							psetting.lockDown = !psetting.lockDown;
							return new string[] { myPlayer.Name + " Lockdown: " + psetting.lockDown };
						}
					else
						return new string[] { myPlayer.Name + " is not in the dictionary" };

				}
				return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
