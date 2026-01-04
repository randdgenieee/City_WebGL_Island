using CIG.Translation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public abstract class PlingManager : MonoBehaviour
	{
		public const float PlingOffset = 30f;

		private OverlayManager _overlayManager;

		protected abstract Vector3 PlingPosition
		{
			get;
		}

		public void Initialize(OverlayManager overlayManager)
		{
			_overlayManager = overlayManager;
		}

		public void ShowPling(PlingType plingType, ILocalizedString text, TextStyleType textStyle = TextStyleType.None)
		{
			CreatePling(plingType, text, textStyle, Vector3.zero, null).Show();
		}

		public void ShowCurrencyPlings(Timing timing, Currency currency, Clip? sound = default(Clip?))
		{
			ShowCurrencyPlings(timing, new Currencies(currency), sound);
		}

		public void ShowCurrencyPlings(Timing timing, Currencies currencies, Clip? sound = default(Clip?))
		{
			List<Pling> list = new List<Pling>();
			int i = 0;
			for (int keyCount = currencies.KeyCount; i < keyCount; i++)
			{
				Currency currency = currencies.GetCurrency(i);
				if (currency.Value > decimal.Zero && TryGetPlingData(currency.Name, out PlingType plingType, out ParticleType? particleType))
				{
					Pling item = CreatePling(plingType, Localization.Integer(currency.Value), TextStyleType.None, list.Count * Vector3.down * 30f, particleType);
					list.Add(item);
				}
			}
			if (list.Count > 0 && sound.HasValue)
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(sound.Value);
			}
			StartCoroutine(ShowPlings(timing, list));
		}

		private Pling CreatePling(PlingType plingType, ILocalizedString text, TextStyleType textStyle, Vector3 offset, ParticleType? particleType)
		{
			Vector3 position = PlingPosition + offset;
			Pling asset = SingletonMonobehaviour<PlingAssetCollection>.Instance.GetAsset(plingType);
			Pling pling = _overlayManager.CreatePling(asset, position);
			pling.Initialize(text, textStyle, particleType);
			return pling;
		}

		private bool TryGetPlingData(string currency, out PlingType plingType, out ParticleType? particleType)
		{
			if (!(currency == "Cash"))
			{
				if (!(currency == "Gold"))
				{
					if (!(currency == "XP"))
					{
						if (!(currency == "SilverKey"))
						{
							if (!(currency == "GoldKey"))
							{
								if (currency == "Token")
								{
									plingType = PlingType.Token;
									particleType = null;
									return true;
								}
								plingType = (PlingType)(-1);
								particleType = null;
								return false;
							}
							plingType = PlingType.GoldKey;
							particleType = null;
							return true;
						}
						plingType = PlingType.SilverKey;
						particleType = null;
						return true;
					}
					plingType = PlingType.Xp;
					particleType = null;
					return true;
				}
				plingType = PlingType.Gold;
				particleType = ParticleType.GoldFountain;
				return true;
			}
			plingType = PlingType.Cash;
			particleType = ParticleType.CashFountain;
			return true;
		}

		private IEnumerator ShowPlings(Timing timing, List<Pling> plings)
		{
			int i = 0;
			for (int length = plings.Count; i < length; i++)
			{
				plings[i].Show();
				yield return new WaitForAnimationTimeSeconds(timing, 0.06);
			}
		}
	}
}
