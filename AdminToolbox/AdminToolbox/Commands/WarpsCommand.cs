using Smod2.Commands;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using System.Linq;
using ServerMod2.API;
using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;

namespace AdminToolbox.Command
{
	class WarpsCommmand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public WarpsCommmand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "";
		}

		public string GetUsage()
		{
			return "WARP [PlayerName] [WarpPointName]\nWARP LIST\nWARP [ADD/+] [PlayerName] [YourWarpPointName]\nWARP [REMOVE/-] [YourWarpPointName]";
		}

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            AdminToolbox.AddMissingPlayerVariables();
            Server server = PluginManager.Manager.Server;

            if (AdminToolbox.warpVectors.Count < 1) { return new string[] { "No warp points created yet!" }; }
            string str = "\nWarp Points:";
            var list = AdminToolbox.warpVectors.Keys.ToList();
            list.Sort();
            foreach (var i in list)
                str += "\n - " + i;
            return new string[] { str };
        }
	}
}
