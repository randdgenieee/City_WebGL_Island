using ArabicSupport;

namespace UPersian.Utils
{
	public static class UPersianUtils
	{
		private static readonly char[] _persianNumbers = new char[10]
		{
			'۰',
			'١',
			'۲',
			'۳',
			'۴',
			'۵',
			'۶',
			'۷',
			'۸',
			'۹'
		};

		public static string RtlFix(this string str)
		{
			for (int i = 0; i < 10; i++)
			{
				str = str.Replace(i.ToString()[0], _persianNumbers[i]);
			}
			str = str.Replace('ی', 'ﻱ');
			str = str.Replace('ک', 'ﻙ');
			str = ArabicFixer.Fix(str, showTashkeel: false, useHinduNumbers: false);
			str = str.Replace('ﺃ', 'آ');
			return str;
		}

		public static bool IsRtl(this string str)
		{
			bool result = false;
			foreach (char c in str)
			{
				if ((c >= '\u0600' && c <= 'ۿ') || (c >= 'ﹰ' && c <= '\ufeff'))
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
