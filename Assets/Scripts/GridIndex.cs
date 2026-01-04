using System;
using UnityEngine;

namespace CIG
{
	[Serializable]
	public struct GridIndex : IStorable
	{
		public static readonly GridIndex invalid = new GridIndex(-1, -1);

		public int u;

		public int v;

		private const string UKey = "U";

		private const string VKey = "V";

		public bool isInvalid
		{
			get
			{
				if (u >= 0)
				{
					return v < 0;
				}
				return true;
			}
		}

		public GridIndex(int u, int v)
		{
			this.u = u;
			this.v = v;
		}

		public GridIndex(GridPoint point)
		{
			u = Mathf.FloorToInt(point.U);
			v = Mathf.FloorToInt(point.V);
		}

		public GridIndex(StorageDictionary storage)
		{
			u = storage.Get("U", -1);
			v = storage.Get("V", -1);
		}

		public GridIndex GetNeighbour(Direction direction)
		{
			int num = direction.Contains(Direction.NW) ? 1 : (direction.Contains(Direction.SE) ? (-1) : 0);
			int num2 = direction.Contains(Direction.NE) ? 1 : (direction.Contains(Direction.SW) ? (-1) : 0);
			return new GridIndex(u + num, v + num2);
		}

		public override string ToString()
		{
			return $"({u},{v})";
		}

		public override int GetHashCode()
		{
			return u + 1000 * v;
		}

		public override bool Equals(object obj)
		{
			if (obj is GridIndex)
			{
				GridIndex gridIndex = (GridIndex)obj;
				if (gridIndex.u == u)
				{
					return gridIndex.v == v;
				}
				return false;
			}
			return ((ValueType)this).Equals(obj);
		}

		StorageDictionary IStorable.Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("U", u);
			storageDictionary.Set("V", v);
			return storageDictionary;
		}
	}
}
