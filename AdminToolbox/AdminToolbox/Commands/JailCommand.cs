using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Command
{
    class JailCommand : ICommandHandler
    {
        public string GetCommandDescription()
        {
            return "Jails player for a (optional) specified time";
        }

        public string GetUsage()
        {
            return "JAIL [PLAYER] (time)";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            AdminToolbox.AddMissingPlayerVariables();
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
                if (args.Length == 2)
                {
                    if (Int32.TryParse(args[1], out int x))
                    {
                        if (x > 0)
                        {
                            AdminToolbox.playerdict[myPlayer.SteamId].JailedToTime = DateTime.Now.AddSeconds(x);
                            AdminToolbox.SendToJail(myPlayer);
                            return new string[] { "\"" + myPlayer.Name + "\" sendt to jail for: " + x + " seconds." };
                        }
                        else
                        {
                            AdminToolbox.playerdict[myPlayer.SteamId].JailedToTime = DateTime.Now.AddYears(1);
                            AdminToolbox.SendToJail(myPlayer);
                            return new string[] { "\"" + myPlayer.Name + "\" sendt to jail for 1 year" };
                        }
                    }
                    else
                        return new string[] { args[1] + " is not a valid number!" };
                }
                else if (args.Length == 1)
                {
                    if (AdminToolbox.playerdict[myPlayer.SteamId].isInJail || AdminToolbox.playerdict[myPlayer.SteamId].isJailed)
                    {
                        AdminToolbox.ReturnFromJail(myPlayer);
                        return new string[] { "\""+ myPlayer.Name + "\" returned from jail" };
                    }
                    else
                    {
                        AdminToolbox.SendToJail(myPlayer);
                        AdminToolbox.playerdict[myPlayer.SteamId].JailedToTime = DateTime.Now.AddYears(1);
                        return new string[] { "\"" + myPlayer.Name + "\" sendt to jail for 1 year" };
                    }
                }
                else
                    return new string[] { GetUsage() };

            }
            else
                return new string[] { GetUsage() };
        }
    }
}
