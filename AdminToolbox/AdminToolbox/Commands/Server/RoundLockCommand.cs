using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.IO;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class RoundLockCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		private static IConfigFile Config => ConfigManager.Manager.Config;

		private Server Server => PluginManager.Manager.Server;

		public RoundLockCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "Forces the round to never end";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") <bool enabled>";

		public static readonly string[] CommandAliases = new string[] { "ROUNDLOCK", "LOCKROUND", "RLOCK", "LOCKR" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				Player caller = (sender is Player _p) ? _p : null;
				if (args.Length > 0)
				{
					if (bool.TryParse(args[0], out bool k))
					{
						AdminToolbox.lockRound = k;
						if (caller != null) plugin.Info("Round lock: " + k);
						return new string[] { "Round lock: " + k };
					}
					else if (int.TryParse(args[0], out int l))
					{
						if (l < 1)
						{
							AdminToolbox.lockRound = false;
							if (caller != null) plugin.Info("Round lock: " + AdminToolbox.lockRound);
							return new string[] { "Round lock: " + AdminToolbox.lockRound };
						}
						else
						{
							AdminToolbox.lockRound = true;
							if (caller != null) plugin.Info("Round lock: " + AdminToolbox.lockRound);
							return new string[] { "Round lock: " + AdminToolbox.lockRound };
						}
					}
					else
						return new string[] { GetUsage() };
				}
				else
				{
					AdminToolbox.lockRound = !AdminToolbox.lockRound;
					if (caller != null) plugin.Info("Round lock: " + AdminToolbox.lockRound);
					return new string[] { "Round lock: " + AdminToolbox.lockRound };
				}
			}
			else
				return deniedReply;
		}
	}
}
