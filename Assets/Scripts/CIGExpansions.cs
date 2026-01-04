using CIG;
using SparkLinq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CIGExpansions : MonoBehaviour
{
	public delegate void ExpansionUnlockedEventHandler(ExpansionBlock block);

	public delegate void ExpansionCanUnlockChangedEventHandler(ExpansionBlock block);

	private const string GoldCostCounterKey = "ExpansionsGoldCostCounter";

	public const int BlockSizeU = 8;

	public const int BlockSizeV = 8;

	private const float DestroyDuration = 1f;

	[SerializeField]
	private BuySign _buySignPrefab;

	[SerializeField]
	private ExpansionChest _chestPrefab;

	private GameStats _gameStats;

	private IsometricGrid _isometricGrid;

	private Builder _builder;

	private ExpansionCostManager _expansionCostManager;

	private Properties _properties;

	private ExpansionBlock[,] _blocks;

	private readonly HashSet<ExpansionBlock> _unlockableBlocks = new HashSet<ExpansionBlock>();

	public List<IslandSetup.Expansion> ExpansionData
	{
		get;
		private set;
	}

	public static int GoldCostCounter
	{
		get
		{
			return StorageController.GameRoot.Get("ExpansionsGoldCostCounter", 0);
		}
		protected set
		{
			StorageController.GameRoot.Set("ExpansionsGoldCostCounter", value);
		}
	}

	public int BlocksU
	{
		get;
		private set;
	}

	public int BlocksV
	{
		get;
		private set;
	}

	public bool HaveExpansionsLeft => _unlockableBlocks.Count > 0;

	public IEnumerable<ExpansionBlock> AllBlocks
	{
		get
		{
			ExpansionBlock[,] blocks = _blocks;
			foreach (ExpansionBlock expansionBlock in blocks)
			{
				if (expansionBlock != null)
				{
					yield return expansionBlock;
				}
			}
		}
	}

	public event ExpansionUnlockedEventHandler ExpansionUnlockedEvent;

	public event ExpansionCanUnlockChangedEventHandler ExpansionCanUnlockChangedEvent;

	private void FireExpansionUnlocked(ExpansionBlock block)
	{
		if (this.ExpansionUnlockedEvent != null)
		{
			this.ExpansionUnlockedEvent(block);
		}
	}

	private void FireExpansionCanUnlockChangedEvent(ExpansionBlock block)
	{
		if (this.ExpansionCanUnlockChangedEvent != null)
		{
			this.ExpansionCanUnlockChangedEvent(block);
		}
	}

	public void Initialize(StorageDictionary storage, Multipliers multipliers, IslandSetup islandSetup, GameStats gameStats, IsometricGrid isometricGrid, Builder builder, ExpansionCostManager expansionCostManager, Properties properties)
	{
		ExpansionData = islandSetup.Expansions;
		_gameStats = gameStats;
		_isometricGrid = isometricGrid;
		_builder = builder;
		_expansionCostManager = expansionCostManager;
		_properties = properties;
		CreateBlocks(storage, multipliers);
	}

	public ExpansionBlock GetBlock(int u, int v)
	{
		if (u < 0 || u >= BlocksU || v < 0 || v >= BlocksV)
		{
			return null;
		}
		return _blocks[u, v];
	}

	public ExpansionBlock GetBlockForIndex(GridIndex index)
	{
		int num = index.u / 8;
		if (num < 0 || num >= BlocksU)
		{
			if (index.u >= _isometricGrid.Size.u)
			{
				UnityEngine.Debug.LogWarning("Out of bounds");
			}
			return null;
		}
		int num2 = index.v / 8;
		if (num2 < 0 || num2 >= BlocksV)
		{
			if (index.v >= _isometricGrid.Size.v)
			{
				UnityEngine.Debug.LogWarning("Out of bounds");
			}
			return null;
		}
		return _blocks[num, num2];
	}

	public static GridIndex GetExpansionIndex(GridIndex tileIndex)
	{
		return new GridIndex(tileIndex.u / 8, tileIndex.v / 8);
	}

	private void CreateBlocks(StorageDictionary storage, Multipliers multipliers)
	{
		int num3 = BlocksU = (BlocksV = 0);
		int i = 0;
		for (int count = ExpansionData.Count; i < count; i++)
		{
			IslandSetup.Expansion expansion = ExpansionData[i];
			BlocksU = Mathf.Max(BlocksU, expansion.Index.u + 1);
			BlocksV = Mathf.Max(BlocksV, expansion.Index.v + 1);
		}
		if (BlocksU * 8 > _isometricGrid.Size.u || BlocksV * 8 > _isometricGrid.Size.v)
		{
			UnityEngine.Debug.LogWarning($"Grid size mismatch: {BlocksU} x {8} > {_isometricGrid.Size.u} or {BlocksV} x {8} > {_isometricGrid.Size.v}");
		}
		_blocks = new ExpansionBlock[BlocksU, BlocksV];
		int j = 0;
		for (int count2 = ExpansionData.Count; j < count2; j++)
		{
			IslandSetup.Expansion expansion2 = ExpansionData[j];
			GridIndex index = expansion2.Index;
			index.u *= 8;
			index.v *= 8;
			ExpansionBlock expansionBlock = new ExpansionBlock(size: new GridSize(8, 8), storage: storage.GetStorageDict(expansion2.Index.ToString()), multipliers: multipliers, isometricGrid: _isometricGrid, builder: _builder, expansionCostManager: _expansionCostManager, properties: _properties, def: expansion2, origin: index, buySignPrefab: _buySignPrefab, expansionChestPrefab: _chestPrefab);
			expansionBlock.CanUnlockChangedEvent += OnCanUnlockChanged;
			expansionBlock.UnlockedEvent += OnUnlocked;
			_blocks[expansion2.Index.u, expansion2.Index.v] = expansionBlock;
		}
		ExpansionBlock[,] blocks = _blocks;
		foreach (ExpansionBlock expansionBlock2 in blocks)
		{
			expansionBlock2?.UpdateBlock(AreNeighboursUnlocked(expansionBlock2));
		}
	}

	private void UpdateNeighbours(ExpansionBlock expansionBlock)
	{
		UpdateNeighbour(expansionBlock.U + 1, expansionBlock.V);
		UpdateNeighbour(expansionBlock.U - 1, expansionBlock.V);
		UpdateNeighbour(expansionBlock.U, expansionBlock.V + 1);
		UpdateNeighbour(expansionBlock.U, expansionBlock.V - 1);
	}

	private void UpdateNeighbour(int u, int v)
	{
		ExpansionBlock block = GetBlock(u, v);
		block?.UpdateBlock(AreNeighboursUnlocked(block));
	}

	private bool AreNeighboursUnlocked(ExpansionBlock expansionBlock)
	{
		if (!IsNeighbourUnlocked(expansionBlock.U + 1, expansionBlock.V) && !IsNeighbourUnlocked(expansionBlock.U - 1, expansionBlock.V) && !IsNeighbourUnlocked(expansionBlock.U, expansionBlock.V + 1))
		{
			return IsNeighbourUnlocked(expansionBlock.U, expansionBlock.V - 1);
		}
		return true;
	}

	private bool IsNeighbourUnlocked(int u, int v)
	{
		return GetBlock(u, v)?.Unlocked ?? false;
	}

	private void OnCanUnlockChanged(ExpansionBlock expansionBlock, bool canUnlock)
	{
		if (canUnlock)
		{
			_unlockableBlocks.Add(expansionBlock);
		}
		else
		{
			_unlockableBlocks.Remove(expansionBlock);
		}
		FireExpansionCanUnlockChangedEvent(expansionBlock);
	}

	private void OnUnlocked(ExpansionBlock expansionBlock, Currencies price)
	{
		if (price.ContainsPositive("Gold"))
		{
			GoldCostCounter = Math.Max(-3, GoldCostCounter + 1);
		}
		else
		{
			GoldCostCounter--;
		}
		_unlockableBlocks.Remove(expansionBlock);
		UpdateNeighbours(expansionBlock);
		RemoveScenery(expansionBlock);
		_gameStats.OnNewBlockUnlocked(_isometricGrid, expansionBlock);
		FireExpansionUnlocked(expansionBlock);
	}

	private void RemoveScenery(ExpansionBlock expansionBlock)
	{
		List<GridTile> list = new List<GridTile>();
		for (int i = expansionBlock.Origin.v; i < expansionBlock.Origin.v + expansionBlock.Size.v; i++)
		{
			for (int j = expansionBlock.Origin.u; j < expansionBlock.Origin.u + expansionBlock.Size.u; j++)
			{
				Scenery scenery = _isometricGrid.GetTileAt(new GridIndex(j, i)) as Scenery;
				if (scenery != null)
				{
					list.Add(scenery);
				}
			}
		}
		StartCoroutine(RemoveSceneryRoutine(list));
	}

	private IEnumerator RemoveSceneryRoutine(List<GridTile> sceneryTiles)
	{
		sceneryTiles.Shuffle();
		float interval = 1f / (float)sceneryTiles.Count;
		SpawnedParticles particlesPrefab = SingletonMonobehaviour<ParticlesAssetCollection>.Instance.GetAsset(ParticleType.SceneryDestroy);
		int i = 0;
		for (int length = sceneryTiles.Count; i < length; i++)
		{
			GridTile gridTile = sceneryTiles[i];
			if (gridTile != null)
			{
				UnityEngine.Object.Instantiate(particlesPrefab, _isometricGrid.GetWorldPositionForGridPoint(gridTile.Middle), Quaternion.identity).Play();
				gridTile.DestroyTile();
				SingletonMonobehaviour<AudioManager>.Instance.PlayClip(Clip.SceneryDestroy);
				yield return new WaitForSeconds(interval);
			}
		}
	}
}
