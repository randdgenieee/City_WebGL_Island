using System;
using UnityEngine;

namespace CIG
{
	public class WaitUntilUTCDateTime : CustomYieldInstruction
	{
		private readonly DateTime _endDateTime;

		public override bool keepWaiting => _endDateTime > AntiCheatDateTime.UtcNow;

		public WaitUntilUTCDateTime(DateTime endDateTime)
		{
			_endDateTime = endDateTime;
		}
	}
}
