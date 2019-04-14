using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class ServerCommand : ICommandHandler
	{
		private static IConfigFile Config => ConfigManager.Manager.Config;

		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() =>"Gets toolbox info about the server";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";

		public static readonly string[] CommandAliases = new string[] { "SERVERINFO", "S", "SERVER", "SINFO" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				int minutes = (int)(Server.Round.Duration / 60), duration = Server.Round.Duration;
				string timeString = (duration < 60) ? duration + " seconds" : minutes + " minutes, " + (duration - (minutes * 60)) + " seconds";
				string pJail = "No jailed players!";

				Player[] players = Server.GetPlayers().ToArray();
				Player[] jailedPlayers = Server.GetPlayers().JailedPlayers();
				
				if (jailedPlayers != null && jailedPlayers.Length > 0)
				{
					pJail = string.Empty;
					foreach (Player pl in jailedPlayers)
						pJail += pl.Name + ", ";
				}

				string x = "Server info: \n " +
					"\n Server Name: " + Server.Name +
					"\n - Server IP: " + Server.IpAddress + ":" + Server.Port +
					"\n - PlayerCount: " + Server.NumPlayers +
					"\n - AdminToolbox Toggables: " +
					"\n     - isColored: " + AdminToolbox.isColored +
					"\n     - IntercomLock: " + AdminToolbox.intercomLock +
					"\n     - LockRound: " + AdminToolbox.lockRound +
					"\n     - Jailed Players: " + pJail +
					"\n - Stats:" +
					"\n     - Round Number: " + AdminToolbox.RoundCount +
					"\n     - Round Duration: " + timeString;

				if (sender.IsPlayer())
					return new string[] { x.Replace("True ", "<color=green>" + "True" + " </color>").Replace("False", "<color=red>" + "False" + "</color>") };
				else
					return new string[] { x };
			}
			else
				return deniedReply;
		}
	}
}
