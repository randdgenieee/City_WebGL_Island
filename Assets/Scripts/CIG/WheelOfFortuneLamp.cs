using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class WheelOfFortuneLamp : MonoBehaviour
	{
		public const float AnimationDuration = 1f;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private Sprite _onSprite;

		[SerializeField]
		private Sprite _offSprite;

		private bool _isOn;

		private IEnumerator _animationRoutine;

		public void Initialize(bool isOn)
		{
			_isOn = isOn;
		}

		private void OnEnable()
		{
			StopAnimation();
			StartCoroutine(_animationRoutine = ToggleAnimationRoutine());
		}

		private void OnDisable()
		{
			StopAnimation();
		}

		private void OnDestroy()
		{
			StopAnimation();
		}

		private void StopAnimation()
		{
			if (_animationRoutine != null)
			{
				StopCoroutine(_animationRoutine);
				_animationRoutine = null;
			}
		}

		private IEnumerator ToggleAnimationRoutine()
		{
			while (true)
			{
				_isOn = !_isOn;
				_image.sprite = (_isOn ? _onSprite : _offSprite);
				yield return new WaitForSecondsRealtime(1f);
			}
		}
	}
}
