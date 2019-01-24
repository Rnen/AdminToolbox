using System;
using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdminToolbox.API;

namespace AdminToolbox.Command
{
	class TimedCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		static IConfigFile Config => ConfigManager.Manager.Config;
		Server Server => PluginManager.Manager.Server;

		public TimedCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "";

		public string GetUsage() => "";

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			Player caller = (sender is Player _p) ? _p : null;

			DateTime restartTime = DateTime.Now;

			if (args.Length > 1)
			{
				switch (args[0].ToUpper())
				{
					case "REMOVE":
					case "RESET":
					case "STOP":
						if(plugin.scheduledCommands.Select(s => s.command).Contains(args[1].ToUpper()))
						{
							plugin.scheduledCommands.RemoveAll(s => s.command == args[1].ToUpper());
							return new string[] { "Stopped all timed" + args[1] + " commands!" };
						}
						else
							return new string[] { "No command of the " + args[1] + " type was timed!" };
					default:
						break;
				}
				string valueType = string.Empty;
				int value = 0;

				foreach (string arg in args)
				{
					int.TryParse(Regex.Replace(arg, @"[A-Za-z]+", string.Empty), out value);
					value = Math.Abs(value);
					valueType = Regex.Replace(arg, @"\d", string.Empty).ToUpper().Trim();

					plugin.AtInfo("ValueType: " + valueType + " Value: " + value);

					switch (valueType)
					{
						case "MS":
						case "MILLIS":
						case "MILLISEC":
						case "MILLISECOND":
						case "MILLISECONDS":
							plugin.AtInfo("Added: MS" + value);
							restartTime = restartTime.AddMilliseconds(value);
							break;
						case "S":
						case "SECOND":
						case "SECONDS":
							plugin.AtInfo("Added: S" + value);
							restartTime = restartTime.AddSeconds(value);
							break;
						case "M":
						case "MINUTE":
						case "MINUTES":
							plugin.AtInfo("Added: M" + value);
							restartTime = restartTime.AddMinutes(value);
							break;
						case "H":
						case "HOUR":
						case "HOURS":
							plugin.AtInfo("Added: H" + value);
							restartTime = restartTime.AddHours(value);
							break;
						case "D":
						case "DAY":
						case "DAYS":
							plugin.AtInfo("Added: D" + value);
							restartTime = restartTime.AddDays(value);
							break;
						case "Y":
						case "YEAR":
						case "YEARS":
							plugin.AtInfo("Added: Y" + value);
							restartTime = restartTime.AddYears(value);
							break;
						default:
							plugin.AtInfo("Default case for: " + valueType);
							break;
					}
				}
				try
				{
					if (plugin.scheduledRestart != null)
					{
						plugin.scheduledRestart.restartTime = restartTime;
						plugin.scheduledRestart.enabled = true;
					}
				}
				catch { plugin.Info("Failed doing scheduling a restart!"); }
				return new string[] { "Datetime now: " + DateTime.Now, "Restart time: " + restartTime, "Difference: " + Math.Round((restartTime - DateTime.Now).TotalMinutes) + "min" };
			}
			else
				return new string[] {  };
		}
	}
}