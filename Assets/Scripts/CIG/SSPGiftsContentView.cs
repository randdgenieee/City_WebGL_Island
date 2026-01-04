using CIG.Translation;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CIG
{
	public class SSPGiftsContentView : SSPMenuContentView
	{
		[SerializeField]
		private LocalizedText _friendCodeLabel;

		[SerializeField]
		private InputField _codeField;

		[SerializeField]
		private Button _redeemButton;

		[SerializeField]
		private Button _closeButton;

		private Properties _properties;

		public override ILocalizedString HeaderText => Localization.Key("SSP_MENU_GIFTS");

		public override SSPMenuPopup.SSPMenuTab Tab => SSPMenuPopup.SSPMenuTab.Gifts;

		public override void Initialize(SSPMenuPopup popup, Model model)
		{
			base.Initialize(popup, model);
			_properties = model.Game.Properties;
		}

		public void OnRedeemClicked()
		{
			string text = _codeField.text.Trim();
			if (text.Length > 0)
			{
				StartCoroutine(RedeemRoutine(text));
			}
		}

		public void OnFacebookClicked()
		{
			Application.OpenURL("https://www.facebook.com/SparklingSocietyCityBuildingGames/");
		}

		public void OnTwitterClicked()
		{
			Application.OpenURL("https://twitter.com/sprklingsociety");
		}

		public void SetFriendCode(string friendcode)
		{
			ILocalizedString localizedString = string.IsNullOrEmpty(friendcode) ? Localization.Key("social_loading") : Localization.Literal(friendcode);
			_friendCodeLabel.LocalizedString = Localization.Format(Localization.Key("loading_screen.social_your_friendcode"), localizedString);
		}

		private void DisableInput()
		{
			SpinnerManager.PushSpinnerRequest(this);
			_codeField.enabled = false;
			_redeemButton.enabled = false;
			_closeButton.enabled = false;
		}

		private void EnableInput()
		{
			_codeField.enabled = true;
			_redeemButton.enabled = true;
			_closeButton.enabled = true;
			SpinnerManager.PopSpinnerRequest(this);
		}

		private void ShowErrorPopup(ILocalizedString text)
		{
			GenericPopupRequest request = new GenericPopupRequest("gift_error").SetTexts(Localization.Key("error"), text).SetGreenOkButton();
			_popupManager.RequestPopup(request);
		}

		private IEnumerator RedeemRoutine(string code)
		{
			DisableInput();
			using (UnityWebRequest webRequest = _webService.RedeemCode(code))
			{
				yield return webRequest.SendWebRequest();
				if (!string.IsNullOrEmpty(webRequest.error))
				{
					UnityEngine.Debug.LogError("Redeem code error: " + webRequest.error);
					ShowErrorPopup(Localization.Key("social_code_error"));
				}
				else
				{
					string text = webRequest.downloadHandler.text;
					switch (text)
					{
					case "FRIENDCODE_ALREADY_USED":
						ShowErrorPopup(Localization.Key("social_code_already_used_friendcode"));
						break;
					case "FRIENDCODE_LIMIT_REACHED":
						ShowErrorPopup(Localization.Key("social_code_maximum_reached"));
						break;
					case "GIFTCODE_ALREADY_USED":
						ShowErrorPopup(Localization.Key("social_code_already_used_giftcode"));
						break;
					case "GIFTCODE_EXPIRED":
						ShowErrorPopup(Localization.Key("social_code_expired"));
						break;
					case "DB_IP == empty":
						UnityEngine.Debug.LogWarning("The gift code database does not work yet");
						goto case "UNKNOWN_CODE";
					case "UNKNOWN_CODE":
						ShowErrorPopup(Localization.Key("social_code_unknown"));
						break;
					default:
					{
						Reward reward = Reward.ParseFromServer(text, _properties);
						if (reward.IsEmpty)
						{
							UnityEngine.Debug.LogError("Result did not have a reward: '" + text + "'");
						}
						else
						{
							Analytics.LogEvent("giftcode_redeemed");
							_popup.ServerGifts.AddGift(reward);
							_popup.ServerGifts.ReceiveUndeliveredGifts();
							_popup.OnCloseClicked();
						}
						break;
					}
					}
				}
			}
			EnableInput();
		}
	}
}
