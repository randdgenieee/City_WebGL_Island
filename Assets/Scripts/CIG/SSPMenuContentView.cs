using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public abstract class SSPMenuContentView : MonoBehaviour
	{
		protected SSPMenuPopup _popup;

		protected PopupManager _popupManager;

		protected WebService _webService;

		public abstract SSPMenuPopup.SSPMenuTab Tab
		{
			get;
		}

		public abstract ILocalizedString HeaderText
		{
			get;
		}

		public virtual void Initialize(SSPMenuPopup popup, Model model)
		{
			_popup = popup;
			_popupManager = model.Game.PopupManager;
			_webService = model.GameServer.WebService;
		}

		public virtual void Deinitialize()
		{
		}

		public void Toggle(bool active)
		{
			base.gameObject.SetActive(active);
			if (active)
			{
				Open();
			}
			else
			{
				Close();
			}
		}

		protected virtual void Open()
		{
		}

		protected virtual void Close()
		{
		}
	}
}
