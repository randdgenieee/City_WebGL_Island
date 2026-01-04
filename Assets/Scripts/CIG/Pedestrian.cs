namespace CIG
{
	public class Pedestrian : RoadAgent
	{
		protected override RoadAgentPath CreatePath(Direction originDirection, Direction targetDirection, GridIndex originIndex)
		{
			return new PedestrianPath(originDirection, targetDirection, originIndex);
		}
	}
}
