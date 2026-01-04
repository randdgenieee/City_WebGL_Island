namespace CIG.Translation
{
	public class RawPercentageString : ILocalizedString
	{
		private float _percentage;

		private int _decimals;

		public RawPercentageString(float percentage, int decimals)
		{
			_percentage = percentage;
			_decimals = decimals;
		}

		public override string ToString()
		{
			return "[PercantageString=" + _percentage + ",Decimals=" + _decimals + "]";
		}

		public string Translate()
		{
			return Localization.Format(Localization.Key("percentage"), Localization.Float(_percentage, _decimals, showTrailingZeroes: false)).Translate();
		}
	}
}
