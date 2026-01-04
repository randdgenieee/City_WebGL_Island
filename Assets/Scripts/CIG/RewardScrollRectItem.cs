using Tweening;
using UnityEngine;

namespace CIG
{
	public class RewardScrollRectItem : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _scaleContainer;

		[SerializeField]
		private Tweener _slideInTweener;

		public bool IsAnimating => _slideInTweener.IsPlaying;

		public void AnimateIn()
		{
			_slideInTweener.Play();
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ShowReward);
		}

		public void ResetAnimation(bool resetToEnd)
		{
			_slideInTweener.StopAndReset(resetToEnd);
		}

		public void SetScale(float scale)
		{
			_scaleContainer.transform.localScale = scale * Vector3.one;
		}
	}
}
