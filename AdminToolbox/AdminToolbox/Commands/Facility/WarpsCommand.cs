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
		public string GetCommandDescription()
		{
			return "Returns a list of warps";
		}

		public string GetUsage()
		{
			return "WARPS";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			return PluginManager.Manager.CommandManager.CallCommand(sender, "warp", new string[] { "list" });
		}
	}
}