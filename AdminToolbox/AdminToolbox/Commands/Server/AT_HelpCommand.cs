using Smod2.Commands;

namespace AdminToolbox.Command
{
	class AT_HelpCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Opens the AdminToolbox GitHub page";

		public string GetUsage() => "ATHELP";

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			try
			{
				System.Diagnostics.Process.Start("https://github.com/Rnen/AdminToolbox");
				return new string[] { "Opening GitHub page..." };
			}
			catch { return new string[] { "Could not open browser!", "Visit: " + "https://github.com/Rnen/AdminToolbox" }; }
		}
	}
}