using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
	class LockdownCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public LockdownCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "";
		}

		public string GetUsage()
		{
			return "LOCKDOWN [BOOLEAN]";
		}

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                bool j;
                if (bool.TryParse(args[0], out j))
                {
                    AdminToolbox.lockDown = j;
                }
                else
                    return new string[] { GetUsage() };
            }
            else
                AdminToolbox.lockDown = !AdminToolbox.lockDown;
            plugin.Info("Lockdown: " + AdminToolbox.lockDown);
            return new string[] { "Lockdown: "+AdminToolbox.lockDown };
        }
	}
}
