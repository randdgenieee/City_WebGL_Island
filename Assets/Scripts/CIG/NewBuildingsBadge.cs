using CIG.Translation;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class NewBuildingsBadge : MonoBehaviour
	{
		[SerializeField]
		private GameObject _badgeRoot;

		[SerializeField]
		private LocalizedText _badgeLabel;

		private GameState _gameState;

		public void Initialize(GameState gameState)
		{
			_gameState = gameState;
		}

		public void UpdateNewBuildingsBadge(List<BuildingProperties> buildings, List<string> viewedBuildingHistory)
		{
			int num = 0;
			int i = 0;
			for (int count = buildings.Count; i < count; i++)
			{
				BuildingProperties buildingProperties = buildings[i];
				if (!buildingProperties.Activatable && buildingProperties.IsUnlocked(_gameState.Level) && !viewedBuildingHistory.Contains(buildingProperties.BaseKey))
				{
					num++;
				}
			}
			_badgeRoot.SetActive(num > 0);
			_badgeLabel.LocalizedString = Localization.Integer(num);
		}
	}
}
