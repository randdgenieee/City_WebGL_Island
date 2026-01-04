using CIG.Translation;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class HUDLikesAmount : MonoBehaviour
	{
		[SerializeField]
		private Tweener _likeChangedTweener;

		[SerializeField]
		private LocalizedText _text;

		private int _likes;

		private void Start()
		{
			_likeChangedTweener.FinishedPlaying += OnLikeChangedTweenerFinishedPlaying;
		}

		private void OnDestroy()
		{
			if (_likeChangedTweener != null)
			{
				_likeChangedTweener.FinishedPlaying -= OnLikeChangedTweenerFinishedPlaying;
			}
		}

		public void Initialize(int likes)
		{
			_likes = likes;
			_text.LocalizedString = Localization.Integer(likes);
		}

		public void AddLike()
		{
			_likes++;
			_likeChangedTweener.StopAndReset();
			_likeChangedTweener.Play();
		}

		public void RemoveLike()
		{
			_likes--;
			_likeChangedTweener.StopAndReset();
			_likeChangedTweener.Play();
		}

		private void OnLikeChangedTweenerFinishedPlaying(Tweener tweener)
		{
			if (_likeChangedTweener.IsAtEnd)
			{
				_text.LocalizedString = Localization.Integer(_likes);
				_likeChangedTweener.PlayReverse();
			}
		}
	}
}
