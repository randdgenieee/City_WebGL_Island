using System.Collections;
using UnityEngine;

namespace CIG
{
	public class ScrollingText : MonoBehaviour
	{
		public enum Direction
		{
			Horizontal,
			Vertical
		}

		[SerializeField]
		private RectTransform _view;

		[SerializeField]
		private RectTransform _content;

		[SerializeField]
		private float _scrollSpeed = 10f;

		[SerializeField]
		private float _pauseDuration = 2f;

		[SerializeField]
		private Direction _direction;

		private float _contentSize;

		private float _viewSize;

		private IEnumerator _scrollRoutine;

		private bool ShouldScroll
		{
			get
			{
				if (Application.isPlaying)
				{
					return _contentSize > _viewSize;
				}
				return false;
			}
		}

		private void LateUpdate()
		{
			float num = (_direction == Direction.Horizontal) ? _content.rect.width : _content.rect.height;
			float num2 = (_direction == Direction.Horizontal) ? _view.rect.width : _view.rect.height;
			if (!Mathf.Approximately(_contentSize, num) || !Mathf.Approximately(_viewSize, num2))
			{
				_contentSize = num;
				_viewSize = num2;
				if (ShouldScroll)
				{
					StartScrolling();
				}
				else
				{
					StopScrolling();
				}
			}
		}

		private void OnEnable()
		{
			if (ShouldScroll)
			{
				StartScrolling();
			}
		}

		private void OnDisable()
		{
			StopScrolling();
		}

		private void StartScrolling()
		{
			StopScrolling();
			StartCoroutine(_scrollRoutine = ScrollRoutine());
		}

		private void StopScrolling()
		{
			if (_scrollRoutine != null)
			{
				StopCoroutine(_scrollRoutine);
				_scrollRoutine = null;
				_content.anchoredPosition = ((_direction == Direction.Horizontal) ? new Vector2(0f, _content.anchoredPosition.y) : new Vector2(_content.anchoredPosition.x, 0f));
			}
		}

		private IEnumerator ScrollRoutine()
		{
			float num = _contentSize - _viewSize;
			float num2 = num * ((_direction == Direction.Horizontal) ? _content.pivot.x : _content.pivot.y);
			float num3 = num2 - num;
			float duration = num / _scrollSpeed;
			float timeLeft = duration;
			_content.anchoredPosition = ((_direction == Direction.Horizontal) ? new Vector2(num2, _content.anchoredPosition.y) : new Vector2(_content.anchoredPosition.x, num2));
			float from = num2;
			float to = num3;
			while (true)
			{
				float t = (duration - timeLeft) / duration;
				timeLeft -= Time.deltaTime;
				_content.anchoredPosition = ((_direction == Direction.Horizontal) ? new Vector2(Mathf.Lerp(from, to, t), _content.anchoredPosition.y) : new Vector2(_content.anchoredPosition.x, Mathf.Lerp(from, to, t)));
				if (Mathf.Approximately((_direction == Direction.Horizontal) ? _content.anchoredPosition.x : _content.anchoredPosition.y, to))
				{
					float num4 = from;
					from = to;
					to = num4;
					timeLeft = duration;
					yield return new WaitForSeconds(_pauseDuration);
				}
				else
				{
					yield return null;
				}
			}
		}
	}
}
