using System.Collections;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class OpeningBannerGridTileIcon : ButtonGridTileIcon
	{
		[SerializeField]
		private GameObject _background;

		[SerializeField]
		private AnimatedSpriteImage _scissorsAnimation;

		[SerializeField]
		private Tweener _scissorsTweener;

		[SerializeField]
		private ParticleSystem _confettiParticles;

		[SerializeField]
		private GameObject _banner;

		[SerializeField]
		private GameObject _bannerSplit;

		[SerializeField]
		private AnimatedSpriteImage _bannerLeftAnimation;

		[SerializeField]
		private Tweener _bannerSplitTweener;

		[SerializeField]
		private AnimatedSpriteImage _bannerRightAnimation;

		[SerializeField]
		private GameObject _balloonsLeft;

		[SerializeField]
		private ParticleSystem _balloonsLeftParticles;

		[SerializeField]
		private GameObject _balloonsRight;

		[SerializeField]
		private ParticleSystem _balloonsRightParticles;

		[SerializeField]
		private GraphicRaycastTarget _raycastTarget;

		private IEnumerator _grandOpeningRoutine;

		private void OnDisable()
		{
			if (_grandOpeningRoutine != null)
			{
				_grandOpeningRoutine = null;
				OnOverlayRemoved();
			}
		}

		public override void Remove()
		{
			if (base.gameObject.activeInHierarchy)
			{
				if (_grandOpeningRoutine != null)
				{
					StopCoroutine(_grandOpeningRoutine);
				}
				StartCoroutine(_grandOpeningRoutine = GrandOpeningRoutine());
			}
			else
			{
				OnOverlayRemoved();
			}
		}

		private IEnumerator GrandOpeningRoutine()
		{
			_raycastTarget.enabled = false;
			_scissorsAnimation.Play();
			_scissorsTweener.Play();
			yield return new WaitForSeconds((float)_scissorsAnimation.Sprites.Length / _scissorsAnimation.FPS);
			_background.SetActive(value: false);
			_banner.SetActive(value: false);
			_bannerSplit.SetActive(value: true);
			_balloonsLeft.SetActive(value: false);
			_balloonsRight.SetActive(value: false);
			_confettiParticles.Play();
			_bannerLeftAnimation.Play();
			_bannerRightAnimation.Play();
			_bannerSplitTweener.Play();
			_balloonsLeftParticles.Play();
			_balloonsRightParticles.Play();
			yield return new WaitWhile(() => (!_balloonsLeftParticles.isPlaying && !_balloonsRightParticles.isPlaying) ? _bannerSplitTweener.IsPlaying : true);
			OnOverlayRemoved();
		}
	}
}
