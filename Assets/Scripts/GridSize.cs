using System;

namespace CIG
{
	[Serializable]
	public struct GridSize
	{
		public static readonly GridSize One = new GridSize(1, 1);

		public int u;

		public int v;

		public GridSize(int u, int v)
		{
			this.u = u;
			this.v = v;
		}

		public override string ToString()
		{
			return $"({u}x{v})";
		}
	}
}
