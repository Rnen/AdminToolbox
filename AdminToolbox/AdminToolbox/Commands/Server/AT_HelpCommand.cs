using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class AT_HelpCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Opens the AdminToolbox GitHub page";

		public string GetUsage() => "ATHELP";

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender is Player p && p != null)
				return new string[] { "This command is only for use in server window!" };
			else
			{
				try
				{
					System.Diagnostics.Process.Start("https://github.com/Rnen/AdminToolbox");
					return new string[] { "Opening browser..." };
				}
				catch { return new string[] { "Could not open browser!" }; }
			}
		}
	}
}