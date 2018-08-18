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
			return "";
		}

		public string GetUsage()
		{
			return "WARP [PlayerName] [WarpPointName]" + "\n" + "WARP LIST" + "\n" + "WARP [ADD/+] [PlayerName] [YourWarpPointName]" + "\n" + "WARP [REMOVE/-] [YourWarpPointName]";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			AdminToolbox.AddMissingPlayerVariables();
			Server server = PluginManager.Manager.Server;

			if (AdminToolbox.warpVectors.Count < 1) { return new string[] { "No warp points created yet!" }; }
			string str = "\n" + "Warp Points:";
			List<string> list = AdminToolbox.warpVectors.Keys.ToList();
			list.Sort();
			foreach (string i in list)
				str += "\n - " + i;
			return new string[] { str };
		}
	}
}