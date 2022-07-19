using System;
using System.Linq;
using System.Reflection;
using Smod2.API;

namespace AdminToolbox.API
{
	using Events;
	/// <summary>
	/// Struct used by <see cref="LateOnCheckRoundEndEvent"/> and <see cref="AdminToolbox.RoundStats"/> to store round victory statistics
	/// </summary>
	public struct RoundStats
	{
		/// <summary>
		/// Chaos-Only Victory
		/// </summary>
		public uint Chaos_Victory { get; private set; }
		/// <summary>
		/// SCP and Chaos Victory
		/// </summary>
		public uint SCP_Chaos_Victory { get; private set; }
		/// <summary>
		/// SCP-Only Victory
		/// </summary>
		public uint SCP_Victory { get; private set; }
		/// <summary>
		/// MTF-Only Victory
		/// </summary>
		public uint MTF_Victory { get; private set; }
		/// <summary>
		/// Other Victory
		/// </summary>
		public uint Other_Victory { get; private set; }
		/// <summary>
		/// No Victory
		/// </summary>
		public uint No_Victory { get; private set; }
		/// <summary>
		/// Round forced to end without a victory team
		/// </summary>
		public uint Forced_Round_End { get; private set; }

		/// <summary>
		/// Adds a victory point to the supplied round status
		/// </summary>
		/// <param name="status">The <seealso cref="RoundEndStatus"/> to calculate a point for </param>
		public void AddPoint(RoundEndStatus status)
		{
			switch (status)
			{
				case RoundEndStatus.CI_VICTORY:
					this.Chaos_Victory++;
					break;
				case RoundEndStatus.SCP_CI_VICTORY:
					this.SCP_Chaos_Victory++;
					break;
				case RoundEndStatus.SCP_VICTORY:
					this.SCP_Victory++;
					break;
				case RoundEndStatus.MTF_VICTORY:
					this.MTF_Victory++;
					break;
				case RoundEndStatus.OTHER_VICTORY:
					this.Other_Victory++;
					break;
				case RoundEndStatus.NO_VICTORY:
					this.No_Victory++;
					break;
				case RoundEndStatus.FORCE_END:
					this.Forced_Round_End++;
					break;
			}
		}

		/// <summary>
		/// Formats the class to a string for the console
		/// </summary>
		/// <returns>Formatted string</returns>
		public override string ToString()
		{
			string reply = Environment.NewLine + "Round Stats: ";
			foreach (PropertyInfo property in this.GetType().GetProperties().OrderBy(s => s.Name))
				reply += Environment.NewLine + " - " + property.Name.Replace("_", " ") + ": " + property.GetValue(this) + "";
			return reply;
		}
	}
}
