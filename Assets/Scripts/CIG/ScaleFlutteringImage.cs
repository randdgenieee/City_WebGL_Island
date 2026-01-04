using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class ScaleFlutteringImage : FlutteringImage
	{
		[SerializeField]
		private Image _image;

		[SerializeField]
		private float _minScale = 0.6f;

		[SerializeField]
		private float _maxScale = 1f;

		public override Vector3 Extents => _image.sprite.bounds.extents * _image.sprite.pixelsPerUnit;

		public override void Initialize()
		{
			float num = Random.Range(_minScale, _maxScale);
			base.transform.localScale = new Vector3(num, num, 1f);
		}
	}
}
