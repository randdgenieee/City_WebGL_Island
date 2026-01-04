using System.Collections.Generic;

namespace CIG
{
	public static class IslandExtensions
	{
		public static IList<IslandId> AllIslandIds
		{
			get;
		}

		static IslandExtensions()
		{
			List<IslandId> list = new List<IslandId>();
			for (int i = 0; i < 9; i++)
			{
				list.Add((IslandId)i);
			}
			AllIslandIds = list.AsReadOnly();
		}

		public static bool IsValid(this IslandId islandId)
		{
			if (islandId > IslandId.None)
			{
				return islandId < IslandId.Count;
			}
			return false;
		}

		public static int GetIndex(this IslandId islandId)
		{
			if (islandId.IsValid())
			{
				return (int)islandId;
			}
			return -1;
		}

		public static IslandId GetIsland(int index)
		{
			if (((IslandId)index).IsValid())
			{
				return (IslandId)index;
			}
			return IslandId.None;
		}
	}
}
