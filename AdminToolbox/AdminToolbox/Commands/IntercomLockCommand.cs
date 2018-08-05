using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
    class IntercomLockCommand : ICommandHandler
    {
        private AdminToolbox plugin;

        public IntercomLockCommand(AdminToolbox plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "Enables/Disables the intercom for non-whitelisted players";
        }

        public string GetUsage()
        {
            return "(IL / ILOCK / INTERCOMLOCK) (bool)";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length >= 1)
            {
                if (bool.TryParse(args[0], out bool x))
                {
                    AdminToolbox.isColored = x;
                    if(!AdminToolbox.intercomLockChanged) AdminToolbox.intercomLockChanged = true;
                    return new string[] { "IntercomLock set to: " + AdminToolbox.intercomLock };
                }
                else
                    return new string[] { "\"ATCOLOR "+ args[0] +"\"  is not a valid bool" };
            }
            else if (args.Length == 0)
            {
                AdminToolbox.intercomLock = !AdminToolbox.intercomLock;
                if (!AdminToolbox.intercomLockChanged) AdminToolbox.intercomLockChanged = true;
                return new string[] { "IntercomLock set to: " + AdminToolbox.intercomLock };
            }
            else
                return new string[] { GetUsage() };
        }
	}
}
