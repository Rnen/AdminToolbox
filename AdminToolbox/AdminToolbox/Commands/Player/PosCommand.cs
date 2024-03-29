using System.Collections.Generic;
using System.Linq;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
	using API;
	using API.Extentions;
	public class PosCommand : ICommandHandler
	{
		private const bool Unstuck = false;

		public string GetCommandDescription() => "Alters the player position";
		public string GetUsage() => "(" + string.Join(" / ", CommandAliases) + ") [PLAYER] (ADD/SET/GET) <x=> <y=> <z=>\nVisit the \"github.com/Rnen/AdminToolbox/Wiki\" for more info";

		public static readonly string[] CommandAliases = new string[] { "POS", "ATPOS", "AT-POS" };

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (sender.IsPermitted(CommandAliases, out string[] deniedReply))
			{

				if (args.Length > 0)
				{
					Player myPlayer = GetFromString.GetPlayer(args[0]);
					if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; ; }
					Managers.ATFile.AddMissingPlayerVariables(myPlayer);
					if (args.Length > 1)
					{
						if (args.Length > 2)
						{
							if (args[1].ToLower() == "add")
							{
								Dictionary<string, int> myPos = new Dictionary<string, int>()
							{
								{"x",0 },
								{"y",0 },
								{"z",0 }
							};
								int xInput = 0, yInput = 0, zInput = 0;
								char[] delimiterChars = { '.', ',', '=', '_', ':', '^', '/', '>', '<', '\'' };
								string[] charsAllowed = { "x", "y", "z" };
								for (int i = 2; i < args.Length; i++)
								{
									string[] cordinates = args[i].ToLower().Split(delimiterChars);
									if (cordinates.Length < 2) { return new string[] { GetUsage() }; }
									if (!charsAllowed.Any(cordinates[0].Contains))
										return new string[] { cordinates[0] + " is not a valid axis, use X, Y or Z" };
									if (int.TryParse(cordinates[1], out int xyz))
									{
										if (!myPos.ContainsKey(cordinates[0]))
											myPos.Add(cordinates[0], xyz);
										else
											myPos[cordinates[0]] = xyz;
									}
									else
										return new string[] { cordinates[1] + " is not a valid number" };
								}
								xInput = myPos["x"];
								yInput = myPos["y"];
								zInput = myPos["z"];
								Vector myvector = new Vector(myPlayer.GetPosition().x + xInput, myPlayer.GetPosition().y + yInput, myPlayer.GetPosition().z + zInput);
								myPlayer.Teleport(myvector, Unstuck);
								return new string[] { "Teleported " + myPlayer.Name + " to x:" + myvector.x + " y:" + myvector.y + " z:" + myvector.z };
							}
							else if (args[1].ToLower() == "set")
							{
								Dictionary<string, int> myPos = new Dictionary<string, int>()
							{
								{"x",0 },
								{"y",0 },
								{"z",0 }
							};
								int xInput = 0, yInput = 0, zInput = 0;
								char[] delimiterChars = { '.', ',', '=', '_', ':', ';', ' ' };
								string[] charsAllowed = { "x", "y", "z" };
								for (int i = 2; i < args.Length; i++)
								{
									string[] cordinates = args[i].ToLower().Split(delimiterChars);
									if (cordinates.Length < 2) { return new string[] { GetUsage() }; }
									if (!charsAllowed.Any(cordinates[0].Contains))
									{
										return new string[] { cordinates[0] + " is not a valid axis, use X, Y or Z" };
									}
									if (int.TryParse(cordinates[1], out int xyz))
									{
										if (!myPos.ContainsKey(cordinates[0].ToLower()))
										{
											myPos.Add(cordinates[0].ToLower(), xyz);
										}
										else
										{
											myPos[cordinates[0]] = xyz;
										}
									}
									else
									{
										return new string[] { cordinates[1] + " is not a valid number" };
									}

								}
								xInput = myPos["x"];
								yInput = myPos["y"];
								zInput = myPos["z"];
								Vector myvector = new Vector(xInput, yInput, zInput);

								myPlayer.Teleport(myvector, Unstuck);
								return new string[] { "Teleported " + myPlayer.Name + " to x:" + myvector.x + " y:" + myvector.y + " z:" + myvector.z };
							}
							else
								return new string[] { GetUsage() };
						}
						else if (args[1].ToLower() == "get")
						{
							return new string[] { myPlayer.Name + " Pos x:" + myPlayer.GetPosition().x + " y:" + myPlayer.GetPosition().y + " z:" + myPlayer.GetPosition().z };
						}
						else
							return new string[] { GetUsage() };
					}
					else
						return new string[] { GetUsage() };
				}
				else
					return new string[] { GetUsage() };
			}
			else
				return deniedReply;
		}
	}
}
