using System;
using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.API;
using UnityEngine;

namespace AdminToolbox.API
{
	public class ScheduledRestart
	{
		IConfigFile Config => ConfigManager.Manager.Config;
		Server Server => PluginManager.Manager.Server;

		readonly AdminToolbox plugin;

		public bool enabled;
		public DateTime restartTime;
		public Dictionary<int, string> scheduledMessages;

		public ScheduledRestart(AdminToolbox plugin)
		{
			this.plugin = plugin;
			this.enabled = false;
			this.restartTime = DateTime.MaxValue;
			scheduledMessages = new Dictionary<int, string>();
		}

		public void Cancel()
		{
			this.enabled = false;
			this.restartTime = DateTime.MaxValue;
			scheduledMessages = new Dictionary<int, string>();
		}

		public void Enable(DateTime time, Dictionary<int, string> scheduledMessages)
		{
			this.restartTime = time;
			this.scheduledMessages = new Dictionary<int, string>(scheduledMessages);
			this.enabled = true;
		}

		public void CallRestart()
		{
			try
			{
				AdminToolbox.atfileManager.PlayerStatsFileManager(AdminToolbox.ATPlayerDict.Keys.ToList(), Managers.ATFileManager.PlayerFile.Write);
				AdminToolbox.warpManager.WriteWarpsToFile();
			}
			catch { throw new Exception(this.plugin.Details.name + " failed to save files!"); }
			try
			{
				if (PlayerManager.singleton.players.Length > 0)
					foreach (PlayerStats ps in PlayerManager.singleton.players.Where(p => p.GetComponent<PlayerStats>()).Select(s => s.GetComponent<PlayerStats>()))
						ps.CallRpcRoundrestart();

				foreach(Plugin pl in PluginManager.Manager.Plugins.Where(i => i.Details.id != this.plugin.Details.id))
					PluginManager.Manager.DisablePlugin(pl);
				PluginManager.Manager.DisablePlugin(this.plugin);
				Application.Quit();
			}
			catch { throw new Exception(this.plugin.Details.name + " restart procedure failed!"); }
		}
	}
}
