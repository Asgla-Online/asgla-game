using System.Collections.Generic;
using UnityEngine;

namespace AsglaUI.UI {
	public class UIWindowManager : MonoBehaviour {

		[SerializeField] private string m_EscapeInputName = "Cancel";

		/// <summary>
		///     Gets the current instance of the window manager.
		/// </summary>
		public static UIWindowManager Instance { get; private set; }

		/// <summary>
		///     Gets the escape input name.
		/// </summary>
		public string escapeInputName => m_EscapeInputName;

		/// <summary>
		///     Gets a value indicating whether the escape input was used to hide a window in this frame.
		/// </summary>
		public bool escapedUsed { get; private set; }

		protected virtual void Awake() {
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
		}

		protected virtual void Update() {
			// Reset the escape used variable
			if (escapedUsed)
				escapedUsed = false;

			// Check for escape key press
			if (Input.GetButtonDown(m_EscapeInputName)) {
				// Check for currently opened modal and exit this method if one is found
				UIModalBox[] modalBoxes = FindObjectsOfType<UIModalBox>();

				if (modalBoxes.Length > 0)
					foreach (UIModalBox box in modalBoxes) // If the box is active
						if (box.IsActive && box.isActiveAndEnabled && box.gameObject.activeInHierarchy)
							return;

				// Get the windows list
				List<UIWindow> windows = UIWindow.GetWindows();

				// Loop through the windows and hide if required
				foreach (UIWindow window in windows) // Check if the window has escape key action
					if (window.escapeKeyAction !=
					    UIWindow.EscapeKeyAction.None) // Check if the window should be hidden on escape
						if (window.IsOpen && (window.escapeKeyAction == UIWindow.EscapeKeyAction.Hide ||
						                      window.escapeKeyAction == UIWindow.EscapeKeyAction.Toggle ||
						                      window.escapeKeyAction == UIWindow.EscapeKeyAction.HideIfFocused &&
						                      window.IsFocused)) {
							// Hide the window
							window.Hide();

							// Mark the escape input as used
							escapedUsed = true;
						}

				// Exit the method if the escape was used for hiding windows
				if (escapedUsed)
					return;

				// Loop through the windows again and show any if required
				foreach (UIWindow window in
					windows) // Check if the window has escape key action toggle and is not shown
					if (!window.IsOpen && window.escapeKeyAction == UIWindow.EscapeKeyAction.Toggle) // Show the window
						window.Show();
			}
		}

		protected virtual void OnDestroy() {
			if (Instance.Equals(this))
				Instance = null;
		}

	}
}