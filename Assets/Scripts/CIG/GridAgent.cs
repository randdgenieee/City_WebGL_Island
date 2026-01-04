namespace CIG
{
	public abstract class GridAgent : MovingSpriteAgent
	{
		protected IsometricGrid _isometricGrid;

		protected GridPoint _gridPosition;

		protected void InitializeGridAgent(IsometricGrid isometricGrid, Timing timing)
		{
			_isometricGrid = isometricGrid;
			InitializeMovingAgent(timing);
		}

		protected override void SetSprites(MovingAgentSprite sprites)
		{
			base.SetSprites(sprites);
			_spriteRenderer.sortingOrder = GetSortingOrder();
		}

		protected override void UpdatePositionAndAngle(double deltaTime)
		{
			UpdateGridPositionAndAngle(deltaTime);
			_position = IsometricGrid.GetPositionForGridPoint(_gridPosition);
		}

		protected abstract void UpdateGridPositionAndAngle(double deltaTime);

		protected virtual int GetSortingOrder()
		{
			return GridTile.GetSortingOrder(_gridPosition, GridSize.One);
		}
	}
}
