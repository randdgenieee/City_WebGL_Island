using UnityEngine;

namespace CIG
{
	public class BoostUnderlay : MonoBehaviour
	{
		public void SetData(int widthInTiles, int buildingHeightInTiles)
		{
			base.transform.localScale = Vector3.one * widthInTiles;
			base.transform.localPosition = Vector3.up * ((float)buildingHeightInTiles / 2f) * IsometricGrid.ElementSize.y;
		}
	}
}
