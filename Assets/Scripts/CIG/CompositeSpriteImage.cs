using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class CompositeSpriteImage : MonoBehaviour
	{
		[Serializable]
		public class CompositeSpriteData
		{
			[SerializeField]
			private Sprite _mainSprite;

			[SerializeField]
			private Sprite _childSprite;

			[SerializeField]
			private Vector3 _childSpritePosition;

			[SerializeField]
			private Vector2 _childSpriteSize;

			public Sprite MainSprite => _mainSprite;

			public Sprite ChildSprite => _childSprite;

			public Vector3 ChildSpritePosition => _childSpritePosition;

			public Vector2 ChildSpriteSize => _childSpriteSize;
		}

		[SerializeField]
		private Image _mainImage;

		[SerializeField]
		private Image _childImage;

		public void Initialize(CompositeSpriteData data)
		{
			_mainImage.sprite = data.MainSprite;
			_childImage.gameObject.SetActive(data.ChildSprite != null);
			_childImage.sprite = data.ChildSprite;
			RectTransform obj = (RectTransform)_childImage.transform;
			obj.sizeDelta = data.ChildSpriteSize;
			obj.anchoredPosition = data.ChildSpritePosition;
		}
	}
}
