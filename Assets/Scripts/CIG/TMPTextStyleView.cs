using TMPro;
using UnityEngine;

namespace CIG
{
	public class TMPTextStyleView : MonoBehaviour
	{
		[SerializeField]
		private TMPTextStyleType _initialTextStyle = TMPTextStyleType.None;

		[SerializeField]
		private TMP_Text _text;

		private void Start()
		{
			ApplyStyle(_initialTextStyle);
		}

		public void ApplyStyle(TMPTextStyleType style)
		{
			if (style != TMPTextStyleType.None)
			{
				TMPTextStyle asset = SingletonMonobehaviour<TMPTextStyleAssetCollection>.Instance.GetAsset(style);
				ApplyStyle(asset);
			}
		}

		private void ApplyStyle(TMPTextStyle textStyle)
		{
			_text.fontSharedMaterial = textStyle.Material;
		}
	}
}
