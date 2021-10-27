using UnityEngine;

namespace AsglaUI.UI {
	/// <summary>
	///     The scroll rect expander is really simple, you place it on the scroll bar and it monitors when the bar is disabled.
	/// </summary>
	public class UIScrollRectExpander : MonoBehaviour {

		private bool m_Expanded;

		protected void OnEnable() {
			if (gameObject.activeSelf)
				Collapse();
		}

		protected void OnDisable() {
			if (!gameObject.activeSelf)
				Expand();
		}

		private void Expand() {
			if (m_Expanded || m_Target == null)
				return;

			m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_Target.rect.width + m_ExpandWidth);
			m_Expanded = true;
		}

		private void Collapse() {
			if (!m_Expanded || m_Target == null)
				return;

			m_Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_Target.rect.width - m_ExpandWidth);
			m_Expanded = false;
		}
#pragma warning disable 0649
		[SerializeField] private float m_ExpandWidth;
		[SerializeField] private RectTransform m_Target;
#pragma warning restore 0649

	}
}