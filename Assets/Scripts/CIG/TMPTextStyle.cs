using System;
using UnityEngine;

namespace CIG
{
	[Serializable]
	public class TMPTextStyle
	{
		[SerializeField]
		private Material _material;

		public Material Material => _material;
	}
}
