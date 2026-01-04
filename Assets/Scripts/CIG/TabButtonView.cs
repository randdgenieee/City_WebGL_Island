using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class TabButtonView : MonoBehaviour
	{
		[SerializeField]
		private Sprite _inactiveTabBackgroundSprite;

		[SerializeField]
		private Sprite _activeTabBackgroundSprite;

		[SerializeField]
		private Image _tabBackground;

		private bool _isActive;

		public bool IsActive
		{
			get
			{
				return _isActive;
			}
			set
			{
				_isActive = value;
				if (_isActive)
				{
					_tabBackground.sprite = _activeTabBackgroundSprite;
				}
				else
				{
					_tabBackground.sprite = _inactiveTabBackgroundSprite;
				}
			}
		}

		public bool IsVisible
		{
			get
			{
				return base.gameObject.activeSelf;
			}
			set
			{
				base.gameObject.SetActive(value);
			}
		}
	}
}
