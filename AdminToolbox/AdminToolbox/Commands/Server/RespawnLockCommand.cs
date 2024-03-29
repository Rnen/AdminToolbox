using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API.Extentions;
	public class RespawnLockCommand : ICommandHandler
	{
		public string GetCommandDescription() => "Keeps players from spawning";
		public string GetUsage() => $"({string.Join(" / ", CommandAliases)})";
		public static readonly string[] CommandAliases = new string[] { "RESPAWNLOCK", "RSL", "RSPL" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] denied))
				if (args.Length > 0)
				{
					if (bool.TryParse(args[0], out bool b))
					{
						AdminToolbox.respawnLock = b;
						return new string[] { "RespawnLock set to: " + b };
					}
					else
						return new string[] { args[0] + " is not a valid bool!" };
				}
				else
				{
					AdminToolbox.respawnLock = !AdminToolbox.respawnLock;
					return new string[] { "RespawnLock toggled to: " + AdminToolbox.respawnLock };
				}
			else
				return denied;
		}
	}
}
