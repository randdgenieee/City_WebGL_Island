using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class SpecialQuestPopupRequest : PopupRequest
	{
		public string AnalyticsScreenName
		{
			get;
		}

		public SpecialQuest SpecialQuest
		{
			get;
		}

		public ILocalizedString LocalizedTitle
		{
			get;
		}

		public string Title
		{
			get;
		}

		public Sprite Icon
		{
			get;
		}

		public SpecialQuestPopupRequest(SpecialQuest specialQuest, ILocalizedString localizedTitle, string title, Sprite icon, string analyticsScreenName)
			: base(typeof(SpecialQuestPopup))
		{
			SpecialQuest = specialQuest;
			LocalizedTitle = localizedTitle;
			Title = title;
			Icon = icon;
			AnalyticsScreenName = analyticsScreenName;
		}
	}
}
