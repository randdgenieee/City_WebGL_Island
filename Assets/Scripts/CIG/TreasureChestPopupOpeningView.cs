using Tweening;
using UnityEngine;

namespace CIG
{
	public class TreasureChestPopupOpeningView : MonoBehaviour
	{
		[SerializeField]
		private TreasureChestSingleRewardItem _singleRewardItemPrefab;

		[SerializeField]
		private Transform _singleItemContainer;

		[SerializeField]
		private GameObject _continueButton;

		[SerializeField]
		private Tweener _openingGroupTweener;

		[SerializeField]
		private Tweener _tapToOpenTextTweener;

		[SerializeField]
		private PopupHeader _popupHeader;

		private TreasureChestView _treasureChestView;

		private TreasureChestSingleRewardItem _singleItem;

		public void Initialize(Timing timing)
		{
			_popupHeader.Initialize(timing);
		}

		public void Initialize(TreasureChestView treasureChestView)
		{
			_treasureChestView = treasureChestView;
			_openingGroupTweener.StopAndReset();
			_continueButton.SetActive(value: false);
			_tapToOpenTextTweener.gameObject.SetActive(value: true);
			_treasureChestView.TreasureChestTappedEvent += OnTreasureChestTapped;
			_treasureChestView.TreasureChestOpenedEvent += OnTreasureChestOpened;
			_treasureChestView.TreasureChestRewardFlyStartEvent += OnTreasureChestRewardFlyStarted;
			_treasureChestView.TreasureChestClosedEvent += OnTreasureChestClosed;
		}

		public void Deinitialize()
		{
			if (_singleItem != null)
			{
				UnityEngine.Object.Destroy(_singleItem.gameObject);
				_singleItem = null;
			}
			if (_treasureChestView != null)
			{
				_treasureChestView.TreasureChestTappedEvent -= OnTreasureChestTapped;
				_treasureChestView.TreasureChestOpenedEvent -= OnTreasureChestOpened;
				_treasureChestView.TreasureChestRewardFlyStartEvent -= OnTreasureChestRewardFlyStarted;
				_treasureChestView.TreasureChestClosedEvent -= OnTreasureChestClosed;
			}
		}

		private void OnDestroy()
		{
			Deinitialize();
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
			if (active)
			{
				_popupHeader.Play();
			}
			else
			{
				_popupHeader.Stop();
			}
		}

		public void OnContinueClicked()
		{
			VanishSingleItem();
			_treasureChestView.Leave();
			_continueButton.SetActive(value: false);
		}

		private void VanishSingleItem()
		{
			if (_singleItem != null)
			{
				_singleItem.Vanish();
				_singleItem = null;
			}
		}

		private void OnTreasureChestOpened()
		{
			_tapToOpenTextTweener.gameObject.SetActive(value: false);
		}

		private void OnTreasureChestTapped(bool afterCooldown, bool finalTap)
		{
			if (afterCooldown)
			{
				VanishSingleItem();
			}
			_tapToOpenTextTweener.StopAndReset();
			_tapToOpenTextTweener.Play();
			if (finalTap)
			{
				OnContinueClicked();
			}
		}

		private void OnTreasureChestRewardFlyStarted(RewardItemData itemData)
		{
			_singleItem = UnityEngine.Object.Instantiate(_singleRewardItemPrefab, _singleItemContainer);
			_singleItem.Initialize(itemData);
		}

		private void OnTreasureChestClosed(bool landmark)
		{
			if (landmark)
			{
				_openingGroupTweener.PlayIfStopped();
			}
			else
			{
				_continueButton.SetActive(value: true);
			}
			_tapToOpenTextTweener.gameObject.SetActive(value: false);
		}
	}
}
