namespace CIG.Translation.Native
{
	public static class SystemLocale
	{
		private static string[] _langs;

		public static string[] DefaultLanguages
		{
			get
			{
				if (_langs == null)
				{
					_langs = AndroidLocale.GetLanguages();
				}
				return _langs;
			}
		}
	}
}
