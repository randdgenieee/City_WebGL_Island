namespace CIG
{
	public abstract class BuildingTutorial : IslandTutorial
	{
		public enum TutorialState
		{
			None,
			OpenShop,
			SelectTab,
			SelectBuilding,
			ConfirmBuilding,
			ConfirmBuild
		}

		private TutorialState _state;

		private bool _isBuilding;

		public TutorialState State
		{
			get
			{
				return _state;
			}
			private set
			{
				if (CanChangeState(value))
				{
					_state = value;
					StateChanged((int)_state);
				}
			}
		}

		protected BuildingTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
			_state = TutorialState.None;
		}

		public override void Begin()
		{
			base.Begin();
			State = TutorialState.OpenShop;
		}

		public void ShopPopupOpened(bool correctTab)
		{
			State = (correctTab ? TutorialState.SelectBuilding : TutorialState.SelectTab);
		}

		public void StartBuilding()
		{
			_isBuilding = true;
		}

		public void StopBuilding(bool hasBuiltBuildingOfType)
		{
			_isBuilding = false;
			if (hasBuiltBuildingOfType)
			{
				Finish();
			}
			else
			{
				State = TutorialState.OpenShop;
			}
		}

		protected override void OnPopupOpened(PopupRequest request)
		{
			if (request is BuildingPopupRequest)
			{
				State = TutorialState.ConfirmBuilding;
			}
			else if (request is BuildConfirmPopupRequest)
			{
				State = TutorialState.ConfirmBuild;
			}
			base.OnPopupOpened(request);
		}

		protected override void OnPopupClosed(PopupRequest request, bool instant)
		{
			if (!_popupManager.IsShowingPopup)
			{
				State = TutorialState.OpenShop;
			}
			base.OnPopupClosed(request, instant);
		}

		private bool CanChangeState(TutorialState newState)
		{
			switch (_state)
			{
			case TutorialState.None:
				return true;
			case TutorialState.OpenShop:
			case TutorialState.SelectTab:
			case TutorialState.SelectBuilding:
				if ((uint)newState <= 5u)
				{
					return true;
				}
				break;
			case TutorialState.ConfirmBuilding:
				if ((uint)newState <= 5u)
				{
					return true;
				}
				break;
			case TutorialState.ConfirmBuild:
				switch (newState)
				{
				case TutorialState.None:
					return true;
				case TutorialState.OpenShop:
					return !_isBuilding;
				}
				break;
			}
			return false;
		}
	}
}
