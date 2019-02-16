using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	using System;

	class ATCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;
		private static ICommandManager CommandManager => PluginManager.Manager.CommandManager;

		public ATCommand(AdminToolbox plugin) => this.plugin = plugin;
		public string GetCommandDescription() => "For plugin related stuff";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") (HELP / INFO / DOWNLOAD)";

		public static readonly string[] CommandAliases = new string[] { "AT", "ADMINTOOLBOX", "ATB" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				Player caller = (sender is Player _p) ? _p : null;
				if (args.Length > 0 && (args[0].ToUpper() == "INFO" || args[0].ToUpper() == "I"))
					goto SkipPlayerCheck;
				if (caller != null)
					return new string[] { "This command is for only for use in the server window!" };
				SkipPlayerCheck:;
				if (args.Length > 0)
					switch (args[0].ToUpper())
					{
						case "HELP":
						case "H":
							return CommandManager.CallCommand(sender, "athelp", new string[] { });
						case "INFO":
						case "I":
							return new string[] { "[AdminToolbox Info]", "Your Local Version: " + plugin.Details.version, "Latest GitHub Version: " + plugin.GetGitReleaseInfo().Version };
						case "DOWNLOAD":
						case "DL":
							try
							{
								if (caller == null || caller.IpAddress == PluginManager.Manager.Server.IpAddress)
									System.Diagnostics.Process.Start(plugin.GetGitReleaseInfo().DownloadLink);
								else
									throw new Exception("Tried opening browser as not-host");
							}
							catch
							{
								return new string[] { "Failed to open browser! Please visit GitHub or use \"AT_AutoUpdate.bat\" instead" };
							}
							return new string[] { "Opening browser..." };
						default:
							return new string[] { args[0] + " is not a valid arguement!", GetUsage() };
					}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}