using UnityEngine;

namespace CIG
{
	public class TreasureChestFlyingReward : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer _renderer;

		public void Initialize(RewardItemData itemData)
		{
			_renderer.sprite = itemData.Sprite;
			_renderer.transform.localScale = _renderer.sprite.pixelsPerUnit * 0.003f * Vector3.one;
			_renderer.transform.rotation = Quaternion.identity;
		}

		private void OnFlyingFinished()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
