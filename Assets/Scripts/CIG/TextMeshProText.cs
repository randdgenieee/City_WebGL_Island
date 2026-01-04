using TMPro;
using UnityEngine;

namespace CIG
{
	public class TextMeshProText : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text[] _texts;

		public string Text
		{
			get
			{
				if (_texts.Length != 0)
				{
					return _texts[0].text;
				}
				return string.Empty;
			}
			set
			{
				int i = 0;
				for (int num = _texts.Length; i < num; i++)
				{
					_texts[i].text = value;
				}
			}
		}
	}
}
