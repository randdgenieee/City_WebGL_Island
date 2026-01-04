using CIG.Translation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class TreasureChestContentsRegularElement : MonoBehaviour
	{
		[Serializable]
		private struct Style
		{
			[SerializeField]
			private Sprite _backgroundSprite;

			[SerializeField]
			private Color _rayColor;

			[SerializeField]
			private Color _packedWithOutlineColor;

			[SerializeField]
			private Color _aColllectionOfColor;

			[SerializeField]
			private Color _currenciesShadowColor;

			[SerializeField]
			private Sprite _sprite;

			public Sprite BackgroundSprite => _backgroundSprite;

			public Color RayColor => _rayColor;

			public Color PackedWithOutlineColor => _packedWithOutlineColor;

			public Color ACollectionOfColor => _aColllectionOfColor;

			public Color CurrenciesEffectColor => _currenciesShadowColor;

			public Sprite Sprite => _sprite;
		}

		private static readonly Dictionary<string, string> LocalizationDict = new Dictionary<string, string>
		{
			["BuildingGold"] = "rare_buildings",
			["BuildingCash"] = "buildings",
			["Gold"] = "gold_capital",
			["Cash"] = "iap.tab.cash",
			["Crane"] = "cranes",
			["GoldKey"] = "gold_keys",
			["SilverKey"] = "silver_key$n",
			["LevelUp"] = "level_up$n",
			["Token"] = "token$n"
		};

		[SerializeField]
		private Image _backgroundImage;

		[SerializeField]
		private Image _rayImage;

		[SerializeField]
		private Outline _packedWithOutline;

		[SerializeField]
		private Text _aCollectionOfText;

		[SerializeField]
		private LocalizedText _currenciesText;

		[SerializeField]
		private Outline _currenciesOutline;

		[SerializeField]
		private Shadow _currenciesShadow;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private FlutteringImageSystem _goldFlutteringImageSystem;

		[SerializeField]
		private FlutteringImageSystem _cashFlutteringImageSystem;

		[SerializeField]
		private Style _silverChestStyle;

		[SerializeField]
		private Style _goldChestStyle;

		public void Initialize(Timing timing)
		{
			_goldFlutteringImageSystem.Initialize(timing);
			_cashFlutteringImageSystem.Initialize(timing);
		}

		public void Enable(TreasureChest treasureChest)
		{
			base.gameObject.SetActive(value: true);
			switch (treasureChest.Properties.TreasureChestType)
			{
			case TreasureChestType.Silver:
				ApplyStyle(_silverChestStyle);
				_cashFlutteringImageSystem.Play();
				break;
			case TreasureChestType.Gold:
				ApplyStyle(_goldChestStyle);
				_goldFlutteringImageSystem.Play();
				break;
			default:
				UnityEngine.Debug.LogError($"Treasure chest type '{treasureChest.Properties.TreasureChestType}' is not supported.");
				break;
			}
			_currenciesText.LocalizedString = BuildContentsString(treasureChest.Properties);
		}

		public void Disable()
		{
			_goldFlutteringImageSystem.Stop();
			_cashFlutteringImageSystem.Stop();
			base.gameObject.SetActive(value: false);
		}

		private void ApplyStyle(Style style)
		{
			_backgroundImage.sprite = style.BackgroundSprite;
			_rayImage.color = style.RayColor;
			_packedWithOutline.effectColor = style.PackedWithOutlineColor;
			_aCollectionOfText.color = style.ACollectionOfColor;
			_currenciesOutline.effectColor = style.CurrenciesEffectColor;
			_currenciesShadow.effectColor = style.CurrenciesEffectColor;
			_image.sprite = style.Sprite;
			_image.SetNativeSize();
		}

		private ILocalizedString BuildContentsString(TreasureChestProperties properties)
		{
			List<ILocalizedString> list = new List<ILocalizedString>();
			int i = 0;
			for (int count = properties.VisibleItems.Count; i < count; i++)
			{
				if (LocalizationDict.TryGetValue(properties.VisibleItems[i], out string value))
				{
					list.Add(Localization.Key(value));
				}
				else
				{
					UnityEngine.Debug.LogError("No localization key could be found for treasure chest visible item '" + properties.VisibleItems[i] + "'");
				}
			}
			return Localization.Join(Localization.LiteralNewLineString, list.ToArray());
		}
	}
}
