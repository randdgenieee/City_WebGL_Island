using System.Collections.Generic;

namespace CIG
{
	public class PropertiesDictionary
	{
		private const char BaseKeySeparator = '.';

		private static readonly char[] SplitChar = new char[1]
		{
			'.'
		};

		protected Dictionary<string, string> _data;

		private Dictionary<string, List<string>> _baseKeys;

		public bool HasData => _data.Count > 0;

		protected PropertiesDictionary()
		{
		}

		public PropertiesDictionary(Dictionary<string, string> data)
		{
			_data = data;
			Initialize();
		}

		protected void Initialize()
		{
			_baseKeys = new Dictionary<string, List<string>>();
			foreach (string key2 in _data.Keys)
			{
				if (key2.IndexOf('.') >= 0)
				{
					string key = key2.Split(SplitChar, 2)[0];
					if (!_baseKeys.ContainsKey(key))
					{
						_baseKeys.Add(key, new List<string>());
					}
					_baseKeys[key].Add(key2);
				}
			}
		}

		public static string CombineKeys(string baseKey, string key)
		{
			return baseKey + '.' + key;
		}

		public bool HasBaseKey(string key)
		{
			return _baseKeys.ContainsKey(key);
		}

		public bool HasFullKey(string fullkey)
		{
			return _data.ContainsKey(fullkey);
		}

		public bool HasKey(string baseKey, string key)
		{
			return HasFullKey(CombineKeys(baseKey, key));
		}

		public string GetValue(string baseKey, string key)
		{
			return GetValue(CombineKeys(baseKey, key));
		}

		public virtual string GetValue(string fullKey)
		{
			return _data[fullKey];
		}

		public bool TryGetValue(string baseKey, string key, out string value)
		{
			return TryGetValue(CombineKeys(baseKey, key), out value);
		}

		public virtual bool TryGetValue(string fullKey, out string value)
		{
			return _data.TryGetValue(fullKey, out value);
		}

		public List<string> GetBaseKeysByKeyValue(string key, string value)
		{
			List<string> list = new List<string>();
			foreach (string key2 in _baseKeys.Keys)
			{
				if (HasKey(key2, key) && GetValue(key2, key) == value)
				{
					list.Add(key2);
				}
			}
			return list;
		}

		protected void RenameKey(string oldFullKeyName, string newFullKeyName)
		{
			_data[newFullKeyName] = _data[oldFullKeyName];
			_data.Remove(oldFullKeyName);
			if (oldFullKeyName.IndexOf('.') >= 0 && newFullKeyName.IndexOf('.') >= 0)
			{
				string key = oldFullKeyName.Split(SplitChar, 2)[0];
				string text = newFullKeyName.Split(SplitChar, 2)[0];
				List<string> list = _baseKeys[key];
				list.Remove(oldFullKeyName);
				if (list.Count == 0)
				{
					_baseKeys.Remove(key);
				}
				if (!_baseKeys.ContainsKey(text))
				{
					_baseKeys.Add(text, new List<string>());
				}
				_baseKeys[text].Add(text);
			}
		}
	}
}
