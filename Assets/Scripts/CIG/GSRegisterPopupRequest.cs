using System;

namespace CIG
{
	public class GSRegisterPopupRequest : PopupRequest
	{
		public Action<bool> CloseAction
		{
			get;
			private set;
		}

		public GSRegisterPopupRequest(Action<bool> closeAction)
			: base(typeof(GSRegisterPopup), enqueue: false)
		{
			CloseAction = closeAction;
		}
	}
}
