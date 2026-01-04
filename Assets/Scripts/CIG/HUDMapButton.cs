using UnityEngine;

namespace CIG
{
	public class HUDMapButton : HUDRegionElement
	{
		[SerializeField]
		private GameObject _badge;

		private GameStats _gameStats;

		private WorldMap _worldMap;

		private IslandsManager _islandsManager;

		public void Initialize(GameStats gameStats, WorldMap worldMap, IslandsManager islandsManager)
		{
			_gameStats = gameStats;
			_worldMap = worldMap;
			_islandsManager = islandsManager;
			if (_badge != null)
			{
				_gameStats.ValueChangedEvent += OnGameStateValueChanged;
				_worldMap.VisibilityChangedEvent += OnWorldMapVisibilityChanged;
			}
		}

		private void OnDestroy()
		{
			if (_gameStats != null)
			{
				_gameStats.ValueChangedEvent -= OnGameStateValueChanged;
				_gameStats = null;
			}
			if (_worldMap != null)
			{
				_worldMap.VisibilityChangedEvent -= OnWorldMapVisibilityChanged;
				_worldMap = null;
			}
		}

		public void OnMapClicked()
		{
			_islandsManager.CloseCurrentIsland();
			IsometricIsland.Current.CameraOperator.ZoomTo(1000f, null, 1.2f);
		}

		private void OnGameStateValueChanged(string key, object oldValue, object newValue)
		{
			if (!_worldMap.IsVisible && key.StartsWith("NumberOfIslandsUnlocked") && (oldValue == null || !oldValue.Equals(newValue)))
			{
				_badge.SetActive(value: true);
			}
		}

		private void OnWorldMapVisibilityChanged(bool visible)
		{
			if (visible)
			{
				_badge.SetActive(value: false);
			}
		}
	}
}
