using UnityEngine;
using UnityEngine.UI;

public class ScrollArrow : MonoBehaviour
{
	private enum Direction
	{
		Up,
		Down,
		Left,
		Right
	}

	[SerializeField]
	private ScrollRect _scrollRect;

	[SerializeField]
	private Image _image;

	[SerializeField]
	private Direction _direction;

	private bool ContentExceedsViewWidth => _scrollRect.content.rect.width > _scrollRect.viewport.rect.width;

	private bool ContentExceedsViewHeight => _scrollRect.content.rect.height > _scrollRect.viewport.rect.height;

	private void Awake()
	{
		_scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
	}

	private void OnScrollRectValueChanged(Vector2 newValue)
	{
		bool enabled = false;
		switch (_direction)
		{
		case Direction.Up:
			enabled = (ContentExceedsViewHeight && newValue.y < 1f && !Mathf.Approximately(newValue.y, 1f));
			break;
		case Direction.Down:
			enabled = (ContentExceedsViewHeight && newValue.y > 0f && !Mathf.Approximately(newValue.y, 0f));
			break;
		case Direction.Left:
			enabled = (ContentExceedsViewWidth && newValue.x > 0f && !Mathf.Approximately(newValue.x, 0f));
			break;
		case Direction.Right:
			enabled = (ContentExceedsViewWidth && newValue.x < 1f && !Mathf.Approximately(newValue.x, 1f));
			break;
		}
		_image.enabled = enabled;
	}
}
