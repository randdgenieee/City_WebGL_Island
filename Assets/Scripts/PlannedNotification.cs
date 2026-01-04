using CIG.Translation;

public class PlannedNotification
{
	public int Seconds
	{
		get;
		private set;
	}

	public ILocalizedString Description
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

	public bool IsValid => Seconds > 0;

	public PlannedNotification(int seconds, ILocalizedString description, bool sound = false, int? id = default(int?))
	{
		Seconds = seconds;
		Sound = sound;
		Description = description;
		Id = id;
	}
}
