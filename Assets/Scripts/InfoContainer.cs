using CIG;
using System;
using UnityEngine;

public class InfoContainer : MonoBehaviour
{
	[SerializeField]
	private InfoLine _profit;

	[SerializeField]
	private InfoLine _profitXp;

	[SerializeField]
	private InfoLine _people;

	[SerializeField]
	private InfoLine _employees;

	[SerializeField]
	private InfoLine _happiness;

	[SerializeField]
	private InfoLine _timeToProfit;

	[SerializeField]
	private InfoLine _constructionXp;

	[SerializeField]
	private InfoLine _boostPercentage;

	[SerializeField]
	private InfoLine _boostReach;

	[SerializeField]
	private GameObject _bonusContainer;

	[SerializeField]
	private GameObject _boostContainer;

	public void HideAllLines()
	{
		BuildingProperty[] array = (BuildingProperty[])Enum.GetValues(typeof(BuildingProperty));
		foreach (BuildingProperty property in array)
		{
			GetInfoLine(property).SetActive(active: false);
		}
	}

	public void ToggleBoostAndBonus(bool showBonus, bool showBoost)
	{
		_bonusContainer.SetActive(showBonus);
		_boostContainer.SetActive(showBoost);
	}

	public InfoLine GetInfoLine(BuildingProperty property)
	{
		switch (property)
		{
		case BuildingProperty.People:
			return _people;
		case BuildingProperty.Employees:
			return _employees;
		case BuildingProperty.Profit:
			return _profit;
		case BuildingProperty.ProfitXp:
			return _profitXp;
		case BuildingProperty.TimeUntilProfit:
			return _timeToProfit;
		case BuildingProperty.Happiness:
			return _happiness;
		case BuildingProperty.ConstructionXp:
			return _constructionXp;
		case BuildingProperty.BoostPercentage:
			return _boostPercentage;
		case BuildingProperty.BoostReach:
			return _boostReach;
		default:
			return null;
		}
	}
}
