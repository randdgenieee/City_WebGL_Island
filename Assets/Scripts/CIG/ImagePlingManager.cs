using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class ImagePlingManager : PlingManager
	{
		[SerializeField]
		private Image _image;

		protected override Vector3 PlingPosition => _image.transform.position + (Vector3)_image.rectTransform.rect.center;
	}
}
