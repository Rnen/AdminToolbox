using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class JailCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Jails player for a <optional> specified time";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] <time>";

		public static readonly string[] CommandAliases = new string[] { "JAIL", "J" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if(sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				Server server = PluginManager.Manager.Server;
				if (args.Length > 0)
				{
					Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
					AdminToolbox.AddMissingPlayerVariables(myPlayer);
					if (args.Length > 1)
					{
						if (int.TryParse(args[1], out int x))
						{
							if (x > 0)
							{
								JailHandler.SendToJail(myPlayer, DateTime.Now.AddSeconds(x));
								return new string[] { "\"" + myPlayer.Name + "\" sent to jail for: " + x + " seconds." };
							}
							else
							{
								JailHandler.SendToJail(myPlayer);
								return new string[] { "\"" + myPlayer.Name + "\" sent to jail for 1 year" };
							}
						}
						else
							return new string[] { args[1] + " is not a valid number!" };
					}
					else if (args.Length == 1)
					{
						if (!AdminToolbox.ATPlayerDict.ContainsKey(myPlayer.SteamId)) return new string[] { "Failed to jail/unjail " + myPlayer.Name + "!", "Error: Player not in dictionary" };
						if (AdminToolbox.ATPlayerDict[myPlayer.SteamId].isJailed)
						{
							JailHandler.ReturnFromJail(myPlayer);
							return new string[] { "\"" + myPlayer.Name + "\" returned from jail" };
						}
						else
						{
							JailHandler.SendToJail(myPlayer);
							return new string[] { "\"" + myPlayer.Name + "\" sent to jail for 1 year" };
						}
					}
					else
						return new string[] { GetUsage() };

				}
				else
					return new string[] { GetUsage() };
			}
			else
			{
				return deniedReply;
			}
		}
	}
}
