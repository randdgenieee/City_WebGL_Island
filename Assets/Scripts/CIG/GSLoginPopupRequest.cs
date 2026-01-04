using System;

namespace CIG
{
	public class GSLoginPopupRequest : PopupRequest
	{
		public Action<bool> CloseAction
		{
			get;
			private set;
		}

		public GSLoginPopupRequest(Action<bool> closeAction)
			: base(typeof(GSLoginPopup), enqueue: false)
		{
			CloseAction = closeAction;
		}
	}
}
