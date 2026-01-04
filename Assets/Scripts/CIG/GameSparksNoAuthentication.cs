using System;

namespace CIG
{
	public class GameSparksNoAuthentication : GameSparksAuthentication
	{
		public override bool IsAuthenticated => false;

		public GameSparksNoAuthentication(Settings settings)
			: base(null, settings)
		{
		}

		public override void Authenticate(Action<bool> onSuccess, Action<GameSparksException> onError)
		{
			EventTools.Fire(onSuccess, value0: false);
		}

		public override void ChangeDisplayName(string displayName, Action onSuccess, Action<GameSparksException> onError)
		{
			EventTools.Fire(onError, new GameSparksException("GameSparksNoAuthentication Authenticate", GSError.NoConnection));
		}

		public override string ToString()
		{
			return $"GameSparksNoAuthentication: {IsAuthenticated}. ID: {base.PlayerId}";
		}
	}
}
