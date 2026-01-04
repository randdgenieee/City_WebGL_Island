using CIG;
using CIG.Translation;
using System;
using UnityEngine;

public static class CIGUtilities
{
	public static string QuantityKey(float quantity, string baseKey)
	{
		if (IsSingular(quantity))
		{
			return baseKey + "$1";
		}
		return baseKey + "$n";
	}

	public static bool IsSingular(float value)
	{
		if (!Mathf.Approximately(value, 0f) && !Mathf.Approximately(value, 1f))
		{
			if (value > 0f)
			{
				return value < 1f;
			}
			return false;
		}
		return true;
	}

	public static ILocalizedString LocalizedString(this Currencies rawCurrencies)
	{
		Currencies currencies = rawCurrencies.Round();
		ILocalizedString localizedString = Localization.EmptyLocalizedString;
		bool flag = false;
		if (currencies.ContainsApproximate("Cash"))
		{
			localizedString = Localization.Concat(localizedString, Localization.Integer(currencies.GetValue("Cash")), Localization.LiteralWhiteSpace, Localization.Key("cash"));
			flag = true;
		}
		if (currencies.ContainsApproximate("Gold"))
		{
			ILocalizedString localizedString2 = flag ? Localization.Literal(", ") : Localization.EmptyLocalizedString;
			localizedString = Localization.Concat(localizedString, localizedString2, Localization.Integer(currencies.GetValue("Gold")), Localization.LiteralWhiteSpace, Localization.Key("gold"));
			flag = true;
		}
		if (currencies.ContainsApproximate("XP"))
		{
			ILocalizedString localizedString3 = flag ? Localization.Literal(", ") : Localization.EmptyLocalizedString;
			localizedString = Localization.Concat(localizedString, localizedString3, Localization.Integer(currencies.GetValue("XP")), Localization.LiteralWhiteSpace, Localization.Key("experience"));
			flag = true;
		}
		if (!flag)
		{
			return Localization.Integer(0);
		}
		return localizedString;
	}

	public static void CopyPathsFrom(this PolygonCollider2D to, PolygonCollider2D from)
	{
		int num = to.pathCount = from.pathCount;
		for (int i = 0; i < num; i++)
		{
			to.SetPath(i, from.GetPath(i));
		}
	}

	public static decimal Round(decimal value, RoundingMethod method, int precision)
	{
		decimal d = 1.0m;
		if (precision > 0)
		{
			d = (decimal)Math.Pow(10.0, precision);
			value *= d;
		}
		switch (method)
		{
		case RoundingMethod.Nearest:
			value = Math.Round(value);
			break;
		case RoundingMethod.Floor:
			value = Math.Floor(value);
			break;
		case RoundingMethod.Ceiling:
			value = Math.Ceiling(value);
			break;
		}
		if (precision > 0)
		{
			value /= d;
		}
		return value;
	}
}
