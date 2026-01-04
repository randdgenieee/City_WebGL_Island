using CIG;
using System;
using System.Collections.Generic;

public class BuildingsManager
{
	public delegate void ViewedBuildingHistoryChangedHandler(List<string> viewedBuildingHistory);

	private readonly StorageDictionary _storage;

	private readonly GameState _gameState;

	private readonly GameStats _gameStats;

	private const string ViewedBuildingHistoryListKey = "ViewedBuildingHistoryList";

	public List<string> ViewedBuildingHistory
	{
		get;
	}

	public event ViewedBuildingHistoryChangedHandler ViewedBuildingHistoryChangedEvent;

	private void FireViewedBuildingHistoryChangedEvent()
	{
		this.ViewedBuildingHistoryChangedEvent?.Invoke(ViewedBuildingHistory);
	}

	public BuildingsManager(StorageDictionary storage, GameState gameState, GameStats gameStats)
	{
		_storage = storage;
		_gameState = gameState;
		_gameStats = gameStats;
		ViewedBuildingHistory = _storage.GetList<string>("ViewedBuildingHistoryList");
	}

	public bool AddViewedBuilding(string buildingName)
	{
		if (!ViewedBuildingHistory.Contains(buildingName))
		{
			ViewedBuildingHistory.Add(buildingName);
			FireViewedBuildingHistoryChangedEvent();
			return true;
		}
		return false;
	}

	public void BuyBuilding(BuildingProperties buildingProperties, Currency cost, decimal extraGoldCost, bool instant, Action<bool> callback)
	{
		Currencies c = new Currencies(cost, Currency.GoldCurrency(extraGoldCost));
		CurrenciesSpentReason spentReason = (!buildingProperties.IsGoldBuilding) ? (instant ? CurrenciesSpentReason.CashBuildingInstant : CurrenciesSpentReason.CashBuilding) : (instant ? CurrenciesSpentReason.GoldBuildingInstant : CurrenciesSpentReason.GoldBuilding);
		_gameState.SpendCurrencies(c, buildingProperties.GoldCostAtMax, allowShopPopup: true, spentReason, delegate(bool success, Currencies spent)
		{
			EventTools.Fire(callback, success);
			if (success)
			{
				_gameStats.AddBuildingPurchased(buildingProperties, spent);
			}
		});
	}

	public void Serialize()
	{
		_storage.Set("ViewedBuildingHistoryList", ViewedBuildingHistory);
	}
}
