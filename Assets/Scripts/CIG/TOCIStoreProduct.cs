using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class TOCIStoreProduct : StoreProduct, IComparable<TOCIStoreProduct>, IStorable
	{
		private const string GoldValueKey = "gold_value";

		private const string CashValueKey = "cash_value";

		private const string LevelCountKey = "level_count";

		private const string GameProductNameKey = "game_product_name";

		private const string GameCategoryKey = "game_category";

		private const string PropertiesKey = "properties";

		private const string LandmarkNamePropertyKey = "name";

		private const string CraneCountPropertyKey = "crane_count";

		private static readonly Dictionary<string, StoreProductCategory> GameCategoryMapping = new Dictionary<string, StoreProductCategory>
		{
			{
				"shop",
				StoreProductCategory.Shop
			},
			{
				"shopSale",
				StoreProductCategory.ShopSale
			},
			{
				"starterpack",
				StoreProductCategory.StarterDeal
			},
			{
				"crane",
				StoreProductCategory.Cranes
			},
			{
				"chest",
				StoreProductCategory.Chest
			},
			{
				"chestOTO",
				StoreProductCategory.ChestOTO
			},
			{
				"landmark",
				StoreProductCategory.Landmark
			},
			{
				"craneOffer",
				StoreProductCategory.CraneOffer
			},
			{
				"flyingStartDeal",
				StoreProductCategory.FlyingStartDeal
			}
		};

		public StoreProductCategory Category
		{
			get;
			private set;
		}

		public string GameProductName
		{
			get;
			private set;
		}

		public int LevelCount
		{
			get;
			private set;
		}

		public Dictionary<string, string> Properties
		{
			get;
			private set;
		}

		public Currencies Currencies
		{
			get;
			private set;
		}

		public string LandmarkName
		{
			get
			{
				if (!Properties.TryGetValue("name", out string value))
				{
					return null;
				}
				return value;
			}
		}

		public int CraneCount
		{
			get
			{
				if (!Properties.TryGetValue("crane_count", out string value) || !int.TryParse(value, out int result))
				{
					return 0;
				}
				return result;
			}
		}

		public TOCIStoreProduct(StorageDictionary properties)
			: base(properties)
		{
			Init();
		}

		public override void Update(StorageDictionary props)
		{
			base.Update(props);
			Init();
		}

		private void Init()
		{
			Currencies = new Currencies();
			if (base.CustomProperties.TryGetValue("gold_value", out string value) && long.TryParse(value, out long result))
			{
				Currencies += Currency.GoldCurrency(result);
			}
			if (base.CustomProperties.TryGetValue("cash_value", out value) && long.TryParse(value, out result))
			{
				Currencies += Currency.CashCurrency(result);
			}
			if (base.CustomProperties.TryGetValue("game_category", out value))
			{
				if (GameCategoryMapping.TryGetValue(value, out StoreProductCategory value2))
				{
					Category = value2;
				}
				else
				{
					Category = StoreProductCategory.Unknown;
					UnityEngine.Debug.LogWarningFormat("Missing mapping of GameCategory '{0}' for StoreProduct '{1}'", value, base.Identifier);
				}
			}
			else
			{
				Category = StoreProductCategory.Unknown;
			}
			Properties = new Dictionary<string, string>();
			string text = base.CustomProperties.Get("properties", string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(',');
				int i = 0;
				for (int num = array.Length; i < num; i++)
				{
					string text2 = array[i];
					string[] array2 = text2.Split(':');
					if (array2.Length != 2)
					{
						UnityEngine.Debug.LogErrorFormat("Cannot split IAP property pair '{0}' into key/value.", text2);
					}
					else
					{
						Properties[array2[0]] = array2[1];
					}
				}
			}
			GameProductName = Properties.Get("game_product_name", string.Empty);
			LevelCount = Properties.Get("level_count", 0);
		}

		public int CompareTo(TOCIStoreProduct other)
		{
			int num = CompareTo((StoreProduct)other);
			if (num == 0)
			{
				int num2 = Currencies.ContainsApproximate("Gold").CompareTo(other.Currencies.ContainsApproximate("Gold"));
				if (num2 == 0)
				{
					return Currencies.SumValues.CompareTo(other.Currencies.SumValues);
				}
				return num2;
			}
			return num;
		}
	}
}
