namespace CIG.Translation
{
	public class LocalizedConcat : ILocalizedString
	{
		private ILocalizedString[] _args;

		public LocalizedConcat(params ILocalizedString[] args)
		{
			_args = ((args != null) ? args : new ILocalizedString[0]);
		}

		public override string ToString()
		{
			int num = _args.Length;
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = _args[i].ToString();
			}
			return "[Concat=" + string.Join(";", array) + "]";
		}

		public string Translate()
		{
			int num = _args.Length;
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = _args[i].Translate();
			}
			return string.Concat(array);
		}
	}
}
