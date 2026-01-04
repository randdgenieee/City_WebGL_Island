using System;

namespace CIG.Translation
{
	public class RawTimeSpanString : ILocalizedString
	{
		private TimeSpan _dateTime;

		private bool _hidePartWhenZero;

		private int _showParts;

		public RawTimeSpanString(TimeSpan dateTime, bool hidePartWhenZero, int showParts)
		{
			_dateTime = dateTime;
			_hidePartWhenZero = hidePartWhenZero;
			_showParts = showParts;
		}

		public override string ToString()
		{
			return "[TimeSpan=" + _dateTime.ToString() + ",HideZero=" + _hidePartWhenZero + ",#Parts=" + _showParts + "]";
		}

		public string Translate()
		{
			ILocalizedString localizedString = null;
			ILocalizedString localizedString2 = null;
			ILocalizedString localizedString3 = null;
			ILocalizedString localizedString4 = null;
			if ((double)_dateTime.Days > 0.0)
			{
				localizedString = Localization.Format(Localization.Key("day_symbol"), Localization.Literal($"{_dateTime.Days:D}"));
				if (_showParts >= 2 && (_dateTime.Hours > 0 || !_hidePartWhenZero))
				{
					localizedString2 = Localization.Format(Localization.Key("hour_symbol"), Localization.Literal($"{_dateTime.Hours:D}"));
				}
				if (_showParts >= 3 && (_dateTime.Minutes > 0 || !_hidePartWhenZero))
				{
					localizedString3 = Localization.Format(Localization.Key("minute_symbol"), Localization.Literal($"{_dateTime.Minutes:D}"));
				}
				if (_showParts >= 4 && (_dateTime.Seconds > 0 || !_hidePartWhenZero))
				{
					localizedString4 = Localization.Format(Localization.Key("second_symbol"), Localization.Literal($"{_dateTime.Seconds:D}"));
				}
			}
			else if (_dateTime.Hours > 0)
			{
				localizedString = Localization.Format(Localization.Key("hour_symbol"), Localization.Literal($"{_dateTime.Hours:D}"));
				if (_showParts >= 2 && (_dateTime.Minutes > 0 || !_hidePartWhenZero))
				{
					localizedString2 = Localization.Format(Localization.Key("minute_symbol"), Localization.Literal($"{_dateTime.Minutes:D}"));
				}
				if (_showParts >= 3 && (_dateTime.Seconds > 0 || !_hidePartWhenZero))
				{
					localizedString3 = Localization.Format(Localization.Key("second_symbol"), Localization.Literal($"{_dateTime.Seconds:D}"));
				}
			}
			else
			{
				localizedString = ((_dateTime.Minutes > 0) ? Localization.Format(Localization.Key("minute_symbol"), Localization.Literal($"{_dateTime.Minutes:D}")) : ((_showParts < 2) ? Localization.Format(Localization.Key("second_symbol"), Localization.Literal($"{_dateTime.Seconds:D}")) : Localization.Literal(string.Empty)));
				if (_showParts >= 2 && (_dateTime.Seconds > 0 || !_hidePartWhenZero))
				{
					localizedString2 = Localization.Format(Localization.Key("second_symbol"), Localization.Literal($"{_dateTime.Seconds:D}"));
				}
			}
			if (localizedString2 == null)
			{
				return localizedString.Translate();
			}
			if (localizedString3 == null)
			{
				return Localization.Concat(localizedString, Localization.Literal(" "), localizedString2).Translate();
			}
			if (localizedString4 == null)
			{
				return Localization.Concat(localizedString, Localization.Literal(" "), localizedString2, Localization.Literal(" "), localizedString3).Translate();
			}
			return Localization.Concat(localizedString, Localization.Literal(" "), localizedString2, Localization.Literal(" "), localizedString3, Localization.Literal(" "), localizedString4).Translate();
		}
	}
}
