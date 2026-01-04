using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CIG.Translation.Native;
using UnityEngine;

namespace CIG.Translation
{
    public static class Localization
    {
        public static event Localization.CultureEventHandler CurrentCultureChangedEvent;

        private static void FireCultureChangedEvent()
        {
            if (Localization.CurrentCultureChangedEvent != null)
            {
                Localization.CurrentCultureChangedEvent(Localization._currentCulture);
            }
        }

        static Localization()
        {
            Localization.LoadCultures();
            Localization.LoadFallbackTranslation();
        }

        public static Localization.Culture GetSystemDefaultCulture()
        {
            Localization.Culture culture = null;
            string[] defaultLanguages = SystemLocale.DefaultLanguages;
            int num = defaultLanguages.Length;
            for (int i = 0; i < num; i++)
            {
                string text = defaultLanguages[i];
                if (!string.IsNullOrEmpty(text))
                {
                    string text2;
                    string text3;
                    Localization.SplitLocale(text, out text2, out text3);
                    int count = Localization._availableCultures.Count;
                    for (int j = 0; j < count; j++)
                    {
                        Localization.Culture culture2 = Localization._availableCultures[j];
                        int count2 = culture2.Names.Count;
                        for (int k = 0; k < count2; k++)
                        {
                            string value;
                            string value2;
                            Localization.SplitLocale(culture2.Names[k], out value, out value2);
                            if (text2.Equals(value))
                            {
                                if (culture == null)
                                {
                                    culture = culture2;
                                }
                                if (text3 == null || text3.Equals(value2))
                                {
                                    UnityEngine.Debug.LogFormat("SUISS Localization: Found perfect lang-locale match: {0}", new object[]
                                    {
                                        text
                                    });
                                    return culture2;
                                }
                            }
                        }
                    }
                }
            }
            if (culture != null)
            {
                UnityEngine.Debug.LogFormat("SUISS Localization: Found partial lang-locale match: {0} (user's preferred languages: {1})", new object[]
                {
                    culture.Identifier,
                    string.Join(", ", defaultLanguages)
                });
                return culture;
            }
            UnityEngine.Debug.LogFormat("SUISS Localization: No lang-locale match; falling back to {0}. (user's preferred languages: {1})", new object[]
            {
                Localization._availableCultures[0].Identifier,
                string.Join(", ", defaultLanguages)
            });
            return Localization._availableCultures[0];
        }

        private static void LoadCultures()
        {
            List<Localization.Culture> list = new List<Localization.Culture>();
            //try
            //{
            //    TextAsset textAsset = Resources.Load<TextAsset>("Localization/Cultures");
            //    XMLElement xmlelement = new XMLParser(textAsset.text).Parse();
            //    if (xmlelement.value != "Cultures")
            //    {
            //        throw new Exception("SUISS Localization: Unexpected document element: " + xmlelement.value);
            //    }
            //    int count = xmlelement.Children.Count;
            //    for (int i = 0; i < count; i++)
            //    {
            //        IXMLNode ixmlnode = xmlelement.Children[i];
            //        try
            //        {
            //            if (ixmlnode.type == XMLNodeType.Element)
            //            {
            //                if (ixmlnode.value != "Culture")
            //                {
            //                    UnityEngine.Debug.LogWarning("SUISS Localization: Encountered an unexpected element in Cultures: " + ixmlnode.value);
            //                }
            //                else
            //                {
            //                    List<string> list2 = new List<string>();
            //                    string text = null;
            //                    string text2 = null;
            //                    string text3 = null;
            //                    int count2 = ixmlnode.Children.Count;
            //                    for (int j = 0; j < count2; j++)
            //                    {
            //                        IXMLNode ixmlnode2 = ixmlnode.Children[j];
            //                        string value = ixmlnode2.value;
            //                        if (!(value == "Name"))
            //                        {
            //                            if (!(value == "Identifier"))
            //                            {
            //                                if (!(value == "NativeName"))
            //                                {
            //                                    if (value == "TextAsset")
            //                                    {
            //                                        if (text != null)
            //                                        {
            //                                            UnityEngine.Debug.LogWarning("SUISS Localization: Encountered a second TextAssetName node for a Culture (existing TextAssetName '" + text + "')");
            //                                        }
            //                                        text = ixmlnode2.Children[0].value;
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    if (text3 != null)
            //                                    {
            //                                        UnityEngine.Debug.LogWarning("SUISS Localization: Encountered a second NativeName node for a Culture (existing NativeName '" + text2 + "')");
            //                                    }
            //                                    text3 = ixmlnode2.Children[0].value;
            //                                }
            //                            }
            //                            else
            //                            {
            //                                if (text2 != null)
            //                                {
            //                                    UnityEngine.Debug.LogWarning("SUISS Localization: Encountered a second Identifier node for a Culture (existing Identifier '" + text2 + "')");
            //                                }
            //                                text2 = ixmlnode2.Children[0].value;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            list2.Add(ixmlnode2.Children.Find((IXMLNode x) => x.type == XMLNodeType.Text).value);
            //                        }
            //                    }
            //                    if (list2.Count == 0)
            //                    {
            //                        UnityEngine.Debug.LogWarning("SUISS Localization: No Names encountered for a Culture (skipping)");
            //                    }
            //                    else
            //                    {
            //                        if (text == null)
            //                        {
            //                            UnityEngine.Debug.LogWarning("SUISS Localization: No TextAssetName encountered for a Culture");
            //                        }
            //                        if (text2 == null)
            //                        {
            //                            UnityEngine.Debug.LogWarning("SUISS Localization: No Identifier encountered for a Culture");
            //                        }
            //                        if (text3 == null)
            //                        {
            //                            UnityEngine.Debug.LogWarning("SUISS Localization: No NativeName encountered for a Culture");
            //                        }
            //                        Localization.Culture item = new Localization.Culture(text2, text3, list2.AsReadOnly(), new CultureInfo(list2[0]), text);
            //                        list.Add(item);
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception exception)
            //        {
            //            UnityEngine.Debug.LogException(exception);
            //        }
            //    }
            //    Resources.UnloadAsset(textAsset);
            //}
            //catch (Exception exception2)
            //{
            //    UnityEngine.Debug.LogException(exception2);
            //}
            //if (list.Count == 0)
            //{
            //    List<string> list3 = new List<string>
            //    {
            //        "en-US"
            //    };
            //    list.Add(new Localization.Culture("English", "English", list3.AsReadOnly(), new CultureInfo("en-US"), null));
            //}
            //Localization._availableCultures = list.AsReadOnly();



            List<string> list3 = new List<string>
                {
                    "en-US"
                };
            list.Add(new Localization.Culture("English", "English", list3.AsReadOnly(), new CultureInfo("en-US"), "localization/English"));
            List<string> list4 = new List<string>
                {
                    "zh-CN"
                };
            list.Add(new Localization.Culture("Chinese_Simplified", "¼òÌåÖÐÎÄ", list4.AsReadOnly(), new CultureInfo("zh-CN"), "localization/Chinese_Simplified"));
            Localization._availableCultures = list.AsReadOnly();
        }

        private static void LoadFallbackTranslation()
        {
            int count = Localization._availableCultures.Count;
            for (int i = 0; i < count; i++)
            {
                if (Localization._availableCultures[i].Identifier == "English")
                {
                    Localization.ReadTranslations(Localization._availableCultures[i].TextAssetName, Localization._fallbackTranslations);
                    return;
                }
            }
        }

        private static void SplitLocale(string languageLocale, out string languageCode, out string countryCode)
        {
            languageCode = null;
            countryCode = null;
            int num = languageLocale.IndexOf('-');
            if (num == -1)
            {
                num = languageLocale.IndexOf('_');
            }
            if (num != -1)
            {
                languageCode = languageLocale.Substring(0, num);
                countryCode = languageLocale.Substring(num + 1);
                return;
            }
            languageCode = languageLocale;
        }

        public static IList<Localization.Culture> AvailableCultures
        {
            get
            {
                return Localization._availableCultures;
            }
        }

        public static Localization.Culture CurrentCulture
        {
            get
            {
                return Localization._currentCulture;
            }
            set
            {
                if (value != Localization._currentCulture)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException();
                    }
                    if (!Localization._availableCultures.Contains(value))
                    {
                        throw new ArgumentException("SUISS Localization: The specified Culture is not in the list of available cultures");
                    }
                    Localization._currentCulture = value;
                    Localization._isCurrentCultureArabic = (Localization._currentCulture.Info.TwoLetterISOLanguageName == "ar" || Localization._currentCulture.Info.TwoLetterISOLanguageName == "fa");
                    Localization.ReadTranslations(Localization._currentCulture.TextAssetName, Localization._translations);
                    Localization.FireCultureChangedEvent();
                }
            }
        }

        public static bool IsCurrentCultureArabic
        {
            get
            {
                return Localization._isCurrentCultureArabic;
            }
        }

        public static bool ContainsCulture(string identifier)
        {
            int i = 0;
            int count = Localization._availableCultures.Count;
            while (i < count)
            {
                if (Localization._availableCultures[i].Identifier == identifier)
                {
                    return true;
                }
                i++;
            }
            return false;
        }

        public static bool ContainsCulture(Localization.Culture culture)
        {
            return Localization._availableCultures.Contains(culture);
        }

        public static ILocalizedString Literal(string literal)
        {
            return new RawLiteralString(literal);
        }

        public static ILocalizedString Colored(this ILocalizedString original, Color color)
        {
            return StylizedString.Color(original, color);
        }

        public static ILocalizedString FontSize(this ILocalizedString original, int fontSize)
        {
            return StylizedString.FontSize(original, fontSize);
        }

        public static ILocalizedString Italic(this ILocalizedString original)
        {
            return StylizedString.Italic(original);
        }

        public static ILocalizedString Bold(this ILocalizedString original)
        {
            return StylizedString.Bold(original);
        }

        public static ILocalizedString Key(string key)
        {
            return new Localization.RawKeyString(key, long.MinValue);
        }

        public static ILocalizedString ToUpper(ILocalizedString original)
        {
            return new RawCapitalizedString(original);
        }

        public static ILocalizedString ToLower(ILocalizedString original)
        {
            return new RawLowercasedString(original);
        }

        public static ILocalizedString FirstCharToUpper(ILocalizedString original)
        {
            return new RawFirstCharCapitalizedString(original);
        }

        public static ILocalizedString ShortDateTime(DateTime time)
        {
            return new RawDateTimeString(time, "t");
        }

        public static ILocalizedString Date(DateTime time)
        {
            return new RawDateTimeString(time, "dd-MM-yyyy");
        }

        public static ILocalizedString LongDateTime(DateTime time)
        {
            return new RawDateTimeString(time, "dd MMMM yyyy H:mm");
        }

        public static ILocalizedString ShortTimeSpan(TimeSpan timeSpan)
        {
            return new RawTimeSpanString(timeSpan, false, 1);
        }

        public static ILocalizedString TimeSpan(TimeSpan timeSpan, bool hideSecondPartWhenZero)
        {
            return new RawTimeSpanString(timeSpan, hideSecondPartWhenZero, 2);
        }

        public static ILocalizedString FullTimeSpan(TimeSpan timeSpan, bool hidePartWhenZero)
        {
            return new RawTimeSpanString(timeSpan, hidePartWhenZero, 4);
        }

        public static ILocalizedString PluralKey(string key, long count)
        {
            return new Localization.RawKeyString(key, count);
        }

        public static ILocalizedString Integer(int i)
        {
            return new RawIntegerString(i);
        }

        public static ILocalizedString Integer(long l)
        {
            return new RawLongString(l);
        }

        public static ILocalizedString Integer(decimal m)
        {
            return new RawDecimalString(m, 0, true);
        }

        public static ILocalizedString Float(float f, int decimals, bool showTrailingZeroes)
        {
            return new RawFloatString(f, decimals, showTrailingZeroes);
        }

        public static ILocalizedString Float(double d, int decimals, bool showTrailingZeroes)
        {
            return new RawDoubleString(d, decimals, showTrailingZeroes);
        }

        public static ILocalizedString Float(decimal m, int decimals, bool showTrailingZeroes)
        {
            return new RawDecimalString(m, decimals, showTrailingZeroes);
        }

        public static ILocalizedString Percentage(int p)
        {
            return new RawPercentageString((float)p, 0);
        }

        public static ILocalizedString Percentage(float p, int decimals)
        {
            return new RawPercentageString(p * 100f, decimals);
        }

        public static ILocalizedString Format(string literal, params ILocalizedString[] args)
        {
            return new FormatString(new RawLiteralString(literal), args);
        }

        public static ILocalizedString Format(ILocalizedString format, params ILocalizedString[] args)
        {
            return new FormatString(format, args);
        }

        public static ILocalizedString Concat(params ILocalizedString[] args)
        {
            return new LocalizedConcat(args);
        }

        public static ILocalizedString Join(ILocalizedString seperator, params ILocalizedString[] args)
        {
            return new LocalizedJoin(seperator, args);
        }

        public static bool IsNullOrEmpty(ILocalizedString str)
        {
            return str == null || str == Localization.EmptyLocalizedString;
        }

        private static string TranslateInternal(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("SUISS Localization: key");
            }
            string result;
            if (!Localization._translations.TryGetValue(key, out result))
            {
                if (!Localization._fallbackTranslations.TryGetValue(key, out result))
                {
                    UnityEngine.Debug.LogErrorFormat("SUISS Localization: Not even {0} has a translation for {1}", new object[]
                    {
                        "English",
                        key
                    });
                    result = key;
                }
                else
                {
                    UnityEngine.Debug.LogWarningFormat("SUISS Localization: No translation found for '{0}'; falling back to {1}", new object[]
                    {
                        key,
                        "English"
                    });
                }
            }
            return result;
        }

        private static string TranslatePluralInternal(string key, long count)
        {
            if (key == null)
            {
                throw new ArgumentNullException("SUISS Localization: key");
            }
            string result;
            if (!Localization._translations.TryGetValue(key + "$" + count.ToString(CultureInfo.InvariantCulture), out result) && !Localization._translations.TryGetValue(key + "$n", out result) && !Localization._translations.TryGetValue(key, out result))
            {
                if (!Localization._fallbackTranslations.TryGetValue(key + "$" + count.ToString(CultureInfo.InvariantCulture), out result) && !Localization._fallbackTranslations.TryGetValue(key + "$n", out result) && !Localization._fallbackTranslations.TryGetValue(key, out result))
                {
                    UnityEngine.Debug.LogErrorFormat("SUISS Localization: Not even {0} has a plural translation for {1}", new object[]
                    {
                        "English",
                        key
                    });
                    result = key;
                }
                else
                {
                    UnityEngine.Debug.LogWarningFormat("SUISS Localization: No plural translation found for '{0}'; falling back to {1}", new object[]
                    {
                        key,
                        "English"
                    });
                }
            }
            return result;
        }

        private static void ReadTranslations(string textAssetName, Dictionary<string, string> translationDict)
        {
            translationDict.Clear();
            if (string.IsNullOrEmpty(textAssetName))
            {
                return;
            }
            TextAsset textAsset = Resources.Load<TextAsset>(textAssetName);
            if (textAsset == null)
            {
                UnityEngine.Debug.LogWarning("SUISS Localization: Could not find translations at \"" + textAssetName + "\"");
                return;
            }
            try
            {
                using (StringReader stringReader = new StringReader(textAsset.text))
                {
                    int num = 0;
                    string text;
                    while ((text = stringReader.ReadLine()) != null)
                    {
                        num++;
                        if (text.Length != 0)
                        {
                            int num2 = text.IndexOf('=');
                            if (num2 < 0)
                            {
                                UnityEngine.Debug.LogWarning(string.Concat(new object[]
                                {
                                    "SUISS Localization: No equals character found in translation (",
                                    textAssetName,
                                    " at line ",
                                    num,
                                    ")"
                                }));
                            }
                            else
                            {
                                string text2 = text.Substring(0, num2).Trim();
                                string text3 = text.Substring(num2 + 1).Trim();
                                if (translationDict.ContainsKey(text2))
                                {
                                    UnityEngine.Debug.LogWarning(string.Concat(new object[]
                                    {
                                        "SUISS Localization: Duplicate translation found for '",
                                        text2,
                                        "' (",
                                        textAssetName,
                                        " at line ",
                                        num,
                                        ")"
                                    }));
                                }
                                int i = text3.IndexOf('\\');
                                if (i >= 0)
                                {
                                    char[] array = text3.ToCharArray();
                                    int num3 = array.Length;
                                    while (i < num3 - 1)
                                    {
                                        if (array[i] == '\\')
                                        {
                                            char c = '\0';
                                            char c2 = array[i + 1];
                                            if (c2 != '\\')
                                            {
                                                if (c2 != 'n')
                                                {
                                                    if (c2 != 't')
                                                    {
                                                        UnityEngine.Debug.LogWarning(string.Concat(new object[]
                                                        {
                                                            "SUISS Localization: Found unknown escape sequence \\",
                                                            array[i + 1].ToString(),
                                                            " (",
                                                            textAssetName,
                                                            " at line ",
                                                            num,
                                                            ")"
                                                        }));
                                                    }
                                                    else
                                                    {
                                                        c = '\t';
                                                    }
                                                }
                                                else
                                                {
                                                    c = '\n';
                                                }
                                            }
                                            else
                                            {
                                                c = '\\';
                                            }
                                            if (c != '\0')
                                            {
                                                array[i] = c;
                                                for (int j = i + 2; j < num3; j++)
                                                {
                                                    array[j - 1] = array[j];
                                                }
                                                num3--;
                                            }
                                        }
                                        i++;
                                    }
                                    text3 = new string(array, 0, num3);
                                }
                                translationDict[text2] = text3;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogException(exception);
            }
            Resources.UnloadAsset(textAsset);
        }

        public static readonly ILocalizedString EmptyLocalizedString = new RawLiteralString(string.Empty);

        public static readonly ILocalizedString LiteralWhiteSpace = new RawLiteralString(" ");

        public static readonly ILocalizedString LiteralNewLineString = new RawLiteralString("\n");

        public static readonly ILocalizedString LiteralDoubleNewLineString = new RawLiteralString("\n\n");

        public static readonly ILocalizedString LiteralSemiColonSpaceString = new RawLiteralString(": ");

        private const string CulturesFile = "Localization/Cultures";

        private const string CulturesRootNodeName = "Cultures";

        private const string CultureNodeName = "Culture";

        private const string CultureNameNodeName = "Name";

        private const string CultureIdentifierNodeName = "Identifier";

        private const string NativeNameNodeName = "NativeName";

        private const string CultureTextAssetNodeName = "TextAsset";

        private const string FallbackLanguageIdentifier = "English";

        private const string FallbackLanguageCulture = "en-US";

        private const string FallbackLanguageNativeName = "English";

        public const string EnglishTwoLetterISOIdentifier = "en";

        public const string FarsiTwoLetterISOIdentifier = "fa";

        public const string ArabicTwoLetterISOIdentifier = "ar";

        private static IList<Localization.Culture> _availableCultures;

        private static Localization.Culture _currentCulture;

        private static bool _isCurrentCultureArabic;

        private static Dictionary<string, string> _translations = new Dictionary<string, string>();

        private static Dictionary<string, string> _fallbackTranslations = new Dictionary<string, string>();

        public delegate void CultureEventHandler(Localization.Culture culture);

        public sealed class Culture
        {
            public string Identifier { get; }

            public ILocalizedString NativeName { get; }

            public IList<string> Names { get; }

            public CultureInfo Info { get; }

            public string TextAssetName { get; }

            public Culture(string identifier, string nativeName, IList<string> names, CultureInfo info, string textAssetName)
            {
                this.Identifier = identifier;
                this.NativeName = new RawLiteralString(nativeName);
                this.Names = names;
                this.Info = info;
                this.TextAssetName = textAssetName;
            }

            public override string ToString()
            {
                return string.Format("[Culture: Name=\"{0}\", TextAssetName=\"{1}\", Identifier=\"{2}\", NativeName=\"{3}\"]", new object[]
                {
                    this.Info.Name,
                    this.TextAssetName,
                    this.Identifier,
                    this.NativeName
                });
            }
        }

        private class RawKeyString : ILocalizedString
        {
            public RawKeyString(string key, long count)
            {
                if (key == null)
                {
                    throw new ArgumentNullException("SUISS Localization: key");
                }
                this._key = key;
                this._count = count;
            }

            public override string ToString()
            {
                if (this._count == -9223372036854775808L)
                {
                    return "[KeyString=" + this._key + "]";
                }
                return string.Concat(new object[]
                {
                    "[KeyString=",
                    this._key,
                    ",Count=",
                    this._count,
                    "]"
                });
            }

            public string Translate()
            {
                if (this._count == -9223372036854775808L)
                {
                    return Localization.TranslateInternal(this._key);
                }
                return Localization.TranslatePluralInternal(this._key, this._count);
            }

            public const long NoCount = -9223372036854775808L;

            private string _key;

            private long _count;
        }
    }
}
