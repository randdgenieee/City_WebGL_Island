using CIG.Translation;
using UnityEngine;

namespace CIG
{
	public class TreasureChestContentsPlatinumElement : MonoBehaviour
	{
		[SerializeField]
		private FlutteringImageSystem _boostFlutteringImageSystem;

		[SerializeField]
		private FlutteringImageSystem _goldFlutteringImageSystem;

		[SerializeField]
		private LocalizedText _goldChestsText;

		public void Initialize(Timing timing)
		{
			_boostFlutteringImageSystem.Initialize(timing);
			_goldFlutteringImageSystem.Initialize(timing);
			_goldChestsText.LocalizedString = Localization.Format(Localization.Key("content_x_golden_chests"), Localization.Integer(2).FontSize(50));
		}

		public void Enable()
		{
			base.gameObject.SetActive(value: true);
			_boostFlutteringImageSystem.Play();
			_goldFlutteringImageSystem.Play();
		}

		public void Disable()
		{
			_boostFlutteringImageSystem.Stop();
			_goldFlutteringImageSystem.Stop();
			base.gameObject.SetActive(value: false);
		}
	}
}
