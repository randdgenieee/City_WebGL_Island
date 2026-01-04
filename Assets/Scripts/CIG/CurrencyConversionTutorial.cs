namespace CIG
{
	public class CurrencyConversionTutorial : IslandTutorial
	{
		public enum TutorialState
		{
			None,
			IntroText,
			OpenBuilding,
			ShowButtons
		}

		private readonly CurrencyConversionManager _currencyConversionManager;

		private TutorialState _state;

		public override TutorialType TutorialType => TutorialType.CurrencyConversion;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _tutorialManager.InitialTutorialFinished && _currencyConversionManager.FeatureEnabled)
				{
					return FindCommercialBuilding();
				}
				return false;
			}
		}

		public override bool CanQuit => false;

		public TutorialState State
		{
			get
			{
				return _state;
			}
			private set
			{
				_state = value;
				StateChanged((int)_state);
			}
		}

		public CIGCommercialBuilding CommercialBuilding
		{
			get;
			private set;
		}

		public CurrencyConversionTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap, CurrencyConversionManager currencyConversionManager)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
			_state = TutorialState.None;
			_currencyConversionManager = currencyConversionManager;
		}

		public override void Begin()
		{
			base.Begin();
			State = TutorialState.IntroText;
		}

		public void TextDismissed()
		{
			switch (_state)
			{
			case TutorialState.OpenBuilding:
				break;
			case TutorialState.None:
				State = TutorialState.IntroText;
				break;
			case TutorialState.IntroText:
				State = TutorialState.OpenBuilding;
				break;
			case TutorialState.ShowButtons:
				Finish();
				break;
			}
		}

		public void UpdateBuilding()
		{
			if (!FindCommercialBuilding())
			{
				Stop();
			}
		}

		public void TileRemoved()
		{
			if (FindCommercialBuilding())
			{
				FireStateChangedEvent();
			}
			else
			{
				Stop();
			}
		}

		protected override void OnPopupOpened(PopupRequest request)
		{
			base.OnPopupOpened(request);
			BuildingPopupRequest buildingPopupRequest;
			if ((buildingPopupRequest = (request as BuildingPopupRequest)) != null && buildingPopupRequest.Content == BuildingPopupContent.Upgrade && buildingPopupRequest.Building is CIGCommercialBuilding)
			{
				State = TutorialState.ShowButtons;
			}
		}

		protected override void OnPopupClosed(PopupRequest request, bool instant)
		{
			base.OnPopupClosed(request, instant);
			BuildingPopupRequest buildingPopupRequest;
			if (_state == TutorialState.ShowButtons && (buildingPopupRequest = (request as BuildingPopupRequest)) != null && buildingPopupRequest.Content == BuildingPopupContent.Upgrade && buildingPopupRequest.Building is CIGCommercialBuilding)
			{
				Finish();
			}
		}

		private bool FindCommercialBuilding()
		{
			IsometricIsland current = IsometricIsland.Current;
			if (current != null && CommercialBuilding == null)
			{
				int i = 0;
				for (int count = current.BuildingsOnIsland.Count; i < count; i++)
				{
					CIGCommercialBuilding cIGCommercialBuilding = current.BuildingsOnIsland[i] as CIGCommercialBuilding;
					if (cIGCommercialBuilding != null && cIGCommercialBuilding.State == BuildingState.Normal && !cIGCommercialBuilding.IsUpgrading)
					{
						CommercialBuilding = cIGCommercialBuilding;
						break;
					}
				}
			}
			return CommercialBuilding != null;
		}
	}
}
