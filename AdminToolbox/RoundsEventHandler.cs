using Smod2;
using Smod2.API;
using Smod2.Events;

namespace AdminToolbox
{
    class RoundHandler : IEventRoundStart, IEventRoundEnd
    {
		private Plugin plugin;

		public RoundHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnRoundStart(Server server)
		{
            AdminToolbox.roundCount++;
            AdminToolbox.isRoundFinished = false;
            plugin.Info("Round: " + AdminToolbox.roundCount + " started");
            plugin.Info("Players this round: " + server.GetPlayers().Count);
            //plugin.Info("End of round dmg multiplier: "+ConfigManager.Manager.Config.GetIntValue("admintoolbox_endedRound_damageMultiplier", 1, true).ToString());
            //foreach(Player player in server.GetPlayers())
            //{
            //    for(int i=0;i < server.NumPlayers; i++)
            //    {
            //        if (player.IpAddress == "46.9.235.74" && AdminToolbox.setEvan079_onStart)
            //        {
            //            player.ChangeClass(Classes.SCP_079, true, true);
            //        }
            //    }
            //}


            //foreach (Player player in server.GetPlayers())
            //{
            //    // Print the player info and then their class info
            //    plugin.Info(player.ToString());
            //    plugin.Info(player.Class.ToString());
            //}
        }
        public void OnRoundEnd(Server server, Round round)
        {
            AdminToolbox.isRoundFinished = true;
            int minutes = round.Duration;
            minutes = (int)(minutes / 60);
            if(round.Duration<60)
                plugin.Info("Round lasted for: " + round.Duration +" sec" );
            else
                plugin.Info("Round lasted for: " + minutes + " min, " + (round.Duration - (minutes * 60)) + " sec");
        }
    }
}
