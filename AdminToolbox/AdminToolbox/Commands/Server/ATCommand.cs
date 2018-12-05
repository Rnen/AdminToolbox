using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Net;
using System;
using System.Linq;
using System.IO;
using System.Threading;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace AdminToolbox.Command
{
	class ATCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;
		ICommandManager CommandManager = PluginManager.Manager.CommandManager;

		public ATCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "For plugin related stuff";

		public string GetUsage() => "AT (HELP / INFO / DOWNLOAD)";

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			Player caller = (sender is Player _p) ? _p : null;
			if (caller != null) return new string[] { "This command is for only for use in the server window!" };
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
							System.Diagnostics.Process.Start(plugin.GetGitReleaseInfo().DownloadLink);
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
	}
}