using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using SMDoor = Smod2.API.Door;

namespace AdminToolbox.Command
{
	public class ClosestDoorCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		private static IConfigFile Config => ConfigManager.Manager.Config;

		private Server Server => PluginManager.Manager.Server;

		public ClosestDoorCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "This is a description";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";
		public static readonly string[] CommandAliases = new string[] { "CLOSESTDOOR", "CLSDOOR", "ATDOOR", "ATD" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			// Gets the caller as a "Player" object

			if (args.Length > 0)
			{
				//Get player from first arguement of OnCall
				Player targetPlayer = args.Length > 1 ? Server.GetPlayers(args[0]).FirstOrDefault() : sender as Player;

				//If player could not be found, return reply to command user
				if (targetPlayer == null)
					return new string[] { "Could not find player" };

				//Adds player(s) to the AdminToolbox player dictionary
				AdminToolbox.AddMissingPlayerVariables(targetPlayer);

				SMDoor closestDoor = null;
				float dist = float.MaxValue;
				float newDist = float.MaxValue;

				foreach (SMDoor d in Server.Map.GetDoors())
				{
					newDist = Vector.Distance(d.Position, targetPlayer.GetPosition());
					if (newDist < dist)
					{
						closestDoor = d;
						dist = newDist;
					}
				}

				string arg = args.Length > 1 ? args[1].ToUpper() : args[0].ToUpper();

				switch (arg)
				{
					case "BREAK":
					case "DESTROY":
					case "DESTR":
					case "BRK":
						closestDoor.Destroyed = true;
						return new string[] { "Closest door broken." };
					case "LOCK":
					case "L":
						closestDoor.Locked = true;
						return new string[] { "Closest door locked." };
					case "UNLOCK":
					case "UL":
					case "!L":
						closestDoor.Locked = false;
						return new string[] { "Closest door unlocked." };
					case "OPEN":
					case "OP":
						closestDoor.Open = true;
						return new string[] { "Closest door opened." };
					case "CLOSE":
					case "CL":
						closestDoor.Open = false;
						return new string[] { "Closest door closed." };
					default:
						return new string[] { "Word: " + arg + " is not recognized" };
				}
			}
			else
				return new string[] { GetUsage() };
		}
	}
}
