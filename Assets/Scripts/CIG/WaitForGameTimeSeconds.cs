using UnityEngine;

namespace CIG
{
	public class WaitForGameTimeSeconds : CustomYieldInstruction
	{
		private readonly Timing _timing;

		private readonly double _endTime;

		public override bool keepWaiting => _timing.GameTime < _endTime;

		public WaitForGameTimeSeconds(Timing timing, double timeToWait)
		{
			_timing = timing;
			_endTime = _timing.GameTime + timeToWait;
		}
	}
}
