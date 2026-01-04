using UnityEngine;

namespace CIG
{
	public class HUDRegionCurrencyElement : HUDRegionElement
	{
		[SerializeField]
		private HUDCurrencyTweenHelper _hudCurrencyTweenHelper;

		private void Awake()
		{
			_hudCurrencyTweenHelper.FlyingCurrencyPlayingChangedEvent += OnFlyingCurrencyPlayingChanged;
		}

		private void OnDestroy()
		{
			if (_hudCurrencyTweenHelper != null)
			{
				_hudCurrencyTweenHelper.FlyingCurrencyPlayingChangedEvent -= OnFlyingCurrencyPlayingChanged;
			}
		}

		protected override void UpdateVisibility()
		{
			base.gameObject.SetActive(_hudCurrencyTweenHelper.FlyingCurrencyIsPlaying || _regionShowing);
		}

		private void OnFlyingCurrencyPlayingChanged(bool isPlaying)
		{
			UpdateVisibility();
		}
	}
}
