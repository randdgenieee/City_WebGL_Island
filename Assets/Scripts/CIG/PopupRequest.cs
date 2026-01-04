using System;

namespace CIG
{
	public abstract class PopupRequest
	{
		public virtual bool IsValid => true;

		public Type PopupType
		{
			get;
		}

		public bool Enqueue
		{
			get;
		}

		public bool FirstInQueue
		{
			get;
		}

		public bool Dismissable
		{
			get;
			protected set;
		}

		public bool ShowModalBackground
		{
			get;
		}

		public HUDRegionType HiddenHudRegion
		{
			get;
		}

		protected PopupRequest(Type popupType, bool enqueue = true, bool dismissable = true, bool showModalBackground = true, bool firstInQueue = false, HUDRegionType hiddenHudRegion = HUDRegionType.Quests | HUDRegionType.ShopButton | HUDRegionType.RoadsButton | HUDRegionType.MapButton | HUDRegionType.MinigamesButton | HUDRegionType.LeaderboardButton | HUDRegionType.SocialButton | HUDRegionType.SettingsButton | HUDRegionType.MagnifyingGlassButton | HUDRegionType.UpgradesButton | HUDRegionType.KeyDealsButton | HUDRegionType.WarehouseButton | HUDRegionType.FlyingStartDealButton)
		{
			PopupType = popupType;
			Enqueue = enqueue;
			FirstInQueue = firstInQueue;
			Dismissable = dismissable;
			ShowModalBackground = showModalBackground;
			HiddenHudRegion = hiddenHudRegion;
		}
	}
}
