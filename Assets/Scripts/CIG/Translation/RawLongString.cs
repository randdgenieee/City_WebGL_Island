namespace CIG.Translation
{
	public class RawLongString : ILocalizedString
	{
		private long _long;

		public RawLongString(long l)
		{
			_long = l;
		}

		public override string ToString()
		{
			return "[LongString=" + _long.ToString() + "]";
		}

		public string Translate()
		{
			return _long.ToString("N0", Localization.CurrentCulture.Info);
		}
	}
}
