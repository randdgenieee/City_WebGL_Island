namespace CIG
{
	public abstract class Tutorial
	{
		public delegate void StateChangedEventHandler();

		protected readonly TutorialManager _tutorialManager;

		protected readonly IslandsManager _islandsManager;

		protected readonly PopupManager _popupManager;

		private const string IsCompletedKey = "IsCompleted";

		private const string HighestStepReachedKey = "HighestStepReached";

		protected readonly StorageDictionary _storage;

		public abstract TutorialType TutorialType
		{
			get;
		}

		public bool IsCompleted
		{
			get
			{
				return _storage.Get("IsCompleted", defaultValue: false);
			}
			private set
			{
				_storage.Set("IsCompleted", value);
			}
		}

		private int HighestStepReached
		{
			get
			{
				return _storage.Get("HighestStepReached", 0);
			}
			set
			{
				_storage.Set("HighestStepReached", value);
			}
		}

		public virtual bool CanBegin
		{
			get
			{
				if (!IsCompleted)
				{
					return !_islandsManager.IsVisiting;
				}
				return false;
			}
		}

		public abstract bool CanQuit
		{
			get;
		}

		public event StateChangedEventHandler StateChangedEvent;

		protected void FireStateChangedEvent()
		{
			this.StateChangedEvent?.Invoke();
		}

		protected Tutorial(StorageDictionary storage, TutorialManager tutorialManager, IslandsManager islandsManager, PopupManager popupManager)
		{
			_storage = storage;
			_tutorialManager = tutorialManager;
			_islandsManager = islandsManager;
			_popupManager = popupManager;
		}

		public virtual void Begin()
		{
			_popupManager.PopupOpenedEvent += OnPopupOpened;
			_popupManager.PopupClosedEvent += OnPopupClosed;
		}

		public virtual void Finish()
		{
			IsCompleted = true;
			Release();
			_tutorialManager.FinishTutorial(this);
		}

		public void Quit()
		{
			if (CanQuit)
			{
				IsCompleted = true;
				Release();
			}
		}

		public virtual void Release()
		{
			if (_popupManager != null)
			{
				_popupManager.PopupOpenedEvent -= OnPopupOpened;
				_popupManager.PopupClosedEvent -= OnPopupClosed;
			}
		}

		protected void Stop()
		{
			Release();
			_tutorialManager.StopTutorial();
		}

		protected void StateChanged(int stepNumber)
		{
			FireStateChangedEvent();
			if (HighestStepReached < stepNumber)
			{
				HighestStepReached = stepNumber;
				_tutorialManager.StepReached(this, stepNumber);
			}
		}

		protected virtual void OnPopupOpened(PopupRequest request)
		{
		}

		protected virtual void OnPopupClosed(PopupRequest request, bool instant)
		{
		}
	}
}
