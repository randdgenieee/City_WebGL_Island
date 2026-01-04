using SparkLinq;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class IslandVisitingCurrencySpawner : MonoBehaviour
	{
		private IslandVisitingCurrencyManager _islandVisitingCurrencyManager;

		private IsometricGrid _isometricGrid;

		private IslandId _islandId;

		private string _userId;

		public void Initialize(IslandVisitingCurrencyManager islandVisitingCurrencyManager, IsometricGrid isometricGrid, IslandId islandId, string userId)
		{
			_islandVisitingCurrencyManager = islandVisitingCurrencyManager;
			_isometricGrid = isometricGrid;
			_islandId = islandId;
			_userId = userId;
			SpawnCurrencies();
		}

		private void SpawnCurrencies()
		{
			IslandVisitingCurrencyData currencyData = _islandVisitingCurrencyManager.GetCurrencyData(_userId);
			if (currencyData != null)
			{
				List<ReadOnlyWrapper> list = _isometricGrid.FindAll<ReadOnlyWrapper>();
				HashSet<string> buildingsShowingCurrency = currencyData.GetBuildingsShowingCurrency(_islandId);
				int currencyAmountRemaining = _islandVisitingCurrencyManager.GetCurrencyAmountRemaining(_userId, _islandId);
				currencyAmountRemaining = SpawnCurrencies(from b in list.ToList()
					where buildingsShowingCurrency.Contains(b.UniqueIdentifier)
					select b, currencyAmountRemaining);
				currencyAmountRemaining = SpawnCurrencies((from b in list.ToList()
					where b.Type == ReadOnlyWrapper.BuildingType.Commercial && !b.GridTileIconManager.IsShowingIcon(GridTileIconType.IslandVisitingCollect)
					select b).Shuffle(), currencyAmountRemaining);
				SpawnCurrencies(from b in list.ToList()
					where b.Type != ReadOnlyWrapper.BuildingType.Unknown && b.Type != ReadOnlyWrapper.BuildingType.Scenery && !b.GridTileIconManager.IsShowingIcon(GridTileIconType.IslandVisitingCollect)
					select b, currencyAmountRemaining);
			}
		}

		private int SpawnCurrencies(List<ReadOnlyWrapper> buildingCandidates, int currencyCount)
		{
			int num = currencyCount;
			for (int i = 0; i < currencyCount; i++)
			{
				if (buildingCandidates.Count <= 0)
				{
					break;
				}
				ReadOnlyWrapper building = buildingCandidates.Last();
				buildingCandidates.RemoveAt(buildingCandidates.Count - 1);
				building.GridTileIconManager.SetIcon<IslandVisitingCurrencyGridTileIcon>(GridTileIconType.IslandVisitingCollect).Initialize(delegate
				{
					OnCurrencyBalloonClicked(building);
				}, _islandVisitingCurrencyManager.Currency);
				_islandVisitingCurrencyManager.ShowCurrency(_userId, _islandId, building.UniqueIdentifier);
				num--;
			}
			return num;
		}

		private void OnCurrencyBalloonClicked(ReadOnlyWrapper building)
		{
			_islandVisitingCurrencyManager.CollectCurrency(_userId, _islandId, building.UniqueIdentifier, building);
			building.GridTileIconManager.RemoveIcon(GridTileIconType.IslandVisitingCollect);
		}
	}
}
