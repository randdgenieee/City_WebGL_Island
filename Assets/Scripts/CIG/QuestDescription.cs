using CIG.Translation;

namespace CIG
{
	public class QuestDescription
	{
		private readonly string _key;

		private readonly bool _plural;

		private readonly ILocalizedString[] _parameters;

		public QuestDescription(string key, bool plural, params ILocalizedString[] parameters)
		{
			_key = key;
			_plural = plural;
			_parameters = new ILocalizedString[parameters.Length + 1];
			int i = 0;
			for (int num = parameters.Length; i < num; i++)
			{
				_parameters[i + 1] = parameters[i];
			}
		}

		public ILocalizedString Translate(long targetAmount)
		{
			ILocalizedString format = _plural ? Localization.PluralKey(_key, targetAmount) : Localization.Key(_key);
			_parameters[0] = Localization.Integer(targetAmount);
			return Localization.Format(format, _parameters);
		}
	}
}
