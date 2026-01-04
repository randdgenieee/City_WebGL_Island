using System;
using UnityEngine;
using UnityEngine.UI;

public class OverlayButton : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private Sprite _activeSprite;

	[SerializeField]
	private Sprite _inactiveSprite;

	[SerializeField]
	private Button _buttonComponent;

	private Action _onClick;

	public void Initialize(bool active, Action onClick)
	{
		_onClick = onClick;
		_image.sprite = (active ? _activeSprite : _inactiveSprite);
		_buttonComponent.enabled = active;
	}

	public void OnClick()
	{
		if (_onClick != null)
		{
			_onClick();
		}
	}
}
