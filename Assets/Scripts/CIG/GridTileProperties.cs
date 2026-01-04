using UnityEngine;

namespace CIG
{
	[BalanceHiddenProperty("blocksX", typeof(int))]
	[BalanceHiddenProperty("blocksY", typeof(int))]
	[BalancePropertyClass("airshipPlatform", false)]
	[BalancePropertyClass("scenery", false)]
	[BalancePropertyClass("road", false)]
	[BalancePropertyClass("expansionSign", false)]
	public class GridTileProperties : BaseProperties
	{
		private const string GridSizeXKey = "blocksX";

		private const string GridSizeYKey = "blocksY";

		private const string CanMirrorKey = "mirrorable";

		private const string SurfaceTypeKey = "surface";

		public GridSize Size
		{
			get;
			private set;
		}

		[BalanceProperty("mirrorable", RequiredKey = false)]
		public bool CanMirror
		{
			get;
			private set;
		}

		[BalanceProperty("surface", ParseType = typeof(string))]
		public SurfaceType SurfaceType
		{
			get;
			private set;
		}

		public GridTileProperties(PropertiesDictionary propsDict, string baseKey)
			: base(propsDict, baseKey)
		{
			Size = new GridSize(GetProperty("blocksX", 0), GetProperty("blocksY", 0));
			CanMirror = GetProperty("mirrorable", defaultValue: true, optional: true);
			string property = GetProperty("surface", "unknown");
			SurfaceType parsedEnum;
			if (property == "all")
			{
				SurfaceType = SurfaceType.None;
			}
			else if (EnumExtensions.TryParseEnum(property, out parsedEnum))
			{
				SurfaceType = parsedEnum;
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat("Cannot parse '{2}.{0}' to {1}", property, typeof(SurfaceType).Name, baseKey);
			}
		}
	}
}
