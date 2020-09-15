using System;
using System.Linq;
using System.Reflection;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace AdminToolbox
{
	public struct RoundStats
	{
		public uint Chaos_Victory { get; private set; }
		public uint SCP_Chaos_Victory { get; private set; }
		public uint SCP_Victory { get; private set; }
		public uint MTF_Victory { get; private set; }
		public uint Other_Victory { get; private set; }
		public uint No_Victory { get; private set; }
		public uint Forced_Round_End { get; private set; }

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

		public override string ToString()
		{
			string reply = Environment.NewLine + "Round Stats: ";
			foreach (PropertyInfo property in this.GetType().GetProperties().OrderBy(s => s.Name))
				reply += Environment.NewLine + " - " + property.Name.Replace("_", " ") + ": " + property.GetValue(this) + "";
			return reply;
		}
	}

	internal class LateOnCheckRoundEndEvent : IEventHandlerCheckRoundEnd
	{
		private RoundStats Roundstat => AdminToolbox.RoundStats;

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (ev.Status != ROUND_END_STATUS.ON_GOING)
				if (!AdminToolbox.roundStatsRecorded && ev.Round.Duration >= 3)
				{
					AdminToolbox.roundStatsRecorded = true;
					Roundstat.AddPoint(ev.Status);
				}
		}
	}
}
