using System;

namespace CIG
{
	public class ReceiveRewardPopupRequest : PopupRequest
	{
		public Reward Reward
		{
			get;
		}

		public Action<Reward> OnCollect
		{
			get;
		}

		public ReceiveRewardPopupRequest(Reward reward, bool enqueue = true, Action<Reward> onCollect = null)
			: base(typeof(ReceiveRewardPopup), enqueue, dismissable: false)
		{
			Reward = reward;
			OnCollect = onCollect;
		}
	}
}
