using System;

namespace AdminToolbox.API
{
	/// <summary>
	/// Class storing the LevenshteinDistance calculations
	/// </summary>
	public static class LevenshteinDistance
	{
		/// <summary>
		/// Compute the distance between two strings
		/// </summary>
		/// <param name="first">String 1</param>
		/// <param name="second">String 2</param>
		/// <returns>The distance AKA. number of required changes to make the strings equal</returns>
		public static int Compute(string first, string second)
		{
			int n = first.Length;
			int m = second.Length;
			int[,] d = new int[n + 1, m + 1];

			// Step 1
			if (n == 0)
			{
				return m;
			}

			if (m == 0)
			{
				return n;
			}

			// Step 2
			for (int i = 0; i <= n; d[i, 0] = i++)
			{
			}

			for (int j = 0; j <= m; d[0, j] = j++)
			{
			}

			// Step 3
			for (int i = 1; i <= n; i++)
			{
				//Step 4
				for (int j = 1; j <= m; j++)
				{
					// Step 5
					int cost = (second[j - 1] == first[i - 1]) ? 0 : 1;

					// Step 6
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}
			// Step 7
			return d[n, m];
		}
	}
}
