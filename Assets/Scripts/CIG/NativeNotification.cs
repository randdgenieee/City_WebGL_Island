namespace CIG
{
	public class NativeNotification
	{
		public string Title
		{
			get;
			private set;
		}

		public int BadgeCount
		{
			get;
			private set;
		}

		public int Seconds
		{
			get;
			private set;
		}

		public string Description
		{
			get;
			private set;
		}

		public bool Sound
		{
			get;
			private set;
		}

		public int? Id
		{
			get;
			private set;
		}

		public NativeNotification(int seconds, string title, string description, bool sound, int badgeCount, int? id)
		{
			Seconds = seconds;
			Title = title;
			Description = description;
			Sound = sound;
			BadgeCount = badgeCount;
			Id = id;
		}

		public override string ToString()
		{
			return "[Sec: " + Seconds + "; Title: '" + Title + "'; Desc:'" + Description + "'; Sound: " + (Sound ? "Yes" : "No") + "; BadgeCount: " + BadgeCount + "; Id: " + (Id.HasValue ? Id.Value.ToString() : "null") + "]";
		}
	}
}
