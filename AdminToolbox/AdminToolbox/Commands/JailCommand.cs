using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;
using System.Text.RegularExpressions;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.Command
{
	class JailCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public JailCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Jails player for a specified time";
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
                Timer t;
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) {  return new string[] { "Couldn't get player: " + args[0] };; }
                if (args.Length == 2 && Int32.TryParse(args[1], out int x))
                {
                    if (x > 0)
                    {
                        if (AdminToolbox.jailTimer.ContainsKey(myPlayer.SteamId))
                        {
                            AdminToolbox.playerdict[myPlayer.SteamId].isJailed = true;
                            t = AdminToolbox.jailTimer[myPlayer.SteamId];
                            t.Stop();
                            t.Interval = x * 1000;
                            t.Start();
                        }
                        else
                        {
                            AdminToolbox.playerdict[myPlayer.SteamId].isJailed = true;
                            t = new Timer
                            {
                                Interval = x * 1000,
                                AutoReset = false,
                                Enabled = true
                            };
                            t.Elapsed += delegate
                            {
                                AdminToolbox.playerdict[myPlayer.SteamId].isJailed = false;
                                myPlayer.Teleport(AdminToolbox.playerdict[myPlayer.SteamId].originalPos);
                                AdminToolbox.playerdict[myPlayer.SteamId].isInJail = false;
                                t.Enabled = false;
                            };
                            AdminToolbox.jailTimer.Add(myPlayer.SteamId, t);
                        }
                    }
                    else
                    {
                        AdminToolbox.playerdict[myPlayer.SteamId].isJailed = true;
                    }
                    if (AdminToolbox.playerdict[myPlayer.SteamId].isJailed)
                    {
                        AdminToolbox.playerdict[myPlayer.SteamId].originalPos = myPlayer.GetPosition();
                        myPlayer.Teleport(AdminToolbox.warpVectors["jail"]);
                        AdminToolbox.playerdict[myPlayer.SteamId].isInJail = true;
                    }
                }
                else
                    return new string[] { args[1] + " is not a valid number!" };
            }
            return new string[] { GetUsage() };
        }
        void SendToJail(Player ply)
        {
            //Sets original position & teleports to jail
            AdminToolbox.playerdict[ply.SteamId].originalPos = ply.GetPosition();
            ply.Teleport(AdminToolbox.warpVectors["jail"]);
            //Saves inventory and deletes all items
            AdminToolbox.playerdict[ply.SteamId].playerPrevInv = ply.GetInventory();
            foreach (Smod2.API.Item item in ply.GetInventory())
                item.Remove();

            ply.ChangeRole(Role.TUTORIAL);

            AdminToolbox.playerdict[ply.SteamId].isInJail = true;

        }
        void ReturnFromJail(Player ply)
        {
            ply.Teleport(AdminToolbox.playerdict[ply.SteamId].originalPos);
            ply.ChangeRole(AdminToolbox.playerdict[ply.SteamId].previousRole);
            foreach(Smod2.API.Item item in AdminToolbox.playerdict[ply.SteamId].playerPrevInv)
            {
                ply.GiveItem(item.ItemType);
            }
            AdminToolbox.playerdict[ply.SteamId].isInJail = false;
        }
	}
}
