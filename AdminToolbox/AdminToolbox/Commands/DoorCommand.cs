using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class DoorCommand : ICommandHandler
	{
		private AdminToolbox plugin;

		public DoorCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "";
		}

		public string GetUsage()
		{
			return "";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			AdminToolbox.AddMissingPlayerVariables();
			Server server = PluginManager.Manager.Server;
			if (args.Length > 0)
			{
				Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
				if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
				if (args.Length > 1)
				{
					if (args[1].ToLower() == "open")
					{
						foreach (var item in server.Map.GetDoors())
							item.Open = true;
						return new string[] { "Door Opened" };
					}
					else if (args[1].ToLower() == "close")
					{
						foreach (var item in server.Map.GetDoors())
							item.Open = false;
						return new string[] { "Door Closed" };
					}
					else if (args[1].ToLower() == "lock")
					{
						return new string[] { "Door Locked" };
					}
					else if (args[1].ToLower() == "unlock")
					{
						return new string[] { "Door UnLocked" };
					}
					else
						return new string[] { GetUsage() };
				}
				else
					return new string[] { GetUsage() };
			}
			return new string[] { GetUsage() };
		}
	}
}