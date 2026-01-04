using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class SurfaceTypeInfoView : MonoBehaviour
	{
		[SerializeField]
		private Image _surfaceTypeImage;

		[SerializeField]
		private LocalizedText _infoText;

		public void Initialize(SurfaceType surfaceType, float amount)
		{
			SurfaceSpriteAssetCollection.SurfaceSprites asset = SingletonMonobehaviour<SurfaceSpriteAssetCollection>.Instance.GetAsset(surfaceType);
			_surfaceTypeImage.sprite = asset.Icon;
			_infoText.LocalizedString = Localization.Format(Localization.Literal("{0} {1}"), Localization.Percentage(amount, 1), surfaceType.ToLocalizedString());
		}
	}
}
