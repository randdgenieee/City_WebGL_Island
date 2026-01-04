using CIG.Translation;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class Pling : MonoBehaviour
	{
		[SerializeField]
		private LocalizedText _valueLabel;

		[SerializeField]
		private TextStyleView _valueLabelStyle;

		[SerializeField]
		private Tweener _introTweener;

		[SerializeField]
		private Tweener _moveTweener;

		private ParticleType? _particleType;

		public void Initialize(ILocalizedString value, TextStyleType textStyle = TextStyleType.None, ParticleType? particleType = default(ParticleType?))
		{
			_particleType = particleType;
			_valueLabel.LocalizedString = value;
			_valueLabelStyle.ApplyStyle(textStyle);
		}

		public void Show()
		{
			if (_particleType.HasValue)
			{
				Object.Instantiate(SingletonMonobehaviour<ParticlesAssetCollection>.Instance.GetAsset(_particleType.Value), base.transform.position, Quaternion.identity).Play();
			}
			StartAnimation();
		}

		private void StartAnimation()
		{
			if (_introTweener.IsPlaying)
			{
				_introTweener.FinishedPlaying -= OnOutroFinished;
				_introTweener.Stop();
			}
			_introTweener.Play();
			if (_moveTweener.IsPlaying)
			{
				_moveTweener.FinishedPlaying -= OnMoveFinished;
				_moveTweener.Stop();
			}
			_moveTweener.FinishedPlaying += OnMoveFinished;
			_moveTweener.Play();
		}

		private void OnMoveFinished(Tweener tweener)
		{
			tweener.FinishedPlaying -= OnMoveFinished;
			if (_introTweener.IsPlaying)
			{
				_introTweener.FinishedPlaying -= OnOutroFinished;
				_introTweener.Stop();
			}
			_introTweener.FinishedPlaying += OnOutroFinished;
			_introTweener.PlayReverse();
		}

		private void OnOutroFinished(Tweener tweener)
		{
			tweener.FinishedPlaying -= OnOutroFinished;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
