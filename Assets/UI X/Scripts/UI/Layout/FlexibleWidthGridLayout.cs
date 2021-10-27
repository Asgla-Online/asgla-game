using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	/// <summary>
	///     In this class we calculate board cell width prior to layout calculation.
	/// </summary>
	public class FlexibleWidthGridLayout : GridLayoutGroup {

		public override void SetLayoutHorizontal() {
			UpdateCellSize();
			base.SetLayoutHorizontal();
		}

		public override void SetLayoutVertical() {
			UpdateCellSize();
			base.SetLayoutVertical();
		}

		private void UpdateCellSize() {
			float x = (rectTransform.rect.size.x - padding.horizontal - spacing.x * (constraintCount - 1)) /
			          constraintCount;
			constraint = Constraint.FixedColumnCount;
			cellSize = new Vector2(x, cellSize.y);
		}

	}
}