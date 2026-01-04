using UnityEngine;

namespace CIG
{
	public class WaitForUnscaledTimeSeconds : CustomYieldInstruction
	{
		private readonly double _endTime;

		public override bool keepWaiting => Timing.UtcNow < _endTime;

		public WaitForUnscaledTimeSeconds(double timeToWait)
		{
			_endTime = Timing.UtcNow + timeToWait;
		}
	}
}
