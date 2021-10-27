using UnityEngine;

namespace AsglaUI.UI {
	public class UICloseAndShowWindow : MonoBehaviour {

		public void CloseAndShow() {
			if (m_CloseWindow != null) m_CloseWindow.Hide();
			if (m_ShowWindow != null) m_ShowWindow.Show();
		}
#pragma warning disable 0649
		[SerializeField] private UIWindow m_CloseWindow;
		[SerializeField] private UIWindow m_ShowWindow;
#pragma warning restore 0649

	}
}