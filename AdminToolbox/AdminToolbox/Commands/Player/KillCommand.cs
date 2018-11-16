using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	class KillCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;
		Server Server => PluginManager.Manager.Server;
		IConfigFile Config => ConfigManager.Manager.Config;

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
			Player caller = (sender is Player send) ? send : null;
			DamageType killType = (DamageType)Config.GetIntValue("admintoolbox_slaycommand_killtype", 0, true);

			AdminToolbox.AddMissingPlayerVariables();
			if (args.Length > 0)
			{
				if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
				{
					int playerNum = 0;
					foreach (Player pl in Server.GetPlayers())
					{
						if(Server.GetPlayers().Count > 1)
							if (caller != null && pl.PlayerId == caller.PlayerId || (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) && AdminToolbox.ATPlayerDict[pl.SteamId].godMode) || pl.GetGodmode() /*|| (caller.GetUserGroup().Permissions < pl.GetUserGroup().Permissions)*/) continue;
						pl.Kill(killType);
						playerNum++;
					}
					if (caller != null && !string.IsNullOrEmpty(caller.Name) && caller.Name.ToLower() != "server") plugin.Info(caller.Name + " ran the \"SLAY\" command on: " + playerNum + " players");
					return new string[] { playerNum + " players has been slain!" };
				}
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; }
				if (myPlayer.TeamRole.Role != Role.SPECTATOR)
				{
					if (caller != null && !string.IsNullOrEmpty(caller.Name) && caller.Name.ToLower() != "server") plugin.Info(caller.Name + " ran the \"SLAY\" command on: " + myPlayer.Name);
					myPlayer.Kill(killType);
					return new string[] { myPlayer.Name + " has been slain!" };
				}
				else
					return new string[] { myPlayer.Name + " is already dead!" };
			}
			else
			{
				return new string[] { GetUsage() };
			}
		}
	}
}