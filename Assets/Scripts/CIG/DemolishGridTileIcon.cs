using Tweening;
using UnityEngine;

namespace CIG
{
	public class DemolishGridTileIcon : ButtonGridTileIcon
	{
		[SerializeField]
		private Tweener _idleTweener;

		public override void Hide()
		{
			if (_idleTweener.IsPlaying)
			{
				_idleTweener.Stop();
			}
			base.Hide();
		}

		public override void Remove()
		{
			if (_idleTweener.IsPlaying)
			{
				_idleTweener.Stop();
			}
			base.Remove();
		}

		protected override void OnOverlayShown()
		{
			base.OnOverlayShown();
			if (_idleTweener.IsPlaying)
			{
				_idleTweener.Stop();
			}
			_idleTweener.Play();
		}
	}
}
