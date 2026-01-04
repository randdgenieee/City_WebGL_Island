using UnityEngine;

public class BackgroundLoader : MonoBehaviour
{
	private static readonly Vector2 ImageSize = new Vector2(1024f, 1024f);

	[SerializeField]
	private JPEGLoader _jpegLoaderPrefab;

	public void Initialize(TextAsset[] backgroundImages, int width)
	{
		int i = 0;
		for (int num = backgroundImages.Length; i < num; i++)
		{
			JPEGLoader jPEGLoader = UnityEngine.Object.Instantiate(_jpegLoaderPrefab, base.transform);
			jPEGLoader.Init(backgroundImages[i]);
			jPEGLoader.transform.localPosition = new Vector3(ImageSize.x * (float)(i % width), (0f - ImageSize.y) * (float)(i / width), 0f);
		}
	}
}
