using System;
using System.Collections.Generic;
using UnityEngine;

namespace Splines
{
	public abstract class VectorList<TVectorContainer, TVector> : MonoBehaviour
	{
		[SerializeField]
		private List<TVectorContainer> _vectors;

		public int VectorCount => _vectors.Count;

		public abstract TVector GetVector(int index);

		public abstract void SetVector(int index, TVector vector);

		public abstract Vector3 ConvertVectorToVector3(TVector vector);

		public abstract TVector ConvertVector3ToVector(Vector3 vector);

		public void SnapVectors(int snapPrecision)
		{
			int count = _vectors.Count;
			for (int i = 0; i < count; i++)
			{
				_vectors[i] = SnapVector(_vectors[i], snapPrecision);
			}
		}

		public void MoveVectors(TVector delta)
		{
			int count = _vectors.Count;
			for (int i = 0; i < count; i++)
			{
				_vectors[i] = MoveVector(_vectors[i], delta);
			}
		}

		protected abstract TVectorContainer SnapVector(TVectorContainer vectorContainer, int snapPrecision);

		protected abstract TVectorContainer MoveVector(TVectorContainer vectorContainer, TVector delta);

		protected TVectorContainer GetVectorContainer(int index)
		{
			return _vectors[Mathf.Clamp(index, 0, VectorCount)];
		}

		protected TVectorContainer GetVectorContainer(Predicate<TVectorContainer> predicate)
		{
			return _vectors.Find(predicate);
		}

		protected void SetVectorContainer(int index, TVectorContainer vectorContainer)
		{
			_vectors[Mathf.Clamp(index, 0, VectorCount)] = vectorContainer;
		}
	}
	public abstract class VectorList<TVectorAndContainer> : VectorList<TVectorAndContainer, TVectorAndContainer>
	{
	}
}
