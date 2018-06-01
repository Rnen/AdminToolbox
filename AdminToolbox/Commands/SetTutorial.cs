using Smod2.Commands;
using Smod2;
using Smod2.API;

namespace AdminToolbox.Command
{
    class SetTutorial : ICommandHandler
    {
        private AdminToolbox plugin;

        public SetTutorial(AdminToolbox plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "Sets player to TUTORIAL";
        }

        public string GetUsage()
        {
            return "TUT/TUTORIAL [PLAYER]";
        }

        public void OnCall(ICommandManager manager, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0],out myPlayer);
                if (myPlayer == null) { plugin.Info("Couldn't find player: " + args[0]); return; }
                Vector originalPos = myPlayer.GetPosition();
                plugin.Info("Set " + myPlayer.Name + " to TUTORIAL");
                myPlayer.ChangeClass(Classes.TUTORIAL);
                myPlayer.Teleport(originalPos);
            }
            else
                plugin.Info(GetUsage());
        }
    }
}
