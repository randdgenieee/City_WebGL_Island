namespace CIG
{
	public class GameSceneLoader : SceneLoader
	{
		private WorldMapView _worldMapView;

		public void Initialize(WorldMapView worldMapView)
		{
			_worldMapView = worldMapView;
		}

		protected override void OnStartLoading()
		{
			if (IsometricIsland.Current != null)
			{
				IsometricIsland.Current.IslandInput.EventSystem.enabled = false;
			}
			_worldMapView.CameraOperator.EventSystem.enabled = false;
		}
	}
}
