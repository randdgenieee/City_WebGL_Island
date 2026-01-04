using UnityEngine;

namespace CIG
{
	public class WaitForAnimationTimeSeconds : CustomYieldInstruction
	{
		private readonly Timing _timing;

		private readonly double _endTime;

		public override bool keepWaiting => _timing.AnimationTime < _endTime;

		public WaitForAnimationTimeSeconds(Timing timing, double timeToWait)
		{
			_timing = timing;
			_endTime = _timing.AnimationTime + timeToWait;
		}
	}
}
