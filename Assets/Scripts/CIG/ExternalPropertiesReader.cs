using System.IO;
using UnityEngine;

namespace CIG
{
	public static class ExternalPropertiesReader
	{
		public static string ReadExternalObjectProperties(string fileName)
		{
			return null;
		}

		public static string GetDownloadsFolderPath()
		{
			return null;
		}

		private static string GetExternalObjectPropertiesPath(string fileName)
		{
			if (Application.isEditor)
			{
				return null;
			}
			string downloadsFolderPath = GetDownloadsFolderPath();
			if (string.IsNullOrEmpty(downloadsFolderPath))
			{
				throw new IOException("Getting Android downloads folder");
			}
			if (!Directory.Exists(downloadsFolderPath))
			{
				UnityEngine.Debug.LogWarningFormat("Downloads folder not found at {0}", downloadsFolderPath);
			}
			string text = Path.Combine(downloadsFolderPath, $"{fileName}.properties.txt");
			if (File.Exists(text))
			{
				return text;
			}
			text = Path.Combine(downloadsFolderPath, $"{fileName}.properties");
			if (File.Exists(text))
			{
				return text;
			}
			text = Path.Combine("/storage/emulated/legacy/Download", $"{fileName}.properties.txt");
			if (File.Exists(text))
			{
				return text;
			}
			text = Path.Combine("/storage/emulated/legacy/Download", $"{fileName}.properties");
			if (File.Exists(text))
			{
				return text;
			}
			return null;
		}
	}
}
