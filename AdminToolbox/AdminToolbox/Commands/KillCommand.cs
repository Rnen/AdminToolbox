using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	class KillCommand : ICommandHandler
	{
		private AdminToolbox plugin;

		public KillCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Kills the targeted player";
		}

		public string GetUsage()
		{
			return "(KILL / SLAY) [PLAYER]";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			AdminToolbox.AddMissingPlayerVariables();
			Player caller = (sender is Player send) ? send : null;
			Server server = PluginManager.Manager.Server;
			if (args.Length > 0)
			{
				if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
				{
					int playerNum = 0;
					foreach (Player pl in server.GetPlayers())
					{
						if (caller != null && (pl.PlayerId == caller.PlayerId) || (AdminToolbox.playerdict[pl.SteamId].godMode || pl.GetGodmode()) /*|| (caller.GetUserGroup().Permissions < pl.GetUserGroup().Permissions)*/) continue;
						pl.Kill();
						playerNum++;
					}
					return new string[] { playerNum + " players has been slain!" };
				}
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; }
				if (myPlayer.TeamRole.Role != Role.SPECTATOR)
				{
					myPlayer.Kill();
					return new string[] { myPlayer + " has been slain!" };
				}
				else
					return new string[] { myPlayer + " is already dead!" };
			}
			else
			{
				return new string[] { GetUsage() };
			}
		}
	}
}