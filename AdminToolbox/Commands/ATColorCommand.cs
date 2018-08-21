using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
    class ATColorCommand : ICommandHandler
    {
        private AdminToolbox plugin;

        public ATColorCommand(AdminToolbox plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "Enables/Disables color for Admintoolbox in the server window";
        }

        public string GetUsage()
        {
            return "ATCOLOR (bool)";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length >= 1)
            {
                if (bool.TryParse(args[0], out bool x))
                {
                    AdminToolbox.isColored = x;
                    AdminToolbox.isColoredCommand = true;
                    if (AdminToolbox.isColored)
                        plugin.Info("@#fg=Yellow;AdminToolbox@#fg=Default; colors is set to @#fg=Green;" + AdminToolbox.isColored + "@#fg=Default;");
                    else
                        plugin.Info("AdminToolbox colors set to" + AdminToolbox.isColored);
                    return new string[] { "AdminToolbox colors set to" + AdminToolbox.isColored };
                }
                else
                    return new string[] { "\"ATCOLOR "+ args[0] +"\"  is not a valid bool" };
            }
            else if (args.Length == 0)
            {
                AdminToolbox.isColored = !AdminToolbox.isColored;
                AdminToolbox.isColoredCommand = true;
                if (AdminToolbox.isColored)
                    plugin.Info("@#fg=Yellow;AdminToolbox@#fg=Default; colors is set to @#fg=Green;"+AdminToolbox.isColored+"@#fg=Default;");
                else
                    plugin.Info("AdminToolbox colors set to "+AdminToolbox.isColored);
                return new string[] { "AdminToolbox colors set to " + AdminToolbox.isColored };
            }
            else
                return new string[] { GetUsage() };
        }
	}
}
