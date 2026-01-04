using System.Collections.Generic;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class TreasureChestPopup : Popup
	{
		private enum Content
		{
			Opening,
			Landmark,
			Overview
		}

		[SerializeField]
		private TreasureChestRenderer _treasureChestRenderer;

		[SerializeField]
		private RawImage _treasureChestImage;

		[SerializeField]
		private GameObject _landmarkBackground;

		[SerializeField]
		private Tweener _landmarkBackgroundTweener;

		[SerializeField]
		private TreasureChestPopupOpeningView _openingView;

		[SerializeField]
		private TreasureChestPopupLandmarkView _landmarkContentView;

		[SerializeField]
		private TreasureChestPopupContentOverview _contentOverview;

		private TreasureChestView _treasureChestView;

		private TreasureChestManager _treasureChestManager;

		private GameState _gameState;

		private GameStats _gameStats;

		private BuildingWarehouseManager _buildingWarehouseManager;

		private VideoAds3Manager _videoManager;

		private TreasureChestRewards _rewards;

		private List<RewardItemData> _rewardItemData;

		public override string AnalyticsScreenName => "treasure_chest";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_treasureChestManager = model.Game.TreasureChestManager;
			_gameState = model.Game.GameState;
			_gameStats = model.Game.GameStats;
			_buildingWarehouseManager = model.Game.BuildingWarehouseManager;
			Timing timing = model.Game.Timing;
			_openingView.Initialize(timing);
			_landmarkContentView.Initialize(timing);
			_treasureChestImage.enabled = true;
			_treasureChestImage.texture = _treasureChestRenderer.RenderTexture;
			_videoManager = model.Game.VideoAds3Manager;
		}

		protected override void OnDestroy()
		{
			if (_treasureChestView != null)
			{
				_treasureChestView.TreasureChestClosedEvent -= OnTreasureChestClosed;
				_treasureChestView.TreasureChestLeftEvent -= OnTreasureChestLeft;
				_treasureChestView.TreasureChestLandmarkOpenedEvent -= OnTreasureChestLandmarkOpened;
				_treasureChestView = null;
			}
			if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
			{
				SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
			}
			base.OnDestroy();
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
			TreasureChestPopupRequest request2 = GetRequest<TreasureChestPopupRequest>();
			_rewards = request2.Rewards;
			_rewardItemData = RewardItemData.FromReward(_rewards.Reward, _gameState, _gameStats, _buildingWarehouseManager);
			_treasureChestView = _treasureChestRenderer.Initialize(_rewards.TreasureChestType, _rewardItemData);
			_treasureChestView.TreasureChestClosedEvent += OnTreasureChestClosed;
			_treasureChestView.TreasureChestLeftEvent += OnTreasureChestLeft;
			_treasureChestView.TreasureChestLandmarkOpenedEvent += OnTreasureChestLandmarkOpened;
			_openingView.Initialize(_treasureChestView);
			_contentOverview.Initialize(_rewards, _videoManager);
			ToggleContent(Content.Opening);
			_landmarkBackgroundTweener.StopAndReset();
			_landmarkBackground.SetActive(value: false);
			_treasureChestImage.enabled = true;
		}

		public override void OnCloseClicked()
		{
			base.OnCloseClicked();
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.QuestComplete);
			_treasureChestManager.ConsumeRewards(_rewards);
		}

		protected override void Closed()
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
			if (_treasureChestView != null)
			{
				_treasureChestView.TreasureChestClosedEvent -= OnTreasureChestClosed;
				_treasureChestView.TreasureChestLeftEvent -= OnTreasureChestLeft;
				_treasureChestView.TreasureChestLandmarkOpenedEvent -= OnTreasureChestLandmarkOpened;
				_treasureChestView = null;
			}
			_treasureChestRenderer.Deinitialize();
			_openingView.Deinitialize();
			_contentOverview.Deinitialize();
			_treasureChestRenderer.Deinitialize();
			base.Closed();
		}

		protected override void OnFocusChanged()
		{
			base.OnFocusChanged();
			if (_treasureChestView != null)
			{
				_treasureChestView.SetActive(base.IsInFocus);
			}
		}

		private void ToggleContent(Content content)
		{
			_openingView.SetActive(content == Content.Opening);
			_landmarkContentView.SetActive(content == Content.Landmark);
			_contentOverview.SetActive(content == Content.Overview);
		}

		private void ShowRewards()
		{
			ToggleContent(Content.Overview);
		}

		private void OnTreasureChestClosed(bool landmark)
		{
			if (landmark)
			{
				_landmarkBackground.SetActive(value: true);
				_landmarkBackgroundTweener.PlayIfStopped();
			}
		}

		private void OnTreasureChestLeft()
		{
			ShowRewards();
		}

		private void OnTreasureChestLandmarkOpened(BuildingProperties buildingProperties)
		{
			_treasureChestImage.enabled = false;
			_landmarkContentView.Initialize(buildingProperties, ShowRewards);
			ToggleContent(Content.Landmark);
		}
	}
}
