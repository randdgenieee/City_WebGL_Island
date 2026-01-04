namespace CIG.Translation
{
	public class RawLowercasedString : ILocalizedString
	{
		private ILocalizedString _original;

		public RawLowercasedString(ILocalizedString original)
		{
			_original = original;
		}

		public override string ToString()
		{
			return "[LowercasedString=" + _original.ToString() + "]";
		}

		public string Translate()
		{
			return _original.Translate().ToLower(Localization.CurrentCulture.Info);
		}
	}
}
