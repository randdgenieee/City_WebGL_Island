namespace CIG.Translation
{
	public class RawFirstCharCapitalizedString : ILocalizedString
	{
		private ILocalizedString _original;

		public RawFirstCharCapitalizedString(ILocalizedString original)
		{
			_original = original;
		}

		public override string ToString()
		{
			return "[FirstCharCapitalizedString=" + _original.ToString() + "]";
		}

		public string Translate()
		{
			string text = _original.Translate();
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return char.ToUpper(text[0], Localization.CurrentCulture.Info).ToString() + text.Substring(1);
		}
	}
}
