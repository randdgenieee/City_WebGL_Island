namespace CIG
{
	public class LinkDevicePopup : ToggleableInteractionPopup
	{
		private Settings _settings;

		private GameSparksServer _gameSparksServer;

		public override string AnalyticsScreenName => "link_device";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_settings = model.Device.Settings;
			_gameSparksServer = model.GameServer.GameSparksServer;
		}

		public void OnGetCode()
		{
			if (base.Interactable)
			{
				base.Interactable = false;
				_settings.ToggleAuthenticationAllowed(on: true);
				_gameSparksServer.AuthenticationController.AutoAuthenticate(delegate
				{
					_gameSparksServer.GameSparksUser.RequestLinkCode(OnCodeSuccess, OnCodeError);
				}, OnAuthenticationError);
			}
		}

		public void OnUseCode()
		{
			_popupManager.RequestPopup(new LinkUseCodePopupRequest());
		}

		private void OnAuthenticationError()
		{
			base.Interactable = true;
			_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup());
		}

		private void OnCodeSuccess(GameSparksUser.LinkCode linkCode)
		{
			base.Interactable = true;
			_popupManager.RequestPopup(new LinkGetCodePopupRequest(linkCode));
		}

		private void OnCodeError(GameSparksException exc)
		{
			base.Interactable = true;
			GameSparksUtils.LogGameSparksError(exc);
			_popupManager.RequestPopup(GenericPopupRequest.GameSparksErrorPopup());
		}
	}
}
