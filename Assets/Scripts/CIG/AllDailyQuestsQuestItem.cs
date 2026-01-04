using UnityEngine;

namespace CIG
{
	public class AllDailyQuestsQuestItem : QuestItemBase
	{
		[SerializeField]
		private QuestItemStar _leftStar;

		[SerializeField]
		private QuestItemStar _rightStar;

		[SerializeField]
		private QuestItemStar _middleStar;

		private long _targetIndex;

		private long _showingProgress;

		public override void Initialize(Quest quest)
		{
			_showingProgress = quest.Progress;
			_targetIndex = _showingProgress + 1;
			_leftStar.Initialize(_showingProgress >= 1);
			_rightStar.Initialize(_showingProgress >= 2);
			_middleStar.Initialize(_showingProgress >= 3);
			base.Initialize(quest);
		}

		public RectTransform GetNextTarget()
		{
			QuestItemStar obj = (_targetIndex >= 3) ? _middleStar : ((_targetIndex == 2) ? _rightStar : _leftStar);
			_targetIndex++;
			return (RectTransform)obj.transform;
		}

		public void OnAnimationFinished()
		{
			_showingProgress++;
			if (_showingProgress == _quest.Progress)
			{
				UpdateLook();
			}
			else
			{
				UpdateProgress();
			}
		}

		protected override void UpdateProgress()
		{
			_leftStar.UpdateLook(_showingProgress >= 1);
			_rightStar.UpdateLook(_showingProgress >= 2);
			_middleStar.UpdateLook(_showingProgress >= 3);
		}
	}
}
