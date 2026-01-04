using System.Globalization;

namespace CIG.Translation
{
	public class RawDecimalString : ILocalizedString
	{
		private decimal _decimal;

		private string _format;

		public RawDecimalString(decimal m, int decimals, bool showTrailingZeroes)
		{
			_decimal = m;
			_format = (showTrailingZeroes ? ("N" + ((decimals >= 0) ? decimals.ToString(CultureInfo.InvariantCulture) : "")) : ("0." + new string('#', decimals)));
		}

		public override string ToString()
		{
			return "[DecimalString=" + _decimal.ToString() + ",Format=" + _format + "]";
		}

		public string Translate()
		{
			return _decimal.ToString(_format, Localization.CurrentCulture.Info);
		}
	}
}
