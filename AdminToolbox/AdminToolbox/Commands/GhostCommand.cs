using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class GhostCommand : ICommandHandler
	{
		private AdminToolbox plugin;

		public GhostCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Sets the player's visibility to other players";
		}

		public string GetUsage()
		{
			return "GHOST (PLAYER) (BOOL: GHOSTMODE) (BOOL: VISIBLE TO SPECATOR) (BOOL: VISIBLE WHEN TALKING)";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			AdminToolbox.AddMissingPlayerVariables();
			Server server = PluginManager.Manager.Server;
			if(args.Length == 0 && sender is Player p)
			{
				p.SetGhostMode(!p.GetGhostMode());
				return new string[] { "Toggled " + p.Name + "'s GhostMode to " + p.GetGhostMode() };
			}
			if (args.Length > 0)
			{
				if (args[0].ToLower() == "help")
					return new string[] { GetUsage() };
				if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
				{
					if (args.Length > 1)
					{
						if (bool.TryParse(args[1], out bool state))
						{
							bool visibleTospec = (args.Length > 2 && bool.TryParse(args[2], out bool xvis)) ? xvis : true;
							bool visibleTalking = (args.Length > 3 && bool.TryParse(args[3], out bool xtalk)) ? xtalk : true;
							string outPut = string.Empty;
							int playerNum = 0;
							foreach (Player pl in server.GetPlayers())
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
						server.GetPlayers().ForEach(ply => ply.SetGhostMode(!ply.GetGhostMode()));
						return new string[] { "Toggled all players GhostMode!" };
					}
				}
				else if (args[0].ToLower() == "list" || args[0].ToLower() == "get")
				{
					string str = "\nPlayers with GhostMode enabled: \n";
					List<string> myPlayerList = new List<string>();

					foreach (Player pl in server.GetPlayers())
						if (pl.GetGhostMode())
							myPlayerList.Add(pl.Name);

					if (myPlayerList.Count > 0)
					{
						myPlayerList.Sort();
						foreach (var item in myPlayerList)
							str += "\n - " + item;
					}
					else
						str = "\nNo players with \"GhostMode\" enabled!";
					return new string[] { str };
				}
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
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
	}
}