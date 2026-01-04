using System;

namespace CIG
{
	[Flags]
	public enum SaleType
	{
		None = 0x0,
		CashSale = 0x1,
		GoldSale = 0x2,
		CashGoldSale = 0x3
	}
}
