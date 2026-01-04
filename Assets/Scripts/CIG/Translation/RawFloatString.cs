using System.Globalization;

namespace CIG.Translation
{
	public class RawFloatString : ILocalizedString
	{
		private float _float;

		private string _format;

		public RawFloatString(float f, int decimals, bool showTrailingZeroes)
		{
			_float = f;
			_format = (showTrailingZeroes ? ("N" + ((decimals >= 0) ? decimals.ToString(CultureInfo.InvariantCulture) : "")) : ("0." + new string('#', decimals)));
		}

		public override string ToString()
		{
			return "[FloatString=" + _float.ToString() + ",Format=" + _format + "]";
		}

		public string Translate()
		{
			return _float.ToString(_format, Localization.CurrentCulture.Info);
		}
	}
}
