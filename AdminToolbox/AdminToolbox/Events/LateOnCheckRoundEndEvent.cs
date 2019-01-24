using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace AdminToolbox
{
	class LateOnCheckRoundEndEvent : IEventHandlerCheckRoundEnd
	{
		readonly AdminToolbox plugin;

		public LateOnCheckRoundEndEvent(AdminToolbox plugin) => this.plugin = plugin;

		ATRoundStats roundstats => AdminToolbox.roundStats;

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (ev.Status != ROUND_END_STATUS.ON_GOING)
				if (!AdminToolbox.roundStatsRecorded && ev.Round.Duration >= 3)
				{
					AdminToolbox.roundStatsRecorded = true;
					roundstats.AddPoint(ev.Status);
				}
		}
	}

	public class ATRoundStats
	{
		public uint
			Chaos_Victory = 0,
			SCP_Chaos_Victory = 0,
			SCP_Victory = 0,
			MTF_Victory = 0,
			Other_Victory = 0,
			No_Victory = 0,
			Forced_Round_End = 0;

		public void AddPoint(ROUND_END_STATUS status)
		{
			switch (status)
			{
				case ROUND_END_STATUS.CI_VICTORY:
					this.Chaos_Victory++;
					break;
				case ROUND_END_STATUS.SCP_CI_VICTORY:
					this.SCP_Chaos_Victory++;
					break;
				case ROUND_END_STATUS.SCP_VICTORY:
					this.SCP_Victory++;
					break;
				case ROUND_END_STATUS.MTF_VICTORY:
					this.MTF_Victory++;
					break;
				case ROUND_END_STATUS.OTHER_VICTORY:
					this.Other_Victory++;
					break;
				case ROUND_END_STATUS.NO_VICTORY:
					this.No_Victory++;
					break;
				case ROUND_END_STATUS.FORCE_END:
					this.Forced_Round_End++;
					break;
			}
		}
	}
}
