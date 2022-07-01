using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API.Extentions;

	public class IntercomLockCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		public IntercomLockCommand(AdminToolbox plugin) => this.plugin = plugin;
		public string GetCommandDescription() => "Enables/Disables the intercom for non- white-listed players";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") <bool>";

		public static readonly string[] CommandAliases = new string[] { "INTERCOMLOCK", "INTERLOCK", "ILOCK", "IL" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (args.Length >= 1)
				{
					if (bool.TryParse(args[0], out bool x))
					{
						AdminToolbox.isColored = x;
						if (!AdminToolbox.intercomLockChanged) AdminToolbox.intercomLockChanged = true;
						plugin.Info("IntercomLock set to: " + AdminToolbox.intercomLock);
						return new string[] { "IntercomLock set to: " + AdminToolbox.intercomLock };
					}
					else
						return new string[] { "\"ATCOLOR " + args[0] + "\"  is not a valid bool" };
				}
				else if (args.Length == 0)
				{
					AdminToolbox.intercomLock = !AdminToolbox.intercomLock;
					if (!AdminToolbox.intercomLockChanged) AdminToolbox.intercomLockChanged = true;
					plugin.Info("IntercomLock set to: " + AdminToolbox.intercomLock);
					return new string[] { "IntercomLock set to: " + AdminToolbox.intercomLock };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
