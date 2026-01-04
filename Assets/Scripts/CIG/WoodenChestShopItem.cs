using CIG.Translation;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class WoodenChestShopItem : ChestShopItem
	{
		[SerializeField]
		private GameObject _openNowBanner;

		[SerializeField]
		private InteractableButton _openInstantButton;

		[SerializeField]
		private InteractableButton _openWithVideoButton;

		[SerializeField]
		private GameObject _timerGroup;

		[SerializeField]
		private LocalizedText _timerText;

		[SerializeField]
		private Button _frameButton;

		[SerializeField]
		private Image _chestImage;

		private TreasureChestManager _treasureChestManager;

		private InteractableButton _activeOpenButton;

		private IEnumerator _updateRemainingTimeRoutine;

		public override TreasureChestType TreasureChestType => TreasureChestType.Wooden;

		private void OnDestroy()
		{
			if (_treasureChestManager != null)
			{
				_treasureChestManager.ChestOpenableChangedEvent -= OnChestOpenableChanged;
				_treasureChestManager = null;
			}
		}

		public void Initialize(TreasureChestManager treasureChestManager, Action onClick)
		{
			_treasureChestManager = treasureChestManager;
			Initialize(onClick, Localization.Key("rewards.collect"));
			if (treasureChestManager.ShouldWatchVideoToOpenWoodenChest)
			{
				_activeOpenButton = _openWithVideoButton;
				_openInstantButton.gameObject.SetActive(value: false);
			}
			else
			{
				_activeOpenButton = _openInstantButton;
				_openWithVideoButton.gameObject.SetActive(value: false);
			}
			_activeOpenButton.gameObject.SetActive(value: true);
		}

		public override void SetVisible(bool visible)
		{
			base.SetVisible(visible);
			if (visible)
			{
				_treasureChestManager.ChestOpenableChangedEvent -= OnChestOpenableChanged;
				_treasureChestManager.ChestOpenableChangedEvent += OnChestOpenableChanged;
				UpdateVisualState();
				return;
			}
			if (_updateRemainingTimeRoutine != null)
			{
				StopCoroutine(_updateRemainingTimeRoutine);
				_updateRemainingTimeRoutine = null;
			}
			_treasureChestManager.ChestOpenableChangedEvent -= OnChestOpenableChanged;
		}

		private void UpdateVisualState()
		{
			bool flag = _treasureChestManager.CanOpenChest(TreasureChestType.Wooden);
			_openNowBanner.SetActive(flag);
			_activeOpenButton.interactable = flag;
			_frameButton.interactable = flag;
			_chestImage.material = SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(flag ? MaterialType.UIClip : MaterialType.UIClipGreyscale);
			bool flag2 = !_treasureChestManager.WoodenChestTimeExpired;
			_timerGroup.SetActive(flag2);
			if (_updateRemainingTimeRoutine != null)
			{
				StopCoroutine(_updateRemainingTimeRoutine);
				_updateRemainingTimeRoutine = null;
			}
			if (flag2)
			{
				StartCoroutine(_updateRemainingTimeRoutine = UpdateRemainingTimeRoutine());
			}
			else
			{
				_priceLabel.LocalizedString = Localization.Key("rewards.collect");
			}
		}

		private IEnumerator UpdateRemainingTimeRoutine()
		{
			while (true)
			{
				_timerText.LocalizedString = Localization.FullTimeSpan(_treasureChestManager.WoodenChestTimeRemaining, hidePartWhenZero: false);
				yield return new WaitForSecondsRealtime(1f);
			}
		}

		private void OnChestOpenableChanged()
		{
			UpdateVisualState();
		}
	}
}
