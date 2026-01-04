using System;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class FlyingObject : MonoBehaviour
	{
		[SerializeField]
		private Tweener _tweener;

		[SerializeField]
		private TransformPositionXTweenerTrack _transformPositionXTweenerTrack;

		[SerializeField]
		private TransformPositionYTweenerTrack _transformPositionYTweenerTrack;

		private Action _onComplete;

		public Vector2 Size => ((RectTransform)base.transform).sizeDelta;

		private void OnDestroy()
		{
			if (_tweener != null)
			{
				_tweener.FinishedPlaying -= OnTweenerFinishedPlaying;
				_tweener = null;
			}
		}

		public void PlayAnimation(Vector3 start, Vector3 target, Action onComplete)
		{
			_onComplete = onComplete;
			base.transform.position = start;
			_transformPositionXTweenerTrack.SetTarget(target.x);
			_transformPositionYTweenerTrack.SetTarget(target.y);
			_tweener.FinishedPlaying += OnTweenerFinishedPlaying;
			_tweener.Play();
		}

		private void OnTweenerFinishedPlaying(Tweener tweener)
		{
			_tweener.FinishedPlaying -= OnTweenerFinishedPlaying;
			EventTools.Fire(_onComplete);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
