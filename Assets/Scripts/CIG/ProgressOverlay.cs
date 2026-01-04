using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class ProgressOverlay : Overlay
	{
		[SerializeField]
		private RectTransform _barRoot;

		[SerializeField]
		private Image _progress;

		[SerializeField]
		private LocalizedText _timer;

		private UpspeedableProcess _process;

		private void Update()
		{
			_progress.fillAmount = _process.CompletionPercentage;
			_timer.LocalizedString = Localization.TimeSpan(TimeSpan.FromSeconds(_process.TimeLeft), hideSecondPartWhenZero: false);
		}

		public void Initialize(UpspeedableProcess process)
		{
			_process = process;
		}

		public void Initialize(UpspeedableProcess process, float overlayOffset)
		{
			Initialize(process);
			_barRoot.anchoredPosition = Vector3.up * overlayOffset;
		}
	}
}
