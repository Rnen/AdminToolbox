using Smod2.API;

namespace AdminToolbox.API
{
	/// <summary>
	/// The class <see cref="AdminToolbox"/> uses for warp-points
	/// </summary>
	public struct WarpPoint
	{
		public string Name { get; set; }
		public string Description { get; set; } 
		public ATVector Vector { get; set; }
	}

	/// <summary>
	/// This is just a wrapper for the SMod <see cref="Smod2.API.Vector"/> because the SMod Vector is not JSON Serializable
	/// </summary>
	public class ATVector
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public ATVector() { }
		public ATVector(float X, float Y, float Z)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
		}
		public ATVector(Vector vec)
		{
			this.X = vec.x;
			this.Y = vec.y;
			this.Z = vec.z;
		}
		/// <summary>
		/// Converting the ATVector to <see cref="Smod2.API.Vector"/> because the SMod Vector is not JSON Serializable
		/// </summary>
		public Smod2.API.Vector ToSMVector()
		{
			return new Smod2.API.Vector(X, Y, Z);
		}
	}
}
