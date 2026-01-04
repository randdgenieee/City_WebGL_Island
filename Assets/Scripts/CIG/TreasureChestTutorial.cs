namespace CIG
{
	public class TreasureChestTutorial : IslandTutorial
	{
		public enum TutorialState
		{
			None,
			OpenShopPopup,
			OpenChestTab,
			OpenChest
		}

		private readonly TreasureChestManager _treasureChestManager;

		private TutorialState _state;

		public override bool CanQuit => false;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _tutorialManager.InitialTutorialFinished)
				{
					return _treasureChestManager.HasOpenableChest;
				}
				return false;
			}
		}

		public override TutorialType TutorialType => TutorialType.TreasureChest;

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

		public TreasureChestTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap, TreasureChestManager treasureChestManager)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
			_treasureChestManager = treasureChestManager;
		}

		public override void Begin()
		{
			base.Begin();
			_treasureChestManager.ChestOpenableChangedEvent += OnChestOpenableChanged;
			_treasureChestManager.ChestOpenIntentEvent += OnChestOpenIntent;
			State = TutorialState.OpenShopPopup;
		}

		public override void Release()
		{
			if (_treasureChestManager != null)
			{
				_treasureChestManager.ChestOpenableChangedEvent -= OnChestOpenableChanged;
				_treasureChestManager.ChestOpenIntentEvent -= OnChestOpenIntent;
			}
			base.Release();
		}

		public void ShopPopupOpened(bool correctTab)
		{
			State = (correctTab ? TutorialState.OpenChest : TutorialState.OpenChestTab);
		}

		public TreasureChestType? GetOpenableChestType()
		{
			return _treasureChestManager.GetOpenableChestType();
		}

		protected override void OnPopupOpened(PopupRequest request)
		{
			if (request is ShopPopupRequest)
			{
				_treasureChestManager.ChestOpenableChangedEvent -= OnChestOpenableChanged;
			}
			else
			{
				base.OnPopupOpened(request);
			}
		}

		protected override void OnPopupClosed(PopupRequest request, bool instant)
		{
			if (request is ShopPopupRequest)
			{
				_treasureChestManager.ChestOpenableChangedEvent += OnChestOpenableChanged;
			}
			base.OnPopupClosed(request, instant);
		}

		private void OnChestOpenableChanged()
		{
			if (!_treasureChestManager.HasOpenableChest)
			{
				Stop();
			}
		}

		private void OnChestOpenIntent()
		{
			Finish();
		}
	}
}
