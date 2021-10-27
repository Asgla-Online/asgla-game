using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class UITooltipFlipBackground : MonoBehaviour {

		private UIFlippable m_Flippable;

		protected void Awake() {
			if (m_Graphic == null || m_Tooltip == null)
				return;

			m_Flippable = m_Graphic.gameObject.GetComponent<UIFlippable>();

			if (m_Flippable == null)
				m_Flippable = m_Graphic.gameObject.AddComponent<UIFlippable>();
		}

		protected void OnEnable() {
			if (m_Graphic == null || m_Tooltip == null)
				return;

			m_Tooltip.onAnchorEvent.AddListener(OnAnchor);
		}

		protected void OnDisable() {
			if (m_Graphic == null || m_Tooltip == null)
				return;

			m_Tooltip.onAnchorEvent.RemoveListener(OnAnchor);
		}

		public void OnAnchor(UITooltip.Anchor anchor) {
			if (m_Graphic == null || m_Flippable == null)
				return;

			switch (anchor) {
				case UITooltip.Anchor.Left:
					m_Flippable.horizontal = false;
					break;
				case UITooltip.Anchor.Right:
					m_Flippable.horizontal = true;
					break;
				case UITooltip.Anchor.Bottom:
					m_Flippable.vertical = false;
					break;
				case UITooltip.Anchor.Top:
					m_Flippable.vertical = true;
					break;
				case UITooltip.Anchor.BottomLeft:
					m_Flippable.horizontal = false;
					m_Flippable.vertical = false;
					break;
				case UITooltip.Anchor.BottomRight:
					m_Flippable.horizontal = true;
					m_Flippable.vertical = false;
					break;
				case UITooltip.Anchor.TopLeft:
					m_Flippable.horizontal = false;
					m_Flippable.vertical = true;
					break;
				case UITooltip.Anchor.TopRight:
					m_Flippable.horizontal = true;
					m_Flippable.vertical = true;
					break;
			}
		}
#pragma warning disable 0649
		[SerializeField] private UITooltip m_Tooltip;
		[SerializeField] private Graphic m_Graphic;
#pragma warning restore 0649

	}
}