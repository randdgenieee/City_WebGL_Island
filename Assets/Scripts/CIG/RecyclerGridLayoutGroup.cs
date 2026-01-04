using SparkLinq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	[AddComponentMenu("Layout/Recycler Grid Layout Group", 153)]
	public class RecyclerGridLayoutGroup : LayoutGroup
	{
		public enum RecyclerConstraint
		{
			FixedColumnCount,
			FixedRowCount
		}

		public enum Corner
		{
			UpperLeft
		}

		[SerializeField]
		protected Corner _startCorner;

		[SerializeField]
		protected GridLayoutGroup.Axis _startAxis;

		[SerializeField]
		protected Vector2 _cellSize = new Vector2(100f, 100f);

		[SerializeField]
		protected Vector2 _spacing = Vector2.zero;

		[SerializeField]
		protected RecyclerConstraint _constraint;

		[SerializeField]
		protected int _constraintCount = 2;

		[SerializeField]
		protected int _recyclerChildCount = 2;

		[SerializeField]
		private ObjectPool _objectPool;

		private DrivenRectTransformTracker _tracker;

		private int _maxChildren;

		private Func<GameObject, int, bool> _initCallback;

		private readonly List<GameObject> _instances = new List<GameObject>();

		private Dictionary<int, int> _indexMapping = new Dictionary<int, int>();

		private float _lastPosition;

		private int _startIndex;

		public Corner StartCorner
		{
			get
			{
				return _startCorner;
			}
			set
			{
				SetProperty(ref _startCorner, value);
			}
		}

		public GridLayoutGroup.Axis StartAxis => _startAxis;

		public Vector2 CellSize
		{
			get
			{
				return _cellSize;
			}
			set
			{
				SetProperty(ref _cellSize, value);
			}
		}

		public Vector2 Spacing
		{
			get
			{
				return _spacing;
			}
			set
			{
				SetProperty(ref _spacing, value);
			}
		}

		public RecyclerConstraint Constraint => _constraint;

		public int ConstraintCount
		{
			get
			{
				return _constraintCount;
			}
			set
			{
				SetProperty(ref _constraintCount, Mathf.Max(1, value));
			}
		}

		public int RecyclerChildCount
		{
			get
			{
				return _recyclerChildCount;
			}
			set
			{
				SetProperty(ref _recyclerChildCount, Mathf.Max(1, value));
			}
		}

		public int MaxChildren
		{
			get
			{
				return _maxChildren;
			}
			set
			{
				SetProperty(ref _maxChildren, Mathf.Max(1, value));
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_tracker.Add(this, base.rectTransform, DrivenTransformProperties.SizeDelta);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			_tracker.Clear();
		}

		private void Update()
		{
			if (MaxChildren == RecyclerChildCount)
			{
				return;
			}
			float num;
			if (_constraint == RecyclerConstraint.FixedRowCount)
			{
				num = base.rectTransform.anchoredPosition.x * -1f;
			}
			else
			{
				if (_constraint != 0)
				{
					UnityEngine.Debug.LogWarningFormat("Unknown constraint '{0}'", _constraint);
					return;
				}
				num = base.rectTransform.anchoredPosition.y;
			}
			if (Mathf.Approximately(num, _lastPosition))
			{
				return;
			}
			_lastPosition = num;
			float num2;
			if (_constraint == RecyclerConstraint.FixedRowCount)
			{
				num2 = CellSize.x + Spacing.x;
			}
			else
			{
				if (_constraint != 0)
				{
					UnityEngine.Debug.LogWarningFormat("Unknown constraint '{0}'", _constraint);
					return;
				}
				num2 = CellSize.y + Spacing.y;
			}
			int num3 = Mathf.Clamp(Mathf.FloorToInt((_lastPosition - num2) / num2 * (float)_constraintCount), 0, Mathf.Max(0, MaxChildren - RecyclerChildCount));
			if (_startIndex != num3)
			{
				UpdateInstanceOrder(num3 - _startIndex);
				_startIndex = num3;
				UpdateInstances();
			}
		}

		private void SetPosition(float position)
		{
			if (_constraint == RecyclerConstraint.FixedRowCount)
			{
				Vector2 anchoredPosition = base.rectTransform.anchoredPosition;
				anchoredPosition.x = position * -1f;
				base.rectTransform.anchoredPosition = anchoredPosition;
			}
			else if (_constraint == RecyclerConstraint.FixedColumnCount)
			{
				Vector2 anchoredPosition2 = base.rectTransform.anchoredPosition;
				anchoredPosition2.y = position;
				base.rectTransform.anchoredPosition = anchoredPosition2;
			}
			else
			{
				UnityEngine.Debug.LogWarningFormat("Unknown constraint '{0}'", _constraint);
			}
		}

		public void Init(int maxChildren, ObjectPool objectPool, Func<GameObject, int, bool> initCallback)
		{
			_objectPool = objectPool;
			Init(maxChildren, initCallback);
		}

		public void Init(int maxChildren, Func<GameObject, int, bool> initCallback)
		{
			MaxChildren = maxChildren;
			_initCallback = initCallback;
			_lastPosition = 0f;
			_startIndex = 0;
			if (_objectPool.AvailableInstances < _recyclerChildCount)
			{
				UnityEngine.Debug.LogWarningFormat("Insufficient elements available. Expected {0} elements, found {1}", _recyclerChildCount, _objectPool.AvailableInstances);
			}
			PopInstances();
			UpdateInstances();
		}

		public void Init(int maxChildren, List<GameObject> instances, Func<GameObject, int, bool> initCallback)
		{
			MaxChildren = maxChildren;
			_initCallback = initCallback;
			_lastPosition = 0f;
			_startIndex = 0;
			if (instances.Count < _recyclerChildCount)
			{
				UnityEngine.Debug.LogErrorFormat("Insufficient elements available. Expected {0} elements, found {1}", _recyclerChildCount, instances.Count);
			}
			int count = instances.Count;
			for (int i = 0; i < count; i++)
			{
				_instances.Add(instances[i]);
				_indexMapping[i] = -1;
			}
			UpdateInstances();
		}

		public void PushInstances()
		{
			if (_objectPool != null)
			{
				_objectPool.Push(_instances);
			}
			_instances.Clear();
			_indexMapping.Clear();
		}

		public void ScrollToElement(int index, float offset = 0f)
		{
			index = Mathf.Clamp(index, 0, MaxChildren - 1);
			float num = (_constraint == RecyclerConstraint.FixedColumnCount) ? (_cellSize.y + _spacing.y) : (_cellSize.x + _spacing.x);
			SetPosition((float)index * num - offset);
		}

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			int num = 0;
			int num2 = 0;
			if (_constraint == RecyclerConstraint.FixedColumnCount)
			{
				num = (num2 = _constraintCount);
			}
			else if (_constraint == RecyclerConstraint.FixedRowCount)
			{
				num = (num2 = Mathf.CeilToInt((float)_maxChildren / (float)_constraintCount - 0.001f));
			}
			else
			{
				num = 1;
				num2 = Mathf.CeilToInt(Mathf.Sqrt(_maxChildren));
			}
			SetLayoutInputForAxis(Mathf.Max(0f, (float)base.padding.horizontal + (CellSize.x + Spacing.x) * (float)num - Spacing.x), Mathf.Max(0f, (float)base.padding.horizontal + (CellSize.x + Spacing.x) * (float)num2 - Spacing.x), -1f, 0);
		}

		public override void CalculateLayoutInputVertical()
		{
			int num = 0;
			if (_constraint == RecyclerConstraint.FixedColumnCount)
			{
				num = Mathf.CeilToInt((float)_maxChildren / (float)_constraintCount - 0.001f);
			}
			else if (_constraint == RecyclerConstraint.FixedRowCount)
			{
				num = _constraintCount;
			}
			else
			{
				float x = base.rectTransform.rect.size.x;
				int num2 = Mathf.Max(1, Mathf.FloorToInt((x - (float)base.padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x)));
				num = Mathf.CeilToInt((float)_maxChildren / (float)num2);
			}
			float num3 = Mathf.Max(0f, (float)base.padding.vertical + (CellSize.y + Spacing.y) * (float)num - Spacing.y);
			SetLayoutInputForAxis(num3, num3, -1f, 1);
		}

		public override void SetLayoutHorizontal()
		{
			SetCellsAlongAxis(0);
		}

		public override void SetLayoutVertical()
		{
			SetCellsAlongAxis(1);
		}

		public void ForceLayoutRefresh()
		{
			CalculateLayoutInputHorizontal();
			CalculateLayoutInputVertical();
			SetLayoutHorizontal();
			SetLayoutVertical();
		}

		public void Refresh()
		{
			PushInstances();
			PopInstances();
			UpdateInstances();
		}

		public GameObject ScrollToAndGetInstance(int index, float offset = 0f)
		{
			ScrollToElement(index, offset);
			Update();
			if (_indexMapping.TryGetKey(index, out int key))
			{
				return _instances[key];
			}
			return null;
		}

		private void PopInstances()
		{
			for (int i = 0; i < _recyclerChildCount; i++)
			{
				GameObject item = _objectPool.Pop(base.transform);
				_instances.Add(item);
				_indexMapping[i] = -1;
			}
		}

		private void UpdateInstances()
		{
			if (_initCallback != null)
			{
				if (_instances.Count < _recyclerChildCount)
				{
					UnityEngine.Debug.LogWarningFormat("Insufficient elements available. Expected {0} elements, found {1}", _recyclerChildCount, _instances.Count);
				}
				int count = _instances.Count;
				for (int i = 0; i < count; i++)
				{
					_instances[i].SetActive(i < _recyclerChildCount);
					int num = _startIndex + i;
					if (_indexMapping[i] != num)
					{
						_indexMapping[i] = num;
						if (!_initCallback(_instances[i], _indexMapping[i]))
						{
							_instances[i].SetActive(value: false);
						}
					}
				}
			}
			SetCellsAlongAxis(1);
		}

		private void UpdateInstanceOrder(int shift)
		{
			if (shift > 0)
			{
				for (int i = 0; i < shift; i++)
				{
					GameObject gameObject = _instances[0];
					gameObject.transform.SetSiblingIndex(_recyclerChildCount - 1);
					_instances.RemoveAt(0);
					_instances.Add(gameObject);
				}
				return;
			}
			int j = 0;
			for (int num = -shift; j < num; j++)
			{
				GameObject gameObject2 = _instances[_recyclerChildCount - 1];
				gameObject2.transform.SetSiblingIndex(0);
				_instances.RemoveAt(_recyclerChildCount - 1);
				_instances.Insert(0, gameObject2);
			}
		}

		private void SetCellsAlongAxis(int axis)
		{
			base.rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, GetTotalPreferredSize(axis));
			if (axis == 0)
			{
				for (int i = 0; i < base.rectChildren.Count; i++)
				{
					RectTransform rectTransform = base.rectChildren[i];
					m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
					rectTransform.anchorMin = Vector2.up;
					rectTransform.anchorMax = Vector2.up;
					rectTransform.sizeDelta = CellSize;
				}
				return;
			}
			int num = 1;
			int num2 = 1;
			if (_constraint == RecyclerConstraint.FixedColumnCount)
			{
				num = _constraintCount;
				num2 = Mathf.CeilToInt((float)_maxChildren / (float)num - 0.001f);
			}
			else if (_constraint == RecyclerConstraint.FixedRowCount)
			{
				num2 = _constraintCount;
				num = Mathf.CeilToInt((float)_maxChildren / (float)num2 - 0.001f);
			}
			int num3 = (int)StartCorner % 2;
			int num4 = (int)StartCorner / 2;
			int num5;
			int num6;
			int num7;
			if (StartAxis == GridLayoutGroup.Axis.Horizontal)
			{
				num5 = num;
				num6 = Mathf.Clamp(num, 1, base.rectChildren.Count);
				num7 = Mathf.Clamp(num2, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num5));
			}
			else
			{
				num5 = num2;
				num7 = Mathf.Clamp(num2, 1, base.rectChildren.Count);
				num6 = Mathf.Clamp(num, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num5));
			}
			Vector2 vector = new Vector2((float)num6 * CellSize.x + (float)(num6 - 1) * Spacing.x, (float)num7 * CellSize.y + (float)(num7 - 1) * Spacing.y);
			Vector2 vector2 = new Vector2(GetStartOffset(0, vector.x), GetStartOffset(1, vector.y));
			for (int j = 0; j < base.rectChildren.Count; j++)
			{
				if (!_indexMapping.TryGetValue(j, out int value))
				{
					value = j;
				}
				int num8;
				int num9;
				if (StartAxis == GridLayoutGroup.Axis.Horizontal)
				{
					num8 = value % num5;
					num9 = value / num5;
				}
				else
				{
					num8 = value / num5;
					num9 = value % num5;
				}
				if (num3 == 1)
				{
					num8 = num6 - 1 - num8;
				}
				if (num4 == 1)
				{
					num9 = num7 - 1 - num9;
				}
				SetChildAlongAxis(base.rectChildren[j], 0, vector2.x + (CellSize[0] + Spacing[0]) * (float)num8, CellSize[0]);
				SetChildAlongAxis(base.rectChildren[j], 1, vector2.y + (CellSize[1] + Spacing[1]) * (float)num9, CellSize[1]);
			}
		}
	}
}
