using UnityEngine;

namespace CIG
{
	public class PropertiesFile : PropertiesDictionary
	{
		public bool IsExternal
		{
			get;
			private set;
		}

		public PropertiesFile(string fileName)
		{
			string text = LoadFromFile(fileName);
			IsExternal = !string.IsNullOrEmpty(text);
			if (!IsExternal)
			{
				text = LoadFromAsset(fileName);
			}
			_data = Parser.ParsePropertyFileFormat(text);
			Initialize();
		}

		private string LoadFromAsset(string assetName)
		{
			IsExternal = false;
			TextAsset textAsset = Resources.Load<TextAsset>(assetName + ".properties");
			if (textAsset == null)
			{
				UnityEngine.Debug.LogErrorFormat("Unable to open TextAsset {0}.properties.", assetName);
				return string.Empty;
			}
			return textAsset.text;
		}

		private string LoadFromFile(string fileName)
		{
			return null;
		}
	}
}
