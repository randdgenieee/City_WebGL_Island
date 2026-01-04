namespace CIG
{
	public abstract class QuestProgressor
	{
		public delegate void ProgressChangedEventHandler(long oldProgress, long newProgress);

		private long _progress;

		private const string ProgressKey = "Progress";

		public long Progress
		{
			get
			{
				return _progress;
			}
			protected set
			{
				if (_progress != value)
				{
					long progress = _progress;
					_progress = value;
					FireProgressChangedEvent(progress, _progress);
				}
			}
		}

		public event ProgressChangedEventHandler ProgressChangedEvent;

		private void FireProgressChangedEvent(long oldProgress, long newProgress)
		{
			if (this.ProgressChangedEvent != null)
			{
				this.ProgressChangedEvent(oldProgress, newProgress);
			}
		}

		protected QuestProgressor(StorageDictionary storage, long startProgress)
		{
			Progress = storage.Get("Progress", startProgress);
		}

		public virtual void Release()
		{
		}

		public virtual StorageDictionary Serialize()
		{
			StorageDictionary storageDictionary = new StorageDictionary();
			storageDictionary.Set("Progress", Progress);
			return storageDictionary;
		}
	}
}
