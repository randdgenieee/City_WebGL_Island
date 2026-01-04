using CIG.Translation;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class Building : GridTile, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public delegate void TimedBuildingEventHandler(double time);

		private const float LongPressDelay = 0.35f;

		[SerializeField]
		private PolygonCollider2D _collider;

		[SerializeField]
		private PolygonCollider2D _floorCollider;

		[SerializeField]
		protected GridTileIconManager _gridTileIconManager;

		[SerializeField]
		protected PlingManager _plingManager;

		[SerializeField]
		protected CurrencyAnimationSource _currencyAnimSource;

		[SerializeField]
		protected ConstructionYard _constructionPrefab;

		private BuildingState _state;

		private bool _hasRoad;

		private ProgressOverlay _constructionProgressOverlay;

		private ConstructionYard _constructionYard;

		private IEnumerator _constructionRoutine;

		private UpspeedableProcess _constructionUpspeedableProcess;

		private bool _giveXpOnConstruction = true;

		private IEnumerator _demolishBehaviourRoutine;

		private UpspeedableProcess _demolishUpspeedableProcess;

		private int _totalBoostedPercentage;

		private const string StateKey = "State";

		private const string HasRoadKey = "HasRoad";

		public const string BoostedPercentageKey = "BoostedPercentage";

		private const string ConstructionUpspeedableProcessKey = "ConstructionUpspeedableProcess";

		private const string DemolishUpspeedableProcessKey = "DemolishUpspeedableProcess";

		public BuildingProperties BuildingProperties
		{
			get;
			private set;
		}

		public virtual bool InfoRequiresFrequentRefresh => false;

		public BuildingState State
		{
			get
			{
				return _state;
			}
			private set
			{
				if (_state != value)
				{
					_state = value;
					Serialize();
				}
			}
		}

		public virtual bool HasRoad
		{
			get
			{
				if (BuildingProperties.CheckForRoad)
				{
					return _hasRoad;
				}
				return true;
			}
			protected set
			{
				if (_hasRoad != value)
				{
					_hasRoad = value;
					Serialize();
				}
			}
		}

		public double ConstructionTimeLeft
		{
			get
			{
				if (_constructionUpspeedableProcess != null)
				{
					return _constructionUpspeedableProcess.TimeLeft;
				}
				return BuildingProperties.ConstructionDurationSeconds;
			}
		}

		public int ClampedBoostPercentage => Mathf.Clamp(_totalBoostedPercentage, 0, 100);

		protected virtual bool CanDemolish => BuildingProperties.Destructible;

		public virtual bool CanMoveToWarehouse => BuildingProperties.Movable;

		public PolygonCollider2D Collider => _collider;

		public PolygonCollider2D FloorCollider => _floorCollider;

		public event TimedBuildingEventHandler ConstructionFinishedEvent;

		public event TimedBuildingEventHandler DestroyedEvent;

		protected virtual void FireConstructionFinishedEvent(double constructionCompletedTime)
		{
			this.ConstructionFinishedEvent?.Invoke(constructionCompletedTime);
		}

		protected virtual void FireDestroyedEvent(double destroyedTime)
		{
			this.DestroyedEvent?.Invoke(destroyedTime);
		}

		public override void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, GridTileProperties properties, OverlayManager overlayManager, CIGIslandState islandState, GridIndex? index = default(GridIndex?))
		{
			BuildingProperties = (BuildingProperties)properties;
			_gridTileIconManager.Initialize(overlayManager);
			_plingManager.Initialize(overlayManager);
			base.Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState, index);
			_currencyAnimSource.Initialize(this);
			switch (State)
			{
			case BuildingState.Constructing:
				SetConstructingState();
				break;
			case BuildingState.WaitingForConstructionFinish:
				SetWaitingForConstructionFinishState();
				break;
			case BuildingState.Normal:
				SetNormalState();
				break;
			case BuildingState.Demolishing:
				SetDemolishingState();
				break;
			}
		}

		protected override void OnDestroy()
		{
			if (_routineRunner != null)
			{
				if (_constructionRoutine != null)
				{
					_routineRunner.StopCoroutine(_constructionRoutine);
					_constructionRoutine = null;
					_constructionUpspeedableProcess = null;
				}
				if (_demolishBehaviourRoutine != null)
				{
					_routineRunner.StopCoroutine(_demolishBehaviourRoutine);
					_demolishBehaviourRoutine = null;
					_demolishUpspeedableProcess = null;
				}
			}
			base.OnDestroy();
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			OnBuildingPressed();
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (!this.IsInvoking(OnBuildingLongPressed))
			{
				this.Invoke(OnBuildingLongPressed, 0.35f);
			}
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			this.CancelInvoke(OnBuildingLongPressed);
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			this.CancelInvoke(OnBuildingLongPressed);
		}

		public void AddBoostPercentage(int amount)
		{
			int totalBoostedPercentage = _totalBoostedPercentage;
			_totalBoostedPercentage += amount;
			if (totalBoostedPercentage != _totalBoostedPercentage && State != BuildingState.Constructing && State != BuildingState.Demolishing && base.Status != GridTileStatus.Destroyed)
			{
				OnBoostedPercentageChanged();
			}
		}

		public void SpeedupConstruction()
		{
			_constructionUpspeedableProcess?.FreeSpeedup();
		}

		public void OverrideConstructionDuration(double duration)
		{
			_constructionUpspeedableProcess?.OverrideDuration(duration);
			if (_constructionYard != null)
			{
				_constructionYard.ResetAnimation();
			}
		}

		public virtual void BuildFromWarehouse(bool newBuilding, int level)
		{
			_giveXpOnConstruction = newBuilding;
			if (!newBuilding)
			{
				if (_constructionRoutine != null)
				{
					_routineRunner.StopCoroutine(_constructionRoutine);
					_constructionRoutine = null;
					_constructionUpspeedableProcess = null;
				}
				OnConstructionCompleted();
				FinishConstruction();
			}
		}

		public virtual void OnAdjacentRoadsChanged(Road road)
		{
			HasRoad = CheckForRoad();
			UpdateMissingRoadIcon();
		}

		public void StartDemolishing()
		{
			if (!CanDemolish)
			{
				UnityEngine.Debug.LogError("This building cannot be demolished right now: (" + base.Properties.BaseKey + ")!");
				return;
			}
			SetDemolishingState();
			Serialize();
		}

		public bool DemolishImmediately(bool moveToWarehouse)
		{
			if (moveToWarehouse || CanDemolish)
			{
				OnDemolishStarted();
				OnDemolishCompleted();
				DestroyTile();
				return true;
			}
			UnityEngine.Debug.LogError("This building cannot be demolished right now: (" + base.Properties.BaseKey + ")!");
			return false;
		}

		protected virtual void OnBuildingLongPressed()
		{
		}

		protected virtual void OnBuildingPressed()
		{
			switch (State)
			{
			case BuildingState.Normal:
				break;
			case BuildingState.Constructing:
			{
				Sprite asset3 = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.Crane);
				Sprite asset4 = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.WaitIcon);
				SpeedupPopupRequest request2 = new SpeedupPopupRequest(_constructionUpspeedableProcess, BuildingProperties.LocalizedName, Localization.Key("constructing_verb"), Localization.Key("wait"), null, asset4, BuildingProperties, asset3, "building_construction");
				_popupManager.RequestPopup(request2);
				break;
			}
			case BuildingState.WaitingForConstructionFinish:
				FinishConstruction();
				break;
			case BuildingState.Demolishing:
			{
				Sprite asset = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.DemolishIcon);
				Sprite asset2 = SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.DemolishCancelIcon);
				SpeedupPopupRequest request = new SpeedupPopupRequest(_demolishUpspeedableProcess, BuildingProperties.LocalizedName, Localization.Key("demolishing"), Localization.Key("stop"), CancelDemolishment, asset2, BuildingProperties, asset, "building_demolish");
				_popupManager.RequestPopup(request);
				break;
			}
			}
		}

		protected override void OnStatusChanged(GridTileStatus oldStatus)
		{
			base.OnStatusChanged(oldStatus);
			switch (base.Status)
			{
			case GridTileStatus.Created:
				switch (oldStatus)
				{
				case GridTileStatus.Preview:
					HasRoad = CheckForRoad();
					StartConstructing();
					break;
				case GridTileStatus.Moving:
					OnAdjacentRoadsChanged(null);
					break;
				}
				_gridTileIconManager.ShowIcon();
				break;
			case GridTileStatus.Preview:
			case GridTileStatus.Moving:
				_gridTileIconManager.HideIcon();
				break;
			}
		}

		protected override void OnHiddenChanged(bool hidden)
		{
			if (_constructionYard == null)
			{
				base.OnHiddenChanged(hidden);
				_collider.enabled = !hidden;
			}
			else
			{
				base.OnHiddenChanged(hidden: true);
				_collider.enabled = false;
				_constructionYard.OnHiddenChanged(hidden);
			}
		}

		protected virtual void OnConstructionStarted()
		{
		}

		protected virtual void OnConstructionCompleted()
		{
			_craneManager.FinishTracking(this);
			RemoveConstructionProgressBar();
			RemoveConstructionYard();
			if (_giveXpOnConstruction)
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ConstructionFinished);
			}
		}

		protected virtual void OnConstructionFinished()
		{
			if (_giveXpOnConstruction)
			{
				if (BuildingProperties.ConstructionReward.IsMatchingName("XP") && !_gameState.ReachedMaxLevel)
				{
					_gameState.EarnCurrencies(BuildingProperties.ConstructionReward, CurrenciesEarnedReason.BuildingConstruction, new FlyingCurrenciesData(this));
					_plingManager.ShowCurrencyPlings(_timing, BuildingProperties.ConstructionReward, Clip.CollectXP);
				}
				_gameStats.AddGlobalBuildingsBuilt(1);
			}
			ToggleFinishConstructionIcon(show: false);
			FireConstructionFinishedEvent(_timing.GameTime);
		}

		protected virtual void OnDemolishStarted()
		{
		}

		protected virtual void OnDemolishCompleted()
		{
			FireDestroyedEvent(_timing.GameTime);
		}

		protected virtual void OnDemolishCancelled()
		{
			SetNormalState();
			Serialize();
		}

		protected virtual void OnBoostedPercentageChanged()
		{
		}

		private void SetConstructingState()
		{
			State = BuildingState.Constructing;
			if (_constructionRoutine == null)
			{
				_routineRunner.StartCoroutine(_constructionRoutine = ConstructionRoutine());
			}
		}

		private void SetWaitingForConstructionFinishState()
		{
			State = BuildingState.WaitingForConstructionFinish;
			ToggleFinishConstructionIcon(show: true);
		}

		private void SetNormalState()
		{
			State = BuildingState.Normal;
			UpdateMissingRoadIcon();
		}

		private void SetDemolishingState()
		{
			State = BuildingState.Demolishing;
			_gridTileIconManager.SetIcon<ButtonGridTileIcon>(GridTileIconType.Demolish).Init(OnBuildingPressed);
			if (_demolishBehaviourRoutine == null)
			{
				_routineRunner.StartCoroutine(_demolishBehaviourRoutine = DemolishBehaviour());
			}
		}

		private void StartConstructing()
		{
			SetConstructingState();
			OnConstructionStarted();
			_craneManager.StartTracking(this, _constructionUpspeedableProcess.EndTime, BuildingProperties.ConstructionCranes);
			Serialize();
		}

		private void FinishConstruction()
		{
			SetNormalState();
			OnConstructionFinished();
			Serialize();
		}

		private void ShowConstructionYard()
		{
			if (_constructionYard == null)
			{
				UpdateTransform();
				_constructionYard = Object.Instantiate(_constructionPrefab, base.transform);
				_constructionYard.Initialize(this, _timing, _routineRunner);
				OnHiddenChanged(base.Hidden);
			}
		}

		private void RemoveConstructionYard()
		{
			if (_constructionYard != null)
			{
				UnityEngine.Object.Destroy(_constructionYard.gameObject);
				_constructionYard = null;
				UpdateTransform();
				OnHiddenChanged(base.Hidden);
			}
		}

		private void ShowConstructionProgressBar(UpspeedableProcess process)
		{
			if (_constructionProgressOverlay == null)
			{
				_constructionProgressOverlay = _overlayManager.CreateOverlay<ProgressOverlay>(base.gameObject, OverlayType.Progress);
			}
			_constructionProgressOverlay.Initialize(process);
		}

		private void RemoveConstructionProgressBar()
		{
			if (_constructionProgressOverlay != null)
			{
				_constructionProgressOverlay.Remove();
				_constructionProgressOverlay = null;
			}
		}

		private IEnumerator ConstructionRoutine()
		{
			if (BuildingProperties.ConstructionDurationSeconds > 0)
			{
				if (_constructionUpspeedableProcess == null)
				{
					_constructionUpspeedableProcess = new UpspeedableProcess(_timing, _multipliers, _gameState, BuildingProperties.ConstructionDurationSeconds, CurrenciesSpentReason.SpeedupBuild);
				}
				ShowConstructionProgressBar(_constructionUpspeedableProcess);
				ShowConstructionYard();
				yield return _constructionUpspeedableProcess;
			}
			OnConstructionCompleted();
			SetWaitingForConstructionFinishState();
			_constructionRoutine = null;
			_constructionUpspeedableProcess = null;
		}

		private void ToggleFinishConstructionIcon(bool show)
		{
			if (show)
			{
				_gridTileIconManager.SetIcon<OpeningBannerGridTileIcon>(GridTileIconType.OpeningBanner).Init(OnBuildingPressed);
			}
			else
			{
				_gridTileIconManager.RemoveIcon(GridTileIconType.OpeningBanner);
			}
		}

		private void UpdateMissingRoadIcon()
		{
			if (!HasRoad && State != BuildingState.Constructing)
			{
				_gridTileIconManager.SetIcon<ButtonGridTileIcon>(GridTileIconType.MissingRoad).Init(OnBuildingPressed);
			}
			else
			{
				_gridTileIconManager.RemoveIcon(GridTileIconType.MissingRoad);
			}
		}

		private void CancelDemolishment()
		{
			_demolishUpspeedableProcess?.Cancel();
		}

		private IEnumerator DemolishBehaviour()
		{
			OnDemolishStarted();
			if (_demolishUpspeedableProcess == null)
			{
				_demolishUpspeedableProcess = new UpspeedableProcess(_timing, _multipliers, _gameState, BuildingProperties.DemolishDurationSeconds, CurrenciesSpentReason.SpeedupDemolish);
			}
			if (_demolishUpspeedableProcess.TimeLeft <= 0.0)
			{
				yield return null;
			}
			else
			{
				yield return _demolishUpspeedableProcess;
			}
			_gridTileIconManager.RemoveIcon(GridTileIconType.Demolish);
			if (_demolishUpspeedableProcess.Cancelled)
			{
				OnDemolishCancelled();
			}
			else
			{
				Analytics.BuildingDemolished(BuildingProperties.BaseKey);
				OnDemolishCompleted();
				DestroyTile();
			}
			_demolishUpspeedableProcess = null;
			_demolishBehaviourRoutine = null;
		}

		private bool CheckForRoad()
		{
			if (BuildingProperties.CheckForRoad)
			{
				foreach (GridTile neighbourTile in _isometricGrid.GetNeighbourTiles(this))
				{
					if (neighbourTile is Road)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = base.Serialize();
			storageDictionary.Set("State", (int)State);
			storageDictionary.Set("HasRoad", HasRoad);
			storageDictionary.Set("BoostedPercentage", _totalBoostedPercentage);
			storageDictionary.SetOrRemoveStorable("ConstructionUpspeedableProcess", _constructionUpspeedableProcess, _constructionUpspeedableProcess == null);
			storageDictionary.SetOrRemoveStorable("DemolishUpspeedableProcess", _demolishUpspeedableProcess, _demolishUpspeedableProcess == null);
			return storageDictionary;
		}

		protected override void Deserialize(StorageDictionary storage)
		{
			base.Deserialize(storage);
			_hasRoad = storage.Get("HasRoad", HasRoad);
			_totalBoostedPercentage = storage.Get("BoostedPercentage", _totalBoostedPercentage);
			if (storage.Contains("State"))
			{
				_state = (BuildingState)storage.Get("State", (int)State);
				switch (State)
				{
				case BuildingState.Constructing:
					_constructionUpspeedableProcess = new UpspeedableProcess(storage.GetStorageDict("ConstructionUpspeedableProcess"), _timing, _multipliers, _gameState);
					break;
				case BuildingState.Demolishing:
					_demolishUpspeedableProcess = new UpspeedableProcess(storage.GetStorageDict("DemolishUpspeedableProcess"), _timing, _multipliers, _gameState);
					break;
				}
			}
		}
	}
}
