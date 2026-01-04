using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
	public class CloudStorageConflictPopup : ToggleableInteractionPopup
	{
		[SerializeField]
		private LocalizedText _localLevel;

		[SerializeField]
		private LocalizedText _localName;

		[SerializeField]
		private LocalizedText _localDate;

		[SerializeField]
		private LocalizedText _remoteLevel;

		[SerializeField]
		private LocalizedText _remoteName;

		[SerializeField]
		private LocalizedText _remoteDate;

		private GameSparksServer _gameSparksServer;

		private GameState _gameState;

		private CloudStorageConflictPopupRequest.ConflictResolutionCallback _localAction;

		private CloudStorageConflictPopupRequest.ConflictResolutionCallback _remoteAction;

		private Action _closeAction;

		public override string AnalyticsScreenName => "cloud_storage_conflict";

		public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
		{
			base.Initialize(model, canvasScaler);
			_gameSparksServer = model.GameServer.GameSparksServer;
			_gameState = model.Game.GameState;
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			CloudStorageConflictPopupRequest request2 = GetRequest<CloudStorageConflictPopupRequest>();
			_localAction = request2.LocalAction;
			_remoteAction = request2.RemoteAction;
			_closeAction = request2.CloseAction;
			_localName.LocalizedString = Localization.Literal(_gameSparksServer.Authenticator.CurrentAuthentication.DisplayName);
			_localLevel.LocalizedString = Localization.Integer(_gameState.Level);
			_localDate.LocalizedString = GetLocalizedDate(request2.UTCSaveTime);
			_remoteName.LocalizedString = Localization.Literal(request2.RemoteGameState.DisplayName);
			_remoteLevel.LocalizedString = Localization.Integer(request2.RemoteGameState.PlayerLevel);
			_remoteDate.LocalizedString = (request2.RemoteGameState.UTCSaveTime.HasValue ? GetLocalizedDate(request2.RemoteGameState.UTCSaveTime.Value) : Localization.EmptyLocalizedString);
		}

		public override void Close(bool instant)
		{
			base.Close(instant);
			_closeAction?.Invoke();
			_closeAction = null;
		}

		public void OnLocalClicked()
		{
			base.Interactable = false;
			ShowConfirmPopup(_localAction);
		}

		public void OnRemoteClicked()
		{
			base.Interactable = false;
			ShowConfirmPopup(_remoteAction);
		}

		private void ShowConfirmPopup(CloudStorageConflictPopupRequest.ConflictResolutionCallback callback)
		{
			GenericPopupRequest request = new GenericPopupRequest("cloudstorage_conflict_confirm").SetTexts(Localization.Key("conflict_title"), Localization.Concat(Localization.Key("confirmspend.sure"), Localization.LiteralNewLineString, Localization.Key("conflict.warning"))).SetGreenOkButton(delegate
			{
				callback?.Invoke(OnConflictCallback);
			}).SetRedCancelButton(delegate
			{
				base.Interactable = true;
			})
				.SetDismissable(dismissable: false);
			_popupManager.RequestPopup(request);
		}

		private void OnConflictCallback()
		{
			_localAction = null;
			_remoteAction = null;
			base.Interactable = true;
			OnCloseClicked();
		}

		private ILocalizedString GetLocalizedDate(DateTime data)
		{
			return Localization.Concat(Localization.Literal("("), Localization.LongDateTime(data.ToLocalTime()), Localization.Literal(")"));
		}
	}
}
