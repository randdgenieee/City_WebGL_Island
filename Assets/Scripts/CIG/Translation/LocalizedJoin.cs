namespace CIG.Translation
{
	public class LocalizedJoin : ILocalizedString
	{
		private readonly ILocalizedString _seperator;

		private readonly ILocalizedString[] _args;

		public LocalizedJoin(ILocalizedString seperator, params ILocalizedString[] args)
		{
			_seperator = (seperator ?? Localization.EmptyLocalizedString);
			_args = (args ?? new ILocalizedString[0]);
		}

		public string Translate()
		{
			int num = _args.Length;
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = _args[i].Translate();
			}
			return string.Join(_seperator.Translate(), array);
		}
	}
}
