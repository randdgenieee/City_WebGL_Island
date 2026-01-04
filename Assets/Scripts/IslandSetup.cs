using CIG;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IslandSetup : ScriptableObject
{
	public delegate void ExpansionAdded(Expansion expansion);

	public delegate void ExpansionRemoved(Expansion expansion);

	[Serializable]
	public class Expansion
	{
		[SerializeField]
		private GridIndex _index = new GridIndex(0, 0);

		[SerializeField]
		private bool _initiallyUnlocked;

		[SerializeField]
		private bool _automaticallyUnlocked;

		[SerializeField]
		private int _sceneryItemCount = 8;

		[SerializeField]
		private bool _hasExpansionChest = true;

		public GridIndex Index => _index;

		public bool InitiallyUnlocked => _initiallyUnlocked;

		public bool AutomaticallyUnlocked => _automaticallyUnlocked;

		public int SceneryItemCount => _sceneryItemCount;

		public bool HasExpansionChest => _hasExpansionChest;
	}

	[SerializeField]
	private IslandId _island;

	[SerializeField]
	private int _baseHappiness;

	[SerializeField]
	[Header("Grid")]
	private GridSize _size = new GridSize(1, 1);

	[SerializeField]
	private SurfaceType[] _tiles = new SurfaceType[1];

	[SerializeField]
	private Vector2 _offset = Vector2.zero;

	[SerializeField]
	[Header("Expansions")]
	private List<Expansion> _expansions = new List<Expansion>
	{
		new Expansion()
	};

	[SerializeField]
	[Header("Background")]
	private int _xBackgroundImages = 1;

	[SerializeField]
	private TextAsset[] _backgroundImageBytes;

	[SerializeField]
	private Bounds _islandBounds;

	[SerializeField]
	[Header("Air ship")]
	private GridIndex _airshipIndex;

	[SerializeField]
	[Header("Fishing")]
	private List<Vector3> _fishingLocations;

	public IslandId IslandId => _island;

	public int BaseHappiness => _baseHappiness;

	public string IslandName => IslandId.ToString();

	public string ReadOnlyIslandName => IslandName + "ReadOnly";

	public GridSize Size => _size;

	public SurfaceType this[int u, int v] => _tiles[u + v * _size.u];

	public SurfaceType this[GridIndex index] => this[index.u, index.v];

	public Vector2 Offset => _offset;

	public List<Expansion> Expansions => _expansions;

	public TextAsset[] BackgroundImageBytes => _backgroundImageBytes;

	public int XBackgroundImages => _xBackgroundImages;

	public Bounds IslandBounds => _islandBounds;

	public GridIndex AirshipIndex => _airshipIndex;

	public List<Vector3> FishingLocations => _fishingLocations;

	public event ExpansionAdded ExpansionAddedEvent;

	public event ExpansionRemoved ExpansionRemovedEvent;

	private void FireExpansionAddedEvent(Expansion expansion)
	{
		if (this.ExpansionAddedEvent != null)
		{
			this.ExpansionAddedEvent(expansion);
		}
	}

	private void FireExpansionRemovedEvent(Expansion expansion)
	{
		if (this.ExpansionRemovedEvent != null)
		{
			this.ExpansionRemovedEvent(expansion);
		}
	}
}
