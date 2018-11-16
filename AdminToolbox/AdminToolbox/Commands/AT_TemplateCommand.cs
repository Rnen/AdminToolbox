using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;

namespace AdminToolbox.Command
{
	class AT_TemplateCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		static IConfigFile Config => ConfigManager.Manager.Config;
		Server Server => PluginManager.Manager.Server;

		public AT_TemplateCommand(AdminToolbox plugin)
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
			Player caller = (sender is Player _p) ? _p : null;

			if (args.Length > 0)
			{
				//Get player from first arguement of OnCall
				Player targetPlayer = GetPlayerFromString.GetPlayer(args[0]);
				//If player could not be found, return
				if (targetPlayer == null) { return new string[] { "Could not find player: " + args[0] }; ; }

				//Adds player(s) to the AdminToolbox settings
				AdminToolbox.AddMissingPlayerVariables(new List<Player> { targetPlayer, caller });
				//Do whatever with the found player
				return new string[] { "We did something to player: " + targetPlayer.Name + "!" };
			}
			else if (caller != null)
			{
				//Do something on calling player without any arguements
				return new string[] { "We did something to player: " + caller.Name + "!" };
			}
			else
				return new string[] { GetUsage() };
		}
	}
}