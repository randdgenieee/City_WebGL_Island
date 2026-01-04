using System;
using UnityEngine;

namespace Splines
{
	[Serializable]
	public abstract class NamedVector<TVector>
	{
		[SerializeField]
		private string _name;

		[SerializeField]
		private TVector _vector;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public TVector Vector
		{
			get
			{
				return _vector;
			}
			set
			{
				_vector = value;
			}
		}

		public abstract void SnapVector(int snapPrecision);
	}
}
