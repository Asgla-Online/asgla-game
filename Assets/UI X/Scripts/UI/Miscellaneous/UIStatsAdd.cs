using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class UIStatsAdd : MonoBehaviour {

#pragma warning disable 0649
		[SerializeField] private Text m_ValueText;
#pragma warning restore 0649

		public void OnButtonPress() {
			if (m_ValueText == null)
				return;

			m_ValueText.text = (int.Parse(m_ValueText.text) + 1).ToString();
		}

	}
}