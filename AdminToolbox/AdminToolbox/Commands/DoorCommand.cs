using Smod2.Commands;
using Smod2;
using Smod2.API;

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
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) {  return new string[] { "Couldn't get player: " + args[0] };; }
            }
            return new string[] { GetUsage() };
        }
	}
}
