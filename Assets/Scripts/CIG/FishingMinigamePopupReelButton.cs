using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CIG
{
	public class FishingMinigamePopupReelButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		[SerializeField]
		private Image _buttonImage;

		[SerializeField]
		private Image _buttonIcon;

		private FishingMinigame _fishingMinigame;

		private Material _normalMaterial;

		private Material _disabledMaterial;

		public void Initialize(FishingMinigame fishingMinigame)
		{
			_normalMaterial = SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(MaterialType.UITransparent);
			_disabledMaterial = SingletonMonobehaviour<MaterialAssetCollection>.Instance.GetAsset(MaterialType.UITransparentGreyscale);
			_fishingMinigame = fishingMinigame;
			_fishingMinigame.CanCatchChangedEvent += OnCanCatchChanged;
			OnCanCatchChanged(_fishingMinigame.CanCatch);
		}

		public void Deinitialize()
		{
			if (_fishingMinigame != null)
			{
				_fishingMinigame.CanCatchChangedEvent -= OnCanCatchChanged;
				_fishingMinigame = null;
			}
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (_fishingMinigame.CanCatch)
			{
				_fishingMinigame.CatchFish();
			}
		}

		private void OnCanCatchChanged(bool canCatch)
		{
			if (canCatch)
			{
				_buttonImage.material = _normalMaterial;
				_buttonIcon.material = _normalMaterial;
			}
			else
			{
				_buttonImage.material = _disabledMaterial;
				_buttonIcon.material = _disabledMaterial;
			}
		}
	}
}
