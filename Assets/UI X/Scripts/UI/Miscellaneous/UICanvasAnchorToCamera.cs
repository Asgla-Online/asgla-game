using UnityEngine;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class UICanvasAnchorToCamera : MonoBehaviour {

#pragma warning disable 0649
		[SerializeField] private Camera m_Camera;
#pragma warning restore 0649
		[SerializeField] [Range(0f, 1f)] private float m_Vertical;
		[SerializeField] [Range(0f, 1f)] private float m_Horizontal;

		private RectTransform m_RectTransform;

		protected void Awake() {
			m_RectTransform = transform as RectTransform;
		}

		private void Update() {
			if (m_Camera == null)
				return;

			Vector3 newPos =
				m_Camera.ViewportToWorldPoint(new Vector3(m_Horizontal, m_Vertical, m_Camera.farClipPlane));
			newPos.z = m_RectTransform.position.z;
			m_RectTransform.position = newPos;
		}

	}
}