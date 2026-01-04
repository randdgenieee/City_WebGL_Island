using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public abstract class RoadAgent : GridAgent
	{
		[SerializeField]
		private GridTileIconManager _gridTileIconManager;

		[SerializeField]
		private float _minSpeed = 1f;

		[SerializeField]
		private float _maxSpeed = 1f;

		private OverlayManager _overlayManager;

		private Direction _originDirection;

		private Direction _targetDirection;

		private GridIndex _originIndex;

		private RoadAgentPath _path;

		private float _amountTraveledInTile;

		public WalkerBalloon WalkerBalloon
		{
			get;
			private set;
		}

		public RoadType RoadType
		{
			get;
			protected set;
		}

		public void Initialize(IsometricGrid isometricGrid, Timing timing, OverlayManager overlayManager, RoadType roadType)
		{
			RoadType = roadType;
			_overlayManager = overlayManager;
			base.Speed = Random.Range(_minSpeed, _maxSpeed);
			_gridTileIconManager.Initialize(_overlayManager);
			InitializeGridAgent(isometricGrid, timing);
		}

		public void ShowBalloon(WalkerBalloon walkerBalloon)
		{
			WalkerBalloon = walkerBalloon;
			_gridTileIconManager.SetIcon<WalkerBalloonView>(GridTileIconType.WalkerBalloon).Initialize(WalkerBalloon, _timing, _overlayManager, OnBalloonExpired);
		}

		protected Direction CalculateTargetDirection()
		{
			List<Direction> list = FindPossibleDirection(_originIndex);
			if (list.Count == 0)
			{
				return Direction.None;
			}
			return list[Random.Range(0, list.Count)];
		}

		protected abstract RoadAgentPath CreatePath(Direction originDirection, Direction targetDirection, GridIndex originIndex);

		protected override void SetInitialPosition()
		{
			if (CalculateInitialPosition())
			{
				_path = CreatePath(_originDirection, _targetDirection, _originIndex);
				_amountTraveledInTile = 0.5f;
				_gridPosition = _path.Position(_amountTraveledInTile);
				_angle = _path.Rotation(_amountTraveledInTile);
			}
			else
			{
				_gridPosition = GridPoint.One;
				_angle = 0f;
				Remove();
			}
		}

		protected override void UpdateGridPositionAndAngle(double deltaTime)
		{
			float num = Mathf.Min(300f, (float)deltaTime);
			float num2 = base.Speed * num + _amountTraveledInTile;
			do
			{
				if ((double)num2 > 0.0)
				{
					if (num2 < _path.TotalDistance)
					{
						_amountTraveledInTile = num2;
						_gridPosition = _path.Position(num2);
						_angle = _path.Rotation(num2);
						return;
					}
					num2 -= _path.TotalDistance;
					continue;
				}
				return;
			}
			while (FindNextDirection());
			Remove();
		}

		protected virtual bool VisibleOnRoad(Road road)
		{
			return true;
		}

		protected override int GetSortingOrder()
		{
			int num = (!_targetDirection.Contains(Direction.NE) && !_targetDirection.Contains(Direction.SE)) ? 1 : 0;
			return base.GetSortingOrder() + num;
		}

		private bool CalculateInitialPosition()
		{
			GridTile gridTile = _isometricGrid.FindRandomGridTile((GridTile g) => CanMoveHere(g) && FindPossibleDirection(g.Index).Count > 0);
			if (gridTile == null)
			{
				return false;
			}
			GridIndex index = gridTile.Index;
			List<Direction> list = FindPossibleDirection(index);
			_targetDirection = list[Random.Range(0, list.Count)];
			_originIndex = index;
			_originDirection = _targetDirection.Opposite();
			return true;
		}

		private bool CanMoveHere(GridTile gridObject)
		{
			Road road = gridObject as Road;
			if (road != null)
			{
				return road.RoadType.Contains(RoadType);
			}
			return false;
		}

		private bool FindNextDirection()
		{
			_originIndex = _originIndex.GetNeighbour(_targetDirection);
			_originDirection = _targetDirection;
			Direction direction = CalculateTargetDirection();
			if (direction == Direction.None)
			{
				return false;
			}
			_targetDirection = direction;
			_path = CreatePath(_originDirection, _targetDirection, _originIndex);
			_amountTraveledInTile = 0f;
			_spriteRenderer.enabled = VisibleOnRoad(_isometricGrid[_originIndex].Tile as Road);
			return true;
		}

		private List<Direction> FindPossibleDirection(GridIndex gridIndex)
		{
			List<Direction> list = new List<Direction>();
			GridTile tileAt = _isometricGrid.GetTileAt(gridIndex);
			if (tileAt == null || !CanMoveHere(tileAt))
			{
				return list;
			}
			Direction direction = Direction.NE;
			do
			{
				if (CanMoveHere(_isometricGrid.GetTileAt(gridIndex.GetNeighbour(direction))))
				{
					list.Add(direction);
				}
				direction = direction.Next();
			}
			while (direction != Direction.NE);
			if (list.Count > 1)
			{
				list.RemoveAll((Direction d) => d.IsOpposite(_targetDirection));
			}
			return list;
		}

		private void OnBalloonExpired()
		{
			_gridTileIconManager.RemoveIcon(GridTileIconType.WalkerBalloon);
			WalkerBalloon = null;
		}
	}
}
