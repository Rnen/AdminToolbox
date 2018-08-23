using System;
using System.IO;
using Smod2;
using Smod2.API;
using Smod2.Commands;

namespace AdminToolbox.Command
{
    public class ATBanCommand : ICommandHandler
    {

        private Plugin plugin;

        public ATBanCommand(Plugin plugin)
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
                if (args.Length != 3) return new string[] {"Wrong number of arguments."};
                string ipBanPath = FileManager.AppFolder + "IpBans.txt";
                string sidBanPath = FileManager.AppFolder + "SteamIdBans.txt";
                string outs = "";
                string IssuingPlayer = (sender is Player pl && !string.IsNullOrEmpty(pl.SteamId)) ? pl.Name : "Server";
                DateTime lastminute = DateTime.Now;
                if(double.TryParse(args[2],out var minutes))
                    lastminute = lastminute.AddMinutes(minutes);
                else
                    return new string[]{"Wrong time format: " + args[2] };

                if (args[1].Contains("."))
                {
                    if(args[1].Split('.').Length != 4) return new string[] { "Invalid IP: " + args[1] };
                    string ip = (args[1].Contains("::ffff:")) ? args[1] : "::ffff:" + args[1]; 
                    outs += args[0] + ";" + ip + ";" + lastminute.Ticks + ";;" + IssuingPlayer + ";" + DateTime.Now.Ticks;
                    File.AppendAllText(ipBanPath,"\n"+outs);
					if (IssuingPlayer != "Server") plugin.Info("Player with nick: " + args[0] + " and with IP: " + args[1] + " was banned for " + args[2] + " minutes by " + IssuingPlayer);
                    return new string[] {"Player with nick: " + args[0] + " and with IP: " + args[1] + " was banned for " + args[2] + " minutes by "+ IssuingPlayer};
                }
                else
                {
                    outs += args[0] + ";" + args[1] + ";" + lastminute.Ticks + ";;" + IssuingPlayer + ";" + DateTime.Now.Ticks;
                    File.AppendAllText(sidBanPath,"\n"+outs);
					if (IssuingPlayer != "Server") plugin.Info("Player with nick: " + args[0] + " and with SteamID: " + args[1] + " was banned for " + args[2] + " minutes by " + IssuingPlayer);
					return new string[] {"Player with nick: " + args[0] + " and with SteamID: " + args[1] + " was banned for " + args[2] + " minutes by " + IssuingPlayer};
                }
            }
            catch (Exception e)
            {
                return new string[] {e.StackTrace};
            }
        }
    }
}
