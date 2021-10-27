using UnityEngine;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Raycast Filters/Rectangular Raycast Filter")]
	[RequireComponent(typeof(RectTransform))]
	public class UIRectangularRaycastFilter : MonoBehaviour, ICanvasRaycastFilter {

		[SerializeField] private Vector2 m_Offset = Vector2.zero;

		[SerializeField] private RectOffset m_Borders = new RectOffset();

		[Range(0f, 1f)] [SerializeField] private float m_ScaleX = 1f;

		[Range(0f, 1f)] [SerializeField] private float m_ScaleY = 1f;

		/// <summary>
		///     Gets or sets the offset.
		/// </summary>
		/// <value>The offset.</value>
		public Vector2 offset {
			get => m_Offset;
			set => m_Offset = value;
		}

		/// <summary>
		///     Gets or sets the borders.
		/// </summary>
		/// <value>The borders.</value>
		public RectOffset borders {
			get => m_Borders;
			set => m_Borders = value;
		}

		/// <summary>
		///     Gets or sets the X scale.
		/// </summary>
		/// <value>The X scale.</value>
		public float scaleX {
			get => m_ScaleX;
			set => m_ScaleX = value;
		}

		/// <summary>
		///     Gets or sets the Y scale.
		/// </summary>
		/// <value>The Y scale.</value>
		public float scaleY {
			get => m_ScaleY;
			set => m_ScaleY = value;
		}

		/// <summary>
		///     Gets the scaled rect including the offset.
		/// </summary>
		/// <value>The scaled rect.</value>
		public Rect scaledRect {
			get{
				RectTransform rt = (RectTransform) transform;
				return new Rect(
					offset.x + borders.left + (rt.rect.x + (rt.rect.width - rt.rect.width * m_ScaleX) / 2f),
					offset.y + borders.bottom + (rt.rect.y + (rt.rect.height - rt.rect.height * m_ScaleY) / 2f),
					rt.rect.width * m_ScaleX - borders.left - borders.right,
					rt.rect.height * m_ScaleY - borders.top - borders.bottom
				);
			}
		}

		public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
			if (!enabled)
				return true;

			Vector2 localPositionPivotRelative;
			RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) transform, screenPoint, eventCamera,
				out localPositionPivotRelative);
			return scaledRect.Contains(localPositionPivotRelative);
		}

	}
}