using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class FlyingCurrency : MonoBehaviour
	{
		[SerializeField]
		private Image _currencyImage;

		[SerializeField]
		private Tweener _tweener;

		[SerializeField]
		private RectTransformPositionXTweenerTrack _rectTransformPositionXTweenerTrack;

		[SerializeField]
		private RectTransformPositionYTweenerTrack _rectTransformPositionYTweenerTrack;

		[Header("Optional")]
		[SerializeField]
		private ParticleSystem _particleSystem;

		[SerializeField]
		private AnimatedSpriteBase _animatedSprite;

		private Currency _currency;

		public Image CurrencyImage => _currencyImage;

		public ICurrencyAnimationTarget Target
		{
			get;
			private set;
		}

		private void Awake()
		{
			_tweener.FinishedPlaying += OnFinishedPlaying;
		}

		private void OnDestroy()
		{
			if (_tweener != null)
			{
				_tweener.FinishedPlaying -= OnFinishedPlaying;
			}
		}

		public void PlayAnimation(Currency currency, Vector3 startPosition, CurrencyAnimationTarget animationTarget)
		{
			_currency = currency;
			Target = animationTarget.Target;
			_rectTransformPositionXTweenerTrack.SetTarget(animationTarget.Destination);
			_rectTransformPositionYTweenerTrack.SetTarget(animationTarget.Destination);
			base.transform.position = startPosition;
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.Woosh);
			_tweener.Play();
			Target.FlyingCurrencyStartedPlaying();
		}

		private void OnFinishedPlaying(Tweener tweener)
		{
			Target.FlyingCurrencyFinishedPlaying(_currency);
			if (_particleSystem != null)
			{
				if (_animatedSprite != null)
				{
					_animatedSprite.enabled = false;
				}
				_currencyImage.enabled = false;
				UnityEngine.Object.Destroy(base.gameObject, _particleSystem.main.duration);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
