using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
	[SerializeField]
	private Image _buttonImage;

	[SerializeField]
	private Sprite _enabledButtonSprite;

	[SerializeField]
	private Sprite _disabledButtonSprite;

	[SerializeField]
	private LocalizedText _text;

	[SerializeField]
	private Color _enabledTextColor;

	[SerializeField]
	private Color _disabledTextColor;

	[SerializeField]
	private Shadow _textShadow;

	[SerializeField]
	private Color _enabledShadowColor;

	[SerializeField]
	private Color _disabledShadowColor;

	public void SetState(bool isEnabled, ILocalizedString text)
	{
		_text.LocalizedString = text;
		if (isEnabled)
		{
			_buttonImage.sprite = _enabledButtonSprite;
			_text.TextField.color = _enabledTextColor;
			_textShadow.effectColor = _enabledShadowColor;
		}
		else
		{
			_buttonImage.sprite = _disabledButtonSprite;
			_text.TextField.color = _disabledTextColor;
			_textShadow.effectColor = _disabledShadowColor;
		}
	}
}
