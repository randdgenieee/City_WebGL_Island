using CIG;
using UnityEngine;
using UnityEngine.UI;

public class PromoPopup : Popup
{
	private static readonly Vector2 MaxImageSize = new Vector2(750f, 500f);

	[SerializeField]
	private Image _image;

	[SerializeField]
	private LayoutElement _layoutElement;

	private CrossPromoPopupScheduler _crossPromoPopupScheduler;

	private SparkSocGame _game;

	private Sprite _sprite;

	public override string AnalyticsScreenName => "cross_promo";

	public override void Initialize(Model model, CIGCanvasScaler canvasScaler)
	{
		base.Initialize(model, canvasScaler);
		_crossPromoPopupScheduler = model.Game.CrossPromoPopupScheduler;
	}

	public override void Open(PopupRequest request)
	{
		base.Open(request);
		PromoPopupRequest request2 = GetRequest<PromoPopupRequest>();
		_game = request2.Game;
		_crossPromoPopupScheduler.ShowedPromo();
		SetSparkSocGame();
	}

	public void OnPromoClicked()
	{
		_game.OpenInAppStore();
	}

	protected override void Closed()
	{
		_image.sprite = null;
		UnityEngine.Object.DestroyImmediate(_sprite);
		UnityEngine.Object.DestroyImmediate(_game.PromoImage);
		base.Closed();
	}

	private void SetSparkSocGame()
	{
		if (_game.PromoImage != null)
		{
			Rect rect = new Rect(0f, 0f, _game.PromoImage.width, _game.PromoImage.height);
			_image.sprite = (_sprite = Sprite.Create(_game.PromoImage, rect, new Vector2(0.5f, 0.5f)));
			Vector2 imageSize = GetImageSize(_game.PromoImage);
			_layoutElement.preferredWidth = imageSize.x;
			_layoutElement.preferredHeight = imageSize.y;
		}
	}

	private Vector2 GetImageSize(Texture2D texture)
	{
		float num = texture.width;
		float num2 = texture.height;
		float num3 = MaxImageSize.x / num;
		float num4 = MaxImageSize.y / num2;
		float d = Mathf.Min(num3, num4, 1f);
		return new Vector2(num, num2) * d;
	}
}
