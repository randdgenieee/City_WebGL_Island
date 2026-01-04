using Tweening;
using UnityEngine;

namespace CIG
{
	public class TreasureChestSingleRewardItem : MonoBehaviour
	{
		[SerializeField]
		private BuildingImage _image;

		[SerializeField]
		private LocalizedText _text;

		[SerializeField]
		private Tweener _appearTweener;

		[SerializeField]
		private Tweener _vanishTweener;

		[SerializeField]
		private GameObject _particlesObject;

		[SerializeField]
		private CurrencyView _currencyView;

		public void Initialize(RewardItemData data)
		{
			if (data.BuildingProperties == null)
			{
				_image.Initialize(data.Sprite, null, null, MaterialType.UITransparent, data.MaxImageWidth);
				_text.LocalizedString = data.Text;
				_currencyView.gameObject.SetActive(value: false);
			}
			else
			{
				_image.Initialize(data.BuildingProperties, MaterialType.UITransparent, data.MaxImageWidth);
				_text.LocalizedString = data.BuildingProperties.LocalizedName;
				if (data.Price.IsValid)
				{
					_currencyView.gameObject.SetActive(value: true);
					_currencyView.Initialize(data.Price);
				}
				else
				{
					_currencyView.gameObject.SetActive(value: false);
				}
			}
			_particlesObject.SetActive(data.BuildingProperties is LandmarkBuildingProperties);
			_appearTweener.Play();
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.TreasureChestItemDrop, playOneAtATime: false, 1f, 0.3f);
			_vanishTweener.FinishedPlaying += OnVanishTweenerFinishedPlaying;
		}

		private void OnDestroy()
		{
			if (_vanishTweener != null)
			{
				_vanishTweener.FinishedPlaying -= OnVanishTweenerFinishedPlaying;
			}
		}

		public void Vanish()
		{
			_vanishTweener.Play();
			SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.Woosh);
		}

		private void OnVanishTweenerFinishedPlaying(Tweener tweener)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
