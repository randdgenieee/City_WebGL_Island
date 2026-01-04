using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class GridColorManager : MonoBehaviour
	{
		private static readonly Color DefaultTypeColor = new Color(1f, 1f, 1f, 1f);

		private static readonly Color BuildableTileColor = new Color(1f, 1f, 1f, 0.4f);

		private static readonly Color AvailableTileOddColor = new Color(1f, 1f, 1f, 0.15f);

		private static readonly Color AvailableTileEvenColor = new Color(1f, 1f, 1f, 0.2f);

		private static readonly Color AvailableExpansionEvenColor = new Color(1f, 1f, 1f, 0.25f);

		private static readonly Color AvailableExpansionOddColor = new Color(1f, 1f, 1f, 0.4f);

		private static readonly Color OccupiedElementColor = new Color(1f, 1f, 1f, 0.5f);

		private static readonly Dictionary<SurfaceType, Color> ElementTypeColors = new Dictionary<SurfaceType, Color>
		{
			{
				SurfaceType.None,
				new Color(1f, 1f, 1f, 0f)
			},
			{
				SurfaceType.Grass,
				new Color(0.53f, 0.54f, 0.12f, 1f)
			},
			{
				SurfaceType.Beach,
				new Color(0.69f, 0.57f, 0.32f, 1f)
			},
			{
				SurfaceType.Water,
				new Color(0f, 0.28f, 0.34f, 1f)
			},
			{
				SurfaceType.Sand,
				new Color(0.71f, 0.59f, 0.24f, 1f)
			},
			{
				SurfaceType.Rock,
				new Color(0.31f, 0.41f, 0.49f, 1f)
			},
			{
				SurfaceType.Mud,
				new Color(0.36f, 0.23f, 0.13f, 1f)
			},
			{
				SurfaceType.Snow,
				new Color(0.42f, 0.64f, 0.79f, 1f)
			}
		};

		private static readonly Dictionary<SurfaceType, Color> BuildableElementTypeColors = new Dictionary<SurfaceType, Color>
		{
			{
				SurfaceType.None,
				new Color(1f, 1f, 1f, 0f)
			},
			{
				SurfaceType.Grass,
				new Color(0.94f, 0.95f, 0.42f, 1f)
			},
			{
				SurfaceType.Beach,
				new Color(1f, 0.94f, 0.84f, 1f)
			},
			{
				SurfaceType.Water,
				new Color(0.21f, 0.8f, 0.99f, 1f)
			},
			{
				SurfaceType.Sand,
				new Color(1f, 0.89f, 0.53f, 1f)
			},
			{
				SurfaceType.Rock,
				new Color(0.67f, 0.75f, 0.84f, 1f)
			},
			{
				SurfaceType.Mud,
				new Color(0.57f, 0.33f, 0.17f, 1f)
			},
			{
				SurfaceType.Snow,
				new Color(0.88f, 0.95f, 1f, 1f)
			}
		};

		private IsometricGrid _isometricGrid;

		private Builder _builder;

		private CIGExpansions _expansions;

		private SurfaceType _buildableTileType;

		public void Initialize(IsometricGrid isometricGrid, Builder builder, CIGExpansions expansions)
		{
			_isometricGrid = isometricGrid;
			_builder = builder;
			_expansions = expansions;
			ResetAllColors();
			_isometricGrid.GridTileAddedEvent += OnGridTileAdded;
			_isometricGrid.GridTileRemovedEvent += OnGridTileRemoved;
			_builder.StartBuildingEvent += OnStartBuilding;
			_builder.StopBuildingEvent += OnStopBuilding;
			Builder.TilesHiddenChangedEvent += OnTilesHiddenChanged;
			_expansions.ExpansionUnlockedEvent += OnExpansionUnlocked;
			_expansions.ExpansionCanUnlockChangedEvent += OnExpansionCanUnlockChanged;
		}

		private void OnDestroy()
		{
			if (_isometricGrid != null)
			{
				_isometricGrid.GridTileAddedEvent -= OnGridTileAdded;
				_isometricGrid.GridTileRemovedEvent -= OnGridTileRemoved;
				_isometricGrid = null;
			}
			if (_builder != null)
			{
				_builder.StartBuildingEvent -= OnStartBuilding;
				_builder.StopBuildingEvent -= OnStopBuilding;
				_builder = null;
			}
			Builder.TilesHiddenChangedEvent -= OnTilesHiddenChanged;
			if (_expansions != null)
			{
				_expansions.ExpansionUnlockedEvent -= OnExpansionUnlocked;
				_expansions.ExpansionCanUnlockChangedEvent -= OnExpansionCanUnlockChanged;
				_expansions = null;
			}
		}

		private void OnExpansionUnlocked(ExpansionBlock block)
		{
			UpdateColorForBlock(block);
		}

		private void OnExpansionCanUnlockChanged(ExpansionBlock block)
		{
			UpdateColorForBlock(block);
		}

		private void UpdateColorForBlock(ExpansionBlock block)
		{
			for (int i = block.Origin.v; i < block.Origin.v + block.Size.v; i++)
			{
				for (int j = block.Origin.u; j < block.Origin.u + block.Size.u; j++)
				{
					GridElement gridElement = _isometricGrid[j, i];
					gridElement.Color = GetColorForElement(gridElement, block);
				}
			}
		}

		private void OnGridTileAdded(GridTile gridTile)
		{
			UpdateGridTile(gridTile.Index, gridTile.Properties.Size);
		}

		private void OnGridTileRemoved(GridTile gridTile)
		{
			UpdateGridTile(gridTile.Index, gridTile.Properties.Size);
		}

		private void OnStopBuilding()
		{
			_buildableTileType = SurfaceType.None;
			ResetAllColors();
		}

		private void OnStartBuilding(SurfaceType surfacetype)
		{
			_buildableTileType = surfacetype;
			ResetAllColors();
		}

		private void OnTilesHiddenChanged(bool hidden)
		{
			ResetAllColors();
		}

		private void UpdateGridTile(GridIndex index, GridSize size)
		{
			for (int num = index.v; num > index.v - size.v; num--)
			{
				for (int num2 = index.u; num2 > index.u - size.u; num2--)
				{
					GridElement gridElement = _isometricGrid[num2, num];
					ExpansionBlock blockForIndex = _expansions.GetBlockForIndex(new GridIndex(num2, num));
					if (blockForIndex != null)
					{
						gridElement.Color = GetColorForElement(gridElement, blockForIndex);
					}
				}
			}
		}

		private void ResetAllColors()
		{
			GridSize size = _isometricGrid.Size;
			for (int i = 0; i < size.v; i++)
			{
				for (int j = 0; j < size.u; j++)
				{
					GridElement gridElement = _isometricGrid[j, i];
					if (gridElement != null)
					{
						ExpansionBlock blockForIndex = _expansions.GetBlockForIndex(new GridIndex(j, i));
						if (blockForIndex != null)
						{
							gridElement.Color = GetColorForElement(gridElement, blockForIndex);
						}
					}
				}
			}
		}

		private Color GetColorForElement(GridElement element, ExpansionBlock block)
		{
			if (Builder.TilesHidden)
			{
				bool num = element.Tile != null;
				bool flag = num && element.Tile is Road;
				if (num && !flag && block.Unlocked)
				{
					return OccupiedElementColor;
				}
			}
			if (_buildableTileType != 0)
			{
				return GetColorForBuildableTile(element, block);
			}
			if (block.Unlocked)
			{
				return GetColorForUnlockedTile(element);
			}
			return GetColorForExpansion(element, block);
		}

		private Color GetColorForUnlockedTile(GridElement element)
		{
			if (element.Tile != null || element.Type <= SurfaceType.None)
			{
				return GridOverlay.TransparentColor;
			}
			if ((element.Index.u + element.Index.v) % 2 != 0)
			{
				return AvailableTileOddColor;
			}
			return AvailableTileEvenColor;
		}

		private Color GetColorForExpansion(GridElement element, ExpansionBlock block)
		{
			if (block.Unlocked || !block.CanUnlock || element.Type <= SurfaceType.None)
			{
				return GridOverlay.TransparentColor;
			}
			Color b = ((block.U + block.V) % 2 == 0) ? AvailableExpansionEvenColor : AvailableExpansionOddColor;
			if (!ElementTypeColors.TryGetValue(element.Type, out Color value))
			{
				value = DefaultTypeColor;
			}
			return value * b;
		}

		private Color GetColorForBuildableTile(GridElement element, ExpansionBlock block)
		{
			bool flag = element.Type == _buildableTileType || (_buildableTileType == SurfaceType.AnyTypeOfLand && element.Type.IsLand());
			if (element.Tile != null || !block.Unlocked || !flag)
			{
				return GridOverlay.TransparentColor;
			}
			if (!BuildableElementTypeColors.TryGetValue(element.Type, out Color value))
			{
				value = DefaultTypeColor;
			}
			return value * BuildableTileColor;
		}
	}
}
