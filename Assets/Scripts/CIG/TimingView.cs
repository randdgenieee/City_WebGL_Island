namespace CIG
{
	public class TimingView : SingletonMonobehaviour<TimingView>
	{
		private Timing _timing;

		public double GameTime => _timing.GameTime;

		public void Initialize(Timing timing)
		{
			_timing = timing;
		}

		public float GetDeltaTime(DeltaTimeType deltaTimeType)
		{
			return _timing.GetDeltaTime(deltaTimeType);
		}
	}
}
