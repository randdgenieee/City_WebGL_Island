using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class BuildRoadTutorialView : TutorialView<BuildRoadTutorial>
	{
		private RectTransform _roadsButton;

		private RoadBuilder _subscribedRoadBuilder;

		protected override bool CanShow
		{
			get
			{
				if (base.CanShow && base.CanShowOnIsland)
				{
					return !_popupManagerView.IsShowingPopup;
				}
				return false;
			}
		}

		public void Initialize(BuildRoadTutorial tutorial, TutorialDialog tutorialDialog, TutorialPointer tutorialPointer, PopupManagerView popupManagerView, WorldMapView worldMapView, IslandsManagerView islandsManagerView, RectTransform roadsButton)
		{
			_roadsButton = roadsButton;
			Initialize(tutorial, tutorialDialog, tutorialPointer, popupManagerView, worldMapView, islandsManagerView);
			if (BuildingOfTypeHasRoad())
			{
				_tutorial.OnRoadBuilt();
			}
		}

		public override void Deinitialize()
		{
			if (_subscribedRoadBuilder != null)
			{
				_subscribedRoadBuilder.RoadBuiltEvent -= OnRoadBuilt;
				_subscribedRoadBuilder = null;
			}
			base.Deinitialize();
		}

		protected override void Show()
		{
			_tutorialDialog.Show(Localization.Key("tutorial_build_road"), TutorialDialog.AdvisorPositionType.Right, useOverlay: false, useContinueButton: false, null);
			_tutorialPointer.Show(this, _roadsButton);
		}

		protected override void OnIslandChanged(IsometricIsland isometricIsland)
		{
			if (_subscribedRoadBuilder != null)
			{
				_subscribedRoadBuilder.RoadBuiltEvent -= OnRoadBuilt;
				_subscribedRoadBuilder = null;
			}
			base.OnIslandChanged(isometricIsland);
			if (_subscribedIsland != null)
			{
				_subscribedRoadBuilder = _subscribedIsland.RoadBuilder;
				_subscribedRoadBuilder.RoadBuiltEvent += OnRoadBuilt;
			}
		}

		private void OnRoadBuilt(Road road)
		{
			if (BuildingOfTypeHasRoad())
			{
				_tutorial.OnRoadBuilt();
			}
		}

		private bool BuildingOfTypeHasRoad()
		{
			if (_subscribedIsland != null)
			{
				int i = 0;
				for (int count = _subscribedIsland.BuildingsOnIsland.Count; i < count; i++)
				{
					Building building = _subscribedIsland.BuildingsOnIsland[i];
					if (building is CIGResidentialBuilding && building.HasRoad)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
