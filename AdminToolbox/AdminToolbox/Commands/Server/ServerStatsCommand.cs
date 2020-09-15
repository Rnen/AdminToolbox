using System;
using System.Linq;
using System.Reflection;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API.Extentions;

	public class ServerStatsCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Gets the server's round stats since last server restart";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";

		public static readonly string[] CommandAliases = new string[] { "SERVERSTATS", "SSTATS", "RSTATS", "ROUNDSTATS" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				return new string[] { AdminToolbox.RoundStats.ToString() };
			}
			else
				return deniedReply;
		}
	}
}
