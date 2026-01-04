using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class LevelUpRewardItem : MonoBehaviour
	{
		[SerializeField]
		private Image _currencyIcon;

		[SerializeField]
		private LocalizedText _rewardText;

		public void Initialize(Sprite currencyIcon, ILocalizedString rewardText)
		{
			_currencyIcon.sprite = currencyIcon;
			_rewardText.LocalizedString = rewardText;
		}
	}
}
