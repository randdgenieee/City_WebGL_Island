using UnityEngine.Networking;

namespace CIG
{
	public class CachedWebRequest : CachedWebRequestBase
	{
		public string Text => _download.downloadHandler.text;

		public CachedWebRequest(string url, int expiresInDays = 1)
			: base(url, UnityWebRequest.Get, expiresInDays)
		{
		}
	}
}
