using UnityEngine;

namespace AsglaUI.UI {
	public class Demo_LookAtCamera : MonoBehaviour {

		[SerializeField] private Camera m_Camera;

		protected void Awake() {
			if (m_Camera == null) m_Camera = Camera.main;
		}

		private void Update() {
			if (m_Camera)
				transform.rotation = Quaternion.LookRotation(m_Camera.transform.forward);
		}

	}
}