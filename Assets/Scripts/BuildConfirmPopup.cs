using CIG;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BuildConfirmPopup : Popup
{
    [CompilerGenerated]
    private sealed class DisplayClass19_0
    {
        public BuildConfirmPopup _003C_003E4__this;

        public Builder builder;

        internal void g__CheckCrane0(Currency hireCost)
        {
            if (hireCost.IsValid)
            {
                _003C_003E4__this._gameState.SpendCurrencies(hireCost, CurrenciesSpentReason.CraneHire, b__2);
            }
            else
            {
                g__Finish1(success: true);
            }
        }

        internal void b__2(bool success, Currencies spent)
        {
            g__Finish1(success);
        }

        internal void g__Finish1(bool success)
        {
            if (success)
            {
                _003C_003E4__this._buildConfirmed = true;
                builder.FinishBuild(_003C_003E4__this._onBuildConfirmed);
            }
            _003C_003E4__this.OnCloseClicked();
        }
    }

    [SerializeField]
    private Button _mirrorButton;

    private BuildingsManager _buildingsManager;

    private CraneManager _craneManager;

    private GameState _gameState;

    private BuildFinishType _finishType;

    private Action<CIGBuilding> _onBuildConfirmed;

    private bool _isNewBuilding;

    private bool _buildConfirmed;

    private Currency _price;

    public override string AnalyticsScreenName => "build_confirm";

    public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
    {
        base.Initialize(model, canvasScaler);
        _buildingsManager = model.Game.BuildingsManager;
        _craneManager = model.Game.CraneManager;
        _gameState = model.Game.GameState;
    }

    public override void Open(PopupRequest request)
    {
        base.Open(request);
        _buildConfirmed = false;
        BuildConfirmPopupRequest request2 = GetRequest<BuildConfirmPopupRequest>();
        _price = request2.Cost;
        _finishType = request2.FinishType;
        _onBuildConfirmed = request2.OnBuildConfirmed;
        _mirrorButton.interactable = request2.BuildingProperties.CanMirror;
        _isNewBuilding = request2.IsNewBuilding;
        if (request2.IsMoving)
        {
            IsometricIsland.Current.Builder.BeginMove(request2.BuildingPrefab, request2.MoveCameraToTarget);
        }
        else
        {
            IsometricIsland.Current.Builder.BeginBuild(request2.BuildingProperties, request2.MoveCameraToTarget, _isNewBuilding, request2.IsCashBuilding);
        }
    }

    public override void Close(bool instant)
    {
        TryCancelBuild();
        _buildConfirmed = false;
        base.Close(instant);
    }

    public void OnFinishClicked()
    {
        switch (_finishType)
        {
            case BuildFinishType.Normal:
                NormalFinish();
                break;
            case BuildFinishType.Instant:
                InstantFinish();
                break;
            case BuildFinishType.Warehouse:
                WarehouseFinish();
                break;
        }
    }

    public void OnCancelClicked()
    {
        CancelAndClose();
    }

    public void OnFlipClicked()
    {
        Builder builder = IsometricIsland.Current.Builder;
        builder.UpdateMirrored(!builder.PreviewTile.Mirrored);
    }

    private void NormalFinish()
    {
        Builder builder = IsometricIsland.Current.Builder;
        if (builder.IsBuilding)
        {
            if (builder.CanBuild())
            {
                if (builder.PreviewTile.Status == GridTile.GridTileStatus.Moving)
                {
                    _buildConfirmed = true;
                    builder.FinishBuild(_onBuildConfirmed);
                    OnCloseClicked();
                }
                else
                {
                    CIGBuilding building = builder.CurrentBuilding;
                    _craneManager.CheckCraneAvailable(delegate (Currency hireCost)
                    {
                        BuildAction(building, hireCost.IsValid ? hireCost.Value : decimal.Zero);
                    }, CancelAndClose, building.BuildingProperties.ConstructionCranes);
                }
            }
        }
        else
        {
            CancelAndClose();
        }
    }

    private void InstantFinish()
    {
        Builder builder = IsometricIsland.Current.Builder;
        if (builder.IsBuilding)
        {
            if (builder.CanBuild() && builder.PreviewTile.Status == GridTile.GridTileStatus.Preview)
            {
                _buildingsManager.BuyBuilding(builder.CurrentBuilding.BuildingProperties, _price, decimal.Zero, instant: true, delegate (bool success)
                {
                    if (success)
                    {
                        _buildConfirmed = true;
                        builder.FinishBuild(delegate (CIGBuilding builtBuilding)
                        {
                            builtBuilding.SpeedupConstruction();
                            EventTools.Fire(_onBuildConfirmed, builtBuilding);
                        });
                        SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PlaceBuilding);
                        OnCloseClicked();
                    }
                });
            }
        }
        else
        {
            CancelAndClose();
        }
    }

    private void WarehouseFinish()
    {
        DisplayClass19_0 displayClass19_ = new DisplayClass19_0();
        displayClass19_._003C_003E4__this = this;
        displayClass19_.builder = IsometricIsland.Current.Builder;
        if (displayClass19_.builder.IsBuilding)
        {
            if (displayClass19_.builder.CanBuild())
            {
                if (_isNewBuilding)
                {
                    _craneManager.CheckCraneAvailable(displayClass19_.g__CheckCrane0, OnCloseClicked, displayClass19_.builder.CurrentBuilding.BuildingProperties.ConstructionCranes);
                }
                else
                {
                    displayClass19_.g__Finish1(success: true);
                }
            }
        }
        else
        {
            CancelAndClose();
        }
    }

    private void TryCancelBuild()
    {
        Builder builder = IsometricIsland.Current.Builder;
        if (builder.IsBuilding && !_buildConfirmed)
        {
            builder.CancelBuild();
        }
    }

    private void BuildAction(CIGBuilding building, decimal extraGoldCost)
    {
        _buildingsManager.BuyBuilding(building.BuildingProperties, _price, extraGoldCost, instant: false, delegate (bool success)
        {
            if (success)
            {
                _buildConfirmed = true;
                EventTools.Fire(_onBuildConfirmed, building);
                SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.PlaceBuilding);
                IsometricIsland.Current.Builder.FinishBuild();
                OnCloseClicked();
            }
        });
    }

    private void CancelAndClose()
    {
        TryCancelBuild();
        OnCloseClicked();
    }
}
