using System;

namespace CIG
{
	[Flags]
	public enum RoadType
	{
		None = 0x0,
		Road = 0x1,
		Path = 0x2,
		River = 0x4,
		RoadOverWater = 0x5,
		RoadOverPath = 0x3,
		PathOverWater = 0x6,
		Any = 0x7
	}
}
