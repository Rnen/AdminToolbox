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
	class PosCommand : ICommandHandler
	{
		private AdminToolbox plugin;
        
		public PosCommand(AdminToolbox plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Alters the player position";
		}

		public string GetUsage()
		{
			return "pos (player) [add/set/get/help] [x=][y=][z=]\nVisit the \"github.com/Rnen/AdminToolbox\" for more info";
		}


        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't find player: " + args[0] }; ; }
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
                            char[] delimiterChars = { '.', ',', '=', '_', ':', ';', ' ' };
                            string[] charsAllowed = { "x", "y", "z" };
                            for (int i = 2; i < args.Length; i++)
                            {
                                string[] cordinates = args[i].ToLower().Split(delimiterChars);
                                if (cordinates.Length < 2) { return new string[] { GetUsage() }; }
                                if (!charsAllowed.Any(cordinates[0].Contains))
                                {
                                    //plugin.Info(cordinates[0] + " is not a valid axis, use X, Y or Z");
                                    return new string[] { cordinates[0] + " is not a valid axis, use X, Y or Z" };
                                }
                                int xyz = 0;
                                if (Int32.TryParse(cordinates[1], out xyz))
                                {
                                    if (!myPos.ContainsKey(cordinates[0]))
                                    {
                                        //myPos.Remove(cordinates[0]);
                                        //plugin.Info("Added " + cordinates[0] + " value: " + xyz + " to cord dict");
                                        myPos.Add(cordinates[0], xyz);
                                    }
                                    else
                                    {
                                        //plugin.Info("Set " + cordinates[0] + " value: " + xyz);
                                        myPos[cordinates[0]] = xyz;
                                    }
                                }
                                else
                                {
                                    //plugin.Info(cordinates[1] + " is not a valid number");
                                    return new string[] { cordinates[1] + " is not a valid number" };
                                }

                            }
                            xInput = myPos["x"];
                            yInput = myPos["y"];
                            zInput = myPos["z"];
                            //plugin.Info("MyPos: \n xInput: " + xInput + "\n yInput: " + yInput + "\n zInput: " + zInput);
                            Vector myvector = new Vector(myPlayer.GetPosition().x + xInput, myPlayer.GetPosition().y + yInput, myPlayer.GetPosition().z + zInput);

                            myPlayer.Teleport(myvector);
                            return new string[] { "Teleported " + myPlayer.Name + " to x:" + myvector.x + " y:" + myvector.y + " z:" + myvector.z };
                            //plugin.Info("Teleported " + myPlayer.Name + " to x:" + myvector.x + " y:" + myvector.y + " z:" + myvector.z);
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
                                    //plugin.Info(cordinates[0] + " is not a valid axis, use X, Y or Z");
                                    return new string[] { cordinates[0] + " is not a valid axis, use X, Y or Z" };
                                }
                                int xyz = 0;
                                if (Int32.TryParse(cordinates[1], out xyz))
                                {
                                    if (!myPos.ContainsKey(cordinates[0].ToLower()))
                                    {
                                        //myPos.Remove(cordinates[0]);
                                        //plugin.Info("Added " + cordinates[0] + " value: " + xyz + " to cord dict");
                                        myPos.Add(cordinates[0].ToLower(), xyz);
                                    }
                                    else
                                    {
                                        //plugin.Info("Set " + cordinates[0] + " value: " + xyz);
                                        myPos[cordinates[0]] = xyz;
                                    }
                                }
                                else
                                {
                                    //plugin.Info(cordinates[1] + " is not a valid number");
                                    return new string[] { cordinates[1] + " is not a valid number" };
                                }

                            }
                            xInput = myPos["x"];
                            yInput = myPos["y"];
                            zInput = myPos["z"];
                            //plugin.Info("MyPos: \n xInput: " + xInput + "\n yInput: " + yInput + "\n zInput: " + zInput);
                            Vector myvector = new Vector(xInput, yInput, zInput);

                            myPlayer.Teleport(myvector);
                            //plugin.Info("Teleported " + myPlayer.Name + " to x:" + myvector.x + " y:" + myvector.y + " z:" + myvector.z);
                            return new string[] { "Teleported " + myPlayer.Name + " to x:" + myvector.x + " y:" + myvector.y + " z:" + myvector.z };
                        }
                        else
                            return new string[] { GetUsage() };
                        //plugin.Info("Not valid type, use [add/set/get]");
                    }
                    else if (args[1].ToLower() == "get")
                    {
                        //plugin.Info(myPlayer.Name + " Pos[x:" + myPlayer.GetPosition().x + " y:" + myPlayer.GetPosition().y + " z:" + myPlayer.GetPosition().z);
                        return new string[] { myPlayer.Name + " Pos x:" + myPlayer.GetPosition().x + " y:" + myPlayer.GetPosition().y + " z:" + myPlayer.GetPosition().z };
                    }
                    else
                        return new string[] { GetUsage() };
                    //plugin.Info(GetUsage());
                }
                else
                    return new string[] { GetUsage() };
                //plugin.Info(GetUsage());
            }
            //else if (args[0].ToLower() == "help")
            //{
            //    //plugin.Info("\n General info: You dont need to use all X Y Z, you can use only one if you wish\n This command might trigger the anti-cheat if you have it enabled \n Teleporing less than 7 units might not work \n \n Position command examples: \n  pos (player) set x:80 y:-1000 z:95  -Sets the player pos to x y z \n pos (player) add y:10   -Adds to the player's chosen axis pos \n pos (player) get  -Gets the player's current pos \n Write ROOMPOS to see room positions");
            //    return new string[] { "\nGeneral info: You dont need to use all X Y Z, you can use only one if you wish\n This command might trigger the anti-cheat if you have it enabled \n Teleporing less than 7 units might not work \n \n Position command examples: \n  pos (player) set x:80 y:-1000 z:95  -Sets the player pos to x y z \n pos (player) add y:10   -Adds to the player's chosen axis pos \n pos (player) get  -Gets the player's current pos \n Write ROOMPOS to see room positions" };
            //}
            else
                return new string[] { GetUsage() };
        }
	}
}
