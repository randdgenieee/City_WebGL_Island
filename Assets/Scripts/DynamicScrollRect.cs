using UnityEngine;
using UnityEngine.UI;

public class DynamicScrollRect : ScrollRect
{
	protected override void LateUpdate()
	{
		base.LateUpdate();
		if ((base.horizontal && base.content.rect.width > base.viewport.rect.width) || (base.vertical && base.content.rect.height > base.viewport.rect.height))
		{
			base.movementType = MovementType.Elastic;
			return;
		}
		base.movementType = MovementType.Clamped;
		Vector2 anchoredPosition = base.content.anchoredPosition;
		anchoredPosition.x = 0f;
		base.content.anchoredPosition = anchoredPosition;
	}
}
