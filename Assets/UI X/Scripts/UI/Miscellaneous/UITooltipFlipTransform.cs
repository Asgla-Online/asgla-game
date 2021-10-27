using UnityEngine;

namespace AsglaUI.UI {
	public class UITooltipFlipTransform : MonoBehaviour {

		private Vector2 m_OriginalAnchorMax;
		private Vector2 m_OriginalAnchorMin;

		private Vector2 m_OriginalPivot;
		private Vector2 m_OriginalPosition;

		protected void Awake() {
			if (m_Transform == null || m_Tooltip == null)
				return;

			m_OriginalPivot = m_Transform.pivot;
			m_OriginalAnchorMin = m_Transform.anchorMin;
			m_OriginalAnchorMax = m_Transform.anchorMax;
			m_OriginalPosition = m_Transform.anchoredPosition;
		}

		protected void OnEnable() {
			if (m_Transform == null || m_Tooltip == null)
				return;

			m_Tooltip.onAnchorEvent.AddListener(OnAnchor);
		}

		protected void OnDisable() {
			if (m_Transform == null || m_Tooltip == null)
				return;

			m_Tooltip.onAnchorEvent.RemoveListener(OnAnchor);
		}

		public void OnAnchor(UITooltip.Anchor anchor) {
			if (m_Transform == null)
				return;

			switch (anchor) {
				case UITooltip.Anchor.Left:
				case UITooltip.Anchor.BottomLeft:
				case UITooltip.Anchor.TopLeft:
				case UITooltip.Anchor.Bottom:
					m_Transform.pivot = m_OriginalPivot;
					m_Transform.anchorMin = m_OriginalAnchorMin;
					m_Transform.anchorMax = m_OriginalAnchorMax;
					m_Transform.anchoredPosition = m_OriginalPosition;
					break;
				case UITooltip.Anchor.Right:
				case UITooltip.Anchor.BottomRight:
				case UITooltip.Anchor.TopRight:
					m_Transform.pivot = new Vector2(m_OriginalPivot.x == 0f ? 1f : 0f, m_OriginalPivot.y);
					m_Transform.anchorMin = new Vector2(m_OriginalAnchorMin.x == 0f ? 1f : 0f, m_OriginalAnchorMin.y);
					m_Transform.anchorMax = new Vector2(m_OriginalAnchorMax.x == 0f ? 1f : 0f, m_OriginalAnchorMax.y);
					m_Transform.anchoredPosition = new Vector2(m_OriginalPosition.x * -1, m_OriginalPosition.y);
					break;
				case UITooltip.Anchor.Top:
					m_Transform.pivot = new Vector2(m_OriginalPivot.x, m_OriginalPivot.y == 0f ? 1f : 0f);
					m_Transform.anchorMin = new Vector2(m_OriginalAnchorMin.x, m_OriginalAnchorMin.y == 0f ? 1f : 0f);
					m_Transform.anchorMax = new Vector2(m_OriginalAnchorMax.x, m_OriginalAnchorMax.y == 0f ? 1f : 0f);
					m_Transform.anchoredPosition = new Vector2(m_OriginalPosition.x, m_OriginalPosition.y * -1);
					break;
			}
		}
#pragma warning disable 0649
		[SerializeField] private UITooltip m_Tooltip;
		[SerializeField] private RectTransform m_Transform;
#pragma warning restore 0649

	}
}