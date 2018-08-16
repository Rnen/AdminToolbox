using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	class ServerCommand : ICommandHandler
	{
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
            Server server = PluginManager.Manager.Server;
            int minutes = (int)(server.Round.Duration / 60), duration = server.Round.Duration;
            string timeString = string.Empty;
            if (duration < 60)
                timeString =  duration + " seconds";
            else
                timeString = minutes + " minutes, " + (duration - (minutes * 60)) + " seconds";
            bool isPlayer()
			{
				if (sender.GetType() == typeof(Player))
					return true;
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
            int pCount = server.GetPlayers().Count;
            string pJail = string.Empty;
            foreach (Player pl in AdminToolbox.GetJailedPlayers())
                pJail += pl.Name + ", ";

                string x = "Server info: \n " +
                    "\n Server Name: " + server.Name +
                    "\n - Server IP: " + server.IpAddress + ":" + server.Port +
                    "\n - PlayerCount: " + pCount +
                    "\n - AdminToolbox Toggables: " +
                    "\n     - isColored: " + ColoredBools(AdminToolbox.isColored) +
                    "\n     - IntercomLock: " + ColoredBools(AdminToolbox.intercomLock) +
                    "\n     - LockRound: " + ColoredBools(AdminToolbox.lockRound) +
                    "\n     - Jailed Players: " + pJail +
                    "\n - Stats:" +
                    "\n     - Round Number: " + AdminToolbox.roundCount +
                    "\n     - Round Duration: " + timeString;
            return new string[] { x };
        }
	}
}
