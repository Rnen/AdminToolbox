using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
	class HealCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public HealCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Heals player. Use int for spesific amount (optional)";
		}

		public string GetUsage()
		{
			return "HEAL [PLAYER] (AMOUNT)";
		}

        public void OnCall(ICommandManager manager, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { plugin.Info("Couldn't find player: " + args[0]); return; }
                if (args.Length > 1)
                {
                    int j;
                    if (Int32.TryParse(args[1], out j))
                    {
                        plugin.Info("Added " + j + " HP " + " to " + myPlayer.Name);
                        myPlayer.AddHealth(j);
                    }
                    else
                        plugin.Info("Not a valid number!");
                }
                else
                {
                    plugin.Info("Set " + myPlayer.Name + " to full HP");
                    myPlayer.SetHealth(myPlayer.Class.MaxHP);
                }
            }
            else
            {
                plugin.Info(GetUsage());
            }
        }
	}
}
