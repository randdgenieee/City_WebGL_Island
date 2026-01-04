using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	public class TextStyleView : MonoBehaviour
	{
		[SerializeField]
		private TextStyleType _initialTextStyle = TextStyleType.None;

		[SerializeField]
		private Text _text;

		[SerializeField]
		private Shadow _shadow;

		[SerializeField]
		private Outline _outline;

		private void Start()
		{
			ApplyStyle(_initialTextStyle);
		}

		public void ApplyStyle(TextStyleType style)
		{
			if (style != TextStyleType.None)
			{
				TextStyle asset = SingletonMonobehaviour<TextStyleAssetCollection>.Instance.GetAsset(style);
				ApplyStyle(asset);
			}
		}

		private void ApplyStyle(TextStyle textStyle)
		{
			_text.color = textStyle.TextColor;
			_text.fontSize = textStyle.FontSize;
			if (textStyle.HasShadow)
			{
				AddShadow(textStyle.ShadowColor, textStyle.ShadowSize);
			}
			else
			{
				RemoveShadow();
			}
			if (textStyle.HasOutline)
			{
				AddOutline(textStyle.OutlineColor, textStyle.OutlineSize);
			}
			else
			{
				RemoveOutline();
			}
		}

		private void AddShadow(Color effectColor, Vector2 effectDistance)
		{
			if (_shadow == null)
			{
				_shadow = base.gameObject.AddComponent<Shadow>();
			}
			_shadow.effectColor = effectColor;
			_shadow.effectDistance = effectDistance;
		}

		private void RemoveShadow()
		{
			if (_shadow != null)
			{
				UnityEngine.Object.Destroy(_shadow);
				_shadow = null;
			}
		}

		private void AddOutline(Color effectColor, Vector2 effectDistance)
		{
			if (_outline == null)
			{
				_outline = base.gameObject.AddComponent<Outline>();
			}
			_outline.effectColor = effectColor;
			_outline.effectDistance = effectDistance;
		}

		private void RemoveOutline()
		{
			if (_outline != null)
			{
				UnityEngine.Object.Destroy(_outline);
				_outline = null;
			}
		}
	}
}
