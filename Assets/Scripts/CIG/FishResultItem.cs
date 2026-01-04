using Tweening;
using UnityEngine;

namespace CIG
{
	public class FishResultItem : MonoBehaviour
	{
		[SerializeField]
		private GameObject _textGroup;

		[SerializeField]
		private NumberTweenHelper _excessText;

		[SerializeField]
		private Tweener _tweener;

		[SerializeField]
		private AnimatedSpriteBase _animatedSprite;

		public void Initialize()
		{
			_textGroup.SetActive(value: false);
			_excessText.TweenTo(decimal.Zero, 0f);
			_tweener.Play();
		}

		public void ShowExcessText(int excess, float duration)
		{
			_textGroup.SetActive(value: true);
			_excessText.TweenTo(decimal.Zero, excess, duration);
		}

		public void PlaySpriteAnimation()
		{
			_animatedSprite.Reset();
			_animatedSprite.Play();
		}
	}
}
