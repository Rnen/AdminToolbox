using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class NoDmgCommand : ICommandHandler
	{
		private Server Server = PluginManager.Manager.Server;

		public string GetCommandDescription() => "Switch on/off damageOutput for player";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] (BOOL)";

		public static readonly string[] CommandAliases = new string[] { "NODMG", "DMGOFF", "DMGDISABLE" };

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
								foreach (Player pl in Server.GetPlayers())
								{
									AdminToolbox.AddMissingPlayerVariables(pl);
									if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings ps))
									{
										ps.dmgOff = j;
										playerNum++;
									}
								}
								return new string[] { "Set " + playerNum + " player's \"No Dmg\" to " + j };
							}
							else
								return new string[] { "Not a valid bool!" };
						}
						else
						{
							foreach (Player pl in Server.GetPlayers())
							{
								AdminToolbox.AddMissingPlayerVariables(pl);
								if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings ps))
									ps.dmgOff = !ps.dmgOff;
							}
							return new string[] { "Toggled all player's \"No Dmg\"" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						string str = "\nPlayers with \"No Dmg\" enabled: \n";
						List<string> myPlayerList = new List<string>();
						foreach (Player pl in Server.GetPlayers())
						{
							if (AdminToolbox.ATPlayerDict.TryGetValue(pl.UserId, out PlayerSettings pls) && pls.dmgOff)
								myPlayerList.Add(pl.Name);
						}
						if (myPlayerList.Count > 0)
						{
							myPlayerList.Sort();
							foreach (string item in myPlayerList)
							{
								str += "\n - " + item;
							}
						}
						else str = "\nNo players with \"No Dmg\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; }
					AdminToolbox.AddMissingPlayerVariables(myPlayer);
					if (AdminToolbox.ATPlayerDict.TryGetValue(myPlayer.UserId, out PlayerSettings psetting))
						if (args.Length > 1)
						{
							bool changedValue = false;
							if (args.Length > 2) { if (args[2].ToLower() == "godmode") { changedValue = true; } }
							if (args[1].ToLower() == "on" || args[1].ToLower() == "true") { psetting.dmgOff = true; }
							else if (args[1].ToLower() == "off" || args[1].ToLower() == "false") { psetting.dmgOff = false; }
							if (changedValue)
							{
								psetting.godMode = psetting.dmgOff;
								return new string[] { myPlayer.Name + " No Dmg: " + psetting.dmgOff, myPlayer.Name + " Godmode: " + psetting.godMode };
							}
							return new string[] { myPlayer.Name + " No Dmg: " + psetting.dmgOff };
						}
						else
						{
							psetting.dmgOff = !psetting.dmgOff;
							return new string[] { myPlayer.Name + " No Dmg: " + psetting.dmgOff };
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
