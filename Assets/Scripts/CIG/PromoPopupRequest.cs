namespace CIG
{
	public class PromoPopupRequest : PopupRequest
	{
		public SparkSocGame Game
		{
			get;
		}

		public PromoPopupRequest(SparkSocGame game)
			: base(typeof(PromoPopup))
		{
			Game = game;
		}
	}
}
