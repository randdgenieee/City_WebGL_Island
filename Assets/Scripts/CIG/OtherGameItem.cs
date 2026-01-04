using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class OtherGameItem : MonoBehaviour
	{
		[SerializeField]
		private Image _gameImage;

		private SparkSocGame _game;

		public void Initialize(SparkSocGame item)
		{
			_game = item;
			_gameImage.sprite = Sprite.Create(item.BannerImage, new Rect(0f, 0f, item.BannerImage.width, item.BannerImage.height), Vector2.one * 0.5f);
		}

		public void OnDownloadButtonClick()
		{
			_game.OpenInAppStore();
		}
	}
}
