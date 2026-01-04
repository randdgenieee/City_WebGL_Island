using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UPersian.Utils;

namespace UPersian.Components
{
	[AddComponentMenu("UI/RtlText")]
	public class RtlText : Text
	{
		protected char LineEnding = '\n';

		public string BaseText => base.text;

		public override string text
		{
			get
			{
				string text = base.text;
				base.cachedTextGenerator.Populate(text, GetGenerationSettings(base.rectTransform.rect.size));
				List<UILineInfo> list = base.cachedTextGenerator.lines as List<UILineInfo>;
				if (list == null)
				{
					return null;
				}
				string text2 = "";
				for (int i = 0; i < list.Count; i++)
				{
					if (i < list.Count - 1)
					{
						int startCharIdx = list[i].startCharIdx;
						int length = list[i + 1].startCharIdx - list[i].startCharIdx;
						text2 += text.Substring(startCharIdx, length);
						if (text2.Length > 0 && text2[text2.Length - 1] != '\n' && text2[text2.Length - 1] != '\r')
						{
							text2 += LineEnding.ToString();
						}
					}
					else
					{
						text2 += text.Substring(list[i].startCharIdx);
					}
				}
				return text2.RtlFix();
			}
			set
			{
				base.text = value;
			}
		}
	}
}
