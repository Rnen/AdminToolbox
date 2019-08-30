using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class InstantKillCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Lets specified players instantly kill targets";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] [BOOLEAN]";

		public static readonly string[] CommandAliases = new string[] { "IK", "INSTAKILL", "INSTANTKILL" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length > 0)
				{
					if (Utility.AllAliasWords.Contains(args[0].ToUpper()))
					{
						AdminToolbox.AddMissingPlayerVariables();
						if (args.Length > 1)
						{
							if (bool.TryParse(args[1], out bool j))
							{
								string outPut = null;
								int playerNum = 0;
								foreach (Player pl in Server.GetPlayers())
								{
									if (AdminToolbox.ATPlayerDict.TryGetValue(pl.SteamId, out PlayerSettings ps))
									{
										ps.instantKill = j;
										playerNum++;
									}
								}
								outPut += "\nSet " + playerNum + " player's InstantKill to " + j;
								return new string[] { outPut };
							}
							else
							{
								return new string[] { "Not a valid bool!" };
							}
						}
						else
						{
							foreach (Player pl in Server.GetPlayers())
							{
								if (AdminToolbox.ATPlayerDict.TryGetValue(pl.SteamId, out PlayerSettings psetting))
									psetting.instantKill = !psetting.instantKill;
							}
							return new string[] { "Toggled all players InstantKill" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						AdminToolbox.AddMissingPlayerVariables();
						string str = "\nPlayers with InstantKill enabled: \n";
						List<string> myPlayerList = new List<string>();
						foreach (Player pl in Server.GetPlayers())
						{
							if (AdminToolbox.ATPlayerDict.TryGetValue(pl.SteamId, out PlayerSettings psett) && psett.instantKill)
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
						else str = "\nNo players with \"InstantKill\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
					AdminToolbox.AddMissingPlayerVariables(myPlayer);
					if (args.Length > 1)
					{
						if (AdminToolbox.ATPlayerDict.TryGetValue(myPlayer.SteamId, out PlayerSettings ps))
						{
							if (bool.TryParse(args[1], out bool g)) AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill = g;
							else if (args[1].ToLower() == "on") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill = true; }
							else if (args[1].ToLower() == "off") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill = false; }
							else return new string[] { GetUsage() };
							return new string[] { myPlayer.Name + " InstantKill: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill };
						}
						else
							return new string[] { myPlayer.Name + " not in dictionary" };
					}
					else
					{
						AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill = !AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill;
						return new string[] { myPlayer.Name + " InstantKill: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].instantKill };
					}

				}
				return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
