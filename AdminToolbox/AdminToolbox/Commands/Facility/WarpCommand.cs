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
using AdminToolbox.Managers;
using AdminToolbox.API;

namespace AdminToolbox.Command
{
	class WarpCommmand : ICommandHandler
	{
		Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription()
		{
			return "";
		}

		public string GetUsage()
		{
			return
				"WARP [PlayerName] [WarpPointName]" + "\n" +
				"WARP LIST" + "\n" +
				"WARP [ADD/+] [PlayerName] [WarpPointName] (Optional Description)" + "\n" +
				"WARP [REMOVE/-] [WarpPointName]" + "\n" +
				"WARP REFRESH";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender != null && sender is Player p && p != null)
				AdminToolbox.AddMissingPlayerVariables(p);

			if (args.Length > 0)
			{
				switch (args[0].ToUpper())
				{
					case "LIST":
						if (AdminToolbox.warpVectors.Count < 1) { return new string[] { "No warp points created yet!" }; }
						string str = "\n" + "Warp Points:";
						IEnumerable<WarpPoint> list = AdminToolbox.warpVectors.Values.OrderBy(s => s.Name);
						int maxSize = list.Max(s => s.Name.Length);
						bool toggle = false;
						foreach (WarpPoint i in list)
						{
							string name = i.Name;
							int wordSize = (toggle) ? maxSize : maxSize + 1;
							while (name.Length < wordSize) name += " ";
							str += "\n - " + name + (!string.IsNullOrEmpty(i.Description) ? " ---> " + i.Description : "");
							toggle = !toggle;
						}
						return new string[] { str };
					case "REFRESH":
						AdminToolbox.warpManager.RefreshWarps();
						return new string[] { "Refreshed warps!" };
					case "REMOVE":
					case "-":
						if (AdminToolbox.warpVectors.ContainsKey(args[1].ToLower()))
						{
							AdminToolbox.warpVectors.Remove(args[1].ToLower());
							AdminToolbox.warpManager.WriteWarpsToFile();
							return new string[] { "Warp point: " + args[1].ToLower() + " removed." };
						}
						else
							return new string[] { "Warp point " + args[1].ToLower() + " does not exist!" };
					case "ADD":
					case "+":
						if (args.Length > 2)
						{
							if (!AdminToolbox.warpVectors.ContainsKey(args[2].ToLower()))
							{
								Player myPlayer = GetPlayerFromString.GetPlayer(args[1]);
								if (myPlayer == null) { return new string[] { "Could not find player: " + args[1] }; ; }
								Vector myvector = myPlayer.GetPosition();
								string desc = "";
								if (args.Length >= 3)
									for (int i = 3; i < args.Length; i++)
										desc = args[i] + " ";
								AdminToolbox.warpVectors.Add(args[2].ToLower(), new WarpPoint(args[2].ToLower(), desc, myvector));
								AdminToolbox.warpManager.WriteWarpsToFile();
								return new string[] { "Warp point: " + args[2].ToLower() + " added." };
							}
							else
								return new string[] { "A warp point named: " + args[2].ToLower() + " already exists!" };
						}
						else
							return new string[] { GetUsage() };
					default:
						if (args.Length > 1)
						{
							if (args[0] == "*")
							{
								if (Server.GetPlayers().Count == 0)
									return new string[] { "No players to teleport!" };
								else if (!AdminToolbox.warpVectors.ContainsKey(args[1].ToLower()))
									return new string[] { "No warp point called: " + args[1] };
								byte playerNum = 0;
								foreach (Player pl in Server.GetPlayers())
								{
									pl.Teleport(AdminToolbox.warpVectors[args[1].ToLower()].Vector, true);
									playerNum++;
								}
								return new string[] { "Teleported " + playerNum + " players to warp point: " + args[1] };
							}
							Player myPlayer = API.GetPlayerFromString.GetPlayer(args[0]);
							if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
							if (!AdminToolbox.warpVectors.ContainsKey(args[1].ToLower()))
								return new string[] { "No warp point called: " + args[1] };
							myPlayer.Teleport(AdminToolbox.warpVectors[args[1].ToLower()].Vector, true);
							return new string[] { "Teleported: " + myPlayer.Name + " to warp point: " + args[1] };
						}
						else
							return new string[] { GetUsage() };
				}
			}
			else
				return new string[] { GetUsage() };
		}
	}
}