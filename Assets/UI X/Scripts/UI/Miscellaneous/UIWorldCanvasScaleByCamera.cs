using UnityEngine;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	public class UIWorldCanvasScaleByCamera : MonoBehaviour {

		protected void Update() {
			if (m_Camera == null || m_Canvas == null)
				return;

			float camHeight;
			float distanceToMain = Vector3.Distance(m_Camera.transform.position, m_Canvas.transform.position);

			if (m_Camera.orthographic)
				camHeight = m_Camera.orthographicSize * 2.0f;
			else
				camHeight = 2.0f * distanceToMain * Mathf.Tan(m_Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

			float scaleFactor = Screen.height / (m_Canvas.transform as RectTransform).rect.height;
			float scale = camHeight / Screen.height * scaleFactor;

			m_Canvas.transform.localScale = new Vector3(scale, scale, 1.0f);
		}

#pragma warning disable 0649
		[SerializeField] private Camera m_Camera;
		[SerializeField] private Canvas m_Canvas;
#pragma warning restore 0649

	}
}