using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class SSPNewsLetterContentView : SSPMenuContentView
	{
		[SerializeField]
		private InputField _nameField;

		[SerializeField]
		private InputField _emailField;

		[SerializeField]
		private Button _submitButton;

		private NewsletterManager _newsletterManager;

		private bool _isBusy;

		public override ILocalizedString HeaderText => Localization.Key("SSP_MENU_NEWSLETTER");

		public override SSPMenuPopup.SSPMenuTab Tab => SSPMenuPopup.SSPMenuTab.NewsLetter;

		public override void Initialize(SSPMenuPopup popup, Model model)
		{
			base.Initialize(popup, model);
			_newsletterManager = model.Game.NewsletterManager;
		}

		public void OnSubmitClicked()
		{
			Submit(_nameField.text, _emailField.text);
		}

		private void Submit(string playerName, string email)
		{
			if (_isBusy)
			{
				return;
			}
			if (!string.IsNullOrEmpty(playerName))
			{
				if (IsValidEmail(email))
				{
					SubmitEmail(email);
				}
				else
				{
					ShowErrorPopup("register_enter_emailaddress");
				}
			}
			else
			{
				ShowErrorPopup("register_enter_name");
			}
		}

		private void SetData(bool isBusy)
		{
			_submitButton.enabled = !isBusy;
		}

		private void ShowErrorPopup(string bodyLocalizationKey)
		{
			GenericPopupRequest request = new GenericPopupRequest("newsletter_error").SetTexts(Localization.Key("oops_something_went_wrong"), Localization.Key(bodyLocalizationKey)).SetGreenOkButton();
			_popupManager.RequestPopup(request);
		}

		private void SubmitEmail(string email)
		{
			_isBusy = true;
			_submitButton.interactable = false;
			SetData(isBusy: true);
			SpinnerManager.PushSpinnerRequest(this);
			_newsletterManager.SubmitEmailAsync(email, OnSubmitEmailCallback, OnSubmitEmailCallback);
		}

		private void OnSubmitEmailCallback()
		{
			_submitButton.interactable = true;
			SpinnerManager.PopSpinnerRequest(this);
			_isBusy = false;
			SetData(isBusy: false);
		}

		private static bool IsValidEmail(string strIn)
		{
			return strIn.Contains("@");
		}
	}
}
