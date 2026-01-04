namespace CIG
{
	public class LevelUpPopupRequest : PopupRequest
	{
		public int Level
		{
			get;
		}

		public Currencies Reward
		{
			get;
		}

		public LevelUpPopupRequest(int level, Currencies reward)
			: base(typeof(LevelUpPopup), enqueue: true, dismissable: false, showModalBackground: true, firstInQueue: false, HUDRegionType.All)
		{
			Level = level;
			Reward = reward;
		}
	}
}
