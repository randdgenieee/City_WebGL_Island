using UnityEngine;

namespace CIG.Translation
{
	public class StylizedString : ILocalizedString
	{
		private const string BoldTag = "b";

		private const string ItalicTag = "i";

		private const string ColorTag = "color";

		private const string FontSizeTag = "size";

		private readonly ILocalizedString _originalString;

		private readonly string _openTag;

		private readonly string _closeTag;

		public static StylizedString Color(ILocalizedString original, Color color)
		{
			return new StylizedString(original, string.Join("=", "color", ColorUtility.ToHtmlStringRGB(color)), "color");
		}

		public static StylizedString FontSize(ILocalizedString original, int fontSize)
		{
			return new StylizedString(original, string.Join("=", "size", fontSize), "size");
		}

		public static StylizedString Bold(ILocalizedString original)
		{
			return new StylizedString(original, "b", "b");
		}

		public static StylizedString Italic(ILocalizedString original)
		{
			return new StylizedString(original, "i", "i");
		}

		private StylizedString(ILocalizedString original, string openTag, string closeTag)
		{
			_originalString = original;
			_openTag = openTag;
			_closeTag = closeTag;
		}

		public override string ToString()
		{
			return string.Concat($"[StylizedString={_originalString}, OpenTag={_openTag}, CloseTag={_closeTag}]");
		}

		public string Translate()
		{
			string text = _originalString.Translate();
			string format;
			if (Localization.IsCurrentCultureArabic)
			{
				format = ">/" + _closeTag + "< {0} >" + _openTag + "<";
				text = text.Trim(' ');
			}
			else
			{
				format = "<" + _openTag + ">{0}</" + _closeTag + ">";
			}
			return string.Format(format, text);
		}
	}
}
