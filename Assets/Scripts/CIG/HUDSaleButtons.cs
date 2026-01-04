using UnityEngine;

namespace CIG
{
	public class HUDSaleButtons : MonoBehaviour
	{
		[SerializeField]
		private DateTimeTimerView _cashSaleButton;

		[SerializeField]
		private DateTimeTimerView _goldSaleButton;

		private SaleManager _saleManager;

		public void Initialize(SaleManager saleManager)
		{
			_saleManager = saleManager;
			_saleManager.SaleChangedEvent += OnSaleChanged;
			OnSaleChanged(null, _saleManager.CurrentSale);
		}

		private void OnDestroy()
		{
			if (_saleManager != null)
			{
				_saleManager.SaleChangedEvent -= OnSaleChanged;
				_saleManager = null;
			}
		}

		private void OnSaleChanged(Sale oldSale, Sale newSale)
		{
			if (newSale == null || newSale.SaleType == SaleType.None)
			{
				_cashSaleButton.StopTimer();
				_goldSaleButton.StopTimer();
				return;
			}
			if (newSale.IsCashSale)
			{
				_cashSaleButton.StartTimer(newSale.Expiration);
			}
			if (newSale.IsGoldSale)
			{
				_goldSaleButton.StartTimer(newSale.Expiration);
			}
		}
	}
}
