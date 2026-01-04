using System;

namespace CIG
{
	public class CloudStorageConflictPopupRequest : PopupRequest
	{
		public delegate void ConflictResolutionCallback(Action callback);

		public ConflictResolutionCallback LocalAction
		{
			get;
			private set;
		}

		public ConflictResolutionCallback RemoteAction
		{
			get;
			private set;
		}

		public DateTime UTCSaveTime
		{
			get;
			private set;
		}

		public CloudGameState RemoteGameState
		{
			get;
			private set;
		}

		public Action CloseAction
		{
			get;
			private set;
		}

		public CloudStorageConflictPopupRequest(ConflictResolutionCallback localAction, ConflictResolutionCallback remoteAction, DateTime utcSaveTime, CloudGameState remoteGameState, Action closeAction)
			: base(typeof(CloudStorageConflictPopup), enqueue: false, dismissable: false)
		{
			LocalAction = localAction;
			RemoteAction = remoteAction;
			UTCSaveTime = utcSaveTime;
			RemoteGameState = remoteGameState;
			CloseAction = closeAction;
		}
	}
}
