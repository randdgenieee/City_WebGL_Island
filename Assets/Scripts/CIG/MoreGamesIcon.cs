using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class MoreGamesIcon : MonoBehaviour
	{
		[SerializeField]
		private RawImage _icon;

		[SerializeField]
		private Image _badge;

		[SerializeField]
		private Sprite _notInstalledBadge;

		[SerializeField]
		private Sprite _installedBadge;

		private SparkSocGame _item;

		public void Initialize(SparkSocGame item)
		{
			_item = item;
			_icon.texture = item.AppIcon;
			_icon.color = (item.IsInstalled ? Color.gray : Color.white);
			_badge.sprite = (item.IsInstalled ? _installedBadge : _notInstalledBadge);
		}

		public void OnIconClicked()
		{
			_item.OpenInAppStore();
		}
	}
}
