using System;

namespace CIG
{
	[Flags]
	public enum HUDRegionType
	{
		None = 0x0,
		Level = 0x1,
		CurrencyBars = 0x2,
		PopulationBars = 0x4,
		Quests = 0x8,
		ShopButton = 0x10,
		RoadsButton = 0x20,
		MapButton = 0x40,
		MinigamesButton = 0x80,
		LeaderboardButton = 0x100,
		SocialButton = 0x200,
		SettingsButton = 0x400,
		MagnifyingGlassButton = 0x800,
		UpgradesButton = 0x1000,
		KeyDealsButton = 0x2000,
		WarehouseButton = 0x4000,
		VisitingInfo = 0x8000,
		FlyingStartDealButton = 0x10000,
		TopRightButtons = 0x11C00,
		BottomLeft = 0xF0,
		BottomRight = 0x6300,
		TopLeft = 0xB,
		TopRight = 0x11C04,
		Top = 0x11C0F,
		Bottom = 0x63F0,
		All = 0x17FFF
	}
}
