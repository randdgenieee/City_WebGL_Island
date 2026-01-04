using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class SSPFaqContentView : SSPMenuContentView
	{
		public override ILocalizedString HeaderText => Localization.Key("SSP_MENU_FAQ");

		public override SSPMenuPopup.SSPMenuTab Tab => SSPMenuPopup.SSPMenuTab.Faq;

		public void OnFaqClicked()
		{
			Application.OpenURL("https://www.sparklingsociety.net/cig5-forum");
		}
	}
}
