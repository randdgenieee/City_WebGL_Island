namespace CIG
{
	public class Car : RoadAgent
	{
		protected override RoadAgentPath CreatePath(Direction originDirection, Direction targetDirection, GridIndex originIndex)
		{
			return new CarPath(originDirection, targetDirection, originIndex);
		}
	}
}
