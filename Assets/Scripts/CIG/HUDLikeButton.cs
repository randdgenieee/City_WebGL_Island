using CIG.Translation;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class HUDLikeButton : MonoBehaviour
	{
		[SerializeField]
		private InteractableButton _button;

		[SerializeField]
		private LocalizedText _badgeText;

		[SerializeField]
		private Tweener _checkmarkTweener;

		private string _userId;

		private LikeRegistrar _likeRegistrar;

		private HUDLikesAmount _likesAmount;

		private bool _waitingForResponse;

		public void Initialize(string userId, LikeRegistrar likeRegistrar, HUDLikesAmount likesAmount)
		{
			_userId = userId;
			_likeRegistrar = likeRegistrar;
			_likesAmount = likesAmount;
			_checkmarkTweener.StopAndReset();
			UpdateVisualState();
		}

		public void OnButtonClicked()
		{
			if (!_waitingForResponse)
			{
				_waitingForResponse = true;
				UpdateVisualState();
				if (_likeRegistrar.HasLikedUser(_userId))
				{
					_likeRegistrar.UserRemovedLike(_userId, OnLikeRemoved, OnLikeChangeFailed);
				}
				else
				{
					_likeRegistrar.UserAddedLike(_userId, OnLikeAdded, OnLikeChangeFailed);
				}
			}
		}

		private void OnLikeAdded()
		{
			_waitingForResponse = false;
			_likesAmount.AddLike();
			UpdateVisualState();
		}

		private void OnLikeRemoved()
		{
			_waitingForResponse = false;
			_likesAmount.RemoveLike();
			UpdateVisualState();
		}

		private void OnLikeChangeFailed()
		{
			_waitingForResponse = false;
			UpdateVisualState();
		}

		private void UpdateVisualState()
		{
			bool flag = _likeRegistrar.HasLikedUser(_userId);
			if (!_waitingForResponse)
			{
				if (flag && _checkmarkTweener.IsAtStart)
				{
					_checkmarkTweener.Play();
				}
				else if (!flag && _checkmarkTweener.IsAtEnd)
				{
					_checkmarkTweener.PlayReverse();
				}
			}
			else
			{
				_checkmarkTweener.StopAndReset(flag);
			}
			_button.interactable = (_likeRegistrar.AvailableLikes > 0);
			_badgeText.LocalizedString = Localization.Integer(_likeRegistrar.AvailableLikes);
		}
	}
}
