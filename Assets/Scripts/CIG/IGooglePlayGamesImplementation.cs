using System;

namespace CIG
{
	public interface IGooglePlayGamesImplementation
	{
		bool Authenticated
		{
			get;
		}

		void Authenticate(Action onSuccess, Action<string> onError);

		void SignOut();

		void GetServerAuthCode(Action<string> onSuccess, Action<string> onError);

		void GetDisplayName(Action<string> onSuccess, Action<string> onError);

		void GetUserId(Action<string> onSuccess, Action<string> onError);
	}
}
