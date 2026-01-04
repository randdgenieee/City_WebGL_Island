using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class ScrollbarTweener : MonoBehaviour
	{
		[SerializeField]
		private ScrollRect _scrollRect;

		[SerializeField]
		private Scrollbar _scrollBar;

		[SerializeField]
		private Tweener _tweener;

		[SerializeField]
		[Range(0f, 1f)]
		private float _minHandleSize;

		[SerializeField]
		[Range(0f, 1f)]
		private float _maxHandleSize = 1f;

		[SerializeField]
		private CanvasGroup _canvasGroup;

		[SerializeField]
		private bool _startHidden = true;

		private bool _updatingScrollBar;

		private void Start()
		{
			_scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
			_tweener.FinishedPlaying += OnTweenerFinishedPlaying;
		}

		private void OnEnable()
		{
			if (_startHidden)
			{
				_canvasGroup.alpha = 0f;
				_canvasGroup.interactable = false;
			}
			else
			{
				OnScrollRectValueChanged(_scrollRect.normalizedPosition);
			}
		}

		private void LateUpdate()
		{
			_scrollBar.size = Mathf.Clamp(_scrollBar.size, _minHandleSize, _maxHandleSize);
		}

		private void OnDestroy()
		{
			_scrollRect.onValueChanged.RemoveListener(OnScrollRectValueChanged);
			_tweener.FinishedPlaying -= OnTweenerFinishedPlaying;
		}

		public void OnScrollRectValueChanged(Vector2 scrollPosition)
		{
			if (_tweener.IsPlaying)
			{
				_tweener.Stop();
			}
			_tweener.Reset();
			_canvasGroup.interactable = true;
			_updatingScrollBar = true;
			switch (_scrollBar.direction)
			{
			case Scrollbar.Direction.BottomToTop:
			case Scrollbar.Direction.TopToBottom:
			{
				_scrollBar.value = scrollPosition.y;
				float height = _scrollRect.content.rect.height;
				float height2 = _scrollRect.viewport.rect.height;
				break;
			}
			case Scrollbar.Direction.LeftToRight:
			case Scrollbar.Direction.RightToLeft:
			{
				_scrollBar.value = scrollPosition.x;
				float width = _scrollRect.content.rect.width;
				float width2 = _scrollRect.viewport.rect.width;
				break;
			}
			default:
				UnityEngine.Debug.LogWarningFormat("Missing direction: '{0}'", _scrollBar.direction);
				break;
			}
			_scrollBar.size = Mathf.Clamp(_scrollBar.size, _minHandleSize, _maxHandleSize);
			_updatingScrollBar = false;
			_tweener.Play();
		}

		public void OnScrollBarValueChanged(float value)
		{
			if (!_updatingScrollBar)
			{
				switch (_scrollBar.direction)
				{
				case Scrollbar.Direction.BottomToTop:
				case Scrollbar.Direction.TopToBottom:
					_scrollRect.verticalNormalizedPosition = value;
					break;
				case Scrollbar.Direction.LeftToRight:
				case Scrollbar.Direction.RightToLeft:
					_scrollRect.horizontalNormalizedPosition = value;
					break;
				default:
					UnityEngine.Debug.LogWarningFormat("Missing direction: '{0}'", _scrollBar.direction);
					break;
				}
			}
		}

		private void OnTweenerFinishedPlaying(Tweener tweener)
		{
			_canvasGroup.interactable = false;
		}
	}
}
