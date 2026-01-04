using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class TabView : MonoBehaviour
	{
		public delegate void TabIndexChangedEventHandler(int? oldIndex, int newIndex);

		[SerializeField]
		private List<TabButtonView> _orderedTabButtonViews;

		public int? ActiveTabIndex
		{
			get;
			private set;
		}

		public event TabIndexChangedEventHandler TabIndexChangedEvent;

		private void FireTabIndexChangedEvent(int? oldIndex)
		{
			if (this.TabIndexChangedEvent != null && ActiveTabIndex.HasValue)
			{
				this.TabIndexChangedEvent(oldIndex, ActiveTabIndex.Value);
			}
		}

		private void Awake()
		{
		}

		public void OnTabButtonClicked(TabButtonView tabButtonView)
		{
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.ButtonClick);
			SetActiveTab(_orderedTabButtonViews.IndexOf(tabButtonView));
		}

		public void SetTabVisible(int tabIndex, bool visible)
		{
			if (tabIndex > 0 && tabIndex < _orderedTabButtonViews.Count)
			{
				_orderedTabButtonViews[tabIndex].IsVisible = visible;
			}
		}

		public void SetAllTabsVisible()
		{
			int i = 0;
			for (int count = _orderedTabButtonViews.Count; i < count; i++)
			{
				_orderedTabButtonViews[i].IsVisible = true;
			}
		}

		public void SetAllTabsInvisible()
		{
			int i = 0;
			for (int count = _orderedTabButtonViews.Count; i < count; i++)
			{
				_orderedTabButtonViews[i].IsVisible = false;
			}
		}

		public void SetActiveTab(int newIndex)
		{
			if (newIndex < 0 || newIndex >= _orderedTabButtonViews.Count)
			{
				UnityEngine.Debug.LogWarning($"Cannot change ActiveTabIndex to {newIndex} because it is out of bounds[0~{_orderedTabButtonViews.Count - 1}]");
			}
			else if (newIndex != ActiveTabIndex)
			{
				int? activeTabIndex = ActiveTabIndex;
				if (activeTabIndex.HasValue)
				{
					_orderedTabButtonViews[activeTabIndex.Value].IsActive = false;
				}
				_orderedTabButtonViews[newIndex].IsActive = true;
				ActiveTabIndex = newIndex;
				FireTabIndexChangedEvent(activeTabIndex);
			}
		}

		private void SetTabIndexToFirstVisible()
		{
			int i = 0;
			for (int count = _orderedTabButtonViews.Count; i < count; i++)
			{
				if (_orderedTabButtonViews[i].IsVisible)
				{
					SetActiveTab(i);
					return;
				}
			}
			UnityEngine.Debug.LogError("Found no visible tab buttons. There always needs to be a tab button active.");
		}
	}
}
