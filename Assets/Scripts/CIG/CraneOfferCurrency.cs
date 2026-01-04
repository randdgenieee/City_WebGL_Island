using System;

namespace CIG
{
	public class CraneOfferCurrency : CraneOffer
	{
		private readonly CraneOfferCurrencyProperties _properties;

		private readonly CraneManager _craneManager;

		public override int Cranes => _properties.Cranes;

		public Currencies Price => _properties.Price;

		public CraneOfferCurrency(CraneOfferCurrencyProperties properties, RoutineRunner routineRunner, CraneManager craneManager, Action<CraneOffer> onStart, Action onEnd, StorageDictionary storage)
			: base(routineRunner, onStart, onEnd, storage)
		{
			_properties = properties;
			_craneManager = craneManager;
		}

		public CraneOfferCurrency(CraneOfferCurrencyProperties properties, RoutineRunner routineRunner, CraneManager craneManager, Action<CraneOffer> onStart, Action onEnd, DateTime startDateTime, DateTime endDateTime)
			: base(routineRunner, onStart, onEnd, startDateTime, endDateTime)
		{
			_properties = properties;
			_craneManager = craneManager;
		}

		public void RewardOffer()
		{
			_craneManager.AddCranes(_properties.Cranes);
			if (base.IsActive)
			{
				EndOffer();
			}
		}
	}
}
