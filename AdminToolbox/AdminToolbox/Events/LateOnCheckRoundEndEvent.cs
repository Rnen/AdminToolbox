using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;

namespace AdminToolbox.Events
{
	using API;
	/// <summary>
	/// <seealso cref="IEventHandlerCheckRoundEnd"/>
	/// </summary>
	public class LateOnCheckRoundEndEvent : IEventHandlerCheckRoundEnd
	{
		private RoundStats Roundstat => new RoundStats();

		internal LateOnCheckRoundEndEvent() { }
		/// <summary>
		/// Does logic for the recording of winning teams to <see cref="RoundStats"/>. Also see <seealso cref="IEventHandlerCheckRoundEnd"/>
		/// </summary>
		/// <param name="ev"><seealso cref="CheckRoundEndEvent"/></param>
		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
			if (ev.Status != RoundEndStatus.ON_GOING)
				if (!AdminToolbox.roundStatsRecorded && ev.Round.Duration >= 3)
				{
					AdminToolbox.roundStatsRecorded = true;
					Roundstat.AddPoint(ev.Status);
				}
		}
	}
}
