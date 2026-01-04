using System;
using UnityEngine;

namespace CIG
{
	public class ExceptionReporter : MonoBehaviour
	{
		public const string PrefsUserKeyKey = "UserKey";

		public const string PrefsUserIdKey = "UserId";

		private static readonly LogType[] CrashTypes = new LogType[1]
		{
			LogType.Exception
		};

		private ILogHandler _logHandler;

		private ILogHandler LogHandler
		{
			get
			{
				if (_logHandler == null)
				{
					_logHandler = new LogHandler(this);
				}
				return _logHandler;
			}
		}

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		public void Initialize()
		{
			Deinitialize();
			Application.logMessageReceived += HandleLog;
		}

		private void Deinitialize()
		{
			Application.logMessageReceived -= HandleLog;
		}

		private void OnDestroy()
		{
			Deinitialize();
		}

		public static void SetEdwinUserId(string userId, string userKey)
		{
			PlayerPrefs.SetString("UserId", userId);
			PlayerPrefs.SetString("UserKey", userKey);
		}

		private void HandleLog(string logString, string stackTrace, LogType type)
		{
			bool flag = Array.IndexOf(CrashTypes, type) != -1;
			if (flag)
			{
				Application.logMessageReceived -= HandleLog;
			}
			LogHandler.HandleLog(logString, stackTrace, flag, type);
		}
	}
}
