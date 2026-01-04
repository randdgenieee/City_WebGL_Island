using System;
using UnityEngine;

public class JPEGLoader : MonoBehaviour
{
	[SerializeField]
	protected SpriteRenderer _spriteRenderer;

	private static int _memorySize = -1;

	private static int _maxTextureSize = -1;

	private Texture2D _texture;

	private Sprite _sprite;

	private static int TextureScalingFactor
	{
		get
		{
			if (MemorySize < 1024 || MaxTextureSize < 4096)
			{
				return 4;
			}
			if (MemorySize > 1500)
			{
				return 1;
			}
			return 2;
		}
	}

	private static int MemorySize
	{
		get
		{
			if (_memorySize < 0)
			{
				_memorySize = SystemInfo.graphicsMemorySize + SystemInfo.systemMemorySize;
			}
			return _memorySize;
		}
	}

	private static int MaxTextureSize
	{
		get
		{
			if (_maxTextureSize < 0)
			{
				_maxTextureSize = SystemInfo.maxTextureSize;
			}
			return _maxTextureSize;
		}
	}

	public void Init(TextAsset asset)
	{
		LoadTexture(asset.bytes, out _sprite, out _texture);
		_spriteRenderer.sprite = _sprite;
	}

	private void OnDestroy()
	{
		UnityEngine.Object.DestroyImmediate(_sprite);
		UnityEngine.Object.DestroyImmediate(_texture);
	}

	private static void LoadTexture(byte[] jpegBytes, out Sprite sprite, out Texture2D texture)
	{
		float pixelsPerUnit = 1f;
		texture = null;
		int textureScalingFactor = TextureScalingFactor;
		if (textureScalingFactor > 1)
		{
			//TODODO
			//try
			//{
			//	using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.innovattic.BitmapDecoder"))
			//	{
			//		int[] array = androidJavaClass.CallStatic<int[]>("getDimensions", new object[1]
			//		{
			//			jpegBytes
			//		});
			//		int num = array[0];
			//		int num2 = array[1];
			//		int[] array2 = androidJavaClass.CallStatic<int[]>("decodeJPEG", new object[4]
			//		{
			//			jpegBytes,
			//			num,
			//			num2,
			//			textureScalingFactor
			//		});
			//		int num3 = array2.Length;
			//		Color[] array3 = new Color[num3];
			//		for (int i = 0; i < num3; i++)
			//		{
			//			byte r = (byte)((array2[i] & 0xFF0000) >> 16);
			//			byte g = (byte)((array2[i] & 0xFF00) >> 8);
			//			byte b = (byte)(array2[i] & 0xFF);
			//			array3[i] = new Color32(r, g, b, byte.MaxValue);
			//		}
			//		texture = new Texture2D(num / textureScalingFactor, num2 / textureScalingFactor, TextureFormat.RGB24, mipChain: false);
			//		texture.SetPixels(array3);
			//		texture.wrapMode = TextureWrapMode.Clamp;
			//		texture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
			//		pixelsPerUnit = 1f / (float)textureScalingFactor;
			//	}
			//}
			//catch (Exception ex)
			//{
			//	UnityEngine.Debug.LogError("Unable to downscale textures from JPEG because " + ex.Message + " (" + ex.GetType().Name + ")");
			//}
		}
		if (texture == null)
		{
			texture = new Texture2D(0, 0, TextureFormat.RGB24, mipChain: false);
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.LoadImage(jpegBytes);
			texture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
		}
		sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0f, 1f), pixelsPerUnit);
	}
}
