using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace SUISS.Storage
{
	public class Lifecycles
	{
		public const string RootFolder = "Storage";

		public const string GameFolder = "Game";

		public const string PurchasesFolder = "P";

		public const string DefaultPlayer = "Player";

		public static string GetPath(StorageLifecycle lifecycle, string player = "Player")
		{
			string text = null;
			if (lifecycle != StorageLifecycle.Session)
			{
				text = "Storage";
				switch (lifecycle)
				{
				case StorageLifecycle.Purchases:
					text = Path.Combine(text, "P");
					break;
				default:
					text = Path.Combine(text, UnityWebRequest.EscapeURL(player));
					if (lifecycle != StorageLifecycle.Player)
					{
						text = Path.Combine(text, "Game");
					}
					break;
				case StorageLifecycle.Forever:
					break;
				}
			}
			if (text != null)
			{
				text = Path.Combine(Application.persistentDataPath, text);
			}
			return text;
		}
	}
}
