using UnityEngine;

namespace AsglaUI.UI {
	public class UIToggleWindowAndTab : MonoBehaviour {

		public UIWindow window;
		public UITab tab;

		public void Toggle() {
			if (window == null || tab == null)
				return;

			// Check if the window is open
			if (window.IsOpen)
				// Check if the tab is active
				if (tab.isOn) {
					// Close the window since everything was already opened
					window.Hide();
					return;
				}

			// If we have reached this part of the code, that means the we should open up things
			window.Show();
			tab.Activate();
		}

	}
}