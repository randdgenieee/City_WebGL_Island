using CIG;
using CIG.Translation;
using System;
using UnityEngine;

public class LanguageItem : MonoBehaviour
{
	[SerializeField]
	private LocalizedText _nameText;

	[SerializeField]
	private GameObject _button;

	private Localization.Culture _linkedCulture;

	private Action<Localization.Culture> _onClicked;

	public void Initialize(Action<Localization.Culture> onClicked, Localization.Culture culture, bool showButton)
	{
		_onClicked = onClicked;
		_linkedCulture = culture;
		_nameText.LocalizedString = culture.NativeName;
		_button.SetActive(showButton);
	}

	public void OnClicked()
	{
		EventTools.Fire(_onClicked, _linkedCulture);
	}
}
