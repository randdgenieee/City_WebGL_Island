using System.Collections.Generic;

namespace CIG
{
	public class PedestrianPath : RoadAgentPath
	{
		private const float LaneOffset_NE_SE = 0.4f;

		private const float LaneOffset_NW_SW = 0.6f;

		private static readonly Dictionary<Direction, List<Waypoint>> StraightPathWaypoints = new Dictionary<Direction, List<Waypoint>>
		{
			{
				Direction.NE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.4f, 0f), 45f),
					new Waypoint(new GridPoint(0.4f, 1f), 45f)
				}
			},
			{
				Direction.SE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(1f, 0.4f), 135f),
					new Waypoint(new GridPoint(0f, 0.4f), 135f)
				}
			},
			{
				Direction.SW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.6f, 1f), 225f),
					new Waypoint(new GridPoint(0.6f, 0f), 225f)
				}
			},
			{
				Direction.NW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0f, 0.6f), 315f),
					new Waypoint(new GridPoint(1f, 0.6f), 315f)
				}
			}
		};

		private static readonly Dictionary<Direction, List<Waypoint>> UTurnPathWaypoints = new Dictionary<Direction, List<Waypoint>>
		{
			{
				Direction.NE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.6f, 1f), 225f),
					new Waypoint(new GridPoint(0.6f, 0.5f), 135f),
					new Waypoint(new GridPoint(0.4f, 0.5f), 45f),
					new Waypoint(new GridPoint(0.4f, 1f), 45f)
				}
			},
			{
				Direction.SE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0f, 0.6f), 315f),
					new Waypoint(new GridPoint(0.5f, 0.6f), 225f),
					new Waypoint(new GridPoint(0.5f, 0.4f), 135f),
					new Waypoint(new GridPoint(0f, 0.4f), 135f)
				}
			},
			{
				Direction.SW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.4f, 0f), 45f),
					new Waypoint(new GridPoint(0.4f, 0.5f), 315f),
					new Waypoint(new GridPoint(0.6f, 0.5f), 225f),
					new Waypoint(new GridPoint(0.6f, 0f), 225f)
				}
			},
			{
				Direction.NW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(1f, 0.4f), 135f),
					new Waypoint(new GridPoint(0.5f, 0.4f), 45f),
					new Waypoint(new GridPoint(0.5f, 0.6f), 315f),
					new Waypoint(new GridPoint(1f, 0.6f), 315f)
				}
			}
		};

		private static readonly Dictionary<Direction, List<Waypoint>> InnerTurnPathWaypoints = new Dictionary<Direction, List<Waypoint>>
		{
			{
				Direction.NE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0f, 0.6f), 315f),
					new Waypoint(new GridPoint(0.4f, 0.6f), 45f),
					new Waypoint(new GridPoint(0.4f, 1f), 45f)
				}
			},
			{
				Direction.SE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.4f, 0f), 45f),
					new Waypoint(new GridPoint(0.4f, 0.4f), 135f),
					new Waypoint(new GridPoint(0f, 0.4f), 135f)
				}
			},
			{
				Direction.SW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(1f, 0.4f), 135f),
					new Waypoint(new GridPoint(0.6f, 0.4f), 225f),
					new Waypoint(new GridPoint(0.6f, 0f), 225f)
				}
			},
			{
				Direction.NW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.6f, 1f), 225f),
					new Waypoint(new GridPoint(0.6f, 0.6f), 315f),
					new Waypoint(new GridPoint(1f, 0.6f), 315f)
				}
			}
		};

		private static readonly Dictionary<Direction, List<Waypoint>> OuterTurnPathWaypoints = new Dictionary<Direction, List<Waypoint>>
		{
			{
				Direction.NE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(1f, 0.4f), 135f),
					new Waypoint(new GridPoint(0.4f, 0.4f), 45f),
					new Waypoint(new GridPoint(0.4f, 1f), 45f)
				}
			},
			{
				Direction.SE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.6f, 1f), 225f),
					new Waypoint(new GridPoint(0.6f, 0.4f), 135f),
					new Waypoint(new GridPoint(0f, 0.4f), 135f)
				}
			},
			{
				Direction.SW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0f, 0.6f), 315f),
					new Waypoint(new GridPoint(0.6f, 0.6f), 225f),
					new Waypoint(new GridPoint(0.6f, 0f), 225f)
				}
			},
			{
				Direction.NW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.4f, 0f), 45f),
					new Waypoint(new GridPoint(0.4f, 0.6f), 315f),
					new Waypoint(new GridPoint(1f, 0.6f), 315f)
				}
			}
		};

		protected override Dictionary<Direction, List<Waypoint>> StraightPathDirectionWaypoints => StraightPathWaypoints;

		protected override Dictionary<Direction, List<Waypoint>> UTurnPathDirectionWaypoints => UTurnPathWaypoints;

		protected override Dictionary<Direction, List<Waypoint>> InnerTurnPathDirectionWaypoints => InnerTurnPathWaypoints;

		protected override Dictionary<Direction, List<Waypoint>> OuterTurnPathDirectionWaypoints => OuterTurnPathWaypoints;

		public PedestrianPath(Direction originDirection, Direction targetDirection, GridIndex tile)
			: base(originDirection, targetDirection, tile)
		{
		}
	}
}
