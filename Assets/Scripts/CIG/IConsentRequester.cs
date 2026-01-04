namespace CIG
{
	public interface IConsentRequester
	{
		bool IsLocatedInEU
		{
			get;
		}

		bool HasRequestedConsent
		{
			get;
		}

		bool HasConsent
		{
			get;
		}

		long ConsentRequestTimeStamp
		{
			get;
		}

		bool IsShowingConsentDialog
		{
			get;
		}

		void ResetConsent();

		void ShowConsentDialog(string title, string message, string tosButtonLabel, string acceptButtonLabel, string quitButtonLabel, string callbackGameObjectName, string callbackMethodName);
	}
}
