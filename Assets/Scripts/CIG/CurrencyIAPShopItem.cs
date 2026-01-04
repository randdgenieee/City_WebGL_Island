using CIG.Translation;
using System;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
    public class CurrencyIAPShopItem : IAPShopItem
    {
        [Serializable]
        private class ValueBadge
        {
            [SerializeField]
            private Sprite _sprite;

            [SerializeField]
            private int _textSize;

            public Sprite Sprite => _sprite;

            public int TextSize => _textSize;
        }

        [SerializeField]
        private ShopFrameStyleView _shopFrameStyleView;

        [SerializeField]
        private CurrencyView _currencyView;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private ValueBadge[] _valueBadges;

        [SerializeField]
        private Image _valueBadgeImage;

        [SerializeField]
        private LocalizedText _valueBadgeText;

        [SerializeField]
        private GameObject _extraLabel;

        [SerializeField]
        private LocalizedText _extraLabelText;

        [SerializeField]
        private Tweener _valueBadgeTweener;

        public void Initialize(TOCIStoreProduct product, ShopType type, int index, decimal valuePercentage, Action<TOCIStoreProduct> onClick, bool saleItem)
        {
            if (index >= _valueBadges.Length) index = 0;

            Initialize(product, onClick, saleItem);
            _currencyView.Initialize(_storeProduct.Currencies.GetCurrency(0));
            SetFrameStyle(type, index);
            ValueBadge valueBadge = _valueBadges[Mathf.Clamp(index, 0, _valueBadges.Length)];
            bool flag = valuePercentage > decimal.One;
            _valueBadgeImage.gameObject.SetActive(flag);
            if (flag)
            {
                _valueBadgeImage.sprite = valueBadge.Sprite;
                _valueBadgeText.LocalizedString = Localization.Percentage((float)valuePercentage, 0);
                _valueBadgeText.TextField.fontSize = valueBadge.TextSize;
            }
            switch (index)
            {
                case 2:
                    _extraLabelText.LocalizedString = Localization.Key("iap.popular_choice");
                    break;
                case 3:
                    _extraLabelText.LocalizedString = Localization.Key("iap.best_value");
                    break;
                default:
                    _extraLabel.SetActive(value: false);
                    break;
            }
            if (saleItem)
            {
                _valueBadgeTweener.Play();
            }
        }

        private void SetFrameStyle(ShopType shopType, int index)
        {
            ShopFrameStyleType styleType;
            string key;
            if ((uint)shopType <= 1u)
            {
                styleType = ShopFrameStyleType.Gold;
                key = "gold" + (index + 1);
            }
            else
            {
                styleType = ShopFrameStyleType.Cash;
                key = "cash" + (index + 1);
            }
            _shopFrameStyleView.ApplyStyle(styleType);
            _icon.sprite = SingletonMonobehaviour<CurrencyPackSpriteAssetCollection>.Instance.GetAsset(key);
        }
    }
}
