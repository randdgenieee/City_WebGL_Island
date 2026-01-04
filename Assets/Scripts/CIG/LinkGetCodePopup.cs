using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
	public class LinkGetCodePopup : Popup
	{
		[SerializeField]
		private LocalizedText _codeLabel;

		[SerializeField]
		private LocalizedText _bodyText;

		[SerializeField]
		private LocalizedText _timerLabel;

		private GameSparksUser.LinkCode _linkCode;

		public override string AnalyticsScreenName => "link_get_code";

		protected override void OnDestroy()
		{
			this.CancelInvoke(UpdateTimeLeft);
			base.OnDestroy();
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			LinkGetCodePopupRequest linkGetCodePopupRequest = (LinkGetCodePopupRequest)request;
			_linkCode = linkGetCodePopupRequest.LinkCode;
			_bodyText.LocalizedString = Localization.Format(Localization.Key("link_get_code_body"), Localization.Key("link_transfer_to"), Localization.Key("link_device"));
			_codeLabel.LocalizedString = Localization.Literal(_linkCode.Code);
			this.InvokeRepeating(UpdateTimeLeft, 0f, 1f);
		}

		public override void Close(bool instant)
		{
			this.CancelInvoke(UpdateTimeLeft);
			base.Close(instant);
		}

		private void UpdateTimeLeft()
		{
			TimeSpan timeSpan = _linkCode.ExpirationDate.Subtract(AntiCheatDateTime.UtcNow);
			_timerLabel.LocalizedString = Localization.TimeSpan(timeSpan, hideSecondPartWhenZero: false);
			if (timeSpan.TotalSeconds <= 0.0)
			{
				OnCloseClicked();
			}
		}
	}
}
