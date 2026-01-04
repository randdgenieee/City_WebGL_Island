using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class GridTileSpriteRenderer : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		[SerializeField]
		private AnimatedSprite _animatedSprite;

		[SerializeField]
		private List<ChildSpriteRenderer> _childSpriteRenderers = new List<ChildSpriteRenderer>();

		public SpriteRenderer SpriteRenderer => _spriteRenderer;

		public AnimatedSprite AnimatedSprite => _animatedSprite;

		public List<ChildSpriteRenderer> ChildSpriteRenderers => _childSpriteRenderers;

		protected virtual string SortingLayer => "GridObjects";

		public void Copy(GridTileSpriteRenderer other)
		{
			SpriteRenderer.sprite = other.SpriteRenderer.sprite;
			if (AnimatedSprite != null)
			{
				AnimatedSprite.enabled = (other.AnimatedSprite != null);
				if (other.AnimatedSprite != null)
				{
					AnimatedSprite.AnimationMode = other.AnimatedSprite.AnimationMode;
					AnimatedSprite.FPS = other.AnimatedSprite.FPS;
					AnimatedSprite.WaitAtEndSeconds = other.AnimatedSprite.WaitAtEndSeconds;
					AnimatedSprite.ReplaceSprites(other.AnimatedSprite.Sprites);
				}
			}
			for (int num = ChildSpriteRenderers.Count - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(ChildSpriteRenderers[num].gameObject);
			}
			ChildSpriteRenderers.Clear();
			int i = 0;
			for (int count = other.ChildSpriteRenderers.Count; i < count; i++)
			{
				ChildSpriteRenderer childSpriteRenderer = other.ChildSpriteRenderers[i];
				if (childSpriteRenderer.CanCopy)
				{
					ChildSpriteRenderer childSpriteRenderer2 = UnityEngine.Object.Instantiate(childSpriteRenderer, base.transform);
					childSpriteRenderer2.name = childSpriteRenderer.name;
					childSpriteRenderer2.transform.localPosition = childSpriteRenderer.transform.position - other.transform.position;
					ChildSpriteRenderers.Add(childSpriteRenderer2);
				}
			}
			SetSortingOrder(other.SpriteRenderer.sortingOrder);
		}

		public void SetSortingOrder(int sortingOrder)
		{
			_spriteRenderer.sortingOrder = sortingOrder;
			_spriteRenderer.sortingLayerName = SortingLayer;
			int i = 0;
			for (int count = _childSpriteRenderers.Count; i < count; i++)
			{
				_childSpriteRenderers[i].UpdateSortingOrder(sortingOrder, SortingLayer);
			}
		}

		public void SetColor(Color color)
		{
			_spriteRenderer.color = color;
			int i = 0;
			for (int count = _childSpriteRenderers.Count; i < count; i++)
			{
				_childSpriteRenderers[i].SetColor(color);
			}
		}

		public void SetMaterial(Material material)
		{
			_spriteRenderer.material = material;
			int i = 0;
			for (int count = _childSpriteRenderers.Count; i < count; i++)
			{
				_childSpriteRenderers[i].SetMaterial(material);
			}
		}

		public void SetHidden(bool hidden)
		{
			_spriteRenderer.enabled = !hidden;
			int i = 0;
			for (int count = _childSpriteRenderers.Count; i < count; i++)
			{
				_childSpriteRenderers[i].SetHidden(hidden);
			}
		}

		public void SetPaused(bool paused)
		{
			if (_animatedSprite != null)
			{
				if (paused)
				{
					_animatedSprite.Stop();
				}
				else if (_animatedSprite.Sprites.Length != 0)
				{
					_animatedSprite.Play();
				}
			}
			int i = 0;
			for (int count = _childSpriteRenderers.Count; i < count; i++)
			{
				_childSpriteRenderers[i].SetPaused(paused);
			}
		}
	}
}
