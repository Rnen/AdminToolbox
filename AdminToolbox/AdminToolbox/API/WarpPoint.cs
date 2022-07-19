using Smod2.API;

namespace AdminToolbox.API
{
	/// <summary>
	/// The struct <see cref="AdminToolbox.WarpManager"/> uses for warp-points
	/// </summary>
	public struct WarpPoint
	{
		/// <summary>
		/// Name of the WarpPoint. This is used to look it up later
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Description of the WarpPoint. This is displayed when Warps are listed
		/// </summary>
		public string Description { get; set; } 
		/// <summary>
		/// The <see cref="Smod2.API.Vector"/> position of the WarpPoint. Stored in a <see cref="ATVector"/> wrapper class for JSON use
		/// </summary>
		public ATVector Vector { get; set; }
	}

	/// <summary>
	/// This is just a wrapper for the SMod <see cref="Smod2.API.Vector"/> because the SMod Vector can not be JSON Serialized
	/// </summary>
	public class ATVector
	{
		/// <summary>
		/// X axis
		/// </summary>
		public float X { get; set; }
		/// <summary>
		/// Y axis
		/// </summary>
		public float Y { get; set; }
		/// <summary>
		/// Z axis
		/// </summary>
		public float Z { get; set; }

		/// <summary>
		/// Creates a new ATVector
		/// </summary>
		public ATVector() { }

		/// <summary>
		/// Creates a new ATVector with the supplied coordinates
		/// </summary>
		/// <param name="X"><see cref="ATVector.X"/></param>
		/// <param name="Y"><see cref="ATVector.Y"/></param>
		/// <param name="Z"><see cref="ATVector.Z"/></param>
		public ATVector(float X, float Y, float Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}

		/// <summary>
		/// Creates a new ATVector with the supplied vector position
		/// </summary>
		/// <param name="position"><see cref="ATVector"/></param>
		public ATVector(Vector position)
		{
			this.X = position.x;
			this.Y = position.y;
			this.Z = position.z;
		}
		/// <summary>
		/// Converting the <see cref="ATVector"/> to <see cref="Smod2.API.Vector"/>  because the SMod Vector can not be JSON Serialized
		/// </summary>
		public Smod2.API.Vector ToSMVector() => new Smod2.API.Vector(X, Y, Z);
	}
}
