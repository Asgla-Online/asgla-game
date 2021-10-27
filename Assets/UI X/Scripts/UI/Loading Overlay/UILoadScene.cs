using Asgla;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[AddComponentMenu("Miscellaneous/Load Scene")]
	public class UILoadScene : MonoBehaviour {

		protected void Update() {
			if (!isActiveAndEnabled || !gameObject.activeInHierarchy || m_InputKey == InputKey.None)
				return;

			// Break if the currently selected game object is a selectable
			if (EventSystem.current.currentSelectedGameObject != null) {
				// Check for selectable
				Selectable selectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

				if (selectable != null)
					return;
			}

			// Check if we are using the escape input for this and if the escape key was used in the window manager
			if (m_InputKey == InputKey.Cancel)
				if (UIWindowManager.Instance != null && UIWindowManager.Instance.escapeInputName == "Cancel" &&
				    UIWindowManager.Instance.escapedUsed)
					return;

			// Check if we are using the escape input for this and if we have an active modal box
			if (m_InputKey == InputKey.Cancel && UIModalBoxManager.Instance != null &&
			    UIModalBoxManager.Instance.activeBoxes.Length > 0)
				return;

			string buttonName = string.Empty;

			switch (m_InputKey) {
				case InputKey.Submit:
					buttonName = "Submit";
					break;
				case InputKey.Cancel:
					buttonName = "Cancel";
					break;
				case InputKey.Jump:
					buttonName = "Jump";
					break;
				case InputKey.None:
				default:
					buttonName = "None";
					break;
			}

			if (!string.IsNullOrEmpty(buttonName) && Input.GetButtonDown(buttonName))
				LoadScene();
		}

		protected void OnEnable() {
			if (m_HookToButton != null)
				m_HookToButton.onClick.AddListener(LoadScene);
		}

		protected void OnDisable() {
			if (m_HookToButton != null)
				m_HookToButton.onClick.RemoveListener(LoadScene);
		}

		public void LoadScene() {
			if (!string.IsNullOrEmpty(m_Scene)) {
				int id;
				bool isNumeric = int.TryParse(m_Scene, out id);

				if (m_UseLoadingOverlay && UILoadingOverlayManager.Instance != null) {
					UILoadingOverlay loadingOverlay = UILoadingOverlayManager.Instance.Create();

					if (loadingOverlay != null) {
						if (isNumeric)
							loadingOverlay.LoadScene(id);
						else
							loadingOverlay.LoadScene(m_Scene);
					} else {
						Debug.LogWarning(
							"Failed to instantiate the loading overlay prefab, make sure it's assigned on the manager.");
					}
				} else {
					if (isNumeric)
						SceneManager.LoadScene(id);
					else
						SceneManager.LoadScene(m_Scene);
				}
			}

			if (m_Scene == "0")
				Main.Singleton.Network.Connection.Close();
		}

		private enum InputKey {

			None,
			Submit,
			Cancel,
			Jump

		}

#pragma warning disable 0649
		[SerializeField] private string m_Scene;
		[SerializeField] private bool m_UseLoadingOverlay;
		[SerializeField] private InputKey m_InputKey = InputKey.None;
		[SerializeField] private Button m_HookToButton;
#pragma warning restore 0649

	}
}