using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
	public class GenericPopupRequest : PopupRequest
	{
		private const float MinLargeIconWidth = 88f;

		private const float MinLargeIconHeight = 88f;

		private const float MaxLargeIconWidth = 240f;

		private const float MaxLargeIconHeight = 240f;

		private const float MinSmallIconWidth = 48f;

		private const float MinSmallIconHeight = 48f;

		private const float MaxSmallIconWidth = 128f;

		private const float MaxSmallIconHeight = 128f;

		public string AnalyticsScreenName
		{
			get;
		}

		public ILocalizedString Title
		{
			get;
			protected set;
		}

		public ILocalizedString Subtitle
		{
			get;
			private set;
		}

		public ILocalizedString Body
		{
			get;
			protected set;
		}

		public ButtonSettings GreenButton
		{
			get;
			private set;
		}

		public ButtonSettings RedButton
		{
			get;
			private set;
		}

		public Action CloseAction
		{
			get;
			protected set;
		}

		public IconSettings IconSettings
		{
			get;
			private set;
		}

		public GenericPopupRequest(string analyticsScreenName, bool enqueue = false)
			: base(typeof(GenericPopup), enqueue)
		{
			AnalyticsScreenName = analyticsScreenName;
		}

		public GenericPopupRequest SetDismissable(bool dismissable, Action action = null)
		{
			base.Dismissable = dismissable;
			CloseAction = action;
			return this;
		}

		public GenericPopupRequest SetTexts(ILocalizedString title, ILocalizedString body, ILocalizedString subtitle = null)
		{
			Title = title;
			Body = body;
			Subtitle = subtitle;
			return this;
		}

		public GenericPopupRequest SetGreenButton(ILocalizedString text, ILocalizedString upperText = null, Action action = null, Sprite icon = null)
		{
			GreenButton = new ButtonSettings(text, upperText, icon, action);
			return this;
		}

		public GenericPopupRequest SetGreenOkButton(Action action = null, Sprite icon = null)
		{
			GreenButton = new ButtonSettings(Localization.Key("ok"), null, icon, action);
			return this;
		}

		public GenericPopupRequest SetRedButton(ILocalizedString text, ILocalizedString upperText = null, Action action = null, Sprite icon = null)
		{
			RedButton = new ButtonSettings(text, upperText, icon, action);
			return this;
		}

		public GenericPopupRequest SetRedCancelButton(Action action = null, Sprite icon = null)
		{
			RedButton = new ButtonSettings(Localization.Key("cancel"), null, icon, action);
			return this;
		}

		public GenericPopupRequest SetIcon(Sprite largeIcon, Sprite smallIcon = null, Sprite iconBackground = null)
		{
			IconSettings = new IconSettings(iconBackground, largeIcon, 88f, 240f, 88f, 240f, smallIcon, 48f, 128f, 48f, 128f);
			return this;
		}

		public GenericPopupRequest SetIcon(BuildingProperties buildingProperties, Sprite smallIcon = null, Sprite iconBackground = null)
		{
			IconSettings = new IconSettings(buildingProperties, 88f, 240f, 88f, 240f, smallIcon, 48f, 128f, 48f, 128f);
			return this;
		}

		public static GenericPopupRequest UnavailableSurfaceTypePopupRequest(PopupManager popupManager, IslandsManager islandsManager, BuildingProperties buildingProperties)
		{
			Sprite background = SingletonMonobehaviour<SurfaceSpriteAssetCollection>.Instance.GetAsset(buildingProperties.SurfaceType).Background;
			ILocalizedString localizedString = buildingProperties.SurfaceType.ToLocalizedString();
			ILocalizedString subtitle = Localization.Format(Localization.Key("has_to_be_built_on_x"), localizedString);
			ILocalizedString body = Localization.Format(Localization.Key("no_x_available_explore"), localizedString);
			Action action = delegate
			{
				popupManager.CloseAllOpenPopups(instant: true);
				islandsManager.CloseCurrentIsland();
			};
			return new GenericPopupRequest("unavailable_surface").SetTexts(buildingProperties.LocalizedName, body, subtitle).SetGreenButton(Localization.Key("explore"), null, action).SetRedButton(Localization.Key("perhaps_later"))
				.SetIcon(buildingProperties, SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.BalloonIcon), background);
		}

		public static GenericPopupRequest GameSparksErrorPopup(string errorCodeKey = "social_code_error")
		{
			return new GenericPopupRequest("gs_error").SetTexts(Localization.Key("error"), Localization.Key(errorCodeKey)).SetGreenOkButton();
		}
	}
}
