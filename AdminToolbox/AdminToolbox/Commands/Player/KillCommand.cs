using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Linq;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class KillCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;
		Server Server => PluginManager.Manager.Server;
		IConfigFile Config => ConfigManager.Manager.Config;

		public KillCommand(AdminToolbox plugin) => this.plugin = plugin;
		public string GetCommandDescription() => "Kills the targeted player";
		public string GetUsage() => "(KILL / SLAY) [PLAYER]";

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (Server.GetPlayers().Count < 1)
				return new string[] { "The server is empty!" };

			Player caller = (sender is Player send) ? send : null;
			DamageType killType = (DamageType)Config.GetIntValue("admintoolbox_slaycommand_killtype", 0, true);

			if (args.Length > 0)
			{
				if (args[0].ToLower() == "all" || args[0].StartsWith("*"))
				{
					int playerNum = 0;
					foreach(Player p in Server.GetPlayers().Where(pl => pl.PlayerId != (caller != null ? caller.PlayerId : -1) 
					&& !pl.GetGodmode() &&
					(AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) ? !AdminToolbox.ATPlayerDict[pl.SteamId].godMode : true) 
					&& pl.TeamRole.Team != Smod2.API.Team.SPECTATOR).ToList())
					{
						p.Kill(killType);
						playerNum++;
					}
					if (caller != null && !string.IsNullOrEmpty(caller.Name) && caller.Name.ToLower() != "server") plugin.Info(caller.Name + " ran the \"SLAY\" command on: " + playerNum + " players");
					return new string[] { playerNum + " players has been slain!" };
				}
				Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
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