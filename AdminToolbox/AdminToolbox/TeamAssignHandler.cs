using Smod2;
using Smod2.API;
using Smod2.Events;

using System.Linq;

namespace AdminToolbox
{
    class TeamAssignHandler : IEventAssignTeam
    {
        private Plugin plugin;

        public TeamAssignHandler(Plugin plugin)
        {
            this.plugin = plugin;
        }
        public void OnAssignTeam(Player player, Teams team, out Teams teamOutput)
        {
            //if (AdminToolbox.adminSteamID.Contains(player.SteamId) && AdminToolbox.evanSpectator_onRespawn)
            //{
            //    teamOutput = Teams.SPECTATOR;
            //}
            //else teamOutput = team;
            teamOutput = team;
        }
    }
}
