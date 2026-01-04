using UnityEngine;
using UnityEngine.Networking;

namespace CIG
{
	public class CachedWebRequestTexture : CachedWebRequestBase
	{
		public Texture2D Texture => DownloadHandlerTexture.GetContent(_download);

		public CachedWebRequestTexture(string url, int expiresInDays = 1)
			: base(url, UnityWebRequestTexture.GetTexture, expiresInDays)
		{
		}
	}
}
