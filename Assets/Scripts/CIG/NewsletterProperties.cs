namespace CIG
{
	[BalancePropertyClass("newsletter", true)]
	public class NewsletterProperties : BaseProperties
	{
		private const string SubscribeRewardKey = "reward";

		[BalanceProperty("reward")]
		public Currencies SubscribeReward
		{
			get;
			private set;
		}

		public NewsletterProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			SubscribeReward = GetProperty("reward", new Currencies());
		}
	}
}
