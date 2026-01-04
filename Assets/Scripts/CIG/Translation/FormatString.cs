using System;

namespace CIG.Translation
{
	public class FormatString : ILocalizedString
	{
		private ILocalizedString _format;

		private ILocalizedString[] _args;

		public FormatString(ILocalizedString format, params ILocalizedString[] args)
		{
			if (format == null)
			{
				throw new ArgumentNullException("SUISS Localization: format");
			}
			_format = format;
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
			return "[FormatString=" + _format.ToString() + ",Args=[" + string.Join(";", array) + "]]";
		}

		public string Translate()
		{
			int num = _args.Length;
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = _args[i].Translate();
			}
			string format = _format.Translate();
			object[] args = array;
			return string.Format(format, args);
		}
	}
}
