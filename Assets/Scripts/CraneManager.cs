using CIG;
using CIG.Translation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class CraneManager
{
    public delegate void BuildCountChangedEventHandler(int used, int total);

    public delegate void CraneCountChangedEventHandler(int delta);

    public delegate void CraneHireCanceledEventHandler();

    public delegate void CraneAvailableAction(Currency hireCost);

    private class BuildInfo : IStorable
    {
        private readonly Timing _timing;

        private readonly double _finishTime;

        private readonly string _uniqueIdentifier;

        private const string UniqueIdentifierKey = "UniqueIdentifier";

        private const string FinishTimeKey = "FinishTime";

        public int CraneCount
        {
            get;
        }

        public bool IsFinished => _finishTime <= _timing.GameTime;

        public BuildInfo(Timing timing, string uniqueIdentifier, double finishTime, int craneCount)
        {
            _timing = timing;
            _uniqueIdentifier = uniqueIdentifier;
            _finishTime = finishTime;
            CraneCount = craneCount;
        }

        public override string ToString()
        {
            return $"({_finishTime}, {CraneCount})";
        }

        public BuildInfo(StorageDictionary storage, Timing timing)
        {
            _timing = timing;
            _uniqueIdentifier = storage.Get("UniqueIdentifier", string.Empty);
            _finishTime = storage.Get("FinishTime", 0.0);
            CraneCount = storage.Get("craneCount", 0);
        }

        StorageDictionary IStorable.Serialize()
        {
            StorageDictionary storageDictionary = new StorageDictionary();
            storageDictionary.Set("UniqueIdentifier", _uniqueIdentifier);
            storageDictionary.Set("FinishTime", _finishTime);
            storageDictionary.Set("craneCount", CraneCount);
            return storageDictionary;
        }
    }

    [CompilerGenerated]
    private sealed class DisplayClass24_0
    {
        public Action cancelAction;

        public CraneManager _003C_003E4__this;

        public CraneAvailableAction craneAvailableAction;

        internal void g__CancelAction0()
        {
            cancelAction?.Invoke();
            _003C_003E4__this.FireCraneCancelledEventHandler();
        }
    }

    [CompilerGenerated]
    private sealed class DisplayClass24_1
    {
        public Currency hireCost;

        public DisplayClass24_0 CS_0024_003C_003E8__locals1;

        internal void b__1()
        {
            CS_0024_003C_003E8__locals1.craneAvailableAction?.Invoke(hireCost);
        }
    }

    private const int InitialCraneCount = 5;

    private readonly StorageDictionary _storage;

    private readonly Timing _timing;

    private readonly PopupManager _popupManager;

    private readonly Dictionary<string, BuildInfo> _buildings;

    private const string BuildingsKey = "Buildings";

    private const string CraneCountKey = "craneCount";

    public int MaxBuildCount
    {
        get;
        private set;
    }

    public int CurrentBuildCount
    {
        get
        {
            int num = 0;
            foreach (KeyValuePair<string, BuildInfo> building in _buildings)
            {
                BuildInfo value = building.Value;
                if (!value.IsFinished)
                {
                    num += value.CraneCount;
                }
            }
            return num;
        }
    }

    public event BuildCountChangedEventHandler BuildCountChangedEvent;

    public event CraneCountChangedEventHandler CraneCountChangedEvent;

    public event CraneHireCanceledEventHandler CraneHireCanceledEvent;

    private void FireBuildCountChangedEvent()
    {
        this.BuildCountChangedEvent?.Invoke(CurrentBuildCount, MaxBuildCount);
    }

    private void FireCraneCountChangedEvent(int delta)
    {
        this.CraneCountChangedEvent?.Invoke(delta);
    }

    private void FireCraneCancelledEventHandler()
    {
        this.CraneHireCanceledEvent?.Invoke();
    }

    public CraneManager(StorageDictionary storage, Timing timing, PopupManager popupManager)
    {
        _storage = storage;
        _timing = timing;
        _popupManager = popupManager;
        _buildings = _storage.GetDictionaryModels("Buildings", (StorageDictionary sd) => new BuildInfo(sd, _timing));
        MaxBuildCount = _storage.Get("craneCount", 5);
    }

    public void StartTracking(Building b, double finishTime, int craneCount)
    {
        if (_buildings.ContainsKey(b.UniqueIdentifier))
        {
            UnityEngine.Debug.LogErrorFormat("The building '{0}' is already being tracked! The current entry will be replaced (finishTime, craneCount): {1} -> ({2}, {3})", b.BuildingProperties.BaseKey, _buildings[b.UniqueIdentifier], finishTime, craneCount);
        }
        _buildings[b.UniqueIdentifier] = new BuildInfo(_timing, b.UniqueIdentifier, finishTime, craneCount);
        FireBuildCountChangedEvent();
    }

    public void FinishTracking(Building b)
    {
        if (_buildings.Remove(b.UniqueIdentifier))
        {
            FireBuildCountChangedEvent();
        }
    }

    public void CheckCraneAvailable(CraneAvailableAction craneAvailableAction, Action cancelAction, int requiredCraneCount)
    {
        DisplayClass24_0 displayClass24_ = new DisplayClass24_0();
        displayClass24_.cancelAction = cancelAction;
        displayClass24_._003C_003E4__this = this;
        displayClass24_.craneAvailableAction = craneAvailableAction;
        if (CurrentBuildCount + requiredCraneCount <= MaxBuildCount)
        {
            displayClass24_.craneAvailableAction?.Invoke(Currency.Invalid);
            return;
        }
        DisplayClass24_1 displayClass24_2 = new DisplayClass24_1();
        displayClass24_2.CS_0024_003C_003E8__locals1 = displayClass24_;
        displayClass24_2.hireCost = Currency.GoldCurrency(decimal.One);
        ILocalizedString title = Localization.Key("build");
        ILocalizedString body = Localization.Format(Localization.Key("too_many_builds"), Localization.Integer(MaxBuildCount));
        ILocalizedString text = Localization.Format("{0} ({1})", Localization.Key("hire_extra_crane"), Localization.Concat(Localization.Integer(displayClass24_2.hireCost.Value), Localization.Key("gold")));
        GenericPopupRequest request = new GenericPopupRequest("crane_hire").SetTexts(title, body).SetDismissable(dismissable: true, displayClass24_2.CS_0024_003C_003E8__locals1.g__CancelAction0).SetGreenButton(text, null, displayClass24_2.b__1)
            .SetRedCancelButton(displayClass24_2.CS_0024_003C_003E8__locals1.g__CancelAction0)
            .SetIcon(SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.Crane));
        _popupManager.RequestPopup(request);
    }

    public void AddCranes(int count)
    {
        MaxBuildCount += count;
        FireBuildCountChangedEvent();
        FireCraneCountChangedEvent(count);
    }

    public void Serialize()
    {
        _storage.Set("Buildings", _buildings);
        _storage.Set("craneCount", MaxBuildCount);
    }
}
