using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class IslandVisitingCurrencyGridTileIcon : ButtonGridTileIcon
	{
		[SerializeField]
		private Image _icon;

		public void Initialize(Action onClick, Currency currency)
		{
			Init(onClick);
			_icon.sprite = SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(currency);
		}
	}
}
