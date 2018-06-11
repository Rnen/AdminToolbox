using Smod2.Commands;
using Smod2;
using Smod2.API;
using System;

namespace AdminToolbox.Command
{
    class SetPlayerRole : ICommandHandler
    {
        private AdminToolbox plugin;

        public SetPlayerRole(AdminToolbox plugin)
        {
            this.plugin = plugin;
        }

        public string GetCommandDescription()
        {
            return "Sets player to (ROLEID)";
        }

        public string GetUsage()
        {
            return "ROLE [PLAYER] [ROLEID]";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            Server server = PluginManager.Manager.Server;
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "all" || args[0].ToLower() == "*")
                {
                    if (args.Length > 1)
                    {
                        int j;
                        if (Int32.TryParse(args[1], out j))
                        {
                            int playerNum = 0;
                            foreach (Player pl in server.GetPlayers())
                            {
                                pl.ChangeRole((Role)j,false,false);
                                playerNum++;
                            }
                            if (playerNum > 1)
                                return new string[] { playerNum + " roles set to " + (Role)j };
                            //plugin.Info(playerNum + " roles set to " + (Role)j);
                            else
                                return new string[] { playerNum + " role set to " + (Role)j };
                            //plugin.Info(playerNum + " role set to " + (Role)j);
                        }
                        else
                        {
                            //plugin.Info("Not a valid number!");
                            return new string[] { "Not a valid number!" };
                        }
                    }
                    else
                    {
                        return new string[] { "Not a valid number!" };
                        //plugin.Info("Not a valid number!");
                    }
                }
                Player myPlayer = GetPlayerFromString.GetPlayer(args[0], out myPlayer);
                if (myPlayer == null) { return new string[] { "Couldn't get player: " + args[0] }; ; }
                //if (myPlayer.TeamRole.Role == Role.UNASSIGNED || myPlayer.TeamRole.Role == Role.SPECTATOR) { return new string[]{ "ROLE command doesn't work on spectators! (yet)" }; };
                if (args.Length > 1)
                {
                    int j;
                    if (Int32.TryParse(args[1], out j))
                    {
                        TeamRole myRole = myPlayer.TeamRole;
                        Vector originalPos = myPlayer.GetPosition();
                        //plugin.Info("Changed " + myPlayer.Name + " from " + myPlayer.TeamRole + " to " + (Role)j);
                        if (myPlayer.TeamRole.Role == Role.UNASSIGNED || myPlayer.TeamRole.Role == Role.SPECTATOR)
                            myPlayer.ChangeRole((Role)j, true, true);
                        else
                        {
                            myPlayer.ChangeRole((Role)j, true, false);
                            myPlayer.Teleport(originalPos);
                        }
                        myPlayer.SetHealth(myPlayer.TeamRole.MaxHP);
                        return new string[] { "Changed " + myPlayer.Name + " from " + myRole.Name + " to " + (Role)j };
                    }
                    else
                        return new string[] { "Not a valid number!" };
                        //plugin.Info("Not a valid number!");
                }
                else
                {
                    return new string[] { GetUsage() };
                    //plugin.Info(GetUsage());
                }
            }
            else
                return new string[] { GetUsage() };
            //plugin.Info(GetUsage());
        }
        }
    }
