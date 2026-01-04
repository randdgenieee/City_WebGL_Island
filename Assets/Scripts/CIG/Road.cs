using System.Collections.Generic;
using UnityEngine;

namespace CIG
{
	public class Road : GridTile
	{
		[SerializeField]
		private RoadType _roadType;

		[SerializeField]
		private RoadType _interactionMask;

		[SerializeField]
		private Sprite[] _sprites;

		[Header("Optional")]
		[SerializeField]
		private SpriteRenderer _foregroundRenderer;

		[SerializeField]
		private Sprite[] _foregroundSprites;

		private int _spriteIndex;

		private readonly Dictionary<Direction, Road> _neighbours = new Dictionary<Direction, Road>();

		public bool IsNormalRoad
		{
			get
			{
				if (_roadType != RoadType.Road && _roadType != RoadType.Path)
				{
					return _roadType == RoadType.River;
				}
				return true;
			}
		}

		public RoadType RoadType => _roadType;

		public void UpdateNeighbours(Dictionary<Direction, GridTile> neighbours)
		{
			foreach (KeyValuePair<Direction, GridTile> neighbour in neighbours)
			{
				_neighbours[neighbour.Key] = (neighbour.Value as Road);
			}
			UpdateSprite();
		}

		public bool HasNeighbour(RoadType ofType = RoadType.Any)
		{
			foreach (KeyValuePair<Direction, Road> neighbour in _neighbours)
			{
				Road value = neighbour.Value;
				if (value != null && value._roadType.Contains(ofType) && value.Status == GridTileStatus.Created)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateSprite()
		{
			_spriteIndex = 0;
			foreach (KeyValuePair<Direction, Road> neighbour in _neighbours)
			{
				if (InteractsWithRoad(neighbour.Value))
				{
					_spriteIndex |= (int)neighbour.Key;
				}
			}
			Sprite sprite = null;
			if (_sprites != null && _spriteIndex < _sprites.Length)
			{
				sprite = _sprites[_spriteIndex];
				if (_foregroundSprites != null && _spriteIndex < _foregroundSprites.Length && _foregroundSprites[_spriteIndex] != null)
				{
					_foregroundRenderer.sprite = _foregroundSprites[_spriteIndex];
				}
			}
			base.SpriteRenderer.sprite = sprite;
		}

		private bool InteractsWithRoad(Road road)
		{
			if (road != null)
			{
				return _interactionMask.Contains(road._roadType);
			}
			return false;
		}
	}
}
