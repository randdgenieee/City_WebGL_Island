using CIG;
using UnityEngine;

public sealed class TutorialManager
{
	public delegate void TutorialStartedEventHandler(Tutorial tutorial);

	public delegate void TutorialStoppedEventHandler(Tutorial tutorial);

	public delegate void TutorialFinishedEventHandler(Tutorial tutorial);

	public const TutorialType FinalInitialTutorial = TutorialType.BuildCommunity;

	private readonly StorageDictionary _storage;

	private readonly PopupManager _popupManager;

	private Tutorial[] _tutorials;

	private TutorialTrigger[] _triggers;

	private const string ExpansionWarehouseTutorialsEnabledKey = "ExpansionWarehouseTutorialsEnabled";

	public bool InitialTutorialFinished => IsTutorialFinished(TutorialType.BuildCommunity);

	public bool AllTutorialsFinished
	{
		get
		{
			int i = 0;
			for (int num = _tutorials.Length; i < num; i++)
			{
				if (!_tutorials[i].IsCompleted)
				{
					return false;
				}
			}
			return true;
		}
	}

	public Tutorial ActiveTutorial
	{
		get;
		private set;
	}

	public event TutorialStartedEventHandler TutorialStartedEvent;

	public event TutorialStoppedEventHandler TutorialStoppedEvent;

	public event TutorialFinishedEventHandler TutorialFinishedEvent;

	private void FireTutorialStartedEvent(Tutorial tutorial)
	{
		this.TutorialStartedEvent?.Invoke(tutorial);
	}

	private void FireTutorialStoppedEvent(Tutorial tutorial)
	{
		this.TutorialStoppedEvent?.Invoke(tutorial);
	}

	private void FireTutorialFinishedEvent(Tutorial tutorial)
	{
		this.TutorialFinishedEvent?.Invoke(tutorial);
	}

	public TutorialManager(StorageDictionary storage, GameState gameState, GameStats gameStats, WorldMap worldMap, PopupManager popupManager, IslandsManager islandManager, CraneManager craneManager, BuildingWarehouseManager warehouseManager, TreasureChestManager treasureChestManager, CurrencyConversionManager currencyConversionManager, FishingEvent fishingEvent)
	{
		_storage = storage;
		_popupManager = popupManager;
		if (!_storage.Contains("ExpansionWarehouseTutorialsEnabled"))
		{
			bool num = gameStats.NumberOfBuildings < 5;
			Analytics.EligibleForExpansionWarehouseTutorial(num);
			if (num)
			{
				bool flag = Random.value > 0.5f;
				_storage.Set("ExpansionWarehouseTutorialsEnabled", flag);
				Analytics.ExpansionWarehouseTutorialGroupSet(flag ? "A" : "B");
			}
			else
			{
				_storage.Set("ExpansionWarehouseTutorialsEnabled", value: false);
			}
		}
		bool expansionWarehouseTutorialsEnabled = _storage.Get("ExpansionWarehouseTutorialsEnabled", defaultValue: false);
		InitializeTutorials(gameState, gameStats, worldMap, islandManager, craneManager, warehouseManager, treasureChestManager, currencyConversionManager, fishingEvent, expansionWarehouseTutorialsEnabled);
		ToggleTriggers(!AllTutorialsFinished);
		TryStartTutorial();
		gameStats.SpeedupExecutedEvent += OnSpeedupExecuted;
	}

	public void TryStartTutorial()
	{
		if (ActiveTutorial != null)
		{
			return;
		}
		int num = 0;
		int num2 = _tutorials.Length;
		Tutorial tutorial;
		while (true)
		{
			if (num < num2)
			{
				tutorial = _tutorials[num];
				if (tutorial.CanBegin)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		StartTutorial(tutorial);
	}

	public void TryStartTutorial(TutorialType tutorialType)
	{
		if (ActiveTutorial != null)
		{
			return;
		}
		int num = 0;
		int num2 = _tutorials.Length;
		Tutorial tutorial;
		while (true)
		{
			if (num < num2)
			{
				tutorial = _tutorials[num];
				if (tutorial.TutorialType == tutorialType && tutorial.CanBegin)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		StartTutorial(tutorial);
	}

	public void StopTutorial()
	{
		Tutorial activeTutorial = ActiveTutorial;
		ActiveTutorial = null;
		ToggleTriggers(!AllTutorialsFinished);
		if (activeTutorial != null)
		{
			FireTutorialStoppedEvent(activeTutorial);
		}
	}

	public void FinishTutorial(Tutorial tutorial)
	{
		Analytics.TutorialFinished(tutorial.TutorialType);
		StopTutorial();
		FireTutorialFinishedEvent(tutorial);
		if (!AllTutorialsFinished)
		{
			TryStartTutorial();
		}
	}

	public void QuitTutorial()
	{
		Analytics.TutorialStopped(ActiveTutorial?.TutorialType ?? TutorialType.None);
		int i = 0;
		for (int num = _tutorials.Length; i < num; i++)
		{
			Tutorial tutorial = _tutorials[i];
			if (!tutorial.IsCompleted)
			{
				tutorial.Quit();
				FireTutorialFinishedEvent(tutorial);
			}
		}
		ActiveTutorial = null;
		ToggleTriggers(!AllTutorialsFinished);
		TryStartTutorial();
	}

	public void StepReached(Tutorial tutorial, int stepNumber)
	{
		Analytics.TutorialStepReached(tutorial.TutorialType, stepNumber);
	}

	public bool IsTutorialFinished(TutorialType tutorialType)
	{
		int i = 0;
		for (int num = _tutorials.Length; i < num; i++)
		{
			Tutorial tutorial = _tutorials[i];
			if (tutorial.TutorialType == tutorialType)
			{
				return tutorial.IsCompleted;
			}
		}
		return false;
	}

	private void InitializeTutorials(GameState gameState, GameStats gameStats, WorldMap worldMap, IslandsManager islandManager, CraneManager craneManager, BuildingWarehouseManager warehouseManager, TreasureChestManager treasureChestManager, CurrencyConversionManager currencyConversionManager, FishingEvent fishingEvent, bool expansionWarehouseTutorialsEnabled)
	{
		_tutorials = new Tutorial[14]
		{
			new StartTutorial(_storage.GetStorageDict(1.ToString()), this, islandManager, _popupManager, worldMap),
			new BuildResidentialTutorial(_storage.GetStorageDict(2.ToString()), this, islandManager, _popupManager, worldMap),
			new BuildRoadTutorial(_storage.GetStorageDict(3.ToString()), this, islandManager, _popupManager, worldMap),
			new BuildCommercialTutorial(_storage.GetStorageDict(4.ToString()), this, islandManager, _popupManager, worldMap),
			new BuildCommunityTutorial(_storage.GetStorageDict(5.ToString()), this, islandManager, _popupManager, worldMap),
			new WorldMapStartTutorial(_storage.GetStorageDict(6.ToString()), this, islandManager, worldMap, _popupManager),
			new WorldMapUnlockIslandTutorial(_storage.GetStorageDict(7.ToString()), this, islandManager, _popupManager, worldMap),
			new WorldMapWrapUpTutorial(_storage.GetStorageDict(8.ToString()), this, islandManager, _popupManager, worldMap),
			new WarehouseTutorial(_storage.GetStorageDict(10.ToString()), this, islandManager, _popupManager, worldMap, warehouseManager),
			new CurrencyConversionTutorial(_storage.GetStorageDict(11.ToString()), this, islandManager, _popupManager, worldMap, currencyConversionManager),
			new TreasureChestTutorial(_storage.GetStorageDict(12.ToString()), this, islandManager, _popupManager, worldMap, treasureChestManager),
			new FishingEventTutorial(_storage.GetStorageDict(13.ToString()), this, islandManager, _popupManager, fishingEvent),
			new ExpansionTutorial(_storage.GetStorageDict(14.ToString()), this, islandManager, _popupManager, worldMap, gameState, gameStats, expansionWarehouseTutorialsEnabled),
			new BigWarehouseTutorial(_storage.GetStorageDict(15.ToString()), this, islandManager, _popupManager, worldMap, gameStats, warehouseManager, expansionWarehouseTutorialsEnabled)
		};
		_triggers = new TutorialTrigger[7]
		{
			new WorldMapTutorialTrigger(this, worldMap),
			new BuildCountTutorialTrigger(this, craneManager, 0),
			new WarehouseBuildingAddedTutorialTrigger(this, warehouseManager),
			new LevelUpTutorialTrigger(this, gameState),
			new TreasureChestAvailableTrigger(this, treasureChestManager),
			new FishingEventTutorialTrigger(this, fishingEvent),
			new BuildingBuiltTutorialTrigger(this, gameStats)
		};
	}

	private void StartTutorial(Tutorial tutorial)
	{
		ActiveTutorial = tutorial;
		tutorial.Begin();
		if (ActiveTutorial != null)
		{
			ToggleTriggers(active: false);
			FireTutorialStartedEvent(ActiveTutorial);
			Analytics.TutorialStarted(tutorial.TutorialType);
		}
	}

	private void ToggleTriggers(bool active)
	{
		if (active)
		{
			int i = 0;
			for (int num = _triggers.Length; i < num; i++)
			{
				_triggers[i].Enable();
			}
		}
		else
		{
			int j = 0;
			for (int num2 = _triggers.Length; j < num2; j++)
			{
				_triggers[j].Disable();
			}
		}
	}

	private void OnSpeedupExecuted()
	{
		if (!InitialTutorialFinished)
		{
			Analytics.LogEvent("speedup_in_tutorial");
		}
	}
}
