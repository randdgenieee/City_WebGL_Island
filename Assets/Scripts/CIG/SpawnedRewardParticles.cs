using System.Collections;
using UnityEngine;

namespace CIG
{
	public class SpawnedRewardParticles : SpawnedParticles
	{
		public override void Play()
		{
			base.Play();
			StartCoroutine(SfxRoutine());
		}

		private IEnumerator SfxRoutine()
		{
			for (int i = 0; i < 3; i++)
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.RewardPling);
				yield return new WaitForSeconds(0.25f);
			}
		}
	}
}
