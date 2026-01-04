using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CIG
{
	public abstract class CachedWebRequestBase : CustomYieldInstruction, IDisposable
	{
		private const string CacheFolderName = "Cache";

		private const string ExpirationDateKey = "EXPIRATION_DATE_KEY_{0}";

		private const int URLKeyIDSizeA = 6;

		private const int URLKeyIDSizeB = 3;

		protected readonly UnityWebRequest _download;

		private readonly AsyncOperation _downloadOperation;

		private readonly bool _loadFromCache;

		private readonly int _expiresInDays;

		public override bool keepWaiting
		{
			get
			{
				if (_downloadOperation.isDone)
				{
					if (!_loadFromCache && _download.error == null)
					{
						Save(_download.url, _download.downloadHandler.data, _expiresInDays);
					}
					return false;
				}
				return true;
			}
		}

		public string Url
		{
			get;
			private set;
		}

		public string Error => _download.error;

		public bool IsDone => _download.isDone;

		protected CachedWebRequestBase(string url, Func<string, UnityWebRequest> getWebRequest, int expiresInDays = 1)
		{
			Url = url;
			_expiresInDays = expiresInDays;
			if (IsCached(url) && !IsExpired(url))
			{
				url = "file://" + ConvertToFilePath(url);
				_loadFromCache = true;
			}
			_download = getWebRequest(url);
			_downloadOperation = _download.SendWebRequest();
		}

		private bool Save(string url, byte[] bytes, int expireInDays)
		{
			string text = ConvertToFilePath(url);
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(text));
				File.WriteAllBytes(text, bytes);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogErrorFormat("Cannot save {0} to: {1}{2}{3}", url, text, Environment.NewLine, ex);
				return false;
			}
			SaveExpirationDate(GetExpirationKey(url), expireInDays);
			return true;
		}

		private bool IsCached(string url)
		{
			try
			{
				if (File.Exists(ConvertToFilePath(url)))
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogErrorFormat("Error checking download exists in cache: {0}{1}{2}", ConvertToFilePath(url), Environment.NewLine, ex);
				return false;
			}
			return false;
		}

		private string GetExpirationKey(string url)
		{
			return $"EXPIRATION_DATE_KEY_{ConvertToFileName(url)}";
		}

		private bool IsExpired(string url)
		{
			string expirationKey = GetExpirationKey(url);
			if (PlayerPrefs.HasKey(expirationKey))
			{
				bool result = true;
				if (DateTime.TryParse(PlayerPrefs.GetString(expirationKey), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result2))
				{
					result = (result2.Subtract(AntiCheatDateTime.UtcNow).Days < 0);
				}
				return result;
			}
			return true;
		}

		private void SaveExpirationDate(string expirationKey, int expireInDays)
		{
			PlayerPrefs.SetString(expirationKey, AntiCheatDateTime.UtcNow.AddDays(expireInDays).ToString(CultureInfo.InvariantCulture));
		}

		private string ConvertToFilePath(string url)
		{
			string fileName = ConvertToFileName(url);
			return GetFilePath(fileName);
		}

		private string GetFilePath(string fileName)
		{
			return string.Format("{0}/{1}/{2}", Application.temporaryCachePath, "Cache", fileName);
		}

		private string ConvertToFileName(string url)
		{
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = Sha256.HashToBytes(Encoding.ASCII.GetBytes(url));
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			if (stringBuilder.Length >= 6)
			{
				int length = Mathf.Max(stringBuilder.Length - 9, 0);
				stringBuilder.Remove(6, length);
			}
			return stringBuilder.ToString();
		}

		void IDisposable.Dispose()
		{
			if (_download != null)
			{
				_download.Dispose();
			}
		}
	}
}
