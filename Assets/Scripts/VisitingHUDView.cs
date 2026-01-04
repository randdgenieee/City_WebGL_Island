using CIG;
using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

public class VisitingHUDView : MonoBehaviour
{
	[SerializeField]
	private HUDRegionUpdater _hudRegionUpdater;

	[SerializeField]
	private LocalizedText _levelText;

	[SerializeField]
	private Image _levelProgressImage;

	[SerializeField]
	private LocalizedText _nameText;

	[SerializeField]
	private HUDLikeButton _likeButton;

	[SerializeField]
	private HUDLikesAmount _likesAmount;

	[SerializeField]
	private HUDVisitingCityProgressBars _cityProgressBars;

	[SerializeField]
	private VisitingHomeButton _homeButton;

	private PopupManager _popupManager;

	private IslandsManager _islandsManager;

	private IslandVisitingManager _islandVisitingManager;

	private WorldMap _worldMap;

	private LikeRegistrar _likeRegistrar;

	public void Initialize(PopupManager popupManager, IslandsManager islandsManager, IslandVisitingManager islandVisitingManager, WorldMap worldMap, LikeRegistrar likeRegistrar)
	{
		_popupManager = popupManager;
		_islandsManager = islandsManager;
		_islandVisitingManager = islandVisitingManager;
		_worldMap = worldMap;
		_likeRegistrar = likeRegistrar;
		_worldMap.VisibilityChangedEvent += OnWorldMapVisibilityChanged;
		OnWorldMapVisibilityChanged(_worldMap.IsVisible);
		_homeButton.Initialize();
	}

	private void OnDestroy()
	{
		_popupManager = null;
		_islandsManager = null;
		_islandVisitingManager = null;
		_likeRegistrar = null;
		if (_worldMap != null)
		{
			_worldMap.VisibilityChangedEvent -= OnWorldMapVisibilityChanged;
			_worldMap = null;
		}
	}

	public void SetData()
	{
		IslandsVisitingData islandsVisitingData = _islandVisitingManager.GetIslandsVisitingData(_islandsManager.VisitingUserId);
		if (islandsVisitingData == null)
		{
			UnityEngine.Debug.LogErrorFormat("VisitingHUDView can't set it's data because there's no visiting data for user id '{0}'.", _islandsManager.VisitingUserId);
			return;
		}
		_levelText.LocalizedString = Localization.Integer(islandsVisitingData.Level);
		_levelProgressImage.fillAmount = islandsVisitingData.LevelProgress;
		_nameText.LocalizedString = Localization.Literal(islandsVisitingData.DisplayName);
		_likesAmount.Initialize(_likeRegistrar.GetLikesForUser(islandsVisitingData.UserId));
		_likeButton.Initialize(islandsVisitingData.UserId, _likeRegistrar, _likesAmount);
		_cityProgressBars.UpdateValues(islandsVisitingData.Happiness, islandsVisitingData.Population, islandsVisitingData.Housing, islandsVisitingData.Employees, islandsVisitingData.Jobs);
		_homeButton.Activate();
	}

	public void Deactivate()
	{
		_homeButton.Deactivate();
	}

	public void OnLoginClicked()
	{
		_popupManager.RequestPopup(new SSPMenuPopupRequest(SSPMenuPopup.SSPMenuTab.Login));
	}

	public void OnHomeClicked()
	{
		_islandsManager.StopVisiting();
	}

	public void OnWorldMapClicked()
	{
		_islandsManager.CloseCurrentIsland();
		IsometricIsland.Current.CameraOperator.ZoomTo(1000f, null, 1.2f);
	}

	public void OnSettingsClicked()
	{
		_popupManager.RequestPopup(new SettingsPopupRequest());
	}

	private void OnWorldMapVisibilityChanged(bool visible)
	{
		if (visible)
		{
			_hudRegionUpdater.RequestHide(this, HUDRegionType.MapButton);
		}
		else
		{
			_hudRegionUpdater.RequestShow(this);
		}
	}
}
