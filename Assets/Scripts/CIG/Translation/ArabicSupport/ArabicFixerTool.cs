using System;
using System.Collections.Generic;

namespace CIG.Translation.ArabicSupport
{
	public class ArabicFixerTool
	{
		public static bool showTashkeel = true;

		public static bool useHinduNumbers = false;

		private static bool isParsingColor = false;

		private static string RemoveTashkeel(string str, out List<TashkeelLocation> tashkeelLocation)
		{
			tashkeelLocation = new List<TashkeelLocation>();
			char[] array = str.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == '\u064b')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u064b', i));
				}
				else if (array[i] == '\u064c')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u064c', i));
				}
				else if (array[i] == '\u064d')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u064d', i));
				}
				else if (array[i] == '\u064e')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u064e', i));
				}
				else if (array[i] == '\u064f')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u064f', i));
				}
				else if (array[i] == '\u0650')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u0650', i));
				}
				else if (array[i] == '\u0651')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u0651', i));
				}
				else if (array[i] == '\u0652')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u0652', i));
				}
				else if (array[i] == '\u0653')
				{
					tashkeelLocation.Add(new TashkeelLocation('\u0653', i));
				}
			}
			string[] array2 = str.Split('\u064b', '\u064c', '\u064d', '\u064e', '\u064f', '\u0650', '\u0651', '\u0652', '\u0653');
			str = "";
			string[] array3 = array2;
			foreach (string str2 in array3)
			{
				str += str2;
			}
			return str;
		}

		private static char[] ReturnTashkeel(char[] letters, List<TashkeelLocation> tashkeelLocation)
		{
			char[] array = new char[letters.Length + tashkeelLocation.Count];
			int num = 0;
			for (int i = 0; i < letters.Length; i++)
			{
				array[num] = letters[i];
				num++;
				foreach (TashkeelLocation item in tashkeelLocation)
				{
					if (item.position == num)
					{
						array[num] = item.tashkeel;
						num++;
					}
				}
			}
			return array;
		}

		public static string FixLine(string str)
		{
			string str2 = "";
			List<TashkeelLocation> tashkeelLocation;
			string text = RemoveTashkeel(str, out tashkeelLocation);
			char[] array = text.ToCharArray();
			char[] array2 = text.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (char)ArabicTable.ArabicMapper.Convert(array[i]);
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (j + 5 <= str.Length && str.Substring(j, 5) == "color")
				{
					isParsingColor = true;
				}
				if (array[j] == '>')
				{
					isParsingColor = false;
				}
				bool flag = false;
				if (array[j] == 'ﻝ' && j < array.Length - 1)
				{
					if (array[j + 1] == 'ﺇ')
					{
						array[j] = 'ﻷ';
						array2[j + 1] = '\uffff';
						flag = true;
					}
					else if (array[j + 1] == 'ﺍ')
					{
						array[j] = 'ﻹ';
						array2[j + 1] = '\uffff';
						flag = true;
					}
					else if (array[j + 1] == 'ﺃ')
					{
						array[j] = 'ﻵ';
						array2[j + 1] = '\uffff';
						flag = true;
					}
					else if (array[j + 1] == 'ﺁ')
					{
						array[j] = 'ﻳ';
						array2[j + 1] = '\uffff';
						flag = true;
					}
				}
				if (!IsIgnoredCharacter(array[j]) && array[j] != 'A' && array[j] != 'A')
				{
					if (IsMiddleLetter(array, j))
					{
						array2[j] = (char)(array[j] + 3);
					}
					else if (IsFinishingLetter(array, j))
					{
						array2[j] = (char)(array[j] + 1);
					}
					else if (IsLeadingLetter(array, j))
					{
						array2[j] = (char)(array[j] + 2);
					}
				}
				str2 = str2 + Convert.ToString(array[j], 16) + " ";
				if (flag)
				{
					j++;
				}
				if (useHinduNumbers && !isParsingColor)
				{
					if (array[j] == '0')
					{
						array2[j] = '٠';
					}
					else if (array[j] == '1')
					{
						array2[j] = '١';
					}
					else if (array[j] == '2')
					{
						array2[j] = '٢';
					}
					else if (array[j] == '3')
					{
						array2[j] = '٣';
					}
					else if (array[j] == '4')
					{
						array2[j] = '٤';
					}
					else if (array[j] == '5')
					{
						array2[j] = '٥';
					}
					else if (array[j] == '6')
					{
						array2[j] = '٦';
					}
					else if (array[j] == '7')
					{
						array2[j] = '٧';
					}
					else if (array[j] == '8')
					{
						array2[j] = '٨';
					}
					else if (array[j] == '9')
					{
						array2[j] = '٩';
					}
				}
			}
			if (showTashkeel)
			{
				array2 = ReturnTashkeel(array2, tashkeelLocation);
			}
			List<char> list = new List<char>();
			List<char> list2 = new List<char>();
			for (int num = array2.Length - 1; num >= 0; num--)
			{
				if (char.IsPunctuation(array2[num]) && num > 0 && num < array2.Length - 1 && (char.IsPunctuation(array2[num - 1]) || char.IsPunctuation(array2[num + 1])))
				{
					if (array2[num] == '(')
					{
						list.Add(')');
					}
					else if (array2[num] == ')')
					{
						list.Add('(');
					}
					else if (array2[num] == '<')
					{
						list.Add('>');
					}
					else if (array2[num] == '>')
					{
						list.Add('<');
					}
					else if (array2[num] != '\uffff')
					{
						list.Add(array2[num]);
					}
				}
				else if (array2[num] == ' ' && num > 0 && num < array2.Length - 1 && (char.IsLower(array2[num - 1]) || char.IsUpper(array2[num - 1]) || char.IsNumber(array2[num - 1])) && (char.IsLower(array2[num + 1]) || char.IsUpper(array2[num + 1]) || char.IsNumber(array2[num + 1])))
				{
					list2.Add(array2[num]);
				}
				else if (char.IsNumber(array2[num]) || char.IsLower(array2[num]) || char.IsUpper(array2[num]) || char.IsSymbol(array2[num]) || char.IsPunctuation(array2[num]))
				{
					if (array2[num] == '(')
					{
						list2.Add(')');
					}
					else if (array2[num] == ')')
					{
						list2.Add('(');
					}
					else if (array2[num] == '<')
					{
						list2.Add('>');
					}
					else if (array2[num] == '>')
					{
						list2.Add('<');
					}
					else
					{
						list2.Add(array2[num]);
					}
				}
				else if ((array2[num] >= '\ud800' && array2[num] <= '\udbff') || (array2[num] >= '\udc00' && array2[num] <= '\udfff'))
				{
					list2.Add(array2[num]);
				}
				else
				{
					if (list2.Count > 0)
					{
						for (int k = 0; k < list2.Count; k++)
						{
							list.Add(list2[list2.Count - 1 - k]);
						}
						list2.Clear();
					}
					if (array2[num] != '\uffff')
					{
						list.Add(array2[num]);
					}
				}
			}
			if (list2.Count > 0)
			{
				for (int l = 0; l < list2.Count; l++)
				{
					list.Add(list2[list2.Count - 1 - l]);
				}
				list2.Clear();
			}
			char[] array3 = new char[list.Count];
			for (int m = 0; m < array3.Length; m++)
			{
				array3[m] = list[m];
			}
			str = new string(array3);
			return str;
		}

		private static bool IsIgnoredCharacter(char ch)
		{
			bool num = char.IsPunctuation(ch);
			bool flag = char.IsNumber(ch);
			bool flag2 = char.IsLower(ch);
			bool flag3 = char.IsUpper(ch);
			bool flag4 = char.IsSymbol(ch);
			bool flag5 = ch == 'ﭖ' || ch == 'ﭺ' || ch == 'ﮊ' || ch == 'ﮒ';
			bool flag6 = (ch <= '\ufeff' && ch >= 'ﹰ') | flag5;
			if (!(num | flag) && !(flag2 | flag3) && !flag4 && flag6 && ch != 'a' && ch != '>' && ch != '<')
			{
				return ch == '؛';
			}
			return true;
		}

		private static bool IsLeadingLetter(char[] letters, int index)
		{
			if ((index == 0 || letters[index - 1] == ' ' || letters[index - 1] == '*' || letters[index - 1] == 'A' || char.IsPunctuation(letters[index - 1]) || letters[index - 1] == '>' || letters[index - 1] == '<' || letters[index - 1] == 'ﺍ' || letters[index - 1] == 'ﺩ' || letters[index - 1] == 'ﺫ' || letters[index - 1] == 'ﺭ' || letters[index - 1] == 'ﺯ' || letters[index - 1] == 'ﮊ' || letters[index - 1] == 'ﻯ' || letters[index - 1] == 'ﻭ' || letters[index - 1] == 'ﺁ' || letters[index - 1] == 'ﺃ' || letters[index - 1] == 'ﺇ' || letters[index - 1] == 'ﺅ') && letters[index] != ' ' && letters[index] != 'ﺩ' && letters[index] != 'ﺫ' && letters[index] != 'ﺭ' && letters[index] != 'ﺯ' && letters[index] != 'ﮊ' && letters[index] != 'ﺍ' && letters[index] != 'ﺃ' && letters[index] != 'ﺇ' && letters[index] != 'ﻭ' && letters[index] != 'ﺀ' && index < letters.Length - 1 && letters[index + 1] != ' ' && !char.IsPunctuation(letters[index + 1]))
			{
				return letters[index + 1] != 'ﺀ';
			}
			return false;
		}

		private static bool IsFinishingLetter(char[] letters, int index)
		{
			if (index != 0 && letters[index - 1] != ' ' && letters[index - 1] != '*' && letters[index - 1] != 'A' && letters[index - 1] != 'ﺩ' && letters[index - 1] != 'ﺫ' && letters[index - 1] != 'ﺭ' && letters[index - 1] != 'ﺯ' && letters[index - 1] != 'ﮊ' && letters[index - 1] != 'ﻯ' && letters[index - 1] != 'ﻭ' && letters[index - 1] != 'ﺍ' && letters[index - 1] != 'ﺁ' && letters[index - 1] != 'ﺃ' && letters[index - 1] != 'ﺇ' && letters[index - 1] != 'ﺅ' && letters[index - 1] != 'ﺀ' && !char.IsPunctuation(letters[index - 1]) && letters[index - 1] != '>' && letters[index - 1] != '<' && letters[index] != ' ' && index < letters.Length)
			{
				return letters[index] != 'ﺀ';
			}
			return false;
		}

		private static bool IsMiddleLetter(char[] letters, int index)
		{
			if (index == 0 || letters[index] == ' ' || letters[index] == 'ﺍ' || letters[index] == 'ﺩ' || letters[index] == 'ﺫ' || letters[index] == 'ﺭ' || letters[index] == 'ﺯ' || letters[index] == 'ﮊ' || letters[index] == 'ﻯ' || letters[index] == 'ﻭ' || letters[index] == 'ﺁ' || letters[index] == 'ﺃ' || letters[index] == 'ﺇ' || letters[index] == 'ﺅ' || letters[index] == 'ﺀ' || letters[index - 1] == 'ﺍ' || letters[index - 1] == 'ﺩ' || letters[index - 1] == 'ﺫ' || letters[index - 1] == 'ﺭ' || letters[index - 1] == 'ﺯ' || letters[index - 1] == 'ﮊ' || letters[index - 1] == 'ﻯ' || letters[index - 1] == 'ﻭ' || letters[index - 1] == 'ﺁ' || letters[index - 1] == 'ﺃ' || letters[index - 1] == 'ﺇ' || letters[index - 1] == 'ﺅ' || letters[index - 1] == 'ﺀ' || letters[index - 1] == '>' || letters[index - 1] == '<' || letters[index - 1] == ' ' || letters[index - 1] == '*' || char.IsPunctuation(letters[index - 1]) || index >= letters.Length - 1 || letters[index + 1] == ' ' || letters[index + 1] == '\r' || letters[index + 1] == 'A' || letters[index + 1] == '>' || letters[index + 1] == '>' || letters[index + 1] == 'ﺀ')
			{
				return false;
			}
			try
			{
				return !char.IsPunctuation(letters[index + 1]);
			}
			catch
			{
				return false;
			}
		}
	}
}
