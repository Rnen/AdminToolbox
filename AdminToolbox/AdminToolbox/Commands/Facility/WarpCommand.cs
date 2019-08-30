using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;

	public class WarpCommmand : ICommandHandler
	{
		private Server Server => PluginManager.Manager.Server;

		public string GetCommandDescription() => "For the AT Warp system. Type WARP HELP for more info";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") <HELP>";

		private string[] GetHelpReturn() => new string[]
			{
				"Current aliases: " + string.Join(" / ", CommandAliases),
				"WARP [PlayerName] [WarpPointName]",
				"WARP LIST",
				"WARP [ADD/+] [PlayerName] [WarpPointName] <Optional Description>",
				"WARP [REMOVE/-] [WarpPointName]",
				"WARP REFRESH"
			};

		public static readonly string[] CommandAliases = new string[] { "WARP", "ATWARP", "AT-WARP" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{
				if (sender != null && sender is Player p && p != null)
					AdminToolbox.AddMissingPlayerVariables(p);

				if (args.Length > 0)
				{
					switch (args[0].ToUpper())
					{
						case "HELP":
						case "H":
							return GetHelpReturn();
						case "LIST":
							if (AdminToolbox.WarpVectorDict.Count < 1) { return new string[] { "No warp points created yet!" }; }
							string str = "\n" + "Warp Points:";
							IEnumerable<WarpPoint> list = AdminToolbox.WarpVectorDict.Values.OrderBy(s => s.Name);
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
							if (AdminToolbox.WarpVectorDict.ContainsKey(args[1].ToLower()))
							{
								AdminToolbox.WarpVectorDict.Remove(args[1].ToLower());
								AdminToolbox.warpManager.WriteWarpsToFile();
								return new string[] { "Warp point: " + args[1].ToLower() + " removed." };
							}
							else
								return new string[] { "Warp point " + args[1].ToLower() + " does not exist!" };
						case "ADD":
						case "+":
							if (args.Length > 2)
							{
								if (!AdminToolbox.WarpVectorDict.ContainsKey(args[2].ToLower()))
								{
									Player myPlayer = GetPlayerFromString.GetPlayer(args[1]);
									if (myPlayer == null) { return new string[] { "Could not find player: " + args[1] }; ; }
									Vector myvector = myPlayer.GetPosition();
									string desc = "";
									if (args.Length >= 3)
										for (int i = 3; i < args.Length; i++)
											desc = args[i] + " ";
									AdminToolbox.WarpVectorDict.Add(args[2].ToLower(), new WarpPoint { Name = args[2].ToLower(), Description = desc, Vector = new ATVector(myvector) });
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
									else if (!AdminToolbox.WarpVectorDict.ContainsKey(args[1].ToLower()))
										return new string[] { "No warp point called: " + args[1] };
									byte playerNum = 0;
									foreach (Player pl in Server.GetPlayers())
									{
										pl.Teleport(AdminToolbox.WarpVectorDict[args[1].ToLower()].Vector.ToSMVector(), true);
										playerNum++;
									}
									return new string[] { "Teleported " + playerNum + " players to warp point: " + args[1] };
								}
								Player myPlayer = GetPlayerFromString.GetPlayer(args[0]);
								if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
								if (!AdminToolbox.WarpVectorDict.ContainsKey(args[1].ToLower()))
									return new string[] { "No warp point called: " + args[1] };
								myPlayer.Teleport(AdminToolbox.WarpVectorDict[args[1].ToLower()].Vector.ToSMVector(), true);
								return new string[] { "Teleported: " + myPlayer.Name + " to warp point: " + args[1] };
							}
							else
								return new string[] { GetUsage() };
					}
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
