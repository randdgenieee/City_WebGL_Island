namespace CIG
{
	public class BuildingPopupRequest : PopupRequest
	{
		public override bool IsValid => BuildingProperties != null;

		public BuildingProperties BuildingProperties
		{
			get;
			private set;
		}

		public CIGBuilding Building
		{
			get;
			private set;
		}

		public BuildingPopupContent Content
		{
			get;
			private set;
		}

		public BuildingPopupRequest(CIGBuilding building, BuildingPopupContent content)
			: base(typeof(BuildingPopup), enqueue: false)
		{
			Content = content;
			Building = building;
			BuildingProperties = Building.BuildingProperties;
		}

		public BuildingPopupRequest(BuildingProperties buildingProperties, BuildingPopupContent content)
			: base(typeof(BuildingPopup), enqueue: false)
		{
			Content = content;
			BuildingProperties = buildingProperties;
		}
	}
}
