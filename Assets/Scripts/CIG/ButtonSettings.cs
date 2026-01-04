using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class ButtonSettings
	{
		private readonly ILocalizedString _text;

		private readonly ILocalizedString _upperText;

		private readonly Sprite _icon;

		private readonly Action _action;

		public ButtonSettings(ILocalizedString text, ILocalizedString upperText, Sprite icon, Action action)
		{
			_text = text;
			_upperText = upperText;
			_action = action;
			_icon = icon;
		}

		public void ApplyTo(LocalizedText text, LocalizedText upperText, Image icon, out Action action)
		{
			text.LocalizedString = _text;
			action = _action;
			upperText.gameObject.SetActive(_upperText != null);
			upperText.LocalizedString = _upperText;
			icon.gameObject.SetActive(_icon != null);
			icon.sprite = _icon;
		}
	}
}
