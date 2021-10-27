using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[RequireComponent(typeof(Text))]
	public class UITextSetValue : MonoBehaviour {

		public string floatFormat = "0.00";

		private Text m_Text;

		protected void Awake() {
			m_Text = gameObject.GetComponent<Text>();
		}

		public void SetFloat(float value) {
			if (m_Text != null)
				m_Text.text = value.ToString(floatFormat);
		}

	}
}