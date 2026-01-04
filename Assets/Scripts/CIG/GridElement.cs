using UnityEngine;

namespace CIG
{
	public class GridElement
	{
		public delegate void UnlockedChangedEventHandler(bool unlocked);

		private GridIndex index;

		private Color color;

		private GridTile tile;

		private bool unlocked;

		private IsometricGrid grid;

		private Vector2 origin;

		public GridIndex Index => index;

		public SurfaceType Type
		{
			get;
			private set;
		}

		public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				if (!object.Equals(value, color))
				{
					color = value;
					IsometricGrid isometricGrid = Grid;
					if (isometricGrid != null && isometricGrid.GridOverlay != null)
					{
						isometricGrid.GridOverlay.SetColor(index, color);
					}
				}
			}
		}

		public GridTile Tile
		{
			get
			{
				return tile;
			}
			set
			{
				tile = value;
			}
		}

		public IsometricGrid Grid => grid;

		public Vector2 Origin => origin;

		public bool Unlocked
		{
			get
			{
				return unlocked;
			}
			set
			{
				if (unlocked != value)
				{
					unlocked = value;
					FireUnlockedChangedEvent();
				}
			}
		}

		public event UnlockedChangedEventHandler UnlockedChangedEvent;

		private void FireUnlockedChangedEvent()
		{
			if (this.UnlockedChangedEvent != null)
			{
				this.UnlockedChangedEvent(Unlocked);
			}
		}

		public GridElement(IsometricGrid grid, GridIndex index, SurfaceType type)
		{
			this.grid = grid;
			this.index = index;
			Type = type;
			color = GridOverlay.TransparentColor;
			tile = null;
			unlocked = false;
			origin = new Vector2((float)(index.u - index.v) * IsometricGrid.ElementSize.x * 0.5f, (float)(index.u + index.v) * IsometricGrid.ElementSize.y * -0.5f - IsometricGrid.ElementSize.y);
		}
	}
}
