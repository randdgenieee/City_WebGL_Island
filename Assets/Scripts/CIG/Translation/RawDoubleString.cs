using System.Globalization;

namespace CIG.Translation
{
	public class RawDoubleString : ILocalizedString
	{
		private double _double;

		private string _format;

		public RawDoubleString(double m, int decimals, bool showTrailingZeroes)
		{
			_double = m;
			_format = (showTrailingZeroes ? ("N" + ((decimals >= 0) ? decimals.ToString(CultureInfo.InvariantCulture) : "")) : ("0." + new string('#', decimals)));
		}

		public override string ToString()
		{
			return "[DoubleString=" + _double.ToString() + ",Format=" + _format + "]";
		}

		public string Translate()
		{
			return _double.ToString(_format, Localization.CurrentCulture.Info);
		}
	}
}
