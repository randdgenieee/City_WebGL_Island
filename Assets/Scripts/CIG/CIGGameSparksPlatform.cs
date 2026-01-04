using GameSparks;
using GameSparks.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class CIGGameSparksPlatform : IGSPlatform
	{
		private const string API_KEY = "g355052KciA3";

		private const string API_SECRET = "hUFX1nq83NTaf0k7phLX9ntvWWNSPppL";

		private const string API_CREDENTIAL = "device";

		private const string API_DOMAIN = null;

		private const string API_STAGE = "live";

		private const int DEFAULT_TIMEOUT_SECONDS = 30;

		private List<Action> _actions = new List<Action>();

		private List<Action> _currentActions = new List<Action>();

		public string DeviceId
		{
			get;
			private set;
		}

		public string DeviceOS
		{
			get;
			private set;
		}

		public string Platform
		{
			get;
			private set;
		}

		public string SDK
		{
			get;
			private set;
		}

		public string DeviceType
		{
			get;
			private set;
		}

		public GSData DeviceStats
		{
			get;
			private set;
		}

		public bool ExtraDebug => false;

		public string ApiKey => "g355052KciA3";

		public string ApiSecret => "hUFX1nq83NTaf0k7phLX9ntvWWNSPppL";

		public string ApiCredential => "device";

		public string ApiStage => "live";

		public string ApiDomain => null;

		public string AuthToken
		{
			get;
			set;
		}

		public string UserId
		{
			get;
			set;
		}

		public int RequestTimeoutSeconds
		{
			get;
			set;
		}

		public string PersistentDataPath
		{
			get;
			private set;
		}

		public CIGGameSparksPlatform()
		{
			DeviceOS = SystemInfo.operatingSystem;
			Platform = Application.platform.ToString();
			SDK = GS.Version;
			DeviceType = SystemInfo.deviceType.ToString();
			PersistentDataPath = Application.persistentDataPath;
			RequestTimeoutSeconds = 30;
			DeviceStats = new GSData(new Dictionary<string, object>
			{
				{
					"model",
					SystemInfo.deviceModel
				},
				{
					"memory",
					SystemInfo.systemMemorySize + " MB"
				},
				{
					"os.name",
					DeviceOS
				},
				{
					"os.version",
					DeviceOS
				},
				{
					"cpu.cores",
					SystemInfo.processorCount
				},
				{
					"cpu.vendor",
					SystemInfo.processorType
				},
				{
					"resolution",
					Screen.width + " x " + Screen.height
				},
				{
					"gssdk",
					SDK
				}
			});
		}

		public void DebugMsg(string message)
		{
		}

		public void ExecuteOnMainThread(Action action)
		{
			lock (_actions)
			{
				_actions.Add(action);
			}
		}

		public void UpdateFromMainThread()
		{
			lock (_actions)
			{
				_currentActions.AddRange(_actions);
				_actions.Clear();
			}
			int i = 0;
			for (int count = _currentActions.Count; i < count; i++)
			{
				_currentActions[i]?.Invoke();
			}
			_currentActions.Clear();
		}

		public IGameSparksTimer GetTimer()
		{
			return new GameSparksTimer();
		}

		public string MakeHmac(string stringToHmac, string secret)
		{
			return GameSparksUtil.MakeHmac(stringToHmac, secret);
		}

		public IGameSparksWebSocket GetSocket(string url, Action<string> messageReceived, Action closed, Action opened, Action<string> error)
		{
			GameSparksWebSocket gameSparksWebSocket = new GameSparksWebSocket();
			gameSparksWebSocket.Initialize(url, messageReceived, closed, opened, error);
			return gameSparksWebSocket;
		}
	}
}
