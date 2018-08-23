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
            return "ABAN [Nick] [IP/SteamID] [TIME IN MINUTES]";
        }

        public string GetCommandDescription()
        {
            return "Ban someone"; //TODO: Make good Desc
        }
        
        public string[] OnCall(ICommandSender sender, string[] args)
        {
            try
            {
                if (args.Length == 0) return new [] {GetUsage()};
                if (args.Length != 3) return new[] {"Wrong number of arguments."};
                var ipb = FileManager.AppFolder + "IpBans.txt";
                var sib = FileManager.AppFolder + "SteamIdBans.txt";
                var outs = "";
                var lastminute = DateTime.Now;
                if(double.TryParse(args[2],out var oarg))
                {
                    lastminute = lastminute.AddMinutes(oarg);
                }
                else
                {
                    return new []{"Wrong time format: " + args[2] };
                }

                if (args[1].Contains("."))
                {
                    string ip = (args[1].Contains("::ffff:")) ? args[1] : "::ffff:" + args[1]; 
                    outs += args[0] + ";" + ip + ";" + lastminute.Ticks + ";;Server;" + DateTime.Now.Ticks;
                    File.AppendAllText(ipb,"\n"+outs);
                    return new[] {"Player with nick: " + args[0] + " and with IP: " + args[1] + " has banned for " + args[2] + " minutes."};
                }
                else
                {
                    outs += args[0] + ";" + args[1] + ";" + lastminute.Ticks + ";;Server;" + DateTime.Now.Ticks;
                    File.AppendAllText(sib,"\n"+outs);
                    return new[] {"Player with nick: " + args[0] + " and with SteamID: " + args[1] + " has banned for " + args[2] + " minutes."};
                }
            }
            catch (Exception e)
            {
                return new [] {e.StackTrace};
            }
        }
    }
}
