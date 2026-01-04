using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public abstract class QuestButton : MonoBehaviour
	{
		[SerializeField]
		private CompositeSpriteImage _questSprite;

		[SerializeField]
		private GameObject _achievedRoot;

		[SerializeField]
		private GameObject _collectedRoot;

		[SerializeField]
		private Image _progressBar;

		private Quest _quest;

		public void Initialize(Quest quest)
		{
			Deinitialize();
			_quest = quest;
			_quest.StateChangedEvent += OnStateChanged;
			_quest.ProgressChangedEvent += OnProgressChanged;
			UpdateVisual();
		}

		public virtual void Deinitialize()
		{
			if (_quest != null)
			{
				_quest.StateChangedEvent -= OnStateChanged;
				_quest.ProgressChangedEvent -= OnProgressChanged;
				_quest = null;
			}
		}

		protected virtual void OnDestroy()
		{
			Deinitialize();
		}

		public abstract void OnClicked();

		private void UpdateVisual()
		{
			_questSprite.Initialize(SingletonMonobehaviour<QuestSpriteAssetCollection>.Instance.GetAsset(_quest.QuestSpriteType).SmallSpriteData);
			_progressBar.fillAmount = _quest.ProgressPercent;
			_achievedRoot.SetActive(_quest.State == QuestState.Achieved);
			_collectedRoot.SetActive(_quest.State == QuestState.Completed);
		}

		private void OnStateChanged(Quest quest, QuestState newstate)
		{
			UpdateVisual();
		}

		private void OnProgressChanged(long oldprogess, long newprogress)
		{
			UpdateVisual();
		}
	}
}
