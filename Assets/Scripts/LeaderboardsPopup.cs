using CIG;
using CIG.Translation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardsPopup : Popup
{
	public enum RegionTab
	{
		Local,
		Global
	}

	public enum PositionTab
	{
		Top,
		Around
	}

	private enum Content
	{
		Login,
		Leaderboards,
		NoConnection
	}

	private const string InfoURL = "https://forums.sparklingsociety.net/forum/games/city-island-1-premium-amp-winter/city-island-5/11991-leaderboard-information";

	private const string LoadingKey = "loading";

	private const string ErrorKey = "SSP_ERROR";

	[SerializeField]
	private TabView _regionTabView;

	[SerializeField]
	private TabView _positionTabView;

	[SerializeField]
	private RecyclerGridLayoutGroup _recyclerGrid;

	[SerializeField]
	private LocalizedText _footerTextLabel;

	[SerializeField]
	private Button _footerTextButton;

	[SerializeField]
	private LocalizedText _infoText;

	[SerializeField]
	private LocalizedText _subTitle;

	[SerializeField]
	private GameObject _leaderboardContentContainer;

	[SerializeField]
	private GameObject _loginContentContainer;

	[SerializeField]
	private GameObject _noConnectionContainer;

	[SerializeField]
	private SocialLoginButton _socialLoginButton;

	[SerializeField]
	private RectTransform _scrollView;

	private readonly Dictionary<GameObject, LeaderboardItem> _leaderboardItemCache = new Dictionary<GameObject, LeaderboardItem>();

	private Settings _settings;

	private GameSparksServer _gameSparksServer;

	private LeaderboardsManager _leaderboardsManager;

	private GameState _gameState;

	private GameStats _gameStats;

	private IslandsManager _islandsManager;

	private LikeRegistrar _likeRegistrar;

	private LeaderboardType _currentLeaderboardType;

	private Leaderboard _currentLeaderboard;

	public override string AnalyticsScreenName => string.Format("leaderboards_{0}_{1}", (CurrentPositionTab == PositionTab.Around) ? "around_me" : "top", (CurrentRegionTab == RegionTab.Global) ? "global" : "local");

	private RegionTab CurrentRegionTab => (RegionTab)_regionTabView.ActiveTabIndex.GetValueOrDefault();

	private PositionTab CurrentPositionTab => (PositionTab)_positionTabView.ActiveTabIndex.GetValueOrDefault();

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_settings = model.Device.Settings;
		_gameSparksServer = model.GameServer.GameSparksServer;
		_leaderboardsManager = model.Game.LeaderboardsManager;
		_gameState = model.Game.GameState;
		_gameStats = model.Game.GameStats;
		_islandsManager = model.Game.IslandsManager;
		_likeRegistrar = model.Game.LikeRegistrar;
		_socialLoginButton.Initialize(_popupManager, _settings, _gameSparksServer);
		_regionTabView.SetActiveTab(1);
		_positionTabView.SetActiveTab(0);
		_regionTabView.TabIndexChangedEvent += OnTabIndexChanged;
		_positionTabView.TabIndexChangedEvent += OnTabIndexChanged;
	}

	protected override void OnDestroy()
	{
		_regionTabView.TabIndexChangedEvent -= OnTabIndexChanged;
		_positionTabView.TabIndexChangedEvent -= OnTabIndexChanged;
		if (_gameSparksServer != null)
		{
			_gameSparksServer.Authenticator.AuthenticationChangedEvent -= OnAuthenticationChanged;
			CIGGameSparksInstance gameSparksInstance = _gameSparksServer.GameSparksInstance;
			gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Remove(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
			_gameSparksServer = null;
		}
		if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		if (_leaderboardsManager != null)
		{
			_leaderboardsManager.LeaderboardChangedEvent -= OnLeaderboardChanged;
			_leaderboardsManager.LeaderboardErrorEvent -= OnLeaderboardError;
			_leaderboardsManager = null;
		}
		base.OnDestroy();
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		_gameSparksServer.Authenticator.AuthenticationChangedEvent += OnAuthenticationChanged;
		CIGGameSparksInstance gameSparksInstance = _gameSparksServer.GameSparksInstance;
		gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Combine(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
		_leaderboardsManager.LeaderboardChangedEvent += OnLeaderboardChanged;
		_leaderboardsManager.LeaderboardErrorEvent += OnLeaderboardError;
		LeaderboardsPopupRequest request2 = GetRequest<LeaderboardsPopupRequest>();
		RegionTab regionTab = request2.RegionTab ?? CurrentRegionTab;
		PositionTab positionTab = request2.PositionTab ?? CurrentPositionTab;
		_regionTabView.SetActiveTab((int)regionTab);
		_positionTabView.SetActiveTab((int)positionTab);
		UpdateContent(regionTab, positionTab);
	}

	protected override void Closed()
	{
		SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		_recyclerGrid.PushInstances();
		_gameSparksServer.Authenticator.AuthenticationChangedEvent -= OnAuthenticationChanged;
		CIGGameSparksInstance gameSparksInstance = _gameSparksServer.GameSparksInstance;
		gameSparksInstance.GameSparksAvailable = (Action<bool>)Delegate.Remove(gameSparksInstance.GameSparksAvailable, new Action<bool>(OnGameSparksAvailable));
		_leaderboardsManager.LeaderboardChangedEvent -= OnLeaderboardChanged;
		_leaderboardsManager.LeaderboardErrorEvent -= OnLeaderboardError;
		base.Closed();
	}

	public void OnLoginClicked()
	{
		_popupManager.RequestPopup(new SSPMenuPopupRequest(SSPMenuPopup.SSPMenuTab.Login));
	}

	public void OnInfoClicked()
	{
		Application.OpenURL("https://forums.sparklingsociety.net/forum/games/city-island-1-premium-amp-winter/city-island-5/11991-leaderboard-information");
	}

	private void UpdateContent(RegionTab regionTab, PositionTab positionTab)
	{
		LeaderboardType leaderboardType = GetLeaderboardType(regionTab, positionTab);
		switch ((_gameSparksServer.Authenticator.CurrentAuthentication.IsAuthenticated || _leaderboardsManager.HasCachedLeaderboard(leaderboardType)) ? 1 : (_settings.SocialAuthenticationAllowed ? 2 : 0))
		{
		case 0:
			_regionTabView.SetAllTabsInvisible();
			_positionTabView.SetAllTabsInvisible();
			_loginContentContainer.SetActive(value: true);
			_leaderboardContentContainer.SetActive(value: false);
			_noConnectionContainer.SetActive(value: false);
			break;
		case 1:
			_regionTabView.SetAllTabsVisible();
			_positionTabView.SetAllTabsVisible();
			_loginContentContainer.SetActive(value: false);
			_leaderboardContentContainer.SetActive(value: true);
			_noConnectionContainer.SetActive(value: false);
			SwitchLeaderboardType(leaderboardType);
			break;
		case 2:
			_regionTabView.SetAllTabsVisible();
			_positionTabView.SetAllTabsVisible();
			_loginContentContainer.SetActive(value: false);
			_leaderboardContentContainer.SetActive(value: false);
			_noConnectionContainer.SetActive(value: true);
			break;
		}
	}

	private LeaderboardType GetLeaderboardType(RegionTab regionTab, PositionTab positionTab)
	{
		switch (regionTab)
		{
		case RegionTab.Global:
			switch (positionTab)
			{
			case PositionTab.Around:
				return LeaderboardType.GlobalAround;
			case PositionTab.Top:
				return LeaderboardType.GlobalTop;
			}
			break;
		case RegionTab.Local:
			switch (positionTab)
			{
			case PositionTab.Around:
				return LeaderboardType.CountryAround;
			case PositionTab.Top:
				return LeaderboardType.CountryTop;
			}
			break;
		}
		UnityEngine.Debug.LogError($"Could not find leaderboard type for region '{regionTab}' and position '{positionTab}'");
		return LeaderboardType.GlobalTop;
	}

	private void OnTabIndexChanged(int? oldIndex, int newIndex)
	{
		UpdateContent(CurrentRegionTab, CurrentPositionTab);
		PushScreenView();
	}

	private void SwitchLeaderboardType(LeaderboardType leaderboardType)
	{
		string key = ((uint)leaderboardType <= 1u || (uint)(leaderboardType - 2) > 1u) ? "leaderboard.global_ranking" : "leaderboard_country";
		_subTitle.LocalizedString = Localization.Key(key);
		_currentLeaderboardType = leaderboardType;
		_currentLeaderboard = null;
		SetInfoText("loading");
		_leaderboardsManager.GetLeaderboard(leaderboardType);
	}

	private void OnAuthenticationChanged(GameSparksAuthentication newAuthentication, GameSparksAuthentication previousAuthentication)
	{
		UpdateContent(CurrentRegionTab, CurrentPositionTab);
	}

	private void OnGameSparksAvailable(bool available)
	{
		UpdateContent(CurrentRegionTab, CurrentPositionTab);
	}

	private void SetInfoText(string key)
	{
		_infoText.gameObject.SetActive(key != null);
		if (key != null)
		{
			_infoText.LocalizedString = Localization.Key(key);
		}
		_recyclerGrid.gameObject.SetActive(key == null);
	}

	private void SetLeaderboardItems(LeaderboardType leaderboardType, Leaderboard leaderboard)
	{
		GameSparksAuthentication currentAuthentication = _gameSparksServer.Authenticator.CurrentAuthentication;
		string userID = currentAuthentication.UserId;
		bool flag = leaderboard.ContainsUser(userID);
		if (!flag && currentAuthentication.IsCheater)
		{
			leaderboard.AddLocalEntry(_gameSparksServer.Authenticator, _gameStats.GetIslandScore(), _gameState.Level, _gameState.GlobalPopulation);
			flag = true;
		}
		_footerTextLabel.gameObject.SetActive(!flag && (leaderboardType == LeaderboardType.CountryAround || leaderboardType == LeaderboardType.GlobalAround));
		_footerTextButton.gameObject.SetActive(!_gameSparksServer.AuthenticationController.IsSocialAuthentication(currentAuthentication));
		_recyclerGrid.PushInstances();
		_recyclerGrid.Init(leaderboard.Entries.Count, (GameObject go, int index) => SetEntry(go, index, leaderboard, userID));
		if (leaderboardType == LeaderboardType.CountryAround || (leaderboardType == LeaderboardType.GlobalAround && flag))
		{
			_recyclerGrid.ForceLayoutRefresh();
			_recyclerGrid.ScrollToElement(leaderboard.Entries.FindIndex(0, (LeaderboardEntry entry) => entry.UserId == userID), _scrollView.rect.height / 2f - _recyclerGrid.CellSize.y / 2f - _recyclerGrid.Spacing.y);
		}
		else
		{
			_recyclerGrid.ScrollToElement(0);
		}
	}

	private bool SetEntry(GameObject go, int index, Leaderboard leaderboard, string userID)
	{
		if (index < 0 || index >= leaderboard.Entries.Count)
		{
			return false;
		}
		go.SetActive(value: true);
		LeaderboardItem cachedLeaderboardItem = GetCachedLeaderboardItem(go);
		LeaderboardEntry leaderboardEntry = leaderboard.Entries[index];
		cachedLeaderboardItem.Initialize(leaderboardEntry, _popupManager, _islandsManager, _likeRegistrar, leaderboardEntry.UserId == userID);
		return true;
	}

	private LeaderboardItem GetCachedLeaderboardItem(GameObject instance)
	{
		if (!_leaderboardItemCache.ContainsKey(instance))
		{
			_leaderboardItemCache.Add(instance, instance.GetComponent<LeaderboardItem>());
		}
		return _leaderboardItemCache[instance];
	}

	private void OnLeaderboardChanged(LeaderboardType leaderboardType, Leaderboard leaderboard)
	{
		if (leaderboardType == _currentLeaderboardType)
		{
			_currentLeaderboard = leaderboard;
			if (_currentLeaderboard != null)
			{
				SetInfoText(null);
				SetLeaderboardItems(_currentLeaderboardType, _currentLeaderboard);
			}
		}
	}

	private void OnLeaderboardError(LeaderboardType leaderboardType)
	{
		if (leaderboardType == _currentLeaderboardType)
		{
			SetInfoText("SSP_ERROR");
		}
	}
}
