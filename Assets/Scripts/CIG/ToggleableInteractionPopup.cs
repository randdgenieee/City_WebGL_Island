using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public abstract class ToggleableInteractionPopup : Popup
	{
		[SerializeField]
		private List<Button> _buttons;

		private bool _interactable;

		protected bool Interactable
		{
			get
			{
				return _interactable;
			}
			set
			{
				_interactable = value;
				SetButtons(_interactable);
			}
		}

		public override void Open(PopupRequest request)
		{
			base.Open(request);
			Interactable = true;
		}

		public override void OnCloseClicked()
		{
			if (_interactable)
			{
				base.OnCloseClicked();
			}
		}

		protected void ForceClose()
		{
			base.OnCloseClicked();
		}

		private void SetButtons(bool interactable)
		{
			int i = 0;
			for (int count = _buttons.Count; i < count; i++)
			{
				_buttons[i].interactable = interactable;
			}
		}
	}
}
