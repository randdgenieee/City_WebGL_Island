namespace CIG
{
	public class SessionManager
	{
		public delegate void LeaveGameHandler();

		public delegate void ReturnToGameHandler();

		public event LeaveGameHandler LeaveGame;

		public event ReturnToGameHandler ReturnToGame;

		private void FireLeaveGame()
		{
			this.LeaveGame?.Invoke();
		}

		private void FireReturnToGame()
		{
			this.ReturnToGame?.Invoke();
		}

		public void ApplicationPause(bool pause)
		{
			if (pause)
			{
				FireLeaveGame();
			}
			else
			{
				FireReturnToGame();
			}
		}

		public void ApplicationQuit()
		{
			FireLeaveGame();
		}
	}
}
