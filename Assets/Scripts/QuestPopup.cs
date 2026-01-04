using CIG;
using CIG.Translation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopup : Popup
{
	private enum QuestTab
	{
		Daily,
		Ongoing
	}

	private const float DailyQuestsWidth = 552f;

	private const float OngoingQuestsWidth = 740f;

	private const float OneDayInSeconds = 86400f;

	[SerializeField]
	private TabView _tabView;

	[SerializeField]
	private LocalizedText _timerLabel;

	[SerializeField]
	private LayoutElement _questsLayoutElement;

	[SerializeField]
	private RectTransform _windowTransform;

	[SerializeField]
	private LocalizedText _titleText;

	[SerializeField]
	[Header("Daily Quests")]
	private Image _dailyQuestButtonRadial;

	[SerializeField]
	private GameObject _allDailyQuestsQuestRoot;

	[SerializeField]
	private AllDailyQuestsQuestItem _allDailyQuestsQuestItem;

	[SerializeField]
	private RectTransform _flyingQuestObjectParent;

	[SerializeField]
	private FlyingQuestObject _flyingQuestObjectPrefab;

	[SerializeField]
	private RecyclerGridLayoutGroup _recyclerGrid;

	private readonly Dictionary<GameObject, QuestItem> _gameObjectToItemMapping = new Dictionary<GameObject, QuestItem>();

	private readonly List<FlyingQuestObject> _flyingQuestObjects = new List<FlyingQuestObject>();

	private QuestsManager _questsManager;

	private DailyQuestsManager _dailyQuestsManager;

	private OngoingQuestsManager _ongoingQuestsManager;

	public override string AnalyticsScreenName => string.Format("quests_{0}", (CurrentRegionTab == QuestTab.Daily) ? "daily" : "ongoing");

	private QuestTab CurrentRegionTab => (QuestTab)_tabView.ActiveTabIndex.GetValueOrDefault();

	private int VisibleQuestCount
	{
		get
		{
			if (CurrentRegionTab != 0)
			{
				return _ongoingQuestsManager.OngoingQuests.Count;
			}
			return _dailyQuestsManager.DailyQuests.Count;
		}
	}

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_questsManager = model.Game.QuestsManager;
		_ongoingQuestsManager = _questsManager.OngoingQuestsManager;
		_dailyQuestsManager = _questsManager.DailyQuestsManager;
		_recyclerGrid.PushInstances();
		_recyclerGrid.Init(_ongoingQuestsManager.OngoingQuests.Count, InitializeQuestItem);
		_tabView.TabIndexChangedEvent += OnTabIndexChanged;
	}

	protected override void OnDestroy()
	{
		if (_tabView != null)
		{
			_tabView.TabIndexChangedEvent -= OnTabIndexChanged;
		}
		if (_dailyQuestsManager != null)
		{
			_dailyQuestsManager.DailyQuestsChangedEvent -= OnDailyQuestsChanged;
			_dailyQuestsManager = null;
		}
		if (SingletonMonobehaviour<FPSLimiter>.IsAvailable)
		{
			SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		}
		_ongoingQuestsManager = null;
		_questsManager = null;
		base.OnDestroy();
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		SingletonMonobehaviour<FPSLimiter>.Instance.PushUnlimitedFPSRequest(this);
		QuestPopupRequest request2 = GetRequest<QuestPopupRequest>();
		_dailyQuestsManager.DailyQuestsChangedEvent += OnDailyQuestsChanged;
		this.InvokeRepeating(UpdateDailyQuestRadial, 0f, 1f, realtime: true);
		if (request2.OpenDailyQuests)
		{
			ShowDailyQuests();
		}
		else
		{
			ShowOngoingQuests();
		}
	}

	protected override void Closed()
	{
		SingletonMonobehaviour<FPSLimiter>.Instance.PopUnlimitedFPSRequest(this);
		int i = 0;
		for (int count = _flyingQuestObjects.Count; i < count; i++)
		{
			UnityEngine.Object.Destroy(_flyingQuestObjects[i].gameObject);
		}
		_flyingQuestObjects.Clear();
		if (_dailyQuestsManager != null)
		{
			_dailyQuestsManager.DailyQuestsChangedEvent -= OnDailyQuestsChanged;
		}
		foreach (KeyValuePair<GameObject, QuestItem> item in _gameObjectToItemMapping)
		{
			item.Value.Deinitialize();
		}
		_allDailyQuestsQuestItem.Deinitialize();
		this.CancelInvoke(UpdateTimer);
		this.CancelInvoke(UpdateDailyQuestRadial);
		base.Closed();
	}

	private bool InitializeQuestItem(GameObject go, int index)
	{
		if (index < 0 || index >= VisibleQuestCount)
		{
			return false;
		}
		Quest quest = (CurrentRegionTab == QuestTab.Daily) ? _dailyQuestsManager.DailyQuests[index] : _ongoingQuestsManager.OngoingQuests[index];
		QuestItem item = GetItem(go);
		item.CollectAnimationFinishedEvent -= OnQuestItemCollectAnimationFinished;
		item.Initialize(quest);
		if (CurrentRegionTab == QuestTab.Daily)
		{
			item.CollectAnimationFinishedEvent += OnQuestItemCollectAnimationFinished;
		}
		return true;
	}

	private void UpdateView()
	{
		_recyclerGrid.Refresh();
		_recyclerGrid.MaxChildren = VisibleQuestCount;
	}

	private QuestItem GetItem(GameObject instance)
	{
		if (!_gameObjectToItemMapping.TryGetValue(instance, out QuestItem value))
		{
			value = (_gameObjectToItemMapping[instance] = instance.GetComponent<QuestItem>());
		}
		return value;
	}

	private void UpdateTimer()
	{
		_timerLabel.LocalizedString = Localization.FullTimeSpan(_questsManager.TimeLeft, hidePartWhenZero: false);
	}

	private void UpdateDailyQuestRadial()
	{
		_dailyQuestButtonRadial.fillAmount = 1f - (float)_questsManager.TimeLeft.TotalSeconds / 86400f;
	}

	private void SetWindow(float size, float xOffset)
	{
		_questsLayoutElement.preferredWidth = size;
		Vector2 anchoredPosition = _windowTransform.anchoredPosition;
		anchoredPosition.x = xOffset;
		_windowTransform.anchoredPosition = anchoredPosition;
	}

	private void ShowDailyQuests()
	{
		if (!this.IsInvoking(UpdateTimer))
		{
			this.InvokeRepeating(UpdateTimer, 0f, 1f, realtime: true);
		}
		_tabView.SetActiveTab(0);
		_titleText.LocalizedString = Localization.Key("quests.title_daily");
		_timerLabel.gameObject.SetActive(value: true);
		SetWindow(552f, -50f);
		UpdateView();
		_allDailyQuestsQuestItem.Initialize(_dailyQuestsManager.AllDailyQuestsQuest);
		_allDailyQuestsQuestRoot.SetActive(value: true);
	}

	private void ShowOngoingQuests()
	{
		this.CancelInvoke(UpdateTimer);
		_tabView.SetActiveTab(1);
		_titleText.LocalizedString = Localization.Key("quests.title");
		_timerLabel.gameObject.SetActive(value: false);
		SetWindow(740f, 0f);
		UpdateView();
		_allDailyQuestsQuestRoot.SetActive(value: false);
	}

	private void OnDailyQuestsChanged()
	{
		if (_tabView.ActiveTabIndex == 0)
		{
			ShowDailyQuests();
		}
	}

	private void OnTabIndexChanged(int? oldIndex, int newIndex)
	{
		if (oldIndex != newIndex)
		{
			if (newIndex == 0)
			{
				ShowDailyQuests();
			}
			else
			{
				ShowOngoingQuests();
			}
			PushScreenView();
		}
	}

	private void OnQuestItemCollectAnimationFinished(Vector3 from, QuestSpriteType questSpriteType)
	{
		FlyingQuestObject flyingQuestObject = Object.Instantiate(_flyingQuestObjectPrefab, _flyingQuestObjectParent);
		_flyingQuestObjects.Add(flyingQuestObject);
		flyingQuestObject.Initialize(questSpriteType, from, _allDailyQuestsQuestItem.GetNextTarget().position, OnFlyingQuestObjectCompleted);
	}

	private void OnFlyingQuestObjectCompleted(FlyingQuestObject flyingQuestObject)
	{
		_flyingQuestObjects.Remove(flyingQuestObject);
		_allDailyQuestsQuestItem.OnAnimationFinished();
	}
}
