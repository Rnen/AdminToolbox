using System;
using System.IO;
using Smod2;
using Smod2.Commands;

namespace AdminToolbox.Command
{
    public class ABanCommand : ICommandHandler
    {

        private Plugin plugin;

        public ABanCommand(Plugin plugin)
        {
            this.plugin = plugin;
        }
        
        public string GetUsage()
        {
            return "ATBAN [Nick] [IP/SteamID] [TIME IN MINUTES]";
        }

        public string GetCommandDescription()
        {
            return "Alternative ban for offline users";
        }
        
        public string[] OnCall(ICommandSender sender, string[] args)
        {
            try
            {
                if (args.Length == 0) return new string[] {GetUsage()};
                if (args.Length != 3) return new string[] {"Wrong number of arguments."};
                string ipb = FileManager.AppFolder + "IpBans.txt";
                string sib = FileManager.AppFolder + "SteamIdBans.txt";
                string outs = "";
                DateTime lastminute = DateTime.Now;
                if(double.TryParse(args[2],out var oarg))
                {
                    lastminute = lastminute.AddMinutes(oarg);
                }
                else
                {
                    return new string[]{"Wrong time format: " + args[2] };
                }

                if (args[1].Contains("."))
                {
                    if(args[1].Split('.').Length != 4) return new string[] { "Invalid IP: " + args[1] };
                    string ip = (args[1].Contains("::ffff:")) ? args[1] : "::ffff:" + args[1]; 
                    outs += args[0] + ";" + ip + ";" + lastminute.Ticks + ";;Server;" + DateTime.Now.Ticks;
                    File.AppendAllText(ipb,"\n"+outs);
                    return new string[] {"Player with nick: " + args[0] + " and with IP: " + args[1] + " has banned for " + args[2] + " minutes."};
                }
                else
                {
                    outs += args[0] + ";" + args[1] + ";" + lastminute.Ticks + ";;Server;" + DateTime.Now.Ticks;
                    File.AppendAllText(sib,"\n"+outs);
                    return new string[] {"Player with nick: " + args[0] + " and with SteamID: " + args[1] + " has banned for " + args[2] + " minutes."};
                }
            }
            catch (Exception e)
            {
                return new string[] {e.StackTrace};
            }
        }
    }
}
