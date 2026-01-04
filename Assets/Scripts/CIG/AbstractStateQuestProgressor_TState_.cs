namespace CIG
{
	public class AbstractStateQuestProgressor<TState> : QuestProgressor where TState : AbstractState
	{
		private TState _abstractState;

		private readonly string _key;

		private long _lastStateValue;

		private const string LastStateValueKey = "LastStateValue";

		public AbstractStateQuestProgressor(StorageDictionary storage, TState abstractState, string key, long startProgress, long currentStateValue)
			: base(storage, startProgress)
		{
			_abstractState = abstractState;
			_key = key;
			_lastStateValue = storage.Get("LastStateValue", currentStateValue);
			_abstractState.ValueChangedEvent += OnValueChanged;
			OnValueChanged(_key, _lastStateValue, currentStateValue);
		}

		public override void Release()
		{
			if (_abstractState != null)
			{
				_abstractState.ValueChangedEvent -= OnValueChanged;
				_abstractState = null;
			}
			base.Release();
		}

		private void OnValueChanged(string key, object oldvalue, object newvalue)
		{
			if (key == _key)
			{
				long num = 0L;
				long num2 = 0L;
				if (newvalue is long)
				{
					num = (((long?)oldvalue) ?? num);
					num2 = (long)newvalue;
				}
				else if (newvalue is int)
				{
					num = (((long?)(int?)oldvalue) ?? num);
					num2 = (int)newvalue;
				}
				else
				{
					num = (long)(((decimal?)oldvalue) ?? ((decimal)num));
					num2 = (long)(decimal)newvalue;
				}
				base.Progress += num2 - num;
				_lastStateValue = num2;
			}
		}

		public override StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = base.Serialize();
			storageDictionary.Set("LastStateValue", _lastStateValue);
			return storageDictionary;
		}
	}
}
