using UnityEngine;

namespace CIG
{
	public class TreasureChestRewardItem : RewardItem
	{
		private const float CurrencyMaxImageWidth = 112f;

		private const float BuildingMaxImageWidth = 225f;

		[SerializeField]
		private GameObject _particlesObject;

		[SerializeField]
		private CurrencyView _currencyView;

		private float _maxImageWidth;

		protected override float MaxImageWidth => _maxImageWidth;

		public override void Initialize(RewardItemData data)
		{
			if (data.BuildingProperties == null)
			{
				_currencyView.gameObject.SetActive(value: false);
				_maxImageWidth = 112f;
			}
			else
			{
				Currency price = data.Price;
				if (price.IsValid)
				{
					_currencyView.gameObject.SetActive(value: true);
					_currencyView.Initialize(price);
				}
				else
				{
					_currencyView.gameObject.SetActive(value: false);
				}
				_maxImageWidth = 225f;
			}
			_particlesObject.SetActive(data.BuildingProperties is LandmarkBuildingProperties);
			base.Initialize(data);
		}
	}
}
