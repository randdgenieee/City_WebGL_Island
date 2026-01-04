using CIG.Translation;
using System;
using Tweening;
using UnityEngine;

namespace CIG
{
	public class IAPShopItem : ShopItem
	{
		[SerializeField]
		private Tweener _raysTweener;

		protected TOCIStoreProduct _storeProduct;

		protected void Initialize(TOCIStoreProduct product, Action<TOCIStoreProduct> onClick, bool playRayTweener)
		{
			_storeProduct = product;
			Initialize(delegate
			{
				onClick(_storeProduct);
			}, Localization.Literal(_storeProduct.FormattedPrice));
			if (playRayTweener)
			{
				_raysTweener.Play();
			}
		}

		public override void SetVisible(bool visible)
		{
			base.SetVisible(visible);
			if (visible)
			{
				Analytics.IAPViewed(_storeProduct.Identifier);
			}
		}
	}
}
