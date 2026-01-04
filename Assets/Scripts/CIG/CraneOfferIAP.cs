using System;
using UnityEngine;

namespace CIG
{
	public class CraneOfferIAP : CraneOffer
	{
		private readonly CraneOfferIAPProperties _properties;

		private readonly IAPStore<TOCIStoreProduct> _storeManager;

		public TOCIStoreProduct Product => _storeManager.FindProduct((TOCIStoreProduct storeProduct) => storeProduct.Category == StoreProductCategory.CraneOffer && storeProduct.GameProductName == _properties.IAP);

		public override int Cranes => Product.CraneCount;

		public CraneOfferIAP(CraneOfferIAPProperties properties, RoutineRunner routineRunner, IAPStore<TOCIStoreProduct> storeManager, Action<CraneOffer> onStart, Action onEnd, StorageDictionary storage)
			: base(routineRunner, onStart, onEnd, storage)
		{
			_properties = properties;
			_storeManager = storeManager;
		}

		public CraneOfferIAP(CraneOfferIAPProperties properties, RoutineRunner routineRunner, IAPStore<TOCIStoreProduct> storeManager, Action<CraneOffer> onStart, Action onEnd, DateTime startDateTime, DateTime endDateTime)
			: base(routineRunner, onStart, onEnd, startDateTime, endDateTime)
		{
			_properties = properties;
			_storeManager = storeManager;
		}

		public override void StartRoutine()
		{
			if (Product != null)
			{
				base.StartRoutine();
				return;
			}
			UnityEngine.Debug.LogError("Crane offer IAP could not find '" + _properties.IAP + "' IAP.");
			EndOffer();
		}
	}
}
