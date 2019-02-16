using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	class KeepSettingsCommand : ICommandHandler
	{
		public string GetCommandDescription() =>"Toggles that players keeping settings on round restart";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] [BOOLEAN]";

		public static readonly string[] CommandAliases = new string[] { "KEEP", "KEEPSETTINGS" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if(sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				
				Server server = PluginManager.Manager.Server;
				if (args.Length > 0)
				{
					if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
					{
						AdminToolbox.AddMissingPlayerVariables();
						if (args.Length > 1)
						{
							if (bool.TryParse(args[1], out bool j))
							{
								string outPut = null;
								int playerNum = 0;
								foreach (Player pl in server.GetPlayers())
								{
									AdminToolbox.ATPlayerDict[pl.SteamId].keepSettings = j;
									playerNum++;
								}
								outPut += "\nSet " + playerNum + " player's KeepSettings to " + j;
								return new string[] { outPut };
							}
							else
							{
								return new string[] { "Not a valid bool!" };
							}
						}
						else
						{
							foreach (Player pl in server.GetPlayers()) { AdminToolbox.ATPlayerDict[pl.SteamId].keepSettings = !AdminToolbox.ATPlayerDict[pl.SteamId].keepSettings; }
							return new string[] { "Toggled all players KeepSettings" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						AdminToolbox.AddMissingPlayerVariables();
						string str = "\nPlayers with KeepSettings enabled: \n";
						List<string> myPlayerList = new List<string>();
						foreach (Player pl in server.GetPlayers())
						{
							if (AdminToolbox.ATPlayerDict[pl.SteamId].keepSettings)
							{
								myPlayerList.Add(pl.Name);
							}
						}
						if (myPlayerList.Count > 0)
						{
							myPlayerList.Sort();
							foreach (var item in myPlayerList)
							{
								str += "\n - " + item;
							}
						}
						else str = "\nNo players with \"KeepSettings\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
					AdminToolbox.AddMissingPlayerVariables(myPlayer);
					if (args.Length > 1)
					{
						if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].keepSettings = true; }
						else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { AdminToolbox.ATPlayerDict[myPlayer.SteamId].keepSettings = false; }
						return new string[] { myPlayer.Name + " KeepSettings: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].keepSettings };
					}
					else
					{
						AdminToolbox.ATPlayerDict[myPlayer.SteamId].keepSettings = !AdminToolbox.ATPlayerDict[myPlayer.SteamId].keepSettings;
						return new string[] { myPlayer.Name + " KeepSettings: " + AdminToolbox.ATPlayerDict[myPlayer.SteamId].keepSettings };
					}

				}
				return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}