using UnityEngine;

namespace CIG
{
	public struct GridPoint
	{
		public static readonly GridPoint One = new GridPoint(1f, 1f);

		public float U
		{
			get;
			private set;
		}

		public float V
		{
			get;
			private set;
		}

		public GridPoint Normalized
		{
			get
			{
				float num = Mathf.Sqrt(U * U + V * V) + 5E-05f;
				return new GridPoint(U / num, V / num);
			}
		}

		public GridPoint(float u, float v)
		{
			U = u;
			V = v;
		}

		public GridPoint(GridIndex index)
		{
			U = index.u;
			V = index.v;
		}

		public override string ToString()
		{
			return $"({U}, {V})";
		}

		public float DistanceTo(GridPoint other)
		{
			return Mathf.Sqrt((U - other.U) * (U - other.U) + (V - other.V) * (V - other.V));
		}

		public static GridPoint GetRandom(float urange, float vrange)
		{
			return new GridPoint(Random.Range(0f - urange, urange), Random.Range(0f - vrange, vrange));
		}

		public static GridPoint Lerp(GridPoint a, GridPoint b, float t)
		{
			return new GridPoint(Mathf.Lerp(a.U, b.U, t), Mathf.Lerp(a.V, b.V, t));
		}

		public static GridPoint operator -(GridPoint left, GridPoint right)
		{
			return new GridPoint(left.U - right.U, left.V - right.V);
		}

		public static GridPoint operator +(GridPoint left, GridPoint right)
		{
			return new GridPoint(left.U + right.U, left.V + right.V);
		}

		public static GridPoint operator +(GridPoint left, GridIndex right)
		{
			return new GridPoint(left.U + (float)right.u, left.V + (float)right.v);
		}

		public static GridPoint operator +(GridIndex left, GridPoint right)
		{
			return right + left;
		}

		public static GridPoint operator *(GridPoint index, float scalar)
		{
			return new GridPoint(index.U * scalar, index.V * scalar);
		}
	}
}
