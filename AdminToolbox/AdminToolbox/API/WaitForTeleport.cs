using System;
using Smod2.API;

namespace AdminToolbox.API
{
	internal class WaitForTeleport
	{
		internal Player Player { get; set; }
		internal Vector Pos { get; set; }
		internal DateTime DateTime { get; set; }
		internal bool Done { get; set; } = false;
	}
}
