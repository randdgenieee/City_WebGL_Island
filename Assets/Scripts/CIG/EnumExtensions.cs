using CIG.Translation;
using System;
using UnityEngine;

namespace CIG
{
	public static class EnumExtensions
	{
		public static bool IsLand(this SurfaceType type)
		{
			if (type > SurfaceType.None)
			{
				return type != SurfaceType.Water;
			}
			return false;
		}

		public static bool TryParseEnum<T>(string enumString, out T parsedEnum)
		{
			try
			{
				parsedEnum = (T)Enum.Parse(typeof(T), enumString, ignoreCase: true);
				return true;
			}
			catch (Exception)
			{
				parsedEnum = default(T);
				return false;
			}
		}

		public static bool Contains(this RoadType types, RoadType type)
		{
			return (types & type) != RoadType.None;
		}

		public static bool Contains(this Direction dirs, Direction dir)
		{
			return (dirs & dir) != Direction.None;
		}

		public static bool Contains(this SaleType types, SaleType type)
		{
			return (types & type) != SaleType.None;
		}

		public static Direction Opposite(this Direction dir1)
		{
			switch (dir1)
			{
			case Direction.None:
				return Direction.None;
			case Direction.NE:
				return Direction.SW;
			case Direction.SE:
				return Direction.NW;
			case Direction.SW:
				return Direction.NE;
			case Direction.NW:
				return Direction.SE;
			default:
				UnityEngine.Debug.LogWarningFormat("Missing Opposite case for '{0}'", dir1);
				return Direction.None;
			}
		}

		public static bool IsOpposite(this Direction dir1, Direction dir2)
		{
			return dir1.Opposite() == dir2;
		}

		public static Direction Next(this Direction dir)
		{
			switch (dir)
			{
			case Direction.NE:
				return Direction.SE;
			case Direction.SE:
				return Direction.SW;
			case Direction.SW:
				return Direction.NW;
			case Direction.NW:
				return Direction.NE;
			default:
				UnityEngine.Debug.LogWarningFormat("Missing Next case for '{0}'", dir);
				return Direction.None;
			}
		}

		public static Direction Previous(this Direction dir)
		{
			switch (dir)
			{
			case Direction.NE:
				return Direction.NW;
			case Direction.SE:
				return Direction.NE;
			case Direction.SW:
				return Direction.SE;
			case Direction.NW:
				return Direction.SW;
			default:
				UnityEngine.Debug.LogWarningFormat("Missing Previous case for '{0}'", dir);
				return Direction.None;
			}
		}

		public static ILocalizedString ToLocalizedString(this SurfaceType surfaceType)
		{
			string arg = surfaceType.ToString().ToLower();
			return Localization.Key($"surfacetype_{arg}");
		}
	}
}
