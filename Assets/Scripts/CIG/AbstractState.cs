namespace CIG
{
	public abstract class AbstractState
	{
		public delegate void ValueChangedHandler(string key, object oldValue, object newValue);

		public delegate void AnyValueChangedHandler();

		protected readonly StorageDictionary _storage;

		public event ValueChangedHandler ValueChangedEvent;

		public event AnyValueChangedHandler AnyValueChangedEvent;

		protected void FireValueChangedHandler(string key, object oldValue, object newValue)
		{
			if (this.ValueChangedEvent != null)
			{
				this.ValueChangedEvent(key, oldValue, newValue);
			}
			FireAnyValueChangedEvent();
		}

		private void FireAnyValueChangedEvent()
		{
			if (this.AnyValueChangedEvent != null)
			{
				this.AnyValueChangedEvent();
			}
		}

		protected AbstractState(StorageDictionary storage)
		{
			_storage = storage;
		}

		protected void OnValueChanged(string key, object oldValue, object newValue)
		{
			if (oldValue != newValue)
			{
				FireValueChangedHandler(key, oldValue, newValue);
			}
		}
	}
}
