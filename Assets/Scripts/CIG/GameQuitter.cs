using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class GameQuitter : MonoBehaviour
	{
		private PopupManager _popupManager;

		private PopupManagerView _popupManagerView;

		private GenericPopupRequest _popupRequest;

		private Popup _popup;

		public void Initialize(PopupManagerView popupManagerView)
		{
			_popupManagerView = popupManagerView;
			_popupManager = _popupManagerView.PopupManager;
			_popupRequest = new GenericPopupRequest("quit_game_confirm").SetTexts(Localization.Key("confirm_exit.title"), Localization.Key("confirm_exit.body")).SetGreenOkButton(CIGApp.Quit).SetRedCancelButton();
			_popupManagerView.PopupShownEvent += OnPopupShown;
			_popupManagerView.PopupHiddenEvent += OnPopupHidden;
		}

		private void OnDestroy()
		{
			if (_popupManagerView != null)
			{
				_popupManagerView.PopupShownEvent -= OnPopupShown;
				_popupManagerView.PopupHiddenEvent -= OnPopupHidden;
			}
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				if (_popupManager.IsPopupRequested(_popupRequest))
				{
					TryClosePopup();
				}
				else
				{
					_popupManager.RequestPopup(_popupRequest);
				}
			}
		}

		private void TryClosePopup()
		{
			if (_popup != null)
			{
				_popup.OnCloseClicked();
				_popup = null;
			}
		}

		private void OnPopupShown(Popup popup)
		{
			if (popup.Request == _popupRequest)
			{
				_popup = popup;
			}
		}

		private void OnPopupHidden(Popup popup)
		{
			if (popup.Request == _popupRequest)
			{
				_popup = null;
			}
		}
	}
}
