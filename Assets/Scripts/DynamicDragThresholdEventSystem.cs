using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicDragThresholdEventSystem : EventSystem
{
	private const int DefaultPixelDragThreshold = 5;

	private const float ReferenceDPI = 160f;

	protected override void Awake()
	{
		base.Awake();
		base.pixelDragThreshold = Mathf.Max(5, (int)(5f * Screen.dpi / 160f));
	}
}
