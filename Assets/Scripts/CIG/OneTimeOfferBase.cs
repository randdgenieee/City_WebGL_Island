using System;

namespace CIG
{
	public abstract class OneTimeOfferBase
	{
		private readonly OneTimeOfferBaseProperties _properties;

		public abstract float CurrentDiscountPercentage
		{
			get;
			protected set;
		}

		public abstract decimal NormalPrice
		{
			get;
		}

		public bool OfferEnabled => _properties.Enabled;

		public decimal DiscountedPrice => GetDiscountedPrice(NormalPrice);

		protected OneTimeOfferBase(OneTimeOfferBaseProperties properties)
		{
			_properties = properties;
		}

		public abstract void IgnoreOffer();

		public abstract bool CanDealBeOffered(int level);

		protected abstract void OnOfferShown();

		protected decimal GetDiscountedPrice(decimal normalPrice)
		{
			return Math.Ceiling(normalPrice * (decimal.One - (decimal)CurrentDiscountPercentage));
		}
	}
}
