using CIG.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class WheelOfFortunePartition : MonoBehaviour
	{
		[Serializable]
		private class Colors
		{
			[SerializeField]
			private Color _neutralColor;

			[SerializeField]
			private Color _disabledColor;

			[SerializeField]
			private Color _highlightColor;

			public Color NeutralColor => _neutralColor;

			public Color DisabledColor => _disabledColor;

			public Color HighlightColor => _highlightColor;
		}

		[Serializable]
		private class CurrencyBackgroundColor
		{
			[SerializeField]
			private string _currency;

			[SerializeField]
			private Colors _colors;

			public string Currency => _currency;

			public Colors Colors => _colors;
		}

		private class ItemLook
		{
			public ILocalizedString Text
			{
				get;
			}

			public Sprite Icon
			{
				get;
			}

			public Colors BackgroundColors
			{
				get;
			}

			public ItemLook(ILocalizedString text, Sprite icon, Colors backgroundColors)
			{
				Text = text;
				Icon = icon;
				BackgroundColors = backgroundColors;
			}
		}

		private const float HighlightFlashDuration = 0.25f;

		[SerializeField]
		private Image _backgroundLeft;

		[SerializeField]
		private Image _backgroundRight;

		[SerializeField]
		private LocalizedText _amountText;

		[SerializeField]
		private Image _prizeIcon;

		[SerializeField]
		private List<CurrencyBackgroundColor> _currencyBackgroundColors;

		[SerializeField]
		private Colors _emptyItemBackgroundColors;

		[SerializeField]
		private Colors _textColors;

		[SerializeField]
		private Colors _iconColors;

		[SerializeField]
		private AnimationCurve _highlightColorLerpCurve;

		[SerializeField]
		private Tweener _iconTweener;

		[SerializeField]
		private RectTransform _currencyParticlesPosition;

		[SerializeField]
		private CurrencyAnimationSource _currencyAnimationSource;

		[SerializeField]
		private NumberTweenHelper _numberTweenHelper;

		[SerializeField]
		private Graphic[] _fadeGraphics;

		private Timing _timing;

		private Currency _reward;

		private Colors _backgroundColors;

		private Color _backgroundColor;

		private IEnumerator _highlightRoutine;

		public Vector3 ParticlePosition => _currencyParticlesPosition.position;

		public bool IsHighlighting => _highlightRoutine != null;

		public void Initialize(Timing timing, Currency reward, float angle, float rotation)
		{
			_timing = timing;
			_reward = reward;
			_currencyAnimationSource.Initialize(this);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
			float fillAmount = angle / 180f;
			_backgroundLeft.fillAmount = fillAmount;
			_backgroundRight.fillAmount = fillAmount;
			ItemLook itemLook = GetItemLook(_reward);
			_amountText.LocalizedString = itemLook.Text;
			_prizeIcon.sprite = itemLook.Icon;
			_backgroundColors = itemLook.BackgroundColors;
			SetNeutral();
		}

		public void SetHighlight()
		{
			_amountText.TextField.color = _textColors.HighlightColor;
			_prizeIcon.color = _iconColors.HighlightColor;
			if (_reward.IsValid)
			{
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.RewardPling);
			}
			StartHighlightRoutine(1);
		}

		public void SetNeutral()
		{
			_backgroundColor = _backgroundColors.NeutralColor;
			_backgroundLeft.color = _backgroundColor;
			_backgroundRight.color = _backgroundColor;
			_amountText.TextField.color = _textColors.NeutralColor;
			_prizeIcon.color = _iconColors.NeutralColor;
		}

		public void SetDisabled()
		{
			_backgroundColor = _backgroundColors.DisabledColor;
			_backgroundLeft.color = _backgroundColor;
			_backgroundRight.color = _backgroundColor;
			_amountText.TextField.color = _textColors.DisabledColor;
			_prizeIcon.color = _iconColors.DisabledColor;
		}

		public void StartCollectAnimation()
		{
			if (_reward.IsValid)
			{
				_numberTweenHelper.TweenTo(_reward.Value, decimal.Zero);
			}
			StartHighlightRoutine(3);
		}

		public void Fade(float amount)
		{
			int i = 0;
			for (int num = _fadeGraphics.Length; i < num; i++)
			{
				Color color = _fadeGraphics[i].color;
				color.a = amount;
				_fadeGraphics[i].color = color;
			}
			Color color2 = Color.Lerp(Color.black, _backgroundColor, amount);
			_backgroundLeft.color = color2;
			_backgroundRight.color = color2;
		}

		private void StartHighlightRoutine(int flashes)
		{
			StopHighlightRoutine();
			StartCoroutine(_highlightRoutine = HighlightRoutine(flashes));
		}

		private void StopHighlightRoutine()
		{
			if (_highlightRoutine != null)
			{
				StopCoroutine(_highlightRoutine);
				EndHighlightRoutine();
			}
		}

		private ItemLook GetItemLook(Currency reward)
		{
			if (reward.IsValid)
			{
				CurrencyBackgroundColor currencyBackgroundColor = _currencyBackgroundColors.Find((CurrencyBackgroundColor cbc) => cbc.Currency == reward.Name);
				if (currencyBackgroundColor != null)
				{
					return new ItemLook(Localization.Integer(reward.Value), SingletonMonobehaviour<CurrencySpriteAssetCollection>.Instance.GetCurrencySprite(reward, CurrencySpriteSize.Medium), currencyBackgroundColor.Colors);
				}
				UnityEngine.Debug.LogError("Could not find WheelOfFortunePartition background color for currency: " + reward.Name);
			}
			return new ItemLook(Localization.Key("too_bad"), SingletonMonobehaviour<UISpriteAssetCollection>.Instance.GetAsset(UISpriteType.RedCross), _emptyItemBackgroundColors);
		}

		private IEnumerator HighlightRoutine(int flashes)
		{
			for (int i = 0; i < flashes; i++)
			{
				float time = 0f;
				_iconTweener.StopAndReset();
				_iconTweener.Play();
				while (time < 0.25f)
				{
					time += _timing.GetDeltaTime(DeltaTimeType.Unscaled);
					float t = _highlightColorLerpCurve.Evaluate(time / 0.25f);
					_backgroundColor = Color.Lerp(_backgroundColors.NeutralColor, _backgroundColors.HighlightColor, t);
					_backgroundLeft.color = _backgroundColor;
					_backgroundRight.color = _backgroundColor;
					yield return null;
				}
			}
			EndHighlightRoutine();
		}

		private void EndHighlightRoutine()
		{
			_backgroundColor = _backgroundColors.NeutralColor;
			_backgroundLeft.color = _backgroundColor;
			_backgroundRight.color = _backgroundColor;
			_highlightRoutine = null;
		}
	}
}
