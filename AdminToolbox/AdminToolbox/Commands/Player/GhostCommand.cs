using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class GhostCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;
		public string GetCommandDescription() => "Sets the player's visibility to other players";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") <PLAYER> <BOOL: ENABLED> <BOOL: VISIBLE TO SPECATOR> <BOOL: VISIBLE WHEN TALKING>";

		public static readonly string[] CommandAliases = new string[] { "GHOST", "GHOSTMODE", "GHOSTM", "INVISIBLE", "GH" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (!ConfigManager.Manager.Config.GetBoolValue("sm_enable_ghostmode", false)) return new string[] { "\"sm_enable_ghostmode\" needs to be set TRUE to use this!" };
				if (args.Length == 0 && sender is Player p)
				{
					p.SetGhostMode(!p.GetGhostMode());
					return new string[]
					{
					"\nSet " + p.Name + " 's GhostMode to:" +
					"\n - Enabled: " + p.GetGhostMode() +
					"\n - Visible to specators: " + true +
					"\n - Visible while talking: " + true
					};
				}
				if (args.Length > 0)
				{
					if (args[0].ToLower() == "help")
						return new string[] { GetUsage() };
					if (Utility.AllAliasWords.Contains(args[0].ToUpper()))
					{
						if (args.Length > 1)
						{
							if (bool.TryParse(args[1], out bool state))
							{
								bool visibleTospec = (args.Length > 2 && bool.TryParse(args[2], out bool xvis)) ? xvis : true;
								bool visibleTalking = (args.Length > 3 && bool.TryParse(args[3], out bool xtalk)) ? xtalk : true;
								string outPut = string.Empty;
								int playerNum = 0;
								foreach (Player pl in Server.GetPlayers())
								{
									pl.SetGhostMode(state, visibleTospec, visibleTalking);
									playerNum++;
								}
								if (state)
									outPut += "\nSet " + playerNum + " player's GhostMode to:"
										+ "\n - Enabled: " + state
										+ "\n - Visible to specators: " + visibleTospec
										+ "\n - Visible while talking: " + visibleTalking;
								else
									outPut += "\nSet " + playerNum + " player's GhostMode to " + state;
								return new string[] { outPut };
							}
							else
								return new string[] { "Not a valid bool!" };
						}
						else
						{
							foreach (Player ply in Server.GetPlayers())
								ply.SetGhostMode(!ply.GetGhostMode());
							return new string[] { "Toggled all players GhostMode!" };
						}
					}
					else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
					{
						string str = "\nPlayers with GhostMode enabled: \n";
						List<string> myPlayerList = new List<string>();

						foreach (Player pl in Server.GetPlayers())
							if (pl.GetGhostMode())
								myPlayerList.Add(pl.Name);

						if (myPlayerList.Count > 0)
						{
							myPlayerList.Sort();
							foreach (string item in myPlayerList)
								str += "\n - " + item;
						}
						else
							str = "\nNo players with \"GhostMode\" enabled!";
						return new string[] { str };
					}
					Player myPlayer = GetFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; }
					if (args.Length > 1)
					{
						if (bool.TryParse(args[1], out bool state))
						{
							bool visibleTospec = (args.Length > 2 && bool.TryParse(args[2], out bool xvis)) ? xvis : true;
							bool visibleTalking = (args.Length > 3 && bool.TryParse(args[3], out bool xtalk)) ? xtalk : true;
							string outPut = string.Empty;

							myPlayer.SetGhostMode(state, visibleTospec, visibleTalking);

							if (state)
								outPut += "\nSet " + myPlayer.Name + " GhostMode to:"
									+ "\n - Enabled: " + state
									+ "\n - Visible to specators: " + visibleTospec
									+ "\n - Visible while talking: " + visibleTalking;
							else
								outPut += "\nSet " + myPlayer.Name + " GhostMode to " + state;
							return new string[] { outPut };
						}
						else
							return new string[] { "Not a valid bool!" };
					}
					else
					{
						myPlayer.SetGhostMode(!myPlayer.GetGhostMode());
						return new string[] { myPlayer.Name + " GhostMode toggled to: " + myPlayer.GetGhostMode() };
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
