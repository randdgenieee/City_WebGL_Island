namespace CIG.Translation
{
	public class RawIntegerString : ILocalizedString
	{
		private int _int;

		public RawIntegerString(int i)
		{
			_int = i;
		}

		public override string ToString()
		{
			return "[IntegerString=" + _int.ToString() + "]";
		}

		public string Translate()
		{
			return _int.ToString("N0", Localization.CurrentCulture.Info);
		}
	}
}
