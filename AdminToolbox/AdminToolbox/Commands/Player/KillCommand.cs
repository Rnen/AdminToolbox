using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class KillCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		private Server Server => PluginManager.Manager.Server;

		private IConfigFile Config => ConfigManager.Manager.Config;

		public KillCommand(AdminToolbox plugin) => this.plugin = plugin;
		public string GetCommandDescription() => "Kills the targeted player";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER]";

		public static readonly string[] CommandAliases = new string[] { "SLAY", "KILL" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (Server.GetPlayers().Count < 1)
					return new string[] { "The server is empty!" };

				Player caller = sender as Player;
				DamageType killType = (DamageType)Config.GetIntValue("admintoolbox_slaycommand_killtype", 0, true);

				if (args.Length > 0)
				{
					if (Utility.AllAliasWords.Contains(args[0].ToUpper()))
					{
						int playerNum = 0;

						foreach (Player p in Server.GetPlayers().Where(pl => pl.PlayerId != (caller != null ? caller.PlayerId : -1)
						 && !pl.GetGodmode() &&
						 (AdminToolbox.ATPlayerDict.ContainsKey(pl.SteamId) ? !AdminToolbox.ATPlayerDict[pl.SteamId].godMode : true)
						 && pl.TeamRole.Team != Smod2.API.Team.SPECTATOR))
						{
							p.Kill(killType);
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
			else
				return deniedReply;
		}
	}
}
