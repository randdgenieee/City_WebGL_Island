using CIG;
using System;
using Tweening;
using UnityEngine;

public class ReceiveRewardPopup : Popup
{
	[SerializeField]
	private FocusRewardsScrollRect _rewardsScrollRect;

	[SerializeField]
	private RewardItem _itemPrefab;

	[SerializeField]
	private RectTransform _rewardItemsParent;

	[SerializeField]
	private Tweener _headerTweener;

	private GameState _gameState;

	private GameStats _gameStats;

	private BuildingWarehouseManager _buildingWarehouseManager;

	private CraneManager _craneManager;

	private Reward _reward;

	private Action<Reward> _onCollect;

	public override string AnalyticsScreenName => "receive_currencies";

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_gameState = model.Game.GameState;
		_gameStats = model.Game.GameStats;
		_buildingWarehouseManager = model.Game.BuildingWarehouseManager;
		_craneManager = model.Game.CraneManager;
	}

	protected override void OnDestroy()
	{
		if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		base.OnDestroy();
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		ReceiveRewardPopupRequest request2 = GetRequest<ReceiveRewardPopupRequest>();
		_reward = request2.Reward;
		_onCollect = request2.OnCollect;
		CreateItems();
		SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		StartAnimation();
	}

	public override void Close(bool instant)
	{
		if (_reward != null)
		{
			_reward.Give(_gameState, _craneManager, _buildingWarehouseManager, WarehouseSource.Gift);
			if (_onCollect != null)
			{
				_onCollect(_reward);
			}
			_reward = null;
		}
		base.Close(instant);
	}

	protected override void Closed()
	{
		SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		_rewardsScrollRect.Deinitialize();
		base.Closed();
	}

	private void CreateItems()
	{
		_rewardsScrollRect.Initialize(_rewardItemsParent, _itemPrefab, RewardItemData.FromReward(_reward, _gameState, _gameStats, _buildingWarehouseManager));
	}

	private void StartAnimation()
	{
		_headerTweener.StopAndReset();
		_headerTweener.Play();
		_rewardsScrollRect.StartAnimation();
	}
}
