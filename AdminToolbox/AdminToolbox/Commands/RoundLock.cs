using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.IO;

namespace AdminToolbox.Command
{
	class RoundLock : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public RoundLock(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Locks the round so it cant end";
		}

		public string GetUsage()
		{
			return "ROUNDLOCK TRUE/FALSE";
		}

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                bool k;
                int l;
                if (bool.TryParse(args[0], out k))
                {
                    AdminToolbox.lockRound = k;
                    plugin.Info("Round lock: " + k);
                    return new string[] { "Round lock: " + k };
                }
                else if (Int32.TryParse(args[0], out l))
                {
                    if (l < 1)
                    {
                        AdminToolbox.lockRound = false;
                        plugin.Info("Round lock: " + AdminToolbox.lockRound);
                        return new string[] { "Round lock: " + AdminToolbox.lockRound };
                    }
                    else
                    {
                        AdminToolbox.lockRound = true;
                        plugin.Info("Round lock: " + AdminToolbox.lockRound);
                        return new string[] { "Round lock: "+AdminToolbox.lockRound };
                    }
                }
                else
                    return new string[] { GetUsage() };
            }
            else
            {
                AdminToolbox.lockRound = !AdminToolbox.lockRound;
                plugin.Info("Round lock: " + AdminToolbox.lockRound);
                return new string[] { "Round lock: "+AdminToolbox.lockRound };
            }
        }
	}
}
