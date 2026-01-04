using CI.WSANative.Dialogs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class WSAConsentRequester : IConsentRequester
	{
		private const string ConsentRequestTimeStampKey = "WSA_ConsentRequestTimeStamp";

		private const string HasConsentKey = "WSA_HasConsent";

		private const string HasRequestedConsentKey = "WSA_HasRequestedConsent";

		public long ConsentRequestTimeStamp
		{
			get
			{
				long result = 0L;
				if (PlayerPrefs.HasKey("WSA_ConsentRequestTimeStamp"))
				{
					long.TryParse(PlayerPrefs.GetString("WSA_ConsentRequestTimeStamp"), out result);
				}
				return result;
			}
			private set
			{
				PlayerPrefs.SetString("WSA_ConsentRequestTimeStamp", value.ToString());
			}
		}

		public bool HasConsent
		{
			get
			{
				if (PlayerPrefs.HasKey("WSA_HasConsent"))
				{
					return PlayerPrefs.GetInt("WSA_HasConsent") == 1;
				}
				return false;
			}
			private set
			{
				PlayerPrefs.SetInt("WSA_HasConsent", value ? 1 : 0);
			}
		}

		public bool HasRequestedConsent
		{
			get
			{
				if (PlayerPrefs.HasKey("WSA_HasRequestedConsent"))
				{
					return PlayerPrefs.GetInt("WSA_HasRequestedConsent") == 1;
				}
				return false;
			}
			private set
			{
				PlayerPrefs.SetInt("WSA_HasRequestedConsent", value ? 1 : 0);
			}
		}

		public bool IsLocatedInEU
		{
			get
			{
				TimeZoneInfo.Local.Id.ToLower().Contains("europe");
				return true;
			}
		}

		public bool IsShowingConsentDialog
		{
			get;
			private set;
		}

		public void ResetConsent()
		{
			HasConsent = false;
			HasRequestedConsent = false;
			IsShowingConsentDialog = false;
			ConsentRequestTimeStamp = 0L;
			PlayerPrefs.Save();
		}

		public void ShowConsentDialog(string title, string message, string tosButtonLabel, string acceptButtonLabel, string quitButtonLabel, string callbackGameObjectName, string callbackMethodName)
		{
			ConsentRequestTimeStamp = (long)(AntiCheatDateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
			HasRequestedConsent = true;
			IsShowingConsentDialog = true;
			PlayerPrefs.Save();
			WSANativeDialog.ShowDialogWithOptions(title, message, new List<WSADialogCommand>
			{
				new WSADialogCommand(tosButtonLabel),
				new WSADialogCommand(acceptButtonLabel)
			}, 1, 2, delegate(WSADialogResult result)
			{
				if (result.ButtonPressed == acceptButtonLabel)
				{
					HasConsent = true;
					IsShowingConsentDialog = false;
					PlayerPrefs.Save();
					if (!string.IsNullOrEmpty(callbackGameObjectName) && !string.IsNullOrEmpty(callbackMethodName))
					{
						GameObject.Find(callbackGameObjectName).SendMessage(callbackMethodName, "Consent Given");
					}
				}
				if (result.ButtonPressed == tosButtonLabel)
				{
					Application.OpenURL("https://www.sparklingsociety.net/terms-of-service/");
					ShowConsentDialog(title, message, tosButtonLabel, acceptButtonLabel, quitButtonLabel, callbackGameObjectName, callbackMethodName);
				}
			});
		}
	}
}
