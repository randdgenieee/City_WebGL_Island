using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class BuildingPopupIconContainer : MonoBehaviour
	{
		private const float MaxImageWidth = 260f;

		[SerializeField]
		private BuildingImage _buildingImage;

		[SerializeField]
		private LocalizedText _sizeLabel;

		[SerializeField]
		private GameObject _boostedIcon;

		public void Show(Building building)
		{
			Show(building.BuildingProperties);
			_boostedIcon.SetActive(building.ClampedBoostPercentage > 0);
		}

		public void Show(BuildingProperties buildingProperties)
		{
			_buildingImage.Initialize(buildingProperties, MaterialType.UITransparent, 260f);
			_sizeLabel.LocalizedString = Localization.Format(Localization.Literal("{0}x{1}"), Localization.Integer(buildingProperties.Size.u), Localization.Integer(buildingProperties.Size.v));
			_boostedIcon.SetActive(value: false);
		}
	}
}
