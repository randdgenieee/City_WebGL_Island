using UnityEngine;
using UnityEngine.UI;

namespace Tweening
{
	public class ScrollingImage : MonoBehaviour
	{
		private const string MainTex = "_MainTex";

		[SerializeField]
		private Material _material;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private Vector2 _scrollOffset;

		private Material _materialInstance;

		private Vector2 _offset;

		private void Awake()
		{
			_image.material = (_materialInstance = new Material(_material));
		}

		private void Update()
		{
			_offset += _scrollOffset * Time.deltaTime;
			_materialInstance.SetTextureOffset("_MainTex", _offset);
		}
	}
}
