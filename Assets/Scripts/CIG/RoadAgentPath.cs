using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public abstract class RoadAgentPath
	{
		protected class Waypoint
		{
			public GridPoint GridPoint
			{
				get;
				private set;
			}

			public float Rotation
			{
				get;
				private set;
			}

			public float DeltaDistance
			{
				get;
				set;
			}

			public float TotalDistance
			{
				get;
				set;
			}

			public Waypoint(GridPoint gridPoint, float rotation)
			{
				GridPoint = gridPoint;
				Rotation = rotation;
				DeltaDistance = 0f;
				TotalDistance = 0f;
			}
		}

		private GridIndex _tileOrigin;

		private Direction _originDirection;

		private Direction _targetDirection;

		private List<Waypoint> _waypoints;

		public float TotalDistance
		{
			get;
			private set;
		}

		protected abstract Dictionary<Direction, List<Waypoint>> StraightPathDirectionWaypoints
		{
			get;
		}

		protected abstract Dictionary<Direction, List<Waypoint>> UTurnPathDirectionWaypoints
		{
			get;
		}

		protected abstract Dictionary<Direction, List<Waypoint>> InnerTurnPathDirectionWaypoints
		{
			get;
		}

		protected abstract Dictionary<Direction, List<Waypoint>> OuterTurnPathDirectionWaypoints
		{
			get;
		}

		protected RoadAgentPath(Direction originDirection, Direction targetDirection, GridIndex tile)
		{
			_tileOrigin = tile;
			_originDirection = originDirection;
			_targetDirection = targetDirection;
			_waypoints = GetWaypoints();
			TotalDistance = 0f;
			int count = _waypoints.Count;
			for (int i = 1; i < count; i++)
			{
				Waypoint waypoint = _waypoints[i];
				Waypoint waypoint2 = _waypoints[i - 1];
				waypoint.DeltaDistance = waypoint2.GridPoint.DistanceTo(waypoint.GridPoint);
				TotalDistance += waypoint.DeltaDistance;
				waypoint.TotalDistance = TotalDistance;
			}
		}

		public float Rotation(float distance)
		{
			int index = CalculateWaypointIndex(distance, _waypoints);
			return _waypoints[index].Rotation;
		}

		public GridPoint Position(float distance)
		{
			return LocalPosition(distance) + _tileOrigin;
		}

		public GridPoint LocalPosition(float distance)
		{
			int num = CalculateWaypointIndex(distance, _waypoints);
			Waypoint waypoint = _waypoints[num];
			Waypoint waypoint2 = _waypoints[Mathf.Min(num + 1, _waypoints.Count - 1)];
			float t = (distance - waypoint.TotalDistance) / waypoint2.DeltaDistance;
			return GridPoint.Lerp(waypoint.GridPoint, waypoint2.GridPoint, t);
		}

		private List<Waypoint> GetWaypoints()
		{
			Dictionary<Direction, List<Waypoint>> dictionary;
			if (_originDirection == Direction.None || _originDirection == _targetDirection)
			{
				dictionary = StraightPathDirectionWaypoints;
			}
			else if (_originDirection.IsOpposite(_targetDirection))
			{
				dictionary = UTurnPathDirectionWaypoints;
			}
			else if (_originDirection.Next() == _targetDirection)
			{
				dictionary = InnerTurnPathDirectionWaypoints;
			}
			else if (_originDirection.Previous() == _targetDirection)
			{
				dictionary = OuterTurnPathDirectionWaypoints;
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat("Unknown path direction '{0}' - Will use straight path.", _targetDirection);
				dictionary = StraightPathDirectionWaypoints;
			}
			return dictionary[_targetDirection];
		}

		private static int CalculateWaypointIndex(float distance, List<Waypoint> waypoints)
		{
			int count = waypoints.Count;
			for (int i = 1; i < count; i++)
			{
				if (waypoints[i].TotalDistance > distance)
				{
					return i - 1;
				}
			}
			return count - 1;
		}
	}
}
