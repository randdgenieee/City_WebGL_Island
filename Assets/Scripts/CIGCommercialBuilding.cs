using System;
using System.Collections;
using CIG;
using CIG.Translation;
using Tweening;
using UnityEngine;

public class CIGCommercialBuilding : CIGBuilding
{
    public CommercialBuildingProperties CommercialProperties { get; private set; }

    public Currencies Profit
    {
        get
        {
            if (this._currentMaxEmployees == 0)
            {
                return new Currencies();
            }
            return this._currentEmployees * this.CurrentProfit / this._currentMaxEmployees;
        }
    }

    public Currencies ExtraProfitNextLevel
    {
        get
        {
            if (base.CurrentLevel >= 0 && base.CurrentLevel < this.CommercialProperties.ProfitPerLevel.Count - 1)
            {
                return this.CommercialProperties.ProfitPerLevel[base.CurrentLevel + 1] - this.CommercialProperties.ProfitPerLevel[base.CurrentLevel];
            }
            return new Currencies();
        }
    }

    public Currencies CurrentProfit
    {
        get
        {
            return this.GetMultipliedProfit(this._currentProfit);
        }
        private set
        {
            this._currentProfit = value;
        }
    }

    public Currencies UnboostedCurrentProfit
    {
        get
        {
            return this.GetMultipliedProfit(this.CommercialProperties.ProfitPerLevel[base.CurrentLevel]);
        }
    }

    public int MaxEmployees
    {
        get
        {
            return this._currentMaxEmployees;
        }
        protected set
        {
            int num = value - this._currentMaxEmployees;
            this._currentMaxEmployees = value;
            if (num != 0)
            {
                this._islandState.AddJobs(num);
            }
        }
    }

    public int UnboostedMaxEmployees
    {
        get
        {
            return this.CommercialProperties.EmployeesPerLevel[base.CurrentLevel];
        }
    }

    public int ExtraEmployeesNextLevel
    {
        get
        {
            if (base.CurrentLevel >= 0 && base.CurrentLevel < this.CommercialProperties.EmployeesPerLevel.Count - 1)
            {
                return this.CommercialProperties.EmployeesPerLevel[base.CurrentLevel + 1] - this.CommercialProperties.EmployeesPerLevel[base.CurrentLevel];
            }
            return 0;
        }
    }

    public int Employees
    {
        get
        {
            return this._currentEmployees;
        }
        protected set
        {
            int num = value - this._currentEmployees;
            this._currentEmployees = value;
            if (num != 0)
            {
                this._islandState.AddEmployees(num);
                this.OnEmployeesChanged(num);
                if (base.gameObject.activeInHierarchy)
                {
                    TextStyleType textStyle = (num > 0) ? TextStyleType.GreenOutlined22 : TextStyleType.RedOutlined22;
                    this._plingManager.ShowPling(PlingType.Worker, Localization.Format((num > 0) ? Localization.Key("plus") : Localization.Key("minus"), new ILocalizedString[]
                    {
                        Localization.Integer(Math.Abs(num))
                    }), textStyle);
                }
            }
        }
    }

    public double ProfitTimeLeft
    {
        get
        {
            if (this.CurrencyConversionProcess != null)
            {
                return this.CurrencyConversionProcess.TimeLeft;
            }
            if (this._profitProcess != null)
            {
                return this._profitProcess.TimeLeft;
            }
            return 0.0;
        }
    }

    public override bool InfoRequiresFrequentRefresh
    {
        get
        {
            if (base.State == BuildingState.Preview || base.State != BuildingState.Normal || base.IsUpgrading || this.CanActivate)
            {
                return base.InfoRequiresFrequentRefresh;
            }
            return this.HasTimeLeft;
        }
    }

    public override Currency UpgradeCost
    {
        get
        {
            return base.UpgradeCost.Multiply(this._multipliers.GetMultiplier(MultiplierType.CommercialCashCostUpgrade), new RoundingMethod?(RoundingMethod.Nearest), 0);
        }
    }

    public override Currency UpgradeInstantCost
    {
        get
        {
            return base.UpgradeInstantCost.Multiply(this._multipliers.GetMultiplier(MultiplierType.CommercialInstantGoldCostUpgrade), new RoundingMethod?(RoundingMethod.Nearest), 0);
        }
    }

    protected virtual bool HasTimeLeft
    {
        get
        {
            return this.ProfitTimeLeft > 0.0;
        }
    }

    private double EmployeeUnchangedUpdateWaitTime
    {
        get
        {
            return (double)UnityEngine.Random.Range(2f, 30f);
        }
    }

    private double EmployeeChangedUpdateWaitTime
    {
        get
        {
            return (double)UnityEngine.Random.Range(1f, 3f);
        }
    }

    public CurrencyConversionProcess CurrencyConversionProcess { get; private set; }

    public override bool CanUpgrade
    {
        get
        {
            return this.ProfitRoutineState != CIGCommercialBuilding.ProfitRoutineStateType.WaitForConversion && base.CanUpgrade;
        }
    }

    protected override bool CanDemolish
    {
        get
        {
            return this.ProfitRoutineState != CIGCommercialBuilding.ProfitRoutineStateType.WaitForConversion && base.CanDemolish;
        }
    }

    public override bool CanMoveToWarehouse
    {
        get
        {
            return this.ProfitRoutineState != CIGCommercialBuilding.ProfitRoutineStateType.WaitForConversion && base.CanMoveToWarehouse;
        }
    }

    private CIGCommercialBuilding.ProfitRoutineStateType ProfitRoutineState
    {
        get
        {
            if (this.CurrencyConversionProcess != null)
            {
                return CIGCommercialBuilding.ProfitRoutineStateType.WaitForConversion;
            }
            if (this._profitCollectBehaviour != null)
            {
                return CIGCommercialBuilding.ProfitRoutineStateType.WaitForCollect;
            }
            return CIGCommercialBuilding.ProfitRoutineStateType.WaitForProfit;
        }
    }

    public event CIGCommercialBuilding.CurrencyConversionStateChangedEventHandler CurrencyConversionStateChangedEvent;

    private void FireCurrencyConversionStateChangedEvent(bool active)
    {
        CIGCommercialBuilding.CurrencyConversionStateChangedEventHandler currencyConversionStateChangedEvent = this.CurrencyConversionStateChangedEvent;
        if (currencyConversionStateChangedEvent == null)
        {
            return;
        }
        currencyConversionStateChangedEvent(active);
    }

    public override void Initialize(StorageDictionary storage, IsometricGrid isometricGrid, IslandsManagerView islandsManagerView, WorldMap worldMap, BuildingWarehouseManager buildingWarehouseManager, CraneManager craneManager, GameStats gameStats, GameState gameState, PopupManager popupManager, Multipliers multipliers, Timing timing, RoutineRunner routineRunner, GridTileProperties properties, OverlayManager overlayManager, CIGIslandState islandState, GridIndex? index = null)
    {
        this.CommercialProperties = (CommercialBuildingProperties)properties;
        base.Initialize(storage, isometricGrid, islandsManagerView, worldMap, buildingWarehouseManager, craneManager, gameStats, gameState, popupManager, multipliers, timing, routineRunner, properties, overlayManager, islandState, index);
        if (base.State == BuildingState.Normal)
        {
            this.UpdateNoEmployeesIcon();
            this.StartEmployeesRoutine();
            if (!base.IsUpgrading)
            {
                this.StartProfitRoutine();
            }
        }
    }

    protected override void OnDestroy()
    {
        if (this._routineRunner != null)
        {
            if (this._profitLoopRoutine != null)
            {
                this._routineRunner.StopCoroutine(this._profitLoopRoutine);
                this._profitLoopRoutine = null;
            }
            if (this._employeesBehaviourRoutine != null)
            {
                this._routineRunner.StopCoroutine(this._employeesBehaviourRoutine);
                this._employeesBehaviourRoutine = null;
            }
        }
        base.OnDestroy();
    }

    public virtual ILocalizedString TimeLeftString()
    {
        return Localization.TimeSpan(TimeSpan.FromSeconds(this.ProfitTimeLeft), false);
    }

    public void StartCurrencyConversion(CurrencyConversionProperties currencyConversionProperties)
    {
        this._gameState.SpendCurrencies(currencyConversionProperties.FromCurrency, CurrenciesSpentReason.CurrencyConversion, delegate (bool success, Currencies spent)
        {
            if (success)
            {
                if (this._profitProcess != null)
                {
                    this._profitProcess.Cancel();
                    this._profitProcess = null;
                }
                Currencies toCurrencies = this.ApplyMultiplierToCurrencyConversion(currencyConversionProperties.ToCurrency);
                this.CurrencyConversionProcess = new CurrencyConversionProcess(this._timing, this._multipliers, this._gameState, currencyConversionProperties.FromCurrency, toCurrencies, (double)currencyConversionProperties.Duration);
                this._gameStats.AddCurrencyConversion(currencyConversionProperties.FromCurrency, toCurrencies);
                this.StartProfitRoutine();
                this.FireCurrencyConversionStateChangedEvent(true);
                this.Serialize();
            }
        });
    }

    public Currencies ApplyMultiplierToCurrencyConversion(Currencies toCurrency)
    {
        return (toCurrency * (decimal)this.CommercialProperties.CurrencyConversionMultiplier).Round(0);
    }

    public static void EnableMultipleCollect(int extraCollectTimes)
    {
        CIGCommercialBuilding._extraCollectTimes = extraCollectTimes;
    }

    public static void DisableMultipleCollect()
    {
        CIGCommercialBuilding._extraCollectTimes = 0;
    }

    protected virtual void OnEmployeesChanged(int diff)
    {
        this.UpdateNoEmployeesIcon();
    }

    protected override void OnConstructionFinished()
    {
        if (!base.BuildingProperties.Activatable)
        {
            this.SetMaxEmployeesAndProfit();
            this.StartProfitRoutine();
            this.StartEmployeesRoutine();
            this.UpdateNoEmployeesIcon();
        }
        base.OnConstructionFinished();
    }

    protected override void OnUpgradeStarted()
    {
        base.OnUpgradeStarted();
        this.StopProfitRoutine();
    }

    protected override void OnUpgradeCompleted(double completionTime)
    {
        base.OnUpgradeCompleted(completionTime);
        this.SetMaxEmployeesAndProfit();
        this._profitProcess = new UpspeedableProcess(this._timing, this._multipliers, this._gameState, Math.Max(0.0, (double)this.CommercialProperties.ProfitDurationSeconds - (this._timing.GameTime - completionTime)), CurrenciesSpentReason.Unknown);
        this.UpdateNoEmployeesIcon();
        this.StartProfitRoutine();
    }

    protected override void OnBoostedPercentageChanged()
    {
        base.OnBoostedPercentageChanged();
        this.SetMaxEmployeesAndProfit();
        this.Serialize();
    }

    protected override void OnDemolishStarted()
    {
        base.OnDemolishStarted();
        this.StopProfitRoutine();
        this.StopEmployeesRoutine();
        this.Employees = 0;
        this.MaxEmployees = 0;
    }

    protected override void OnDemolishCancelled()
    {
        this.SetMaxEmployeesAndProfit();
        base.OnDemolishCancelled();
        this.StartProfitRoutine();
        this.StartEmployeesRoutine();
    }

    protected override void OnBuildingPressed()
    {
        BuildingState state = base.State;
        if (state != BuildingState.Normal)
        {
            base.OnBuildingPressed();
            return;
        }
        if (this._profitCollectBehaviour == null)
        {
            base.OnBuildingPressed();
            return;
        }
        if (this._profitCollectBehaviour.CanCollect)
        {
            Currencies currencies = this._profitCollectBehaviour.Collect();
            this._gameState.EarnCurrencies(currencies, this._profitCollectBehaviour.CurrenciesEarnedReason, new FlyingCurrenciesData(this, 1));
            this._plingManager.ShowCurrencyPlings(this._timing, currencies, null);
            SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.CollectCoinsCash, false, 1f, 0f);
            this._gameStats.AddProfitCollected(currencies);
            this._profitScaleTweener.StopAndReset(false);
            this._profitScaleTweener.Play();
        }
        if (!this._profitCollectBehaviour.CanCollect)
        {
            this._gameStats.AddTimesCollected();
            this._profitCollectBehaviour = null;
        }
        this.Serialize();
    }

    protected override void OnMirroredChanged(bool mirrored)
    {
        base.OnMirroredChanged(mirrored);
        this._scaleTweenerTrack.SetFromAndTo(mirrored ? CIGCommercialBuilding.FromScaleMirrored : CIGCommercialBuilding.FromScale, mirrored ? CIGCommercialBuilding.ToScaleMirrored : CIGCommercialBuilding.ToScale);
    }

    private void SetMaxEmployeesAndProfit()
    {
        this.CurrentProfit = new Currencies(this.CommercialProperties.ProfitPerLevel[base.CurrentLevel]) * ((100 + base.ClampedBoostPercentage) / 100m);
        int num = this.CommercialProperties.EmployeesPerLevel[base.CurrentLevel];
        int num2 = num + Mathf.CeilToInt((float)(base.ClampedBoostPercentage * num) / 100f);
        this.MaxEmployees = num2;
        this.Employees = Mathf.Min(this.Employees, num2);
    }

    private bool UpdateEmployees()
    {
        int globalEmployees = this._gameState.GlobalEmployees;
        int num = this.Employees;
        if (base.BuildingProperties.CheckForRoad && !this.HasRoad)
        {
            num--;
        }
        else if (this._islandState.AvailablePopulation < globalEmployees)
        {
            num--;
        }
        else if (this._islandState.AvailablePopulation > globalEmployees)
        {
            num++;
        }
        num = Math.Max(0, Math.Min(num, this.MaxEmployees));
        if (num != this.Employees)
        {
            this.Employees = num;
            this.Serialize();
            return true;
        }
        return false;
    }

    private void UpdateNoEmployeesIcon()
    {
        if (this.Employees == 0)
        {
            this._gridTileIconManager.SetIcon<ButtonGridTileIcon>(GridTileIconType.NoEmployees).Init(new Action(this.OnBuildingPressed));
            return;
        }
        this._gridTileIconManager.RemoveIcon(GridTileIconType.NoEmployees);
    }

    private IEnumerator ProfitLoopBehaviour()
    {
        for (; ; )
        {
            switch (this.ProfitRoutineState)
            {
                case CIGCommercialBuilding.ProfitRoutineStateType.WaitForProfit:
                    yield return this.WaitForProfitRoutine();
                    break;
                case CIGCommercialBuilding.ProfitRoutineStateType.WaitForConversion:
                    yield return this.WaitForConversionRoutine();
                    break;
                case CIGCommercialBuilding.ProfitRoutineStateType.WaitForCollect:
                    yield return this.WaitForCollectRoutine();
                    break;
            }
        }
        yield break;
    }

    private IEnumerator WaitForProfitRoutine()
    {
        if (this._profitProcess == null)
        {
            this._profitProcess = new UpspeedableProcess(this._timing, this._multipliers, this._gameState, (double)this.CommercialProperties.ProfitDurationSeconds, CurrenciesSpentReason.Unknown);
            this.Serialize();
        }
        yield return this._profitProcess;
        if (!this._profitProcess.Cancelled)
        {
            Currencies profit = this.Profit;
            if (!profit.IsEmpty())
            {
                this._profitCollectBehaviour = new FullProfitCollectBehaviour(profit, CurrenciesEarnedReason.BuildingCollect, profit.MissingCurrencies(this._currentProfit).IsEmpty(), CIGCommercialBuilding._extraCollectTimes);
            }
        }
        this._profitProcess = null;
        this.Serialize();
        yield break;
    }

    private IEnumerator WaitForConversionRoutine()
    {
        this._gridTileIconManager.SetIcon<GridTileIcon>(GridTileIconType.CommercialConvert);
        yield return this.CurrencyConversionProcess;
        this.HideConvertIcon();
        if (!this.CurrencyConversionProcess.Cancelled)
        {
            MultiTapProfitCollectBehaviour profitCollectBehaviour = new MultiTapProfitCollectBehaviour(this.CurrencyConversionProcess.ToCurrencies, CurrenciesEarnedReason.CurrencyConversion);
            this._profitCollectBehaviour = profitCollectBehaviour;
        }
        this.CurrencyConversionProcess = null;
        this.FireCurrencyConversionStateChangedEvent(false);
        this.Serialize();
        yield break;
    }

    private IEnumerator WaitForCollectRoutine()
    {
        this.ShowProfitIcon();
        while (this._profitCollectBehaviour != null && this._profitCollectBehaviour.CanCollect)
        {
            yield return null;
        }
        this._profitCollectBehaviour = null;
        this.HideProfitIcon();
        this.Serialize();
        yield break;
    }

    private IEnumerator EmployeesBehaviour()
    {
        bool flag = true;
        for (; ; )
        {
            yield return new WaitForGameTimeSeconds(this._timing, flag ? this.EmployeeChangedUpdateWaitTime : this.EmployeeUnchangedUpdateWaitTime);
            flag = this.UpdateEmployees();
        }
        yield break;
    }

    private void StartProfitRoutine()
    {
        this.StopProfitRoutine();
        this._routineRunner.StartCoroutine(this._profitLoopRoutine = this.ProfitLoopBehaviour());
    }

    private void StopProfitRoutine()
    {
        if (this._routineRunner != null && this._profitLoopRoutine != null)
        {
            this._routineRunner.StopCoroutine(this._profitLoopRoutine);
            this._profitLoopRoutine = null;
            this.HideProfitIcon();
            this.HideConvertIcon();
        }
    }

    private void StartEmployeesRoutine()
    {
        this.StopEmployeesRoutine();
        this._routineRunner.StartCoroutine(this._employeesBehaviourRoutine = this.EmployeesBehaviour());
    }

    private void StopEmployeesRoutine()
    {
        if (this._routineRunner != null && this._employeesBehaviourRoutine != null)
        {
            this._routineRunner.StopCoroutine(this._employeesBehaviourRoutine);
            this._employeesBehaviourRoutine = null;
        }
    }

    private void ShowProfitIcon()
    {
        this.HideProfitIcon();
        ProfitCollectBehaviour profitCollectBehaviour = this._profitCollectBehaviour;
        if (profitCollectBehaviour != null)
        {
            MultiTapProfitCollectBehaviour multiTapProfitCollectBehaviour;
            if ((multiTapProfitCollectBehaviour = (profitCollectBehaviour as MultiTapProfitCollectBehaviour)) != null)
            {
                MultiTapProfitCollectBehaviour multiTapCollectProfitBehaviour = multiTapProfitCollectBehaviour;
                this._gridTileIconManager.SetIcon<MultiTapCollectGridTileIcon>(GridTileIconType.MultiTapCollect).Initialize(new Action(this.OnBuildingPressed), multiTapCollectProfitBehaviour);
                return;
            }
            FullProfitCollectBehaviour fullProfitCollectBehaviour;
            if ((fullProfitCollectBehaviour = (profitCollectBehaviour as FullProfitCollectBehaviour)) != null)
            {
                GridTileIconType icon = fullProfitCollectBehaviour.Maxed ? GridTileIconType.Profit : GridTileIconType.LowProfit;
                this._gridTileIconManager.SetIcon<ButtonGridTileIcon>(icon).Init(new Action(this.OnBuildingPressed));
            }
        }
    }

    private void HideProfitIcon()
    {
        this._gridTileIconManager.RemoveIcon(GridTileIconType.Profit);
        this._gridTileIconManager.RemoveIcon(GridTileIconType.LowProfit);
        this._gridTileIconManager.RemoveIcon(GridTileIconType.MultiTapCollect);
    }

    private void HideConvertIcon()
    {
        this._gridTileIconManager.RemoveIcon(GridTileIconType.CommercialConvert);
    }

    private Currencies GetMultipliedProfit(Currencies profit)
    {
        Currencies currencies = profit * this._multipliers.GetMultiplier(MultiplierType.CommercialBuildingProfit);
        currencies.SetValue("XP", currencies.GetValue("XP") * this._multipliers.GetMultiplier(MultiplierType.XP));
        return currencies;
    }

    public override StorageDictionary Serialize()
    {
        StorageDictionary storageDictionary = base.Serialize();
        storageDictionary.Set("maxEmployees", this._currentMaxEmployees);
        storageDictionary.Set("employees", this._currentEmployees);
        storageDictionary.Set<Currencies>("profit", this._currentProfit);
        storageDictionary.SetOrRemoveStorable<UpspeedableProcess>("ProfitProcess", this._profitProcess, this._profitProcess == null);
        storageDictionary.SetOrRemoveStorable<CurrencyConversionProcess>("CurrencyConversion", this.CurrencyConversionProcess, this.CurrencyConversionProcess == null);
        storageDictionary.SetOrRemoveStorable<ProfitCollectBehaviour>("ProfitCollectBehaviour", this._profitCollectBehaviour, this._profitCollectBehaviour == null);
        if (this._profitCollectBehaviour != null)
        {
            storageDictionary.Set("ProfitBehaviourType", (int)this._profitCollectBehaviour.ProfitCollectBehaviourType);
        }
        return storageDictionary;
    }

    protected override void Deserialize(StorageDictionary storage)
    {
        base.Deserialize(storage);
        this._currentMaxEmployees = storage.Get<int>("maxEmployees", 0);
        this._currentEmployees = storage.Get<int>("employees", 0);
        this._currentProfit = storage.GetModel<Currencies>("profit", (StorageDictionary sd) => new Currencies(sd), new Currencies());
        if (storage.Contains("ProfitProcess"))
        {
            this._profitProcess = new UpspeedableProcess(storage.GetStorageDict("ProfitProcess"), this._timing, this._multipliers, this._gameState);
        }
        if (storage.Contains("CurrencyConversion"))
        {
            this.CurrencyConversionProcess = new CurrencyConversionProcess(storage.GetStorageDict("CurrencyConversion"), this._timing, this._multipliers, this._gameState);
        }
        ProfitCollectBehaviour.BehaviourType behaviourType = (ProfitCollectBehaviour.BehaviourType)storage.Get<int>("ProfitBehaviourType", -1);
        if (behaviourType == ProfitCollectBehaviour.BehaviourType.Full)
        {
            this._profitCollectBehaviour = storage.GetModel<FullProfitCollectBehaviour>("ProfitCollectBehaviour", (StorageDictionary sd) => new FullProfitCollectBehaviour(sd), null);
            return;
        }
        if (behaviourType != ProfitCollectBehaviour.BehaviourType.MultiTap)
        {
            this._profitCollectBehaviour = null;
            return;
        }
        this._profitCollectBehaviour = storage.GetModel<MultiTapProfitCollectBehaviour>("ProfitCollectBehaviour", (StorageDictionary sd) => new MultiTapProfitCollectBehaviour(sd), null);
    }

    private static readonly Vector3 FromScale = new Vector3(1f, 1f, 1f);

    private static readonly Vector3 FromScaleMirrored = new Vector3(-1f, 1f, 1f);

    private static readonly Vector3 ToScale = new Vector3(1.1f, 1.1f, 1f);

    private static readonly Vector3 ToScaleMirrored = new Vector3(-1.1f, 1.1f, 1f);

    [SerializeField]
    private Tweener _profitScaleTweener;

    [SerializeField]
    private TransformScaleTweenerTrack _scaleTweenerTrack;

    private int _currentEmployees;

    private int _currentMaxEmployees;

    private Currencies _currentProfit = new Currencies();

    private IEnumerator _profitLoopRoutine;

    private IEnumerator _employeesBehaviourRoutine;

    private UpspeedableProcess _profitProcess;

    private ProfitCollectBehaviour _profitCollectBehaviour;

    private static int _extraCollectTimes;

    private const string MaxEmployeesKey = "maxEmployees";

    private const string EmployeesKey = "employees";

    private const string ProfitKey = "profit";

    private const string ProfitProcessKey = "ProfitProcess";

    private const string CurrencyConversionKey = "CurrencyConversion";

    private const string ProfitCollectBehaviourKey = "ProfitCollectBehaviour";

    private const string ProfitCollectBehaviourTypeKey = "ProfitBehaviourType";

    public delegate void CurrencyConversionStateChangedEventHandler(bool active);

    private enum ProfitRoutineStateType
    {
        WaitForProfit,
        WaitForConversion,
        WaitForCollect
    }
}
