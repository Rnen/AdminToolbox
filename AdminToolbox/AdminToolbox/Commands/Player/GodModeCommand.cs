using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class GodModeCommand : ICommandHandler
	{
		public bool noDmg = false;

		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Switch on/off godmode for player";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] (BOOL)";

		public static readonly string[] CommandAliases = new string[] { "ATGOD", "ATGODMODE", "AT-GOD", "AT-GODMODE" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (sender is Player p) AdminToolbox.AddMissingPlayerVariables(new List<Player> { p });
				if (args.Length > 0)
				{
					if (Utility.AllAliasWords.Contains(args[0].ToUpper()))
					{
						if (args.Length > 1)
						{
							if (bool.TryParse(args[1], out bool j))
							{
								string outPut = null;
								bool changedState = false;
								if (args.Length > 2) { if (args[2].ToLower() == "nodmg") { noDmg = j; changedState = true; } }
								int playerNum = 0;
								foreach (Player pl in Server.GetPlayers())
								{
									if (AdminToolbox.ATPlayerDict.TryGetValue(pl.SteamId, out PlayerSettings ps))
									{
										ps.godMode = j;
										if (changedState)
											ps.dmgOff = j;
										playerNum++;
									}
								}
								outPut += "\nSet " + playerNum + " player's AT-Godmode to " + j;
								if (changedState) return new string[] { "\n" + "Set " + playerNum + " player's AT-Godmode to " + j, "\n" + "NoDmg for theese " + playerNum + " players set to: " + j };
								return new string[] { "\n" + "Set " + playerNum + " player's AT-Godmode to " + j };
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
									psetting.godMode = !psetting.godMode;
							}
							return new string[] { "Toggled all players AT-Godmodes" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						string str = "\n" + "Players with AT-Godmode enabled: " + "\n";
						List<string> myPlayerList = new List<string>();
						foreach (Player pl in Server.GetPlayers())
						{
							if (AdminToolbox.ATPlayerDict.TryGetValue(pl.SteamId, out PlayerSettings plsetting) && plsetting.godMode)
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
						else str = "\n" + "No players with \"AT-Godmode\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) return new string[] { "Couldn't find player: " + args[0] };
					if (AdminToolbox.ATPlayerDict.TryGetValue(myPlayer.SteamId, out PlayerSettings pls))
					{
						if (args.Length > 1)
						{
							bool changedValue = false;
							if (args.Length > 2) { if (args[2].ToLower() == "nodmg") { changedValue = true; } }
							if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { pls.godMode = true; }
							else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { pls.godMode = false; }
							if (changedValue)
							{
								pls.dmgOff = pls.godMode;
								return new string[] { myPlayer.Name + " AT-Godmode: " + pls.godMode, myPlayer.Name + " No Dmg: " + pls.dmgOff };
							}
							else
								return new string[] { myPlayer.Name + " AT-Godmode: " + pls.godMode };
						}
						else
						{
							pls.godMode = !pls.godMode;
							return new string[] { myPlayer.Name + " AT-Godmode: " + pls.godMode };
						}
					}
					else
						return new string[] { "Player not in dictionary" };

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
