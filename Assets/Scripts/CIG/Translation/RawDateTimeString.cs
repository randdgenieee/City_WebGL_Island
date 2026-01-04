using System;

namespace CIG.Translation
{
	public class RawDateTimeString : ILocalizedString
	{
		private readonly DateTime _dateTime;

		private readonly string _format;

		public RawDateTimeString(DateTime dateTime, string format)
		{
			_dateTime = dateTime;
			_format = format;
		}

		public override string ToString()
		{
			return "[Date=" + _dateTime.ToString() + ",Format=" + _format + "]";
		}

		public string Translate()
		{
			return _dateTime.ToString(_format, Localization.CurrentCulture.Info);
		}
	}
}
