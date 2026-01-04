using System;
using System.Collections.Generic;

namespace CIG
{
	public class StorageDictionary
	{
		private readonly Dictionary<string, object> _storage;

		public Dictionary<string, object> InternalDictionary => _storage;

		public StorageDictionary()
		{
			_storage = new Dictionary<string, object>();
		}

		public StorageDictionary(Dictionary<string, object> storage)
		{
			_storage = storage;
		}

		public bool Contains(string key)
		{
			return _storage.ContainsKey(key);
		}

		public void Remove(string key)
		{
			_storage.Remove(key);
		}

		public void Set(string key, string value)
		{
			if (value == null)
			{
				throw new InvalidOperationException($"'null' cannot be stored for key '{key}'.");
			}
			_storage[key] = value;
		}

		public void Set(string key, bool value)
		{
			_storage[key] = value;
		}

		public void Set(string key, char value)
		{
			_storage[key] = value;
		}

		public void Set(string key, float value)
		{
			_storage[key] = value;
		}

		public void Set(string key, double value)
		{
			_storage[key] = value;
		}

		public void Set(string key, sbyte value)
		{
			_storage[key] = value;
		}

		public void Set(string key, byte value)
		{
			_storage[key] = value;
		}

		public void Set(string key, short value)
		{
			_storage[key] = value;
		}

		public void Set(string key, ushort value)
		{
			_storage[key] = value;
		}

		public void Set(string key, int value)
		{
			_storage[key] = value;
		}

		public void Set(string key, uint value)
		{
			_storage[key] = value;
		}

		public void Set(string key, long value)
		{
			_storage[key] = value;
		}

		public void Set(string key, ulong value)
		{
			_storage[key] = value;
		}

		public void Set(string key, decimal value)
		{
			_storage[key] = value;
		}

		public void Set(string key, DateTime value)
		{
			_storage[key] = value.ToBinary();
		}

		public void Set<T>(string key, T value) where T : IStorable
		{
			if (value == null)
			{
				throw new InvalidOperationException($"'null' cannot be stored for key '{key}'.");
			}
			_storage[key] = value.Serialize()._storage;
		}

		public void Set<T>(string key, T value, Func<T, StorageDictionary> serialize)
		{
			_storage[key] = serialize(value)._storage;
		}

		public void SetOrRemove(string key, string value)
		{
			SetOrRemoveInternal(key, value);
		}

		public void SetOrRemove(string key, float? value)
		{
			SetOrRemoveInternal(key, value);
		}

		public void SetOrRemove(string key, double? value)
		{
			SetOrRemoveInternal(key, value);
		}

		public void SetOrRemove(string key, int? value)
		{
			SetOrRemoveInternal(key, value);
		}

		public void SetOrRemove(string key, IStorable value)
		{
			if (value == null)
			{
				_storage.Remove(key);
			}
			else
			{
				_storage[key] = value.Serialize()._storage;
			}
		}

		public void SetOrRemoveStorable<TStorable>(string key, TStorable value, bool remove) where TStorable : IStorable
		{
			if (remove)
			{
				_storage.Remove(key);
			}
			else
			{
				Set(key, value);
			}
		}

		public void Set(string key, List<string> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<bool> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<char> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<float> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<double> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<sbyte> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<byte> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<short> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<ushort> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<int> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<uint> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<long> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<ulong> values)
		{
			SetList(key, values);
		}

		public void Set(string key, List<decimal> values)
		{
			SetList(key, values);
		}

		public void Set(string key, HashSet<string> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<bool> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<char> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<float> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<double> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<sbyte> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<byte> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<short> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<ushort> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<int> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<uint> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<long> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<ulong> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, HashSet<decimal> values)
		{
			SetHashSet(key, values);
		}

		public void Set(string key, List<DateTime> values)
		{
			List<object> list = new List<object>();
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(values[i].ToBinary());
			}
			_storage[key] = list;
		}

		public void Set(string key, Dictionary<string, string> values)
		{
			SetDictionary(key, values);
		}

		public void Set(string key, Dictionary<string, long> values)
		{
			SetDictionary(key, values);
		}

		public void Set(string key, Dictionary<string, int> values)
		{
			SetDictionary(key, values);
		}

		public void Set(string key, Dictionary<string, double> values)
		{
			SetDictionary(key, values);
		}

		public void Set(string key, StorageDictionary values)
		{
			_storage[key] = values._storage;
		}

		public void Set<T>(string key, List<T> values, Func<T, StorageDictionary> serialize)
		{
			List<object> list = new List<object>();
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(serialize(values[i])._storage);
			}
			_storage[key] = list;
		}

		public void Set<T>(string key, List<T> values) where T : IStorable
		{
			List<object> list = new List<object>();
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(values[i].Serialize()._storage);
			}
			_storage[key] = list;
		}

		public void Set<T>(string key, Dictionary<string, T> values) where T : IStorable
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, T> value in values)
			{
				dictionary.Add(value.Key, value.Value.Serialize()._storage);
			}
			_storage[key] = dictionary;
		}

		public void SetWithNulls<T>(string key, Dictionary<string, T> values) where T : IStorable
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, T> value in values)
			{
				if (value.Value != null)
				{
					dictionary.Add(value.Key, value.Value.Serialize()._storage);
				}
				else
				{
					dictionary.Add(value.Key, null);
				}
			}
			_storage[key] = dictionary;
		}

		public void SetWithNulls<T>(string key, List<T> values) where T : IStorable
		{
			List<object> list = new List<object>();
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				if (values[i] != null)
				{
					list.Add(values[i].Serialize()._storage);
				}
				else
				{
					list.Add(null);
				}
			}
			_storage[key] = list;
		}

		private void SetList<T>(string key, List<T> values)
		{
			if (values == null)
			{
				throw new InvalidOperationException($"'null' cannot be stored for key '{key}'.");
			}
			List<object> list = new List<object>();
			int count = values.Count;
			for (int i = 0; i < count; i++)
			{
				if (values[i] == null)
				{
					throw new InvalidOperationException($"'null' cannot be stored for key '{key}'.");
				}
				list.Add(values[i]);
			}
			_storage[key] = list;
		}

		private void SetHashSet<T>(string key, HashSet<T> values)
		{
			if (values == null)
			{
				throw new InvalidOperationException($"'null' cannot be stored for key '{key}'.");
			}
			List<object> list = new List<object>();
			foreach (T value in values)
			{
				if (value == null)
				{
					throw new InvalidOperationException($"'null' cannot be stored for key '{key}'.");
				}
				list.Add(value);
			}
			_storage[key] = list;
		}

		private void SetDictionary<T>(string key, Dictionary<string, T> values)
		{
			if (values == null)
			{
				throw new InvalidOperationException($"'null' cannot be stored for key '{key}'.");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, T> value in values)
			{
				if (value.Value == null)
				{
					throw new InvalidOperationException($"'null' cannot be stored for key '{key}'.");
				}
				dictionary.Add(value.Key, value.Value);
			}
			_storage[key] = dictionary;
		}

		private void SetOrRemoveInternal<T>(string key, T value)
		{
			if (value == null)
			{
				_storage.Remove(key);
			}
			else
			{
				_storage[key] = value;
			}
		}

		public TField Get<TField>(string key, TField defaultValue)
		{
			if (_storage.ContainsKey(key))
			{
				if (!(_storage[key] is TField))
				{
					throw new InvalidOperationException(string.Format("Found {0} at _storage[{1}]. Expecting {2}", (_storage[key] == null) ? "null" : _storage[key].GetType().Name, key, typeof(TField).Name));
				}
				return (TField)_storage[key];
			}
			return defaultValue;
		}

		public TField Get<TField>(string key, TField defaultValue, Func<StorageDictionary, TField> factoryMethod)
		{
			return factoryMethod(GetStorageDict(key));
		}

		public DateTime GetDateTime(string key, DateTime defaultValue)
		{
			if (_storage.ContainsKey(key))
			{
				if (!(_storage[key] is long))
				{
					throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(long).Name}");
				}
				return DateTime.FromBinary((long)_storage[key]);
			}
			return defaultValue;
		}

		public StorageDictionary GetStorageDict(string key)
		{
			if (!_storage.ContainsKey(key))
			{
				SetDictionary(key, new Dictionary<string, object>());
			}
			if (!(_storage[key] is Dictionary<string, object>))
			{
				throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(Dictionary<string, object>).Name}");
			}
			return new StorageDictionary((Dictionary<string, object>)_storage[key]);
		}

		public List<TField> GetList<TField>(string key)
		{
			if (!_storage.ContainsKey(key))
			{
				SetList(key, new List<TField>());
			}
			if (!(_storage[key] is List<object>))
			{
				throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(List<object>).Name}");
			}
			List<TField> list = new List<TField>();
			List<object> list2 = (List<object>)_storage[key];
			int count = list2.Count;
			for (int i = 0; i < count; i++)
			{
				if (!(list2[i] is TField))
				{
					throw new InvalidOperationException(string.Format("Found {0} at _storage[{1}][{2}]. Expecting {3}", (list2[i] == null) ? "null" : list2[i].GetType().Name, key, i, typeof(TField).Name));
				}
				list.Add((TField)list2[i]);
			}
			return list;
		}

		public HashSet<TField> GetHashSet<TField>(string key)
		{
			if (!_storage.ContainsKey(key))
			{
				SetHashSet(key, new HashSet<TField>());
			}
			if (!(_storage[key] is List<object>))
			{
				throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(List<object>).Name}");
			}
			HashSet<TField> hashSet = new HashSet<TField>();
			List<object> list = (List<object>)_storage[key];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				if (!(list[i] is TField))
				{
					throw new InvalidOperationException(string.Format("Found {0} at _storage[{1}][{2}]. Expecting {3}", (list[i] == null) ? "null" : list[i].GetType().Name, key, i, typeof(TField).Name));
				}
				hashSet.Add((TField)list[i]);
			}
			return hashSet;
		}

		public List<DateTime> GetDateTimeList(string key)
		{
			if (_storage.ContainsKey(key))
			{
				if (!(_storage[key] is List<object>))
				{
					throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(List<object>).Name}");
				}
				List<DateTime> list = new List<DateTime>();
				List<object> list2 = (List<object>)_storage[key];
				int count = list2.Count;
				for (int i = 0; i < count; i++)
				{
					if (!(list2[i] is long))
					{
						throw new InvalidOperationException(string.Format("Found {0} at _storage[{1}][{2}]. Expecting {3}", (list2[i] == null) ? "null" : list2[i].GetType().Name, key, i, typeof(long).Name));
					}
					list.Add(DateTime.FromBinary((long)list2[i]));
				}
				return list;
			}
			return new List<DateTime>();
		}

		public Dictionary<string, TField> GetDictionary<TField>(string key)
		{
			if (!_storage.ContainsKey(key))
			{
				SetDictionary(key, new Dictionary<string, TField>());
			}
			if (!(_storage[key] is Dictionary<string, object>))
			{
				throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(Dictionary<string, object>).Name}");
			}
			Dictionary<string, TField> dictionary = new Dictionary<string, TField>();
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)_storage[key];
			foreach (KeyValuePair<string, object> item in dictionary2)
			{
				if (!(dictionary2[item.Key] is TField))
				{
					throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(TField).Name}");
				}
				dictionary.Add(item.Key, (TField)dictionary2[item.Key]);
			}
			return dictionary;
		}

		public Dictionary<string, TModel> GetDictionaryModels<TModel>(string key, Func<StorageDictionary, TModel> factoryMethod)
		{
			Dictionary<string, Dictionary<string, object>> dictionary = GetDictionary<Dictionary<string, object>>(key);
			Dictionary<string, TModel> dictionary2 = new Dictionary<string, TModel>();
			foreach (KeyValuePair<string, Dictionary<string, object>> item in dictionary)
			{
				dictionary2.Add(item.Key, factoryMethod(new StorageDictionary(item.Value)));
			}
			return dictionary2;
		}

		public Dictionary<string, TModel> GetDictionaryModelsWithNulls<TModel>(string key, Func<StorageDictionary, TModel> factoryMethod)
		{
			if (!_storage.ContainsKey(key))
			{
				SetDictionary(key, new Dictionary<string, TModel>());
			}
			if (!(_storage[key] is Dictionary<string, object>))
			{
				throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(List<object>).Name}");
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)_storage[key];
			Dictionary<string, TModel> dictionary2 = new Dictionary<string, TModel>();
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				if (item.Value == null)
				{
					dictionary2.Add(item.Key, default(TModel));
				}
				else
				{
					if (!(dictionary[item.Key] is Dictionary<string, object>))
					{
						throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(Dictionary<string, object>).Name}");
					}
					dictionary2.Add(item.Key, factoryMethod(new StorageDictionary((Dictionary<string, object>)item.Value)));
				}
			}
			return dictionary2;
		}

		public TModel GetModel<TModel>(string key, Func<StorageDictionary, TModel> factoryMethod, TModel defaultValue)
		{
			if (Contains(key))
			{
				return factoryMethod(GetStorageDict(key));
			}
			return defaultValue;
		}

		public List<TModel> GetModels<TModel>(string key, Func<StorageDictionary, TModel> factoryMethod)
		{
			List<Dictionary<string, object>> list = GetList<Dictionary<string, object>>(key);
			List<TModel> list2 = new List<TModel>();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				list2.Add(factoryMethod(new StorageDictionary(list[i])));
			}
			return list2;
		}

		public List<TModel> GetModelsWithNulls<TModel>(string key, Func<StorageDictionary, TModel> factoryMethod)
		{
			if (!(_storage[key] is List<object>))
			{
				throw new InvalidOperationException($"Found {_storage[key].GetType().Name} at _storage[{key}]. Expecting {typeof(List<object>).Name}");
			}
			List<TModel> list = new List<TModel>();
			List<object> list2 = (List<object>)_storage[key];
			int count = list2.Count;
			for (int i = 0; i < count; i++)
			{
				if (list2[i] == null)
				{
					list.Add(default(TModel));
					continue;
				}
				if (!(list2[i] is Dictionary<string, object>))
				{
					throw new InvalidOperationException($"Found {list2[i].GetType().Name} at _storage[{key}][{i}]. Expecting {typeof(Dictionary<string, object>).Name}");
				}
				list.Add(factoryMethod(new StorageDictionary((Dictionary<string, object>)list2[i])));
			}
			return list;
		}
	}
}
