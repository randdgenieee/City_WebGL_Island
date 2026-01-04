using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class WeightedList<T>
	{
		private List<T> _items;

		private List<float> _rollWeights;

		public int Count => _items.Count;

		public WeightedList()
		{
			_items = new List<T>();
			_rollWeights = new List<float>();
		}

		public void Add(T item, float weight)
		{
			int count = _rollWeights.Count;
			_items.Add(item);
			if (count > 0)
			{
				_rollWeights.Add(_rollWeights[count - 1] + Mathf.Max(0f, weight));
			}
			else
			{
				_rollWeights.Add(Mathf.Max(0f, weight));
			}
		}

		public bool Remove(T item)
		{
			int num = _items.IndexOf(item);
			if (num == -1)
			{
				return false;
			}
			float num2 = (num <= 0) ? 0f : _rollWeights[num - 1];
			float num3 = _rollWeights[num] - num2;
			int count = _rollWeights.Count;
			for (int i = num + 1; i < count; i++)
			{
				List<float> rollWeights = _rollWeights;
				int index = i;
				rollWeights[index] -= num3;
			}
			_items.RemoveAt(num);
			_rollWeights.RemoveAt(num);
			return true;
		}

		public T PickRandom()
		{
			int count = _items.Count;
			if (count > 0)
			{
				float num = Random.Range(0f, _rollWeights[count - 1]);
				for (int i = 0; i < count; i++)
				{
					if (num < _rollWeights[i] || Mathf.Approximately(num, _rollWeights[i]))
					{
						return _items[i];
					}
				}
			}
			return default(T);
		}

		public void Clear()
		{
			_items.Clear();
			_rollWeights.Clear();
		}
	}
}
