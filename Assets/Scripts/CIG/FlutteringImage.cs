using UnityEngine;

namespace CIG
{
	public abstract class FlutteringImage : MonoBehaviour
	{
		public abstract Vector3 Extents
		{
			get;
		}

		public abstract void Initialize();
	}
}
