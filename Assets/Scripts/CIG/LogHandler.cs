using CIG.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CIG
{
	public class LogHandler : ILogHandler
	{
		private static readonly LogType[] ReportTypes = new LogType[3]
		{
			LogType.Assert,
			LogType.Error,
			LogType.Exception
		};

		private static readonly string[] Blacklist = new string[1]
		{
			"[EGL]"
		};

		private const float LogCooldownSeconds = 1f;

		private readonly MonoBehaviour _monoBehaviour;

		private readonly Dictionary<string, float> _logCooldowns = new Dictionary<string, float>();

		private static string ErrorUrl => WebService.Domain + "error.php";

		public LogHandler(MonoBehaviour monoBehaviour)
		{
			_monoBehaviour = monoBehaviour;
		}

		void ILogHandler.HandleLog(string logString, string stackTrace, bool isCrash, LogType type)
		{
			if (Array.IndexOf(ReportTypes, type) != -1 && (isCrash || !IsBlacklisted(logString)))
			{
				if (type != LogType.Exception)
				{
					stackTrace = Environment.StackTrace;
				}
				string text = logString + "\n" + stackTrace;
				float unscaledTime = Time.unscaledTime;
				if (!_logCooldowns.TryGetValue(text, out float value) || unscaledTime > value + 1f)
				{
					_logCooldowns[text] = unscaledTime;
					UnityWebRequest reportRequest = GetReportRequest(text, isCrash);
					_monoBehaviour.StartCoroutine(WaitForSend(reportRequest, isCrash));
				}
			}
		}

		private static bool IsBlacklisted(string logString)
		{
			int i = 0;
			for (int num = Blacklist.Length; i < num; i++)
			{
				if (logString.Contains(Blacklist[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static UnityWebRequest GetReportRequest(string log, bool isCrash)
		{
			WWWForm wWWForm = new WWWForm();
			return UnityWebRequest.Post(ErrorUrl, wWWForm);
		}

		private static IEnumerator WaitForSend(UnityWebRequest webRequest, bool notifyAndExit)
		{
			using (webRequest)
			{
				yield return webRequest.SendWebRequest();
				if (notifyAndExit)
				{
					try
					{
						Analytics.LogEvent("exceptionreport");
					}
					catch
					{
					}
					Notifier.ScheduleNotification(new NativeNotification(3, "City Island 5", Localization.Key("notification.crashed").Translate(), sound: false, 0, 0));
					CIGApp.ForceQuit();
				}
			}
		}
	}
}
