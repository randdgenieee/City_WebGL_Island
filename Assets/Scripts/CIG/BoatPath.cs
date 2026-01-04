using System.Collections.Generic;

namespace CIG
{
	public class BoatPath : RoadAgentPath
	{
		private const float LaneOffset_NE_SE = 0.4f;

		private const float LaneOffset_NW_SW = 0.8f;

		private const float LaneOffset_NE_SE_0_1_4 = 0.1f;

		private const float LaneOffset_NE_SE_0_3_4 = 0.3f;

		private const float LaneOffset_NW_SW_0_1_4 = 0.65f;

		private const float LaneOffset_NW_SW_0_3_4 = 0.75f;

		private const float LaneOffset_NE_SE_1_1_4 = 0.5f;

		private const float LaneOffset_NE_SE_1_3_4 = 0.7f;

		private const float LaneOffset_NW_SW_1_1_4 = 0.85f;

		private const float LaneOffset_NW_SW_1_3_4 = 0.95f;

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
					new Waypoint(new GridPoint(0.8f, 1f), 225f),
					new Waypoint(new GridPoint(0.8f, 0f), 225f)
				}
			},
			{
				Direction.NW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0f, 0.8f), 315f),
					new Waypoint(new GridPoint(1f, 0.8f), 315f)
				}
			}
		};

		private static readonly Dictionary<Direction, List<Waypoint>> UTurnPathWaypoints = new Dictionary<Direction, List<Waypoint>>
		{
			{
				Direction.NE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.8f, 1f), 225f),
					new Waypoint(new GridPoint(0.8f, 0.65f), 195f),
					new Waypoint(new GridPoint(0.75f, 0.55f), 165f),
					new Waypoint(new GridPoint(0.65f, 0.5f), 135f),
					new Waypoint(new GridPoint(0.6f, 0.5f), 105f),
					new Waypoint(new GridPoint(0.5f, 0.55f), 75f),
					new Waypoint(new GridPoint(0.4f, 0.65f), 45f),
					new Waypoint(new GridPoint(0.4f, 1f), 45f)
				}
			},
			{
				Direction.SE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0f, 0.8f), 315f),
					new Waypoint(new GridPoint(0.4f, 0.8f), 285f),
					new Waypoint(new GridPoint(0.6f, 0.75f), 255f),
					new Waypoint(new GridPoint(0.700000048f, 0.65f), 225f),
					new Waypoint(new GridPoint(0.700000048f, 0.6f), 195f),
					new Waypoint(new GridPoint(0.6f, 0.5f), 165f),
					new Waypoint(new GridPoint(0.4f, 0.4f), 135f),
					new Waypoint(new GridPoint(0f, 0.4f), 135f)
				}
			},
			{
				Direction.SW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.4f, 0f), 45f),
					new Waypoint(new GridPoint(0.4f, 0.4f), 15f),
					new Waypoint(new GridPoint(0.5f, 0.6f), 345f),
					new Waypoint(new GridPoint(0.6f, 0.700000048f), 315f),
					new Waypoint(new GridPoint(0.65f, 0.700000048f), 285f),
					new Waypoint(new GridPoint(0.75f, 0.6f), 255f),
					new Waypoint(new GridPoint(0.8f, 0.4f), 225f),
					new Waypoint(new GridPoint(0.8f, 0f), 225f)
				}
			},
			{
				Direction.NW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(1f, 0.4f), 135f),
					new Waypoint(new GridPoint(0.65f, 0.4f), 105f),
					new Waypoint(new GridPoint(0.55f, 0.5f), 75f),
					new Waypoint(new GridPoint(0.5f, 0.6f), 45f),
					new Waypoint(new GridPoint(0.5f, 0.65f), 15f),
					new Waypoint(new GridPoint(0.55f, 0.75f), 345f),
					new Waypoint(new GridPoint(0.65f, 0.8f), 315f),
					new Waypoint(new GridPoint(1f, 0.8f), 315f)
				}
			}
		};

		private static readonly Dictionary<Direction, List<Waypoint>> InnerTurnPathWaypoints = new Dictionary<Direction, List<Waypoint>>
		{
			{
				Direction.NE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0f, 0.8f), 315f),
					new Waypoint(new GridPoint(0.1f, 0.8f), 345f),
					new Waypoint(new GridPoint(0.3f, 0.85f), 15f),
					new Waypoint(new GridPoint(0.4f, 0.95f), 45f),
					new Waypoint(new GridPoint(0.4f, 1f), 45f)
				}
			},
			{
				Direction.SE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.4f, 0f), 45f),
					new Waypoint(new GridPoint(0.4f, 0.1f), 75f),
					new Waypoint(new GridPoint(0.3f, 0.3f), 105f),
					new Waypoint(new GridPoint(0.1f, 0.4f), 135f),
					new Waypoint(new GridPoint(0f, 0.4f), 135f)
				}
			},
			{
				Direction.SW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(1f, 0.4f), 135f),
					new Waypoint(new GridPoint(0.95f, 0.4f), 165f),
					new Waypoint(new GridPoint(0.85f, 0.3f), 195f),
					new Waypoint(new GridPoint(0.8f, 0.1f), 225f),
					new Waypoint(new GridPoint(0.8f, 0f), 225f)
				}
			},
			{
				Direction.NW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.8f, 1f), 225f),
					new Waypoint(new GridPoint(0.8f, 0.95f), 255f),
					new Waypoint(new GridPoint(0.85f, 0.85f), 285f),
					new Waypoint(new GridPoint(0.95f, 0.8f), 315f),
					new Waypoint(new GridPoint(1f, 0.8f), 315f)
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
					new Waypoint(new GridPoint(0.7f, 0.4f), 105f),
					new Waypoint(new GridPoint(0.5f, 0.5f), 75f),
					new Waypoint(new GridPoint(0.4f, 0.7f), 45f),
					new Waypoint(new GridPoint(0.4f, 1f), 45f)
				}
			},
			{
				Direction.SE,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.8f, 1f), 225f),
					new Waypoint(new GridPoint(0.8f, 0.7f), 195f),
					new Waypoint(new GridPoint(0.75f, 0.5f), 165f),
					new Waypoint(new GridPoint(0.65f, 0.4f), 135f),
					new Waypoint(new GridPoint(0f, 0.4f), 135f)
				}
			},
			{
				Direction.SW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0f, 0.8f), 315f),
					new Waypoint(new GridPoint(0.65f, 0.8f), 285f),
					new Waypoint(new GridPoint(0.75f, 0.75f), 255f),
					new Waypoint(new GridPoint(0.8f, 0.65f), 225f),
					new Waypoint(new GridPoint(0.8f, 0f), 225f)
				}
			},
			{
				Direction.NW,
				new List<Waypoint>
				{
					new Waypoint(new GridPoint(0.4f, 0f), 45f),
					new Waypoint(new GridPoint(0.4f, 0.65f), 15f),
					new Waypoint(new GridPoint(0.5f, 0.75f), 345f),
					new Waypoint(new GridPoint(0.7f, 0.8f), 315f),
					new Waypoint(new GridPoint(1f, 0.8f), 315f)
				}
			}
		};

		protected override Dictionary<Direction, List<Waypoint>> StraightPathDirectionWaypoints => StraightPathWaypoints;

		protected override Dictionary<Direction, List<Waypoint>> UTurnPathDirectionWaypoints => UTurnPathWaypoints;

		protected override Dictionary<Direction, List<Waypoint>> InnerTurnPathDirectionWaypoints => InnerTurnPathWaypoints;

		protected override Dictionary<Direction, List<Waypoint>> OuterTurnPathDirectionWaypoints => OuterTurnPathWaypoints;

		public BoatPath(Direction originDirection, Direction targetDirection, GridIndex tile)
			: base(originDirection, targetDirection, tile)
		{
		}
	}
}
