using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;
using SMDoor = Smod2.API.Door;

namespace AdminToolbox.Command
{
	public class ClosestDoorCommand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "This is a description";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";
		public static readonly string[] CommandAliases = new string[] { "CLOSESTDOOR", "CLSDOOR", "ATDOOR", "ATD" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length > 0)
			{
				Player targetPlayer = args.Length > 1 ? Server.GetPlayers(args[0]).FirstOrDefault() : sender as Player;

				if (targetPlayer == null)
					return new string[] { "Could not find player" };

				Managers.ATFile.AddMissingPlayerVariables(targetPlayer);

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

				switch ((args.Length > 1 ? args[1] : args[0]).ToUpper())
				{
					case "BREAK":
					case "DESTROY":
					case "DESTR":
					case "BRK":
					case "BR":
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
					case "O":
						closestDoor.Open = true;
						return new string[] { "Closest door opened." };
					case "CLOSE":
					case "CL":
					case "C":
						closestDoor.Open = false;
						return new string[] { "Closest door closed." };
					default:
						return new string[] { "Arguements: \"" + string.Join(" ", args) + "\" is not recognized" };
				}
			}
			else
				return new string[] { GetUsage() };
		}
	}
}
