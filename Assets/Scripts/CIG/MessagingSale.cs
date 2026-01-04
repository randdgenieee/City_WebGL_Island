using System.Collections.Generic;

namespace CIG
{
    public class MessagingSale
    {
        public delegate void ReceivedGiftHandler();

        private IDictionary<string, string> _inactiveSale;

        public bool HasInactiveSale => _inactiveSale != null;

        public event ReceivedGiftHandler ReceivedSaleEvent;

        private void FireReceivedSaleEvent()
        {
            this.ReceivedSaleEvent?.Invoke();
        }


        public Sale ClaimSale()
        {
            Sale result = Sale.ParseFromFirebaseMessage(_inactiveSale);
            _inactiveSale = null;
            return result;
        }
    }
}
