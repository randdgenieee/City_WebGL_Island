using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
	public class SpeedupPopupRequest : PopupRequest
	{
		private const float MinLargeIconWidth = 88f;

		private const float MinLargeIconHeight = 88f;

		private const float MaxLargeIconWidth = 240f;

		private const float MaxLargeIconHeight = 240f;

		private const float MinSmallIconWidth = 48f;

		private const float MinSmallIconHeight = 48f;

		private const float MaxSmallIconWidth = 100f;

		private const float MaxSmallIconHeight = 100f;

		public override bool IsValid
		{
			get
			{
				if (UpspeedableProcess != null)
				{
					return UpspeedableProcess.CanSpeedup;
				}
				return false;
			}
		}

		public string AnalyticsScreenName
		{
			get;
		}

		public UpspeedableProcess UpspeedableProcess
		{
			get;
		}

		public ILocalizedString Title
		{
			get;
		}

		public ILocalizedString Subtitle
		{
			get;
		}

		public ButtonSettings CancelButton
		{
			get;
		}

		public IconSettings IconSettings
		{
			get;
		}

		private SpeedupPopupRequest(UpspeedableProcess upspeedableProcess, ILocalizedString title, ILocalizedString subtitle, ILocalizedString cancelText, Action cancelAction, Sprite cancelIcon, IconSettings iconSettings, string analyticsScreenName)
			: base(typeof(SpeedupPopup), enqueue: false)
		{
			UpspeedableProcess = upspeedableProcess;
			Title = title;
			Subtitle = subtitle;
			AnalyticsScreenName = analyticsScreenName;
			CancelButton = new ButtonSettings(cancelText, Localization.EmptyLocalizedString, cancelIcon, cancelAction);
			IconSettings = iconSettings;
		}

		public SpeedupPopupRequest(UpspeedableProcess upspeedableProcess, ILocalizedString title, ILocalizedString subtitle, ILocalizedString cancelText, Action cancelAction, Sprite cancelIcon, Sprite largeIcon, Sprite smallIcon, Sprite backgroundSprite, string analyticsScreenName)
			: this(upspeedableProcess, title, subtitle, cancelText, cancelAction, cancelIcon, new IconSettings(backgroundSprite, largeIcon, 88f, 240f, 88f, 240f, smallIcon, 48f, 100f, 48f, 100f), analyticsScreenName)
		{
		}

		public SpeedupPopupRequest(UpspeedableProcess upspeedableProcess, ILocalizedString title, ILocalizedString subtitle, ILocalizedString cancelText, Action cancelAction, Sprite cancelIcon, BuildingProperties buildingProperties, Sprite smallIcon, string analyticsScreenName)
			: this(upspeedableProcess, title, subtitle, cancelText, cancelAction, cancelIcon, new IconSettings(buildingProperties, 88f, 240f, 88f, 240f, smallIcon, 48f, 100f, 48f, 100f), analyticsScreenName)
		{
		}
	}
}
