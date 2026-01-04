using SparkLinq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class ExpansionBlock
	{
		public delegate void UnlockedEventHandler(ExpansionBlock expansionBlock, Currencies price);

		public delegate void CanUnlockChangedEventHandler(ExpansionBlock expansionBlock, bool canUnlock);

		private const string UnlockedKey = "Unlocked";

		private const string PriceKey = "Price";

		private const string BuySignIndexKey = "BuySignIndex";

		private const string ChestIndexKey = "ChestIndex";

		private const string PlacedChestKey = "PlacedChest";

		private readonly Multipliers _multipliers;

		private readonly IsometricGrid _isometricGrid;

		private readonly Builder _builder;

		private readonly ExpansionCostManager _expansionCostManager;

		private readonly Properties _properties;

		private readonly StorageDictionary _storage;

		private readonly BuySign _buySignPrefab;

		private readonly ExpansionChest _expansionChestPrefab;

		private readonly bool _initiallyUnlocked;

		private readonly bool _automaticUnlock;

		private readonly bool _shouldPlaceChest;

		private BuySign _buySign;

		private ExpansionChest _chest;

		private int _availableElements;

		public Currencies Price
		{
			get
			{
				Currencies currencies;
				if (_storage.Contains("Price"))
				{
					currencies = _storage.GetModel("Price", (StorageDictionary sd) => new Currencies(sd), null);
				}
				else
				{
					currencies = CalculatePrice();
					_storage.Set("Price", currencies);
				}
				return currencies;
			}
		}

		public int U
		{
			get;
			private set;
		}

		public int V
		{
			get;
			private set;
		}

		public GridIndex Origin
		{
			get;
			private set;
		}

		public GridSize Size
		{
			get;
			private set;
		}

		public bool Unlocked
		{
			get
			{
				return _storage.Get("Unlocked", defaultValue: false);
			}
			private set
			{
				_storage.Set("Unlocked", value);
			}
		}

		public bool CanUnlock
		{
			get;
			private set;
		}

		public float AvailableFraction => (float)_availableElements / (float)(Size.u * Size.v);

		public BuySign BuySign
		{
			get
			{
				if (_buySign == null && _isometricGrid.IsWithinBounds(BuySignIndex))
				{
					BuySign = (_isometricGrid[BuySignIndex].Tile as BuySign);
				}
				return _buySign;
			}
			set
			{
				_buySign = value;
				if (_buySign == null)
				{
					BuySignIndex = GridIndex.invalid;
					return;
				}
				BuySignIndex = _buySign.Index;
				_buySign.Initialize(this);
			}
		}

		private GridIndex BuySignIndex
		{
			get
			{
				return _storage.GetModel("BuySignIndex", (StorageDictionary sd) => new GridIndex(sd), GridIndex.invalid);
			}
			set
			{
				if (value.Equals(GridIndex.invalid))
				{
					_storage.Remove("BuySignIndex");
				}
				else
				{
					_storage.Set("BuySignIndex", value);
				}
			}
		}

		private ExpansionChest Chest
		{
			get
			{
				if (_chest == null && _isometricGrid.IsWithinBounds(ChestIndex))
				{
					_chest = (_isometricGrid[ChestIndex].Tile as ExpansionChest);
				}
				return _chest;
			}
			set
			{
				_chest = value;
				if (_chest == null)
				{
					ChestIndex = GridIndex.invalid;
					return;
				}
				ChestIndex = _chest.Index;
				_chest.Initialize(this, enter: true);
			}
		}

		private GridIndex ChestIndex
		{
			get
			{
				return _storage.GetModel("ChestIndex", (StorageDictionary sd) => new GridIndex(sd), GridIndex.invalid);
			}
			set
			{
				if (value.Equals(GridIndex.invalid))
				{
					_storage.Remove("ChestIndex");
				}
				else
				{
					_storage.Set("ChestIndex", value);
				}
			}
		}

		private bool PlacedChest
		{
			get
			{
				return _storage.Get("PlacedChest", defaultValue: false);
			}
			set
			{
				_storage.Set("PlacedChest", value);
			}
		}

		public event UnlockedEventHandler UnlockedEvent;

		public event CanUnlockChangedEventHandler CanUnlockChangedEvent;

		private void FireUnlockedEvent(Currencies price)
		{
			if (this.UnlockedEvent != null)
			{
				this.UnlockedEvent(this, price);
			}
		}

		private void FireCanUnlockChangedEvent(bool canUnlock)
		{
			if (this.CanUnlockChangedEvent != null)
			{
				this.CanUnlockChangedEvent(this, canUnlock);
			}
		}

		public ExpansionBlock(StorageDictionary storage, Multipliers multipliers, IsometricGrid isometricGrid, Builder builder, ExpansionCostManager expansionCostManager, Properties properties, IslandSetup.Expansion def, GridIndex origin, GridSize size, BuySign buySignPrefab, ExpansionChest expansionChestPrefab)
		{
			_storage = storage;
			_multipliers = multipliers;
			_isometricGrid = isometricGrid;
			_builder = builder;
			_expansionCostManager = expansionCostManager;
			_properties = properties;
			_buySignPrefab = buySignPrefab;
			_expansionChestPrefab = expansionChestPrefab;
			U = def.Index.u;
			V = def.Index.v;
			Origin = origin;
			Size = size;
			_automaticUnlock = def.AutomaticallyUnlocked;
			_initiallyUnlocked = def.InitiallyUnlocked;
			_shouldPlaceChest = def.HasExpansionChest;
			CalculateElementStats();
			CanUnlock = false;
			if (Unlocked)
			{
				for (int i = Origin.v; i < Origin.v + Size.v; i++)
				{
					for (int j = Origin.u; j < Origin.u + Size.u; j++)
					{
						_isometricGrid[j, i].Unlocked = true;
					}
				}
			}
			if (Chest != null)
			{
				Chest.Initialize(this, enter: false);
			}
		}

		public void UpdateBlock(bool areNeighboursUnlocked)
		{
			bool flag = !Unlocked && (areNeighboursUnlocked || _initiallyUnlocked);
			if (CanUnlock != flag)
			{
				CanUnlock = flag;
				FireCanUnlockChangedEvent(CanUnlock);
				if (_automaticUnlock || _initiallyUnlocked)
				{
					Unlock(new Currencies());
				}
			}
			UpdateBuySign();
			UpdateExpansionChest();
		}

		public bool Unlock(Currencies spent)
		{
			if (Unlocked || !CanUnlock)
			{
				UnityEngine.Debug.LogWarning($"Unlocked || !CanUnlock  ({Unlocked} {CanUnlock})");
				return false;
			}
			Unlocked = true;
			CanUnlock = false;
			for (int i = Origin.v; i < Origin.v + Size.v; i++)
			{
				for (int j = Origin.u; j < Origin.u + Size.u; j++)
				{
					_isometricGrid[j, i].Unlocked = true;
				}
			}
			UpdateBuySign();
			FireUnlockedEvent(spent);
			return true;
		}

		public override string ToString()
		{
			return $"[ExpansionBlock: U={U}, V={V}, Origin={Origin}, Size={Size}, Unlocked={Unlocked}]";
		}

		private void UpdateBuySign()
		{
			if (!_buySignPrefab.CanShowOnReadOnlyGrid && _isometricGrid.ReadOnlyGrid)
			{
				return;
			}
			if (CanUnlock && BuySign == null)
			{
				List<GridIndex> list = SortByDistanceToCentre(GetAvailableIndices());
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					if (!(_buySign == null))
					{
						break;
					}
					GridIndex gridIndex = list[i];
					if (TryPlace(_buySignPrefab, _properties.GetProperties<GridTileProperties>("BuySign"), gridIndex))
					{
						BuySign = (_isometricGrid[gridIndex].Tile as BuySign);
					}
				}
			}
			else if (!CanUnlock && BuySign != null)
			{
				BuySign.DestroyTile();
				BuySign = null;
			}
		}

		private void UpdateExpansionChest()
		{
			if ((!_expansionChestPrefab.CanShowOnReadOnlyGrid && _isometricGrid.ReadOnlyGrid) || !_shouldPlaceChest || (!Unlocked && !CanUnlock) || PlacedChest || !(Chest == null))
			{
				return;
			}
			List<GridIndex> availableIndices = GetAvailableIndices();
			availableIndices.Shuffle();
			int i = 0;
			for (int count = availableIndices.Count; i < count; i++)
			{
				if (!(_chest == null))
				{
					break;
				}
				GridIndex gridIndex = availableIndices[i];
				if (TryPlace(_expansionChestPrefab, _properties.GetProperties<GridTileProperties>("ExpansionChest"), gridIndex))
				{
					Chest = (_isometricGrid[gridIndex].Tile as ExpansionChest);
					PlacedChest = (Chest != null);
				}
			}
		}

		private List<GridIndex> GetAvailableIndices()
		{
			List<GridIndex> list = new List<GridIndex>();
			for (int i = Origin.u; i < Origin.u + Size.u; i++)
			{
				for (int j = Origin.v; j < Origin.v + Size.v; j++)
				{
					if (_isometricGrid.IsWithinBounds(i, j) && _isometricGrid[i, j].Type > SurfaceType.None)
					{
						list.Add(new GridIndex(i, j));
					}
				}
			}
			return list;
		}

		private List<GridIndex> SortByDistanceToCentre(List<GridIndex> indices)
		{
			List<GridIndex> list = new List<GridIndex>(indices);
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			int num3 = int.MinValue;
			int num4 = int.MinValue;
			int i = 0;
			for (int count = indices.Count; i < count; i++)
			{
				GridIndex gridIndex = indices[i];
				num = Mathf.Min(num, gridIndex.u);
				num2 = Mathf.Min(num2, gridIndex.v);
				num3 = Mathf.Max(num3, gridIndex.u);
				num4 = Mathf.Max(num4, gridIndex.v);
			}
			GridIndex centre = new GridIndex(num + (num3 - num) / 2, num2 + (num4 - num2) / 2);
			list.Sort(delegate(GridIndex lhs, GridIndex rhs)
			{
				int num5 = lhs.u - centre.u;
				int num6 = lhs.v - centre.v;
				int num7 = rhs.u - centre.u;
				int num8 = rhs.v - centre.v;
				int num9 = num5 * num5 + num6 * num6;
				int num10 = num7 * num7 + num8 * num8;
				return num9 - num10;
			});
			return list;
		}

		private bool TryPlace(GridTile prefab, GridTileProperties properties, GridIndex atIndex)
		{
			GridTile tile = _isometricGrid[atIndex].Tile;
			if (tile != null)
			{
				if (!(tile is Scenery))
				{
					return false;
				}
				tile.DestroyTile();
			}
			return _builder.BuildAt(prefab, properties, atIndex, mirrored: false, forced: true, serialize: true);
		}

		private void CalculateElementStats()
		{
			_availableElements = 0;
			for (int i = Origin.u; i < Origin.u + Size.u; i++)
			{
				for (int j = Origin.v; j < Origin.v + Size.v; j++)
				{
					if (_isometricGrid[i, j].Type > SurfaceType.None)
					{
						_availableElements++;
					}
				}
			}
		}

		private Currencies CalculatePrice()
		{
			if (_availableElements == 0)
			{
				return new Currencies();
			}
			decimal num = (decimal)AvailableFraction;
			decimal num2 = Math.Max(50000m, _expansionCostManager.GetCashCost() * num) * _multipliers.GetMultiplier(MultiplierType.ExpansionCashCost);
			decimal num3 = 5m * (decimal)Math.Pow(10.0, Math.Floor(Math.Log10((double)num2)) - 2.0);
			decimal value = num3 * Math.Ceiling(num2 / num3);
			decimal d = num * (decimal)Math.Max(10, 25 + 5 * CIGExpansions.GoldCostCounter);
			decimal value2 = 5m * Math.Ceiling(d / 5m);
			return new Currencies(new Currency("Cash", value), new Currency("Gold", value2));
		}
	}
}
