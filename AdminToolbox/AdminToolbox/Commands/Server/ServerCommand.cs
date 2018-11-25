using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	class ServerCommand : ICommandHandler
	{
		static IConfigFile Config => ConfigManager.Manager.Config;
		Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription()
		{
			return "Gets toolbox info about the server";
		}

		public string GetUsage()
		{
			return "SERVER / S / SERVERINFO";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			int minutes = (int)(Server.Round.Duration / 60), duration = Server.Round.Duration;
			string timeString = string.Empty;
			if (duration < 60)
				timeString = duration + " seconds";
			else
				timeString = minutes + " minutes, " + (duration - (minutes * 60)) + " seconds";
			bool isPlayer()
			{
				if (sender is Player pl)
					if (!string.IsNullOrEmpty(pl.SteamId))
						return true;
					else
						return false;
				else
					return false;
			}
			string ColoredBools(bool input)
			{
				if (isPlayer() && input)
					return "<color=green>" + input + "</color>";
				else if (isPlayer() && !input)
					return "<color=red>" + input + "</color>";
				else
					return input.ToString();
			}
			int pCount = Server.GetPlayers().Count;
			string pJail = string.Empty;
			foreach (Player pl in API.JailHandler.GetJailedPlayers())
				pJail += pl.Name + ", ";
			if (string.IsNullOrEmpty(pJail))
				pJail = "No jailed players!";

			string x = "Server info: \n " +
				"\n Server Name: " + Server.Name +
				"\n - Server IP: " + Server.IpAddress + ":" + Server.Port +
				"\n - PlayerCount: " + pCount +
				"\n - AdminToolbox Toggables: " +
				"\n     - isColored: " + ColoredBools(AdminToolbox.isColored) +
				"\n     - IntercomLock: " + ColoredBools(AdminToolbox.intercomLock) +
				"\n     - LockRound: " + ColoredBools(AdminToolbox.lockRound) +
				"\n     - Jailed Players: " + pJail +
				"\n - Stats:" +
				"\n     - Round Number: " + AdminToolbox.RoundCount +
				"\n     - Round Duration: " + timeString;
			return new string[] { x };
		}
	}
}