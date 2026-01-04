using CIG.Translation;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class QuestItem : QuestItemBase
	{
		[SerializeField]
		private LocalizedText _progressLabel;

		[SerializeField]
		private Slider _progressBar;

		public override void Initialize(Quest quest)
		{
			base.Initialize(quest);
			_quest.StateChangedEvent += OnStateChanged;
			_quest.ProgressChangedEvent += OnProgressChanged;
		}

		public override void Deinitialize()
		{
			if (_quest != null)
			{
				_quest.StateChangedEvent -= OnStateChanged;
				_quest.ProgressChangedEvent -= OnProgressChanged;
			}
			base.Deinitialize();
		}

		protected override void UpdateProgress()
		{
			long l = Math.Min(_quest.Progress, _quest.TargetAmount);
			_progressLabel.LocalizedString = Localization.Format(Localization.Literal("{0} / {1}"), Localization.Integer(l), Localization.Integer(_quest.TargetAmount));
			_progressBar.value = _quest.ProgressPercent;
		}

		private void OnStateChanged(Quest quest, QuestState state)
		{
			UpdateLook();
		}

		private void OnProgressChanged(long oldProgress, long newProgress)
		{
			UpdateProgress();
		}
	}
}
