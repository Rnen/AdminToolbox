using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
    class SetPlayerClass : ICommandHandler
    {
        private AdminToolbox plugin;

        public SetPlayerClass(AdminToolbox plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "Sets player to (classID)";
        }

        public string GetUsage()
        {
            return "CLASS [PLAYER] [CLASSID]";
        }

        public void OnCall(ICommandManager manager, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { plugin.Info("Couldn't find player: " + args[0]); return; }
                if (myPlayer.Class.ClassType == Classes.UNASSIGNED || myPlayer.Class.ClassType == Classes.SPECTATOR) { plugin.Warn("Dont use CLASS command on spectators!"); return; };
                if (args.Length > 1)
                {
                    int j;
                    if (Int32.TryParse(args[1], out j))
                    {
                        Vector originalPos = myPlayer.GetPosition();
                        plugin.Info("Changed " + myPlayer.Name + " from " + myPlayer.Class + " to " + (Classes)j);
                        myPlayer.ChangeClass((Classes)j, false, true);
                        myPlayer.Teleport(originalPos);
                        myPlayer.SetHealth(myPlayer.Class.MaxHP);
                    }
                    else
                        plugin.Info("Not a valid number!");
                    return;
                }
                else
                {
                    plugin.Info(GetUsage());
                    return;
                }
            }
            else
                plugin.Info(GetUsage());
        }
    }
}
