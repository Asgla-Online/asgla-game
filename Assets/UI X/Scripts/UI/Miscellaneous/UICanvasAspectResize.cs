using UnityEngine;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class UICanvasAspectResize : MonoBehaviour {

#pragma warning disable 0649
		[SerializeField] private Camera m_Camera;
#pragma warning restore 0649

		private RectTransform m_RectTransform;

		protected void Awake() {
			m_RectTransform = transform as RectTransform;
		}

		private void Update() {
			if (m_Camera == null)
				return;

			m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
				m_Camera.aspect * m_RectTransform.rect.size.y);
		}

	}
}