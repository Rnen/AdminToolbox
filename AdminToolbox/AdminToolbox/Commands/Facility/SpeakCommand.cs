using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	class SpeakCommand : ICommandHandler
	{
		Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "Sets specified player as intercom speaker";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYERNAME/ID/STEAMID]";

		public static readonly string[] CommandAliases = new string[] { "ATSPEAK", "ATINTERCOM", "AT-SPEAK" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if(sender.IsPermitted(CommandAliases, out string[] deniedReply))
				if (Server.GetPlayers().Count > 0)
				{
					if (Server.Map.GetIntercomSpeaker() != null)
					{
						Server.Map.SetIntercomSpeaker(null);
						return new string[] { "Stopped Broadcast" };
					}
					Player myPlayer = (args.Length > 0) ? API.GetPlayerFromString.GetPlayer(args[0]) : null;
					if (myPlayer == null && sender is Player sendingPlayer)
						myPlayer = sendingPlayer;
					if (myPlayer == null)
						if (args.Length > 0)
							return new string[] { "Couldn't get player: " + args[0] };
						else
							return new string[] { GetUsage() };
					if (myPlayer != null)
					{
						Server.Map.SetIntercomSpeaker(myPlayer);
						return new string[] { "Intercom speaker set to " + myPlayer.Name };
					}
					else
						return new string[] { GetUsage() };
				}
				else
					return new string[] { "Server is empty!" };
			else
				return deniedReply;
		}
	}
}
