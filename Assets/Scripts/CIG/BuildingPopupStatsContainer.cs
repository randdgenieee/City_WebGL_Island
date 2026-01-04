using CIG.Translation;
using SparkLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class BuildingPopupStatsContainer : MonoBehaviour
	{
		[SerializeField]
		private InfoContainer _infoContainer;

		[SerializeField]
		private GameObject _cashButton;

		[SerializeField]
		private Image _cashButtonImage;

		[SerializeField]
		private LocalizedText _cashButtonCostLabel;

		[SerializeField]
		private LocalizedText _cashDurationLabel;

		[SerializeField]
		private GameObject _silverKeyRewardRoot;

		[SerializeField]
		private LocalizedText _silverKeyRewardLabel;

		[SerializeField]
		private GameObject _instantButton;

		[SerializeField]
		private LocalizedText _instantButtonCostLabel;

		[SerializeField]
		private GameObject _goldKeyRewardRoot;

		[SerializeField]
		private LocalizedText _goldKeyRewardLabel;

		[SerializeField]
		private LocalizedText _bottomText;

		private Timing _timing;

		private GameState _gameState;

		private GameStats _gameStats;

		private BuildingWarehouseManager _buildingWarehouseManager;

		private Multipliers _multipliers;

		private CIGBuilding _building;

		private BuildingProperties _buildingProperties;

		private BuildingPopupContent _content;

		private bool _showExtras;

		private IEnumerator _updateInfoRoutine;

		private ILocalizedString ConstructionXpValue => Localization.Integer(_buildingProperties.ConstructionReward.Value);

		private ILocalizedString ConstructionXpLevelUp
		{
			get
			{
				UnityEngine.Debug.LogWarning("Construction XP is only shown in the shop menu");
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString EmployeesValue
		{
			get
			{
				CIGCommercialBuilding cIGCommercialBuilding = _building as CIGCommercialBuilding;
				if (cIGCommercialBuilding != null && _content == BuildingPopupContent.Upgrade)
				{
					return Localization.Format("{0}/{1}", Localization.Integer(cIGCommercialBuilding.Employees), Localization.Integer(cIGCommercialBuilding.MaxEmployees));
				}
				CommercialBuildingProperties commercialBuildingProperties;
				if ((commercialBuildingProperties = (_buildingProperties as CommercialBuildingProperties)) != null)
				{
					return Localization.Integer(commercialBuildingProperties.EmployeesPerLevel.First());
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString EmployeesLevelUp
		{
			get
			{
				CIGCommercialBuilding cIGCommercialBuilding = _building as CIGCommercialBuilding;
				if (cIGCommercialBuilding != null)
				{
					int extraEmployeesNextLevel = cIGCommercialBuilding.ExtraEmployeesNextLevel;
					if (extraEmployeesNextLevel > 0)
					{
						return Localization.Format(Localization.Key("plus"), Localization.Integer(extraEmployeesNextLevel));
					}
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString HappinessValue
		{
			get
			{
				if (_content == BuildingPopupContent.Preview || (_content == BuildingPopupContent.Activate && !_building.Activated))
				{
					return Localization.Integer(_buildingProperties.HappinessPerLevel.First());
				}
				return Localization.Integer(_building.Happiness);
			}
		}

		private ILocalizedString HappinessLevelUp
		{
			get
			{
				CIGCommunityBuilding cIGCommunityBuilding = _building as CIGCommunityBuilding;
				if (cIGCommunityBuilding != null)
				{
					int extraHappinessNextLevel = cIGCommunityBuilding.ExtraHappinessNextLevel;
					if (extraHappinessNextLevel <= 0)
					{
						return Localization.EmptyLocalizedString;
					}
					return Localization.Format(Localization.Key("plus"), Localization.Integer(extraHappinessNextLevel));
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString PeopleValue
		{
			get
			{
				CIGResidentialBuilding cIGResidentialBuilding = _building as CIGResidentialBuilding;
				if (cIGResidentialBuilding != null && _content == BuildingPopupContent.Upgrade)
				{
					return Localization.Format("{0}/{1}", Localization.Integer(cIGResidentialBuilding.People), Localization.Integer(cIGResidentialBuilding.MaxPeople));
				}
				ResidentialBuildingProperties residentialBuildingProperties;
				if ((residentialBuildingProperties = (_buildingProperties as ResidentialBuildingProperties)) != null)
				{
					return Localization.Integer(residentialBuildingProperties.PeoplePerLevel.First());
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString PeopleLevelUp
		{
			get
			{
				CIGResidentialBuilding cIGResidentialBuilding = _building as CIGResidentialBuilding;
				if (cIGResidentialBuilding != null)
				{
					int extraPeopleNextLevel = cIGResidentialBuilding.ExtraPeopleNextLevel;
					if (extraPeopleNextLevel > 0)
					{
						return Localization.Format(Localization.Key("plus"), Localization.Integer(extraPeopleNextLevel));
					}
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString ProfitValue
		{
			get
			{
				decimal m = default(decimal);
				CIGCommercialBuilding cIGCommercialBuilding = _building as CIGCommercialBuilding;
				CommercialBuildingProperties commercialBuildingProperties;
				if ((commercialBuildingProperties = (_buildingProperties as CommercialBuildingProperties)) != null)
				{
					m = ((!(cIGCommercialBuilding != null)) ? commercialBuildingProperties.ProfitPerLevel.First().GetValue("Cash") : ((_content == BuildingPopupContent.Activate && !_building.Activated) ? commercialBuildingProperties.ProfitPerLevel.First().GetValue("Cash") : ((_content != 0) ? cIGCommercialBuilding.Profit.GetValue("Cash") : cIGCommercialBuilding.CurrentProfit.GetValue("Cash"))));
				}
				return Localization.Integer(m);
			}
		}

		private ILocalizedString ProfitLevelUp
		{
			get
			{
				CIGCommercialBuilding cIGCommercialBuilding = _building as CIGCommercialBuilding;
				if (cIGCommercialBuilding != null)
				{
					int num = (int)cIGCommercialBuilding.ExtraProfitNextLevel.GetValue("Cash");
					if (num > 0)
					{
						return Localization.Format(Localization.Key("plus"), Localization.Integer(num));
					}
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString ProfitXpValue
		{
			get
			{
				decimal m = default(decimal);
				CIGCommercialBuilding cIGCommercialBuilding = _building as CIGCommercialBuilding;
				CommercialBuildingProperties commercialBuildingProperties;
				if (cIGCommercialBuilding != null && (commercialBuildingProperties = (_buildingProperties as CommercialBuildingProperties)) != null)
				{
					m = ((_content == BuildingPopupContent.Preview || (_content == BuildingPopupContent.Activate && !_building.Activated)) ? commercialBuildingProperties.ProfitPerLevel.First().GetValue("XP") : ((_content != 0) ? cIGCommercialBuilding.Profit.GetValue("XP") : cIGCommercialBuilding.CurrentProfit.GetValue("XP")));
				}
				return Localization.Integer(m);
			}
		}

		private ILocalizedString ProfitXpLevelUp
		{
			get
			{
				CIGCommercialBuilding cIGCommercialBuilding = _building as CIGCommercialBuilding;
				if (cIGCommercialBuilding != null)
				{
					decimal value = cIGCommercialBuilding.ExtraProfitNextLevel.GetValue("XP");
					if (value > decimal.Zero)
					{
						return Localization.Format(Localization.Key("plus"), Localization.Integer(value));
					}
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString TimeUntilProfitValue
		{
			get
			{
				CIGCommercialBuilding cIGCommercialBuilding = _building as CIGCommercialBuilding;
				CommercialBuildingProperties commercialBuildingProperties;
				if ((commercialBuildingProperties = (_buildingProperties as CommercialBuildingProperties)) != null)
				{
					if (cIGCommercialBuilding != null)
					{
						if (_content == BuildingPopupContent.Activate && !_building.Activated)
						{
							return Localization.TimeSpan(TimeSpan.FromSeconds(commercialBuildingProperties.ProfitDurationSeconds), hideSecondPartWhenZero: false);
						}
						return cIGCommercialBuilding.TimeLeftString();
					}
					return Localization.TimeSpan(TimeSpan.FromSeconds(commercialBuildingProperties.ProfitDurationSeconds), hideSecondPartWhenZero: false);
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString BoostPercentage
		{
			get
			{
				CIGLandmarkBuilding cIGLandmarkBuilding = _building as CIGLandmarkBuilding;
				if (cIGLandmarkBuilding != null)
				{
					return Localization.Percentage(cIGLandmarkBuilding.BoostPercentage);
				}
				LandmarkBuildingProperties landmarkBuildingProperties;
				if ((landmarkBuildingProperties = (_buildingProperties as LandmarkBuildingProperties)) != null)
				{
					return Localization.Percentage(landmarkBuildingProperties.BoostPercentagePerLevel.First());
				}
				return Localization.Percentage(_building.ClampedBoostPercentage);
			}
		}

		private ILocalizedString BoostPercentageLevelUp
		{
			get
			{
				CIGLandmarkBuilding cIGLandmarkBuilding = _building as CIGLandmarkBuilding;
				if (cIGLandmarkBuilding != null)
				{
					int extraBoostPercentageNextLevel = cIGLandmarkBuilding.ExtraBoostPercentageNextLevel;
					if (extraBoostPercentageNextLevel > 0)
					{
						return Localization.Format(Localization.Key("plus"), Localization.Integer(extraBoostPercentageNextLevel));
					}
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString BoostReach
		{
			get
			{
				CIGLandmarkBuilding cIGLandmarkBuilding = _building as CIGLandmarkBuilding;
				if (cIGLandmarkBuilding != null)
				{
					return Localization.Integer(cIGLandmarkBuilding.BoostTiles);
				}
				LandmarkBuildingProperties landmarkBuildingProperties;
				if ((landmarkBuildingProperties = (_buildingProperties as LandmarkBuildingProperties)) != null)
				{
					return Localization.Integer(landmarkBuildingProperties.BoostTilesPerLevel.First());
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString BoostReachLevelUp
		{
			get
			{
				CIGLandmarkBuilding cIGLandmarkBuilding = _building as CIGLandmarkBuilding;
				if (cIGLandmarkBuilding != null)
				{
					int extraBoostTilesNextLevel = cIGLandmarkBuilding.ExtraBoostTilesNextLevel;
					if (extraBoostTilesNextLevel > 0)
					{
						return Localization.Format(Localization.Key("plus"), Localization.Integer(extraBoostTilesNextLevel));
					}
				}
				return Localization.EmptyLocalizedString;
			}
		}

		private ILocalizedString TimeUntilProfitLevelUp => Localization.EmptyLocalizedString;

		public void Initialize(Timing timing, GameState gameState, GameStats gameStats, BuildingWarehouseManager buildingWarehouseManager, Multipliers multipliers)
		{
			_timing = timing;
			_gameState = gameState;
			_gameStats = gameStats;
			_buildingWarehouseManager = buildingWarehouseManager;
			_multipliers = multipliers;
		}

		public void Show(CIGBuilding building)
		{
			_building = building;
			Show(_building.BuildingProperties);
		}

		public void Show(BuildingProperties buildingProperties)
		{
			base.gameObject.SetActive(value: true);
			_buildingProperties = buildingProperties;
		}

		public void ShowActivateContent(CIGBuilding building)
		{
			Show(building);
			_content = BuildingPopupContent.Activate;
			_showExtras = false;
			SetButtonCosts(_building.UpgradeCost, _building.UpgradeDuration, _building.UpgradeInstantCost, Currency.Invalid, Currency.Invalid);
			SetBottomText(null);
			FillProperties();
			StartUpdateRoutine();
		}

		public void ShowUpgradeContent(CIGBuilding building, bool isMaxLevel)
		{
			Show(building);
			_content = BuildingPopupContent.Upgrade;
			_showExtras = true;
			if (isMaxLevel)
			{
				SetBottomText(Localization.Key("max_level_reached"));
				HideUpgradeButtons();
			}
			else
			{
				SetBottomText(null);
				SetButtonCosts(_building.UpgradeCost, _building.UpgradeDuration, _building.UpgradeInstantCost, _building.UpgradeCashReward, _building.UpgradeGoldReward);
			}
			FillProperties();
			StartUpdateRoutine();
		}

		public void ShowPreviewContent(BuildingProperties buildingProperties)
		{
			Show(buildingProperties);
			_content = BuildingPopupContent.Preview;
			_showExtras = false;
			SetButtonCosts(_buildingProperties.GetConstructionCost(_gameState, _gameStats, _buildingWarehouseManager), _buildingProperties.ConstructionDurationSeconds, _buildingProperties.GetInstantConstructionCost(_gameState, _gameStats, _buildingWarehouseManager, _multipliers), Currency.Invalid, Currency.Invalid);
			SetBottomText(null);
			FillProperties();
			StartUpdateRoutine();
		}

		public void ShowLandmarkPreviewContent(BuildingProperties buildingProperties)
		{
			Show(buildingProperties);
			_content = BuildingPopupContent.LandmarkPreview;
			_showExtras = false;
			HideUpgradeButtons();
			SetBottomText(null);
			FillProperties();
			StopUpdateRoutine();
		}

		public void Hide()
		{
			StopUpdateRoutine();
			_building = null;
			base.gameObject.SetActive(value: false);
		}

		private void StartUpdateRoutine()
		{
			StopUpdateRoutine();
			StartCoroutine(_updateInfoRoutine = UpdateBuildingInfoRoutine());
		}

		private void StopUpdateRoutine()
		{
			if (_updateInfoRoutine != null)
			{
				StopCoroutine(_updateInfoRoutine);
				_updateInfoRoutine = null;
			}
		}

		private void SetButtonCosts(Currency cost, int buildDuration, Currency instantCost, Currency silverKeyReward, Currency goldKeyReward)
		{
			_cashButton.SetActive(value: true);
			bool flag = !cost.IsValid || cost.Value == decimal.Zero;
			_cashDurationLabel.LocalizedString = Localization.TimeSpan(TimeSpan.FromSeconds(buildDuration), hideSecondPartWhenZero: true);
			_cashButtonImage.gameObject.SetActive(!flag);
			if (flag)
			{
				_cashButtonCostLabel.LocalizedString = Localization.Key("free");
			}
			else
			{
				_cashButtonCostLabel.LocalizedString = Localization.Integer(cost.Value);
				_cashButtonImage.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(cost);
			}
			bool flag2 = instantCost.IsValid && instantCost.Value > decimal.Zero;
			_instantButton.SetActive(flag2);
			if (flag2)
			{
				_instantButtonCostLabel.LocalizedString = Localization.Integer(instantCost.Value);
			}
			bool flag3 = silverKeyReward.IsValid && silverKeyReward.Value > decimal.Zero;
			_silverKeyRewardRoot.SetActive(flag3);
			if (flag3)
			{
				_silverKeyRewardLabel.LocalizedString = Localization.Format(Localization.Key("plus"), Localization.Integer(silverKeyReward.Value));
			}
			bool flag4 = goldKeyReward.IsValid && goldKeyReward.Value > decimal.Zero;
			_goldKeyRewardRoot.SetActive(flag4);
			if (flag4)
			{
				_goldKeyRewardLabel.LocalizedString = Localization.Format(Localization.Key("plus"), Localization.Integer(goldKeyReward.Value));
			}
		}

		private void HideUpgradeButtons()
		{
			_cashButton.SetActive(value: false);
			_instantButton.SetActive(value: false);
		}

		private void FillProperties()
		{
			_infoContainer.HideAllLines();
			_infoContainer.ToggleBoostAndBonus(_showExtras, _showExtras);
			List<BuildingProperty> shownProperties = _buildingProperties.GetShownProperties(_building == null);
			if (_building != null && _building.ClampedBoostPercentage > 0)
			{
				shownProperties.Add(BuildingProperty.BoostPercentage);
			}
			foreach (BuildingProperty item in shownProperties)
			{
				ILocalizedString value = GetValue(item);
				ILocalizedString bonus = null;
				ILocalizedString boost = null;
				if (_showExtras)
				{
					bonus = GetLevelUp(item);
					boost = GetBoostForProperty(item);
				}
				InfoLine infoLine = _infoContainer.GetInfoLine(item);
				infoLine.SetActive(active: true);
				infoLine.SetValueBonusAndBoost(value, bonus, boost);
			}
		}

		private ILocalizedString GetBoostForProperty(BuildingProperty property)
		{
			int num = 0;
			switch (property)
			{
			case BuildingProperty.Happiness:
				num = CalculateBoostFromValue(_building.UnboostedHappiness, _building.Happiness);
				break;
			case BuildingProperty.Profit:
			{
				CIGCommercialBuilding cIGCommercialBuilding2;
				if ((object)(cIGCommercialBuilding2 = (_building as CIGCommercialBuilding)) != null)
				{
					num = CalculateBoostFromValue(cIGCommercialBuilding2.UnboostedCurrentProfit.GetValue("Cash"), cIGCommercialBuilding2.CurrentProfit.GetValue("Cash"));
				}
				break;
			}
			case BuildingProperty.ProfitXp:
			{
				CIGCommercialBuilding cIGCommercialBuilding3;
				if ((object)(cIGCommercialBuilding3 = (_building as CIGCommercialBuilding)) != null)
				{
					num = CalculateBoostFromValue(cIGCommercialBuilding3.UnboostedCurrentProfit.GetValue("XP"), cIGCommercialBuilding3.CurrentProfit.GetValue("XP"));
				}
				break;
			}
			case BuildingProperty.People:
			{
				CIGResidentialBuilding cIGResidentialBuilding = _building as CIGResidentialBuilding;
				if (cIGResidentialBuilding != null)
				{
					num = CalculateBoostFromValue(cIGResidentialBuilding.UnboostedMaxPeople, cIGResidentialBuilding.MaxPeople);
				}
				break;
			}
			case BuildingProperty.Employees:
			{
				CIGCommercialBuilding cIGCommercialBuilding;
				if ((object)(cIGCommercialBuilding = (_building as CIGCommercialBuilding)) != null)
				{
					num = CalculateBoostFromValue(cIGCommercialBuilding.UnboostedMaxEmployees, cIGCommercialBuilding.MaxEmployees);
				}
				break;
			}
			}
			if (num == 0)
			{
				return Localization.EmptyLocalizedString;
			}
			return Localization.Format("({0})", Localization.Format(Localization.Key("plus"), Localization.Integer(num)));
		}

		private int CalculateBoostFromValue(int baseValue, int maxValue)
		{
			return maxValue - baseValue;
		}

		private int CalculateBoostFromValue(decimal baseValue, decimal maxValue)
		{
			return Mathf.RoundToInt((float)(maxValue - baseValue));
		}

		private ILocalizedString GetValue(BuildingProperty property)
		{
			switch (property)
			{
			case BuildingProperty.ConstructionXp:
				return ConstructionXpValue;
			case BuildingProperty.Employees:
				return EmployeesValue;
			case BuildingProperty.Happiness:
				return HappinessValue;
			case BuildingProperty.People:
				return PeopleValue;
			case BuildingProperty.Profit:
				return ProfitValue;
			case BuildingProperty.ProfitXp:
				return ProfitXpValue;
			case BuildingProperty.TimeUntilProfit:
				return TimeUntilProfitValue;
			case BuildingProperty.BoostPercentage:
				return BoostPercentage;
			case BuildingProperty.BoostReach:
				return BoostReach;
			default:
				return Localization.Literal("ERROR");
			}
		}

		private ILocalizedString GetLevelUp(BuildingProperty property)
		{
			switch (property)
			{
			case BuildingProperty.ConstructionXp:
				return ConstructionXpLevelUp;
			case BuildingProperty.Employees:
				return EmployeesLevelUp;
			case BuildingProperty.Happiness:
				return HappinessLevelUp;
			case BuildingProperty.People:
				return PeopleLevelUp;
			case BuildingProperty.Profit:
				return ProfitLevelUp;
			case BuildingProperty.ProfitXp:
				return ProfitXpLevelUp;
			case BuildingProperty.TimeUntilProfit:
				return TimeUntilProfitLevelUp;
			case BuildingProperty.BoostPercentage:
				return BoostPercentageLevelUp;
			case BuildingProperty.BoostReach:
				return BoostReachLevelUp;
			default:
				return Localization.Literal("ERROR");
			}
		}

		private void SetBottomText(ILocalizedString text)
		{
			_bottomText.gameObject.SetActive(!Localization.IsNullOrEmpty(text));
			_bottomText.LocalizedString = text;
		}

		private IEnumerator UpdateBuildingInfoRoutine()
		{
			while (_building != null)
			{
				FillProperties();
				bool infoRequiresFrequentRefresh = _building.InfoRequiresFrequentRefresh;
				yield return new WaitForGameTimeSeconds(_timing, infoRequiresFrequentRefresh ? 1.0 : 60.0);
			}
		}
	}
}
