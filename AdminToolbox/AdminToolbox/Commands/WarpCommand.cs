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
	class WarpCommmand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public WarpCommmand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "";
		}

		public string GetUsage()
		{
			return "WARP [LIST]\nWARP [ADD/+] [PlayerName] [YourWarpPointName]\nWARP [REMOVE/-] [YourWarpPointName]\nWARP [PlayerName] [WarpPointName]";
		}

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "list")
                {
                    if(AdminToolbox.warpVectors.Count<1) { return new string[] { "No warp points created yet!" }; }
                    string str = "\nWarp Points:";
                    //AdminToolbox.warpVectors.
                    var list = AdminToolbox.warpVectors.Keys.ToList();
                    list.Sort();
                    foreach (var i in list)
                    {
                        str += "\n - "+i;
                    }
                    //plugin.Info(str);
                    return new string[] { str };
                }
                else if (args[0].ToLower() == "remove" || args[0].ToLower() == "-")
                {
                    if (AdminToolbox.warpVectors.ContainsKey(args[1].ToLower()))
                    {
                        AdminToolbox.warpVectors.Remove(args[1].ToLower());
                        return new string[] { "Warp point: " + args[1].ToLower() + " removed." };
                        //plugin.Info("Warp point: " + args[2].ToLower() + " removed.");
                    }
                    else
                        return new string[] { "Warp point " + args[1].ToLower() + " does not exist!" };
                    //plugin.Info("Warp point " + args[2].ToLower() + " does not exist!");
                }
                else if (args[0].ToLower() == "add" || args[0].ToLower() == "+")
                {
                    if (args.Length > 2)
                    {
                        if (!AdminToolbox.warpVectors.ContainsKey(args[2]))
                        {
                            Player myPlayer = GetPlayerFromString.GetPlayer(args[1], out myPlayer);
                            if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[1] }; ; }
                            Vector myvector = myPlayer.GetPosition();
                            AdminToolbox.warpVectors.Add(args[2].ToLower(), myvector);
                            //plugin.Info("Warp point: " + args[2].ToLower() + " added.");
                            return new string[] { "Warp point: " + args[2].ToLower() + " added." };
                        }
                        else
                            return new string[] { "A warp point named: " + args[2].ToLower() + " already exists!" };
                        //plugin.Info("A warp point named: " + args[2].ToLower() + " already exists!");
                    }
                    else
                        return new string[] { GetUsage() };
                }
                else
                {
                    if (args.Length > 1)
                    {
                        Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                        if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
                        if (!AdminToolbox.warpVectors.ContainsKey(args[1].ToLower()))
                            return new string[] { "No warp point called: " + args[1] };
                        myPlayer.Teleport(AdminToolbox.warpVectors[args[1].ToLower()]);
                        return new string[] { "Teleported: " + myPlayer.Name + " to warp point: " + args[1] };
                    }
                    else
                        return new string[] { GetUsage() };
                }
            }
            else
                return new string[] { GetUsage() };
            //plugin.Info(GetUsage());
        }
	}
}
