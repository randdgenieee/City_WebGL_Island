namespace CIG.Translation
{
	public class RawCapitalizedString : ILocalizedString
	{
		private ILocalizedString _original;

		public RawCapitalizedString(ILocalizedString original)
		{
			_original = original;
		}

		public override string ToString()
		{
			return "[CapitalizedString=" + _original.ToString() + "]";
		}

		public string Translate()
		{
			return _original.Translate().ToUpper(Localization.CurrentCulture.Info);
		}
	}
}
