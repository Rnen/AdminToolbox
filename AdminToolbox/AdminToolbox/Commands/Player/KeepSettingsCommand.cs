using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class KeepSettingsCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Toggles that players keeping settings on round restart";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] [BOOLEAN]";

		public static readonly string[] CommandAliases = new string[] { "KEEP", "KEEPSETTINGS" };

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
									AdminToolbox.AddMissingPlayerVariables(pl);
									if (AdminToolbox.ATPlayerDict.TryGetValue(pl.SteamId, out PlayerSettings ps))
										ps.keepSettings = j;
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
							foreach (Player pl in Server.GetPlayers())
							{
								AdminToolbox.AddMissingPlayerVariables(pl);
								if (AdminToolbox.ATPlayerDict.TryGetValue(pl.SteamId, out PlayerSettings ps))
									ps.keepSettings = !ps.keepSettings;
							}
							return new string[] { "Toggled all players KeepSettings" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						string str = "\nPlayers with KeepSettings enabled: \n";
						List<string> myPlayerList = new List<string>();
						foreach (Player pl in Server.GetPlayers())
						{
							if (AdminToolbox.ATPlayerDict.TryGetValue(pl.SteamId, out PlayerSettings ps) && ps.keepSettings)
							{
								myPlayerList.Add(pl.Name);
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
						else str = "\nNo players with \"KeepSettings\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
					AdminToolbox.AddMissingPlayerVariables(myPlayer);
					if (AdminToolbox.ATPlayerDict.TryGetValue(myPlayer.SteamId, out PlayerSettings psett))
						if (args.Length > 1)
						{
							if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { psett.keepSettings = true; }
							else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { psett.keepSettings = false; }
							return new string[] { myPlayer.Name + " KeepSettings: " + psett.keepSettings };
						}
						else
						{
							psett.keepSettings = !psett.keepSettings;
							return new string[] { myPlayer.Name + " KeepSettings: " + psett.keepSettings };
						}
					else
						return new string[] { myPlayer.Name + " not in dictionary" };

				}
				return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
