using Smod2;
using Smod2.API;
using Smod2.Events;

using System.Linq;

namespace AdminToolbox
{
    class TeamRespawnHandler : IEventTeamRespawn
    {
        private Plugin plugin;

        public TeamRespawnHandler(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnTeamRespawn(Player[] deadPlayers, out Player[] selectedPlayers, bool spawnChaos, out bool spawnChaosOutput)
        {
            selectedPlayers = deadPlayers;
            spawnChaosOutput = spawnChaos;
            //for (int i=0;i<deadPlayers.Length;i++)
            //{
            //    if(AdminToolbox.adminSteamID.Contains(deadPlayers[i].SteamId) && AdminToolbox.evanSpectator_onRespawn)
            //    {
            //        selectedPlayers = deadPlayers.Where((source, index) => index != i).ToArray();
            //    }
            //}
        }
    }
}
