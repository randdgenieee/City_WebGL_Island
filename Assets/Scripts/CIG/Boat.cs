namespace CIG
{
	public class Boat : RoadAgent
	{
		protected override RoadAgentPath CreatePath(Direction originDirection, Direction targetDirection, GridIndex originIndex)
		{
			return new BoatPath(originDirection, targetDirection, originIndex);
		}

		protected override bool VisibleOnRoad(Road road)
		{
			if (road != null)
			{
				return road.IsNormalRoad;
			}
			return base.VisibleOnRoad(null);
		}
	}
}
