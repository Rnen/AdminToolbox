using Smod2.API;
using UnityEngine;
using System.Collections;

namespace AdminToolbox.API
{
	internal class WaitForTeleport : MonoBehaviour
	{
		internal Player player;
		internal Vector pos;

		public WaitForTeleport(Player player, Vector pos, float waitTime = 2f)
		{
			this.player = player;
			this.pos = pos;
			StartCoroutine(WaitAndTeleport(waitTime));
		}

		public bool Done { get; private set; } = false;

		internal IEnumerator WaitAndTeleport(float waitTime)
		{
			if (player == null)
				yield break;
			yield return new WaitForSeconds(waitTime);
			player.Teleport(pos);
			this.player = null;
			this.pos = null;

			Done = true;
			yield break;
		}
	}
}
