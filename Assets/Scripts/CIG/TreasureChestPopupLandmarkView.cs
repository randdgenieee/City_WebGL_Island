using System;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class TreasureChestPopupLandmarkView : MonoBehaviour
	{
		[SerializeField]
		private BuildingImage _buildingImage;

		[SerializeField]
		private LocalizedText _buildingNameText;

		[SerializeField]
		private Tweener _flashTweener;

		[SerializeField]
		private Tweener _showTweener;

		[SerializeField]
		private PopupHeader _popupHeader;

		private Action _onContinueClicked;

		public void Initialize(Timing timing)
		{
			_popupHeader.Initialize(timing);
		}

		public void Initialize(BuildingProperties buildingProperties, Action onContinueClicked)
		{
			_onContinueClicked = onContinueClicked;
			_buildingImage.Initialize(buildingProperties, MaterialType.UIClip, 400f);
			_buildingNameText.LocalizedString = buildingProperties.LocalizedName;
			_flashTweener.StopAndReset();
			_flashTweener.Play();
			_showTweener.StopAndReset();
			_showTweener.Play();
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.TreasureChestItemDrop, playOneAtATime: false, 1f, 0.3f);
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
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
			_onContinueClicked?.Invoke();
		}
	}
}
