using SparklingGameCenterSDK;
using System;
using System.Collections.Generic;

namespace CIG
{
	public class GameCenterService
	{
		public delegate void SignatureVerificationCallback(string publicKeyUrl, string signature, string salt, ulong timestamp);

		private bool _authenticating;

		private readonly Queue<KeyValuePair<Action, Action<string>>> _onAuthenticatedEventQueue = new Queue<KeyValuePair<Action, Action<string>>>();

		public void Authenticate(Action onSuccess, Action<string> onError)
		{
			_onAuthenticatedEventQueue.Enqueue(new KeyValuePair<Action, Action<string>>(onSuccess, onError));
			if (GetIsAuthenticated())
			{
				FireAuthenticatedSucceededEvents();
			}
			else if (!_authenticating)
			{
				_authenticating = true;
				SparklingGameCenterSDK.SparklingGameCenterSDK.AuthenticatePlayer(delegate
				{
					_authenticating = false;
					FireAuthenticatedSucceededEvents();
				}, delegate(string err)
				{
					_authenticating = false;
					FireAuthenticatedErrorEvents(err);
				});
			}
		}

		public void GetSignatureVerification(SignatureVerificationCallback onSuccess, Action<string> onError)
		{
			DoAuthenticated(delegate
			{
				GetSignatureVerificationInternal(onSuccess, onError);
			}, onError);
		}

		public string GetDisplayName()
		{
			return SparklingGameCenterSDK.SparklingGameCenterSDK.GetDisplayName();
		}

		public string GetPlayerId()
		{
			return SparklingGameCenterSDK.SparklingGameCenterSDK.GetPlayerId();
		}

		public bool GetIsAuthenticated()
		{
			return SparklingGameCenterSDK.SparklingGameCenterSDK.GetIsAuthenticated();
		}

		private void DoAuthenticated(Action action, Action<string> error)
		{
			if (GetIsAuthenticated())
			{
				EventTools.Fire(action);
			}
			else
			{
				Authenticate(action, error);
			}
		}

		private void FireAuthenticatedSucceededEvents()
		{
			while (_onAuthenticatedEventQueue.Count > 0)
			{
				EventTools.Fire(_onAuthenticatedEventQueue.Dequeue().Key);
			}
		}

		private void FireAuthenticatedErrorEvents(string error)
		{
			while (_onAuthenticatedEventQueue.Count > 0)
			{
				EventTools.Fire(_onAuthenticatedEventQueue.Dequeue().Value, error);
			}
		}

		private void GetSignatureVerificationInternal(SignatureVerificationCallback success, Action<string> error)
		{
			SparklingGameCenterSDK.SparklingGameCenterSDK.GetSignatureVerification(delegate(string publicKeyUrl, string signature, string salt, ulong timestamp)
			{
				if (success != null)
				{
					success(publicKeyUrl, signature, salt, timestamp);
				}
			}, delegate(string err)
			{
				EventTools.Fire(error, err);
			});
		}
	}
}
