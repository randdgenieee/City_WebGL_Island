using CIG.Translation;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class HUDProgressBar : HUDRegionElement
	{
		[SerializeField]
		private LocalizedText _label;

		[SerializeField]
		private Slider _progressBar;

		public void UpdateProgress(float progress, ILocalizedString label)
		{
			_label.LocalizedString = label;
			_progressBar.value = progress;
		}
	}
}
