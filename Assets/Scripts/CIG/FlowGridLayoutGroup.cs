using UnityEngine;
using UnityEngine.UI;

namespace CIG
{
	[AddComponentMenu("Layout/Flow Grid Layout Group", 152)]
	public class FlowGridLayoutGroup : LayoutGroup
	{
		public enum ConstraintType
		{
			FixedColumnCount,
			FixedRowCount
		}

		[SerializeField]
		private Vector2 _cellSize = new Vector2(100f, 100f);

		[SerializeField]
		private Vector2 _spacing = Vector2.zero;

		[SerializeField]
		private ConstraintType _constraint;

		[SerializeField]
		private int _constraintCount = 2;

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

		public ConstraintType Constraint
		{
			get
			{
				return _constraint;
			}
			set
			{
				SetProperty(ref _constraint, value);
			}
		}

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

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			int num;
			int num2;
			switch (_constraint)
			{
			case ConstraintType.FixedColumnCount:
				num = (num2 = _constraintCount);
				break;
			case ConstraintType.FixedRowCount:
				num = (num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)_constraintCount - 0.001f));
				break;
			default:
				num = 1;
				num2 = Mathf.CeilToInt(Mathf.Sqrt(base.rectChildren.Count));
				break;
			}
			SetLayoutInputForAxis((float)base.padding.horizontal + (CellSize.x + Spacing.x) * (float)num - Spacing.x, (float)base.padding.horizontal + (CellSize.x + Spacing.x) * (float)num2 - Spacing.x, -1f, 0);
		}

		public override void CalculateLayoutInputVertical()
		{
			int num2;
			switch (_constraint)
			{
			case ConstraintType.FixedColumnCount:
				num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)_constraintCount - 0.001f);
				break;
			case ConstraintType.FixedRowCount:
				num2 = _constraintCount;
				break;
			default:
			{
				float x = base.rectTransform.rect.size.x;
				int num = Mathf.Max(1, Mathf.FloorToInt((x - (float)base.padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x)));
				num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num);
				break;
			}
			}
			float num3 = (float)base.padding.vertical + (CellSize.y + Spacing.y) * (float)num2 - Spacing.y;
			SetLayoutInputForAxis(num3, num3, -1f, 1);
		}

		public override void SetLayoutHorizontal()
		{
			int i = 0;
			for (int count = base.rectChildren.Count; i < count; i++)
			{
				RectTransform rectTransform = base.rectChildren[i];
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.SizeDeltaY);
				rectTransform.anchorMin = Vector2.up;
				rectTransform.anchorMax = Vector2.up;
				rectTransform.sizeDelta = CellSize;
			}
		}

		public override void SetLayoutVertical()
		{
			float x = base.rectTransform.rect.size.x;
			float y = base.rectTransform.rect.size.y;
			int num;
			int num2;
			switch (_constraint)
			{
			case ConstraintType.FixedColumnCount:
				num = _constraintCount;
				num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num - 0.001f);
				break;
			case ConstraintType.FixedRowCount:
				num2 = _constraintCount;
				num = Mathf.CeilToInt((float)base.rectChildren.Count / (float)num2 - 0.001f);
				break;
			default:
				num = ((CellSize.x + Spacing.x <= 0f) ? int.MaxValue : Mathf.Max(1, Mathf.FloorToInt((x - (float)base.padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x))));
				num2 = ((CellSize.y + Spacing.y <= 0f) ? int.MaxValue : Mathf.Max(1, Mathf.FloorToInt((y - (float)base.padding.vertical + Spacing.y + 0.001f) / (CellSize.y + Spacing.y))));
				break;
			}
			int num3 = Mathf.Clamp(num, 1, base.rectChildren.Count);
			int num4 = Mathf.Clamp(num2, 1, Mathf.CeilToInt((float)base.rectChildren.Count / (float)num));
			Vector2 vector = new Vector2((float)num3 * CellSize.x + (float)(num3 - 1) * Spacing.x, (float)num4 * CellSize.y + (float)(num4 - 1) * Spacing.y);
			Vector2 vector2 = new Vector2(GetStartOffset(0, vector.x), GetStartOffset(1, vector.y));
			int i = 0;
			for (int count = base.rectChildren.Count; i < count; i++)
			{
				float num5 = 0f;
				int num6 = i % num3;
				int num7 = i / num3;
				if (num7 + 1 == num4)
				{
					int num8 = count - num7 * num3;
					if (num8 < num3)
					{
						num5 = (float)num8 / (float)num3;
					}
				}
				SetChildAlongAxis(base.rectChildren[i], 0, vector2.x + (CellSize.x + Spacing.x) * ((float)num6 + num5), CellSize.x);
				SetChildAlongAxis(base.rectChildren[i], 1, vector2.y + (CellSize.y + Spacing.y) * (float)num7, CellSize.y);
			}
		}
	}
}
