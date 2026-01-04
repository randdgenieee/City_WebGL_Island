using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class NumberTweenHelper : TweenHelper
	{
		[SerializeField]
		private LocalizedText _text;

		[SerializeField]
		private bool _playIncrementSound;

		private int _previousValue;

		protected override void UpdateValue(decimal value)
		{
			_text.LocalizedString = Localization.Integer(value);
			if (_playIncrementSound && (int)value > _previousValue)
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.Click);
			}
			_previousValue = (int)value;
		}
	}
}
