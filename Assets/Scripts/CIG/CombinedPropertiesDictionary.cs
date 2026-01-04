using System.Collections.Generic;

namespace CIG
{
	public class CombinedPropertiesDictionary : PropertiesFile
	{
		public const string NotOverridableTag = "[NotOverridable]";

		private readonly PropertiesDictionary _overriddenProperties;

		private readonly List<string> _notOverridableKeys = new List<string>();

		public CombinedPropertiesDictionary(string fileName, PropertiesDictionary overriddenProperties)
			: base(fileName)
		{
			_overriddenProperties = overriddenProperties;
			foreach (KeyValuePair<string, string> datum in _data)
			{
				if (datum.Key.StartsWith("[NotOverridable]"))
				{
					_notOverridableKeys.Add(datum.Key.Substring("[NotOverridable]".Length));
				}
			}
			int count = _notOverridableKeys.Count;
			for (int i = 0; i < count; i++)
			{
				string text = _notOverridableKeys[i];
				string oldFullKeyName = "[NotOverridable]" + text;
				RenameKey(oldFullKeyName, text);
			}
		}

		public override string GetValue(string fullKey)
		{
			if (HasOverridenFullKey(fullKey))
			{
				return _overriddenProperties.GetValue(fullKey);
			}
			return base.GetValue(fullKey);
		}

		public override bool TryGetValue(string fullKey, out string value)
		{
			if (HasOverridenFullKey(fullKey))
			{
				return _overriddenProperties.TryGetValue(fullKey, out value);
			}
			return base.TryGetValue(fullKey, out value);
		}

		private bool HasOverridenFullKey(string fullKey)
		{
			if (HasFullKey(fullKey) && _overriddenProperties.HasFullKey(fullKey))
			{
				return !_notOverridableKeys.Contains(fullKey);
			}
			return false;
		}
	}
}
