using UnityEngine;

namespace Splines
{
	public abstract class NamedVectorList<TVectorContainer, TVector> : VectorList<TVectorContainer, TVector> where TVectorContainer : NamedVector<TVector>
	{
		public override TVector GetVector(int index)
		{
			return GetVectorContainer(index).Vector;
		}

		public TVector GetVector(string name)
		{
			TVectorContainer vectorContainer = GetVectorContainer((TVectorContainer x) => x.Name == name);
			if (vectorContainer == null)
			{
				return default(TVector);
			}
			return vectorContainer.Vector;
		}

		public string GetVectorName(int index)
		{
			return GetVectorContainer(index).Name;
		}

		public override void SetVector(int index, TVector vector)
		{
			GetVectorContainer(index).Vector = vector;
		}

		public void SetVector(int index, string name, TVector vector)
		{
			TVectorContainer vectorContainer = GetVectorContainer(index);
			vectorContainer.Name = name;
			vectorContainer.Vector = vector;
		}

		public void SetVector(string name, TVector vector)
		{
			TVectorContainer vectorContainer = GetVectorContainer((TVectorContainer x) => x.Name == name);
			if (vectorContainer == null)
			{
				UnityEngine.Debug.LogErrorFormat("Named Vector '{0}' does not exist.", name);
			}
			else
			{
				vectorContainer.Vector = vector;
			}
		}

		public bool VectorExists(string name)
		{
			return GetVectorContainer((TVectorContainer x) => x.Name == name) != null;
		}
	}
}
