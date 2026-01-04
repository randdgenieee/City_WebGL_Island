using UnityEngine;
using UnityEngine.EventSystems;

namespace CIG
{
	public class BuySign : GridTile, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		private Sprite _landSprite;

		[SerializeField]
		private Sprite _waterSprite;

		private ExpansionBlock _expansionBlock;

		public void Initialize(ExpansionBlock expansionBlock)
		{
			_expansionBlock = expansionBlock;
			base.SpriteRenderer.sprite = ((base.Element.Type == SurfaceType.Water) ? _waterSprite : _landSprite);
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			_popupManager.RequestPopup(new BuyExpansionPopupRequest(_expansionBlock));
		}
	}
}
