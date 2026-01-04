using UnityEngine;

namespace CIG
{
	public class VisitingUpgradableBuildingView : MonoBehaviour
	{
		private UpgradeSign _upgradeSign;

		public void Initialize(StorageDictionary storage, ReadOnlyWrapper parent, UpgradeSign upgradeSignPrefab)
		{
			_upgradeSign = UnityEngine.Object.Instantiate(upgradeSignPrefab, base.transform);
			_upgradeSign.Initialize(parent, visiting: true);
			_upgradeSign.UpdateSign(parent.Mirrored, storage.Get("level", 0), ((BuildingProperties)parent.Properties).MaximumLevel);
		}

		public void UpdateSortingOrder(int sortingOrder)
		{
			_upgradeSign.UpdateSortingOrder(sortingOrder + 1);
		}
	}
}
