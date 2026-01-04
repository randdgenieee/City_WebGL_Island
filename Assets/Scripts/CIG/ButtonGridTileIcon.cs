using System;

namespace CIG
{
	public class ButtonGridTileIcon : GridTileIcon
	{
		private Action _onClicked;

		public void Init(Action onClicked)
		{
			_onClicked = onClicked;
		}

		public void OnClicked()
		{
			EventTools.Fire(_onClicked);
		}
	}
}
