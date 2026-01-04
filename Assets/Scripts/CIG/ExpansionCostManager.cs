namespace CIG
{
	public class ExpansionCostManager
	{
		private readonly StorageDictionary _storage;

		private readonly ExpansionProperties _properties;

		private int _currentPosition;

		private const string CurrentPositionKey = "CurrentPosition";

		public ExpansionCostManager(StorageDictionary storage, ExpansionProperties properties)
		{
			_storage = storage;
			_properties = properties;
			_currentPosition = _storage.Get("CurrentPosition", 0);
		}

		public decimal GetCashCost()
		{
			int count = _properties.Costs.Count;
			if (_currentPosition < count)
			{
				_currentPosition++;
				Serialize();
				return _properties.Costs[_currentPosition - 1];
			}
			return _properties.Costs[count - 1];
		}

		public void Serialize()
		{
			_storage.Set("CurrentPosition", _currentPosition);
		}
	}
}
