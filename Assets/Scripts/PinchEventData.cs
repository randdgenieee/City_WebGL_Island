using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class PinchEventData : BaseEventData
{
	public GameObject Target
	{
		get;
		set;
	}

	public int Pointers
	{
		get;
		set;
	}

	public Vector2 Center
	{
		get;
		set;
	}

	public Vector2 Delta
	{
		get;
		set;
	}

	public Vector2 Velocity
	{
		get;
		set;
	}

	public float Distance
	{
		get;
		set;
	}

	public float ScaleDelta
	{
		get;
		set;
	}

	public PinchEventData(EventSystem eventSystem)
		: base(eventSystem)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<b>target</b>: " + Target);
		stringBuilder.AppendLine("<b>pointers</b>: " + Pointers);
		stringBuilder.AppendLine("<b>center</b>: " + Center);
		stringBuilder.AppendLine("<b>delta</b>: " + Delta);
		stringBuilder.AppendLine("<b>velocity</b>: " + Velocity);
		stringBuilder.AppendLine("<b>distance</b>: " + Distance);
		stringBuilder.AppendLine("<b>scaleDelta</b>: " + ScaleDelta);
		return stringBuilder.ToString();
	}
}
