using Tweening;
using UnityEngine;

namespace CIG
{
	public class GridTileIcon : Overlay
	{
		[SerializeField]
		private int _priority;

		private bool _isShowing = true;

		public int Priority => _priority;

		public virtual bool ShowWhileTilesHidden => false;

		public override void Show()
		{
			if (!_isShowing)
			{
				base.Show();
				_isShowing = true;
			}
		}

		public virtual void Hide()
		{
			if (_isShowing)
			{
				TryStopTweener();
				_tweener.FinishedPlaying += OnHideTweenerFinishedPlaying;
				_tweener.PlayReverse();
				_isShowing = false;
			}
		}

		public void HideInstant()
		{
			if (_isShowing)
			{
				_isShowing = false;
				OnIconHidden();
			}
		}

		protected virtual void OnIconHidden()
		{
			base.gameObject.SetActive(value: false);
		}

		protected override void TryStopTweener()
		{
			_tweener.FinishedPlaying -= OnHideTweenerFinishedPlaying;
			base.TryStopTweener();
		}

		private void OnHideTweenerFinishedPlaying(Tweener tweener)
		{
			tweener.FinishedPlaying -= OnHideTweenerFinishedPlaying;
			OnIconHidden();
		}
	}
}
