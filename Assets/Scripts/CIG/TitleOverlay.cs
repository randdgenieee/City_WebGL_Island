using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class TitleOverlay : Overlay
	{
		[SerializeField]
		private LocalizedText _title;

		[SerializeField]
		private LocalizedText _subtitle;

		[SerializeField]
		private GameObject _boostContainer;

		[SerializeField]
		private LocalizedText _boostText;

		public void Initialize(ILocalizedString titleText, ILocalizedString subtitleText = null, ILocalizedString boostText = null)
		{
			_title.LocalizedString = titleText;
			if (Localization.IsNullOrEmpty(subtitleText))
			{
				_subtitle.gameObject.SetActive(value: false);
			}
			else
			{
				_subtitle.gameObject.SetActive(value: true);
				_subtitle.LocalizedString = subtitleText;
			}
			if (Localization.IsNullOrEmpty(boostText))
			{
				_boostContainer.SetActive(value: false);
				return;
			}
			_boostContainer.SetActive(value: true);
			_boostText.LocalizedString = boostText;
		}
	}
}
