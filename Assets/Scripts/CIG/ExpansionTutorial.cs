namespace CIG
{
	public class ExpansionTutorial : IslandTutorial
	{
		public enum TutorialState
		{
			None,
			IntroText1,
			IntroText2,
			IntroText3,
			IntroText4,
			OpenExpansion,
			BuyExpansion,
			FinishText
		}

		private readonly GameState _gameState;

		private readonly GameStats _gameStats;

		private readonly bool _expansionWarehouseTutorialsEnabled;

		private TutorialState _state;

		private const string HasGiftedCurrenciesKey = "HasGiftedCurrencies";

		public override TutorialType TutorialType => TutorialType.Expansion;

		public override bool CanBegin
		{
			get
			{
				if (base.CanBegin && _expansionWarehouseTutorialsEnabled && _gameStats.NumberOfBuildings >= 5)
				{
					return FindExpansionBlock();
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

		public ExpansionBlock ExpansionBlock
		{
			get;
			private set;
		}

		private bool HasProvidedCurrencies
		{
			get
			{
				return _storage.Get("HasGiftedCurrencies", defaultValue: false);
			}
			set
			{
				_storage.Set("HasGiftedCurrencies", value);
			}
		}

		private Currency ExpansionGoldCost => Currency.GoldCurrency(ExpansionBlock.Price.GetValue("Gold"));

		public ExpansionTutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager, WorldMap worldMap, GameState gameState, GameStats gameStats, bool expansionWarehouseTutorialsEnabled)
			: base(storage, tutorialManager, islandsManager, popupManager, worldMap)
		{
			_gameState = gameState;
			_gameStats = gameStats;
			_expansionWarehouseTutorialsEnabled = expansionWarehouseTutorialsEnabled;
		}

		public override void Release()
		{
			ExpansionBlock = null;
			base.Release();
		}

		public override void Begin()
		{
			base.Begin();
			if (!FindExpansionBlock() || (HasProvidedCurrencies && !_gameState.CanAfford(ExpansionGoldCost)))
			{
				Finish();
			}
			else
			{
				State = TutorialState.IntroText1;
			}
		}

		public void TextDismissed()
		{
			switch (_state)
			{
			case TutorialState.OpenExpansion:
			case TutorialState.BuyExpansion:
				break;
			case TutorialState.None:
				State = TutorialState.IntroText1;
				break;
			case TutorialState.IntroText1:
				State = TutorialState.IntroText2;
				break;
			case TutorialState.IntroText2:
				State = TutorialState.IntroText3;
				break;
			case TutorialState.IntroText3:
				State = TutorialState.IntroText4;
				break;
			case TutorialState.IntroText4:
				if (!HasProvidedCurrencies)
				{
					_gameState.EarnCurrencies(ExpansionGoldCost, CurrenciesEarnedReason.TutorialGift, new FlyingCurrenciesData());
					HasProvidedCurrencies = true;
				}
				State = TutorialState.OpenExpansion;
				break;
			case TutorialState.FinishText:
				Finish();
				break;
			}
		}

		public void OnIslandChanged(IsometricIsland isometricIsland)
		{
			ExpansionBlock = null;
			if (isometricIsland == null || !FindExpansionBlock())
			{
				Stop();
			}
		}

		public void ExpansionUnlocked()
		{
			State = TutorialState.FinishText;
		}

		protected override void OnPopupOpened(PopupRequest request)
		{
			base.OnPopupOpened(request);
			if (request is BuyExpansionPopupRequest)
			{
				State = TutorialState.BuyExpansion;
			}
		}

		protected override void OnPopupClosed(PopupRequest request, bool instant)
		{
			base.OnPopupClosed(request, instant);
			if (State == TutorialState.BuyExpansion && request is BuyExpansionPopupRequest)
			{
				State = TutorialState.OpenExpansion;
			}
		}

		private bool FindExpansionBlock()
		{
			IsometricIsland current = IsometricIsland.Current;
			if (current != null && (ExpansionBlock == null || !ExpansionBlock.CanUnlock))
			{
				int num = -1;
				foreach (ExpansionBlock allBlock in current.Expansions.AllBlocks)
				{
					if (allBlock.CanUnlock)
					{
						int num2 = 0;
						for (int i = allBlock.Origin.v; i < allBlock.Origin.v + allBlock.Size.v; i++)
						{
							for (int j = allBlock.Origin.u; j < allBlock.Origin.u + allBlock.Size.u; j++)
							{
								SurfaceType type = current.IsometricGrid[j, i].Type;
								if (type != SurfaceType.Water && type != 0)
								{
									num2++;
								}
							}
						}
						if (num2 > num)
						{
							ExpansionBlock = allBlock;
							num = num2;
						}
					}
				}
			}
			return ExpansionBlock != null;
		}
	}
}
