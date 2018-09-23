using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Command
{
	class SpeakCommand : ICommandHandler
	{
		Server server = PluginManager.Manager.Server;

		public string GetCommandDescription()
		{
			return "Sets specified player as intercom speaker";
		}

		public string GetUsage()
		{
			return "SPEAK [PLAYERNAME/ID/STEAMID]";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (server.GetPlayers().Count > 0)
			{
				if (server.Map.GetIntercomSpeaker() != null)
				{
					server.Map.SetIntercomSpeaker(null);
					return new string[] { "Stopped Broadcast" };
				}
				Player myPlayer = (args.Length > 0) ? GetPlayerFromString.GetPlayer(args[0]) : null;
				if (myPlayer == null && sender is Player sendingPlayer)
					myPlayer = sendingPlayer;
				else if (myPlayer == null)
					if (args.Length > 0)
						return new string[] { "Couldn't get player: " + args[0] };
					else
						return new string[] { GetUsage() };
				if (myPlayer != null)
				{
					server.Map.SetIntercomSpeaker(myPlayer);
					return new string[] { "Intercom speaker set to " + myPlayer.Name };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return new string[] { "Server is empty!" };
		}
	}
}
