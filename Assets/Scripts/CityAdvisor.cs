using CIG;
using CIG.Translation;
using SparkLinq;
using System.Collections.Generic;
using UnityEngine;

public sealed class CityAdvisor
{
	public delegate int GetProperty(BuildingProperties building);

	public class AdviceProperties
	{
		public AdviceType type;

		public int deficiency;

		public GetProperty getter;

		public string textKey;

		public List<BuildingProperties> buildings = new List<BuildingProperties>();

		public BuildingProperties advicedBuilding;

		public ILocalizedString text;

		public ILocalizedString title = Localization.EmptyLocalizedString;

		public ILocalizedString okButtonText = Localization.EmptyLocalizedString;

		public ILocalizedString cancelButtonText = Localization.EmptyLocalizedString;

		public Sprite icon;
	}

	private readonly GameStats _gameStats;

	private readonly GameState _gameState;

	private readonly PopupManager _popupManager;

	private readonly Properties _properties;

	private AdviceProperties _advice;

	private IsometricIsland _island;

	private IsometricGrid _grid;

	private CIGExpansions _land;

	public AdviceType AdviceType
	{
		get
		{
			if (_advice != null)
			{
				return _advice.type;
			}
			return AdviceType.ContinueBuilding;
		}
	}

	public CityAdvisor(GameStats gameStats, GameState gameState, PopupManager popupManager, Properties properties)
	{
		_gameStats = gameStats;
		_gameState = gameState;
		_popupManager = popupManager;
		_properties = properties;
	}

	public void Show()
	{
		if (RetrieveComponents())
		{
			ConstructAdvice();
		}
	}

	public void UpdateAdvice()
	{
		RetrieveComponents();
		if (_advice == null)
		{
			_advice = new AdviceProperties();
		}
		DetermineAdviceType();
	}

	public void Build()
	{
		if (_advice.advicedBuilding != null)
		{
			_popupManager.RequestPopup(new BuildingPopupRequest(_advice.advicedBuilding, BuildingPopupContent.Preview));
		}
	}

	public void Cancel()
	{
	}

	private static int GetHappiness(BuildingProperties x)
	{
		return x.HappinessPerLevel.First();
	}

	private static int GetJobs(BuildingProperties x)
	{
		return ((CommercialBuildingProperties)x).EmployeesPerLevel.First();
	}

	private static int GetHousing(BuildingProperties x)
	{
		return ((ResidentialBuildingProperties)x).PeoplePerLevel.First();
	}

	private bool RetrieveComponents()
	{
		_island = IsometricIsland.Current;
		if (_island == null)
		{
			UnityEngine.Debug.LogError("Cannot find IsometricIsland component");
			return false;
		}
		_grid = _island.IsometricGrid;
		_land = _island.Expansions;
		return true;
	}

	private void ConstructAdvice()
	{
		_advice = new AdviceProperties();
		DetermineAdviceType();
		DetermineAdviceText();
		DetermineParameters();
		ChooseBuilding();
		LocalizeAdviceText();
		_advice.title = Localization.Key("city_advisor");
		if (_advice.advicedBuilding != null)
		{
			_advice.okButtonText = Localization.Key("build");
			_advice.cancelButtonText = Localization.Key("perhaps_later");
		}
		else
		{
			_advice.okButtonText = Localization.Key("thank_you");
			_advice.cancelButtonText = null;
		}
		GenericPopupRequest genericPopupRequest = new GenericPopupRequest("city_advisor").SetTexts(_advice.title, _advice.text).SetGreenButton(_advice.okButtonText, null, Build);
		if (_advice.advicedBuilding != null)
		{
			genericPopupRequest.SetIcon(_advice.advicedBuilding);
		}
		else
		{
			genericPopupRequest.SetIcon(_advice.icon);
		}
		if (_advice.cancelButtonText != null)
		{
			genericPopupRequest.SetRedButton(_advice.cancelButtonText, null, Cancel);
		}
		_popupManager.RequestPopup(genericPopupRequest);
	}

	private void LocalizeAdviceText()
	{
		ILocalizedString localizedString = (_advice.advicedBuilding != null) ? _advice.advicedBuilding.LocalizedName : Localization.EmptyLocalizedString;
		ILocalizedString localizedString2 = Localization.Key(_advice.textKey);
		string text = localizedString2.Translate();
		if (text.Contains("{1}"))
		{
			_advice.text = Localization.Format(localizedString2, Localization.Integer(_advice.deficiency), localizedString);
		}
		else if (text.Contains("{0}"))
		{
			_advice.text = Localization.Format(localizedString2, localizedString);
		}
		else
		{
			_advice.text = localizedString2;
		}
	}

	private void ChooseBuilding()
	{
		int level = _gameState.Level;
		List<BuildingProperties> list = new List<BuildingProperties>();
		HashSet<SurfaceType> availableElementTypes = _grid.AvailableElementTypes;
		foreach (BuildingProperties building in _advice.buildings)
		{
			if (!building.Activatable && building.IsUnlocked(level) && availableElementTypes.Contains(building.SurfaceType))
			{
				list.Add(building);
			}
		}
		list.Sort((BuildingProperties x, BuildingProperties y) => _advice.getter(x) - _advice.getter(y));
		foreach (BuildingProperties item in list)
		{
			if (_advice.getter(item) >= _advice.deficiency)
			{
				_advice.advicedBuilding = item;
				break;
			}
		}
		if (_advice.advicedBuilding == null && list.Count > 0)
		{
			_advice.advicedBuilding = list[list.Count - 1];
		}
	}

	private void DetermineParameters()
	{
		switch (_advice.type)
		{
		case AdviceType.BuyLand:
		case AdviceType.ContinueBuilding:
			break;
		case AdviceType.BuildDecorations:
			_advice.buildings.AddRange(_properties.AllCommunityBuildings);
			_advice.buildings.AddRange(_properties.AllDecorationBuildings);
			_advice.getter = GetHappiness;
			break;
		case AdviceType.CreateJobs:
			_advice.buildings.AddRange(_properties.AllCommercialBuildings);
			_advice.getter = GetJobs;
			break;
		case AdviceType.BuildHouses:
		case AdviceType.AttractPeople:
			_advice.buildings.AddRange(_properties.AllResidentialBuildings);
			_advice.getter = GetHousing;
			break;
		default:
			UnityEngine.Debug.LogError("Unknown advice type encountered");
			break;
		}
	}

	private void DetermineAdviceText()
	{
		CIGIslandState islandState = IsometricIsland.Current.IslandState;
		switch (_advice.type)
		{
		case AdviceType.BuildDecorations:
			_advice.textKey = ((islandState.Population == 0) ? "city_advisor_build_decorations" : "city_advisor_not_happy");
			break;
		case AdviceType.BuyLand:
			_advice.textKey = "city_advisor_expand";
			break;
		case AdviceType.CreateJobs:
			_advice.textKey = ((islandState.Jobs == 0) ? "city_advisor_make_money" : CIGUtilities.QuantityKey(_advice.deficiency, "city_advisor_jobs_needed"));
			break;
		case AdviceType.BuildHouses:
			_advice.textKey = ((islandState.Housing == 0) ? "city_advisor_people_needed" : CIGUtilities.QuantityKey(_advice.deficiency, "city_advisor_more_people_needed"));
			break;
		case AdviceType.AttractPeople:
			_advice.textKey = ((islandState.Housing == 0) ? "city_advisor_atttract_people" : "city_advisor_atttract_more_people");
			break;
		case AdviceType.ContinueBuilding:
			_advice.textKey = "city_advisor_balanced";
			break;
		default:
			UnityEngine.Debug.LogError("Unknown advice type encountered");
			break;
		}
	}

	private void DetermineAdviceType()
	{
		float num = (1f - (float)_gameStats.NumberOfUsedElements / (float)_gameStats.NumberOfUnlockedElements) * 100f;
		if ((double)_gameState.GlobalHappiness <= 0.95 * (double)_gameState.GlobalHousing)
		{
			_advice.type = AdviceType.BuildDecorations;
			_advice.deficiency = _gameState.GlobalHousing - _gameState.GlobalHappiness;
		}
		else if (_land.HaveExpansionsLeft && num <= 5f)
		{
			_advice.type = AdviceType.BuyLand;
		}
		else if (_gameState.GlobalJobs < _gameState.GlobalHousing)
		{
			_advice.type = AdviceType.CreateJobs;
			_advice.deficiency = _gameState.GlobalHousing - _gameState.GlobalJobs;
		}
		else if (_gameState.GlobalJobs > _gameState.GlobalHousing)
		{
			_advice.type = AdviceType.BuildHouses;
			_advice.deficiency = _gameState.GlobalJobs - _gameState.GlobalHousing;
		}
		else if (_gameState.GlobalHousing == 0 || _gameState.GlobalHappiness > _gameState.GlobalHousing)
		{
			_advice.type = AdviceType.AttractPeople;
			_advice.deficiency = _gameState.GlobalHappiness - _gameState.GlobalHousing;
		}
		else if (_land.HaveExpansionsLeft && num <= 15f)
		{
			_advice.type = AdviceType.BuyLand;
		}
		else
		{
			_advice.type = AdviceType.ContinueBuilding;
		}
	}
}
