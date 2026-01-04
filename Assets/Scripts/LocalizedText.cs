using CIG.Translation;
using CIG.Translation.ArabicSupport;
using UnityEngine;
using UnityEngine.UI;
using UPersian.Utils;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
	[SerializeField]
	private string _staticKey = "";

	[SerializeField]
	private Text _text;

	[SerializeField]
	private bool _adjustAlignment = true;

	[SerializeField]
	private bool _toUpper;

	private ILocalizedString _value;

	private TextAnchor _originalAlignment;

	public Text TextField
	{
		get
		{
			if (_text == null)
			{
				_text = GetComponent<Text>();
			}
			_originalAlignment = _text.alignment;
			return _text;
		}
	}

	public ILocalizedString LocalizedString
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
			Apply();
		}
	}

	private void Start()
	{
		if (!string.IsNullOrEmpty(_staticKey) && LocalizedString == null)
		{
			LocalizedString = Localization.Key(_staticKey);
		}
	}

	private void Apply()
	{
		if (_value != null)
		{
			ILocalizedString localizedString = _value;
			if (_toUpper)
			{
				localizedString = Localization.ToUpper(localizedString);
			}
			string text = localizedString.Translate();
			if (Localization.IsCurrentCultureArabic)
			{
				string twoLetterISOLanguageName = Localization.CurrentCulture.Info.TwoLetterISOLanguageName;
				if (!(twoLetterISOLanguageName == "ar"))
				{
					if (twoLetterISOLanguageName == "fa")
					{
						text = text.RtlFix();
					}
				}
				else
				{
					text = ArabicFixer.Fix(text);
				}
			}
			if (_adjustAlignment)
			{
				if (Localization.CurrentCulture.Info.TextInfo.IsRightToLeft)
				{
					TextField.alignment = HorizontallySwapTextAnchor(_originalAlignment);
				}
				else
				{
					TextField.alignment = _originalAlignment;
				}
			}
			TextField.text = text;
		}
		else
		{
			TextField.text = "";
		}
	}

	private TextAnchor HorizontallySwapTextAnchor(TextAnchor anchor)
	{
		switch (anchor)
		{
		case TextAnchor.UpperLeft:
			return TextAnchor.UpperRight;
		case TextAnchor.UpperRight:
			return TextAnchor.UpperLeft;
		case TextAnchor.MiddleLeft:
			return TextAnchor.MiddleRight;
		case TextAnchor.MiddleRight:
			return TextAnchor.MiddleLeft;
		case TextAnchor.LowerLeft:
			return TextAnchor.LowerRight;
		case TextAnchor.LowerRight:
			return TextAnchor.LowerLeft;
		default:
			return anchor;
		}
	}
}
