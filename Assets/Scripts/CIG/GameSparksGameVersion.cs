using GameSparks.Core;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class GameSparksGameVersion
	{
		private const string MajorKey = "Major";

		private const string MinorKey = "Minor";

		private const string RevisionKey = "Revision";

		public int Major
		{
			get;
			private set;
		}

		public int Minor
		{
			get;
			private set;
		}

		public int Revision
		{
			get;
			private set;
		}

		public GameSparksGameVersion(string gameVersion)
		{
			TryParse(gameVersion);
		}

		public GameSparksGameVersion(GSData data)
		{
			if (data != null)
			{
				Major = (data.GetInt("Major") ?? 0);
				Minor = (data.GetInt("Minor") ?? 0);
				Revision = (data.GetInt("Revision") ?? 0);
			}
		}

		public GSData ToGSData()
		{
			return new GSData(new Dictionary<string, object>
			{
				{
					"Major",
					Major
				},
				{
					"Minor",
					Minor
				},
				{
					"Revision",
					Revision
				}
			});
		}

		public override string ToString()
		{
			return $"{Major}.{Minor}.{Revision}";
		}

		private void TryParse(string versionString)
		{
			string[] array = versionString.Split('.');
			if (array.Length != 3)
			{
				UnityEngine.Debug.LogErrorFormat("Invalid version format! Should be x.y.z, is {0}", versionString);
				return;
			}
			if (int.TryParse(array[0], out int result))
			{
				Major = result;
			}
			else
			{
				UnityEngine.Debug.LogErrorFormat("Invalid version format! Major version ({0}) is not an integer!", versionString);
			}
			if (int.TryParse(array[1], out result))
			{
				Minor = result;
			}
			else
			{
				UnityEngine.Debug.LogErrorFormat("Invalid version format! Minor version ({0}) is not an integer!", versionString);
			}
			if (int.TryParse(array[2], out result))
			{
				Revision = result;
			}
			else
			{
				UnityEngine.Debug.LogErrorFormat("Invalid version format! Revision version ({0}) is not an integer!", versionString);
			}
		}
	}
}
