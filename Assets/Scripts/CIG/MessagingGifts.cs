using System.Collections.Generic;

namespace CIG
{
    public class MessagingGifts
    {
        public delegate void ReceivedGiftHandler();

        private readonly List<IDictionary<string, string>> _unclaimedGifts = new List<IDictionary<string, string>>();

        public bool HasUnclaimedGift => _unclaimedGifts.Count > 0;

        public event ReceivedGiftHandler ReceivedGiftEvent;

        private void FireReceivedGiftEvent()
        {
            if (this.ReceivedGiftEvent != null)
            {
                this.ReceivedGiftEvent();
            }
        }


        public Reward ClaimGift(Properties properties)
        {
            Reward reward = new Reward();
            int i = 0;
            for (int count = _unclaimedGifts.Count; i < count; i++)
            {
                reward += Reward.ParseFromFirebaseMessage(_unclaimedGifts[i], properties);
            }
            _unclaimedGifts.Clear();
            return reward;
        }
    }
}
