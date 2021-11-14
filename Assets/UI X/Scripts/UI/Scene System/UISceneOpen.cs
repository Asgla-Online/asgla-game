using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/UI Scene/Open")]
	public class UISceneOpen : MonoBehaviour {

		protected void Update() {
			if (!isActiveAndEnabled || !gameObject.activeInHierarchy || m_InputKey == InputKey.None)
				return;

			// Check if we are using the escape input for this and if the escape key was used in the window manager
			if (m_InputKey == InputKey.Cancel)
				if (UIWindowManager.Instance != null && UIWindowManager.Instance.escapeInputName == "Cancel" &&
				    UIWindowManager.Instance.escapedUsed)
					return;

			// Check if we are using the escape input for this and if we have an active modal box
			if (m_InputKey == InputKey.Cancel && UIModalBoxManager.Instance != null &&
			    UIModalBoxManager.Instance.ActiveBoxes.Length > 0)
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
			}

			if (!string.IsNullOrEmpty(buttonName) && Input.GetButtonDown(buttonName))
				Open();
		}

		protected void OnEnable() {
			if (m_HookToButton != null)
				m_HookToButton.onClick.AddListener(Open);
		}

		protected void OnDisable() {
			if (m_HookToButton != null)
				m_HookToButton.onClick.RemoveListener(Open);
		}

		public void Open() {
			UIScene scene = null;

			switch (m_ActionType) {
				case ActionType.SpecificID:
					scene = UISceneRegistry.instance.GetScene(m_SceneId);
					break;
				case ActionType.LastScene:
					scene = UISceneRegistry.instance.lastScene;
					break;
			}

			if (scene != null)
				scene.TransitionTo();
		}

		private enum ActionType {

			SpecificID,
			LastScene

		}

		private enum InputKey {

			None,
			Submit,
			Cancel,
			Jump

		}

#pragma warning disable 0649
		[SerializeField] private ActionType m_ActionType = ActionType.SpecificID;
		[SerializeField] private int m_SceneId;
		[SerializeField] private InputKey m_InputKey = InputKey.None;
		[SerializeField] private Button m_HookToButton;
#pragma warning restore 0649

	}
}