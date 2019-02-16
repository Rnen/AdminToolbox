using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Command
{
	using API; //This is for accessing AT's API namespace
	using API.Extentions;
	class AT_TemplateCommand : ICommandHandler
	{
		private readonly AdminToolbox plugin;

		static IConfigFile Config => ConfigManager.Manager.Config;
		Server Server => PluginManager.Manager.Server;
		public AT_TemplateCommand(AdminToolbox plugin) => this.plugin = plugin;

		public string GetCommandDescription() => "This is a description";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ")";
		public static readonly string[] CommandAliases = new string[] { "TEMPLATE", "TEMPLATE2" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			// Gets the caller as a "Player" object
			Player caller = sender as Player;

			if (args.Length > 0)
			{
				//Get player from first arguement of OnCall
				Player targetPlayer = Server.GetPlayers(args[0]).FirstOrDefault();

				//If player could not be found, return reply to command user
				if (targetPlayer == null) { return new string[] { "Could not find player: " + args[0] }; ; }

				//Adds player(s) to the AdminToolbox player dictionary
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