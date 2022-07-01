using System;
using Smod2.API;
using Smod2.Commands;

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
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length > 0)
				{
					Player myPlayer = GetFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
					Managers.ATFile.AddMissingPlayerVariables(myPlayer);
					if (args.Length > 1)
					{
						if (int.TryParse(args[1], out int x))
						{
							if (x > 0)
							{
								JailHandler.SendToJail(myPlayer, DateTime.UtcNow.AddSeconds(x));
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
						if (AdminToolbox.ATPlayerDict.TryGetValue(myPlayer.UserID, out PlayerSettings playersetting))
							if (playersetting.isJailed)
							{
								JailHandler.ReturnFromJail(myPlayer);
								return new string[] { "\"" + myPlayer.Name + "\" returned from jail" };
							}
							else
							{
								JailHandler.SendToJail(myPlayer);
								return new string[] { "\"" + myPlayer.Name + "\" sent to jail for 1 year" };
							}
						else 
							return new string[] { "Failed to jail/un-jail " + myPlayer.Name + "!", "Error: Player not in dictionary" };
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
