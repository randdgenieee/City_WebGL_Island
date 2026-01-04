using System;

namespace CIG.Translation
{
	public class RawLiteralString : ILocalizedString
	{
		private string _string;

		public RawLiteralString(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("SUISS Localization: s");
			}
			_string = s;
		}

		public override string ToString()
		{
			return "[Literal=" + _string + "]";
		}

		public string Translate()
		{
			return _string;
		}
	}
}
