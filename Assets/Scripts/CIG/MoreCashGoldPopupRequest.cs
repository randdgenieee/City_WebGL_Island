using System;

namespace CIG
{
	public class MoreCashGoldPopupRequest : PopupRequest
	{
		public Currencies PurchasePrice
		{
			get;
		}

		public int MaxGoldPrice
		{
			get;
		}

		public bool ShowBankButton
		{
			get;
		}

		public Action<bool, Currencies> PurchaseCallback
		{
			get;
		}

		public MoreCashGoldPopupRequest(Currencies purchasePrice, int maxGoldPrice, bool showBankButton, Action<bool, Currencies> purchaseCallback)
			: base(typeof(MoreCashGoldPopup), enqueue: false)
		{
			PurchasePrice = purchasePrice;
			MaxGoldPrice = maxGoldPrice;
			ShowBankButton = showBankButton;
			PurchaseCallback = purchaseCallback;
		}
	}
}
