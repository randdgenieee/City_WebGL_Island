using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	[Serializable]
	public class QuestGroup
	{
		[SerializeField]
		private List<string> _questIdentifiers;

		public List<string> QuestIdentifiers => _questIdentifiers;
	}
}
