using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class UIInputEvent : MonoBehaviour {

		private Selectable m_Selectable;

		protected void Awake() {
			m_Selectable = gameObject.GetComponent<Selectable>();
		}

		protected void Update() {
			if (!isActiveAndEnabled || !gameObject.activeInHierarchy || string.IsNullOrEmpty(m_InputName))
				return;

			// Break if the currently selected game object is a selectable
			if (EventSystem.current.currentSelectedGameObject != null) {
				// Check for selectable
				Selectable targetSelectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

				if (m_Selectable == null && targetSelectable != null || m_Selectable != null &&
					targetSelectable != null && !m_Selectable.Equals(targetSelectable))
					return;
			}

			// Check if we are using the escape input for this and if the escape key was used in the window manager
			if (UIWindowManager.Instance != null && UIWindowManager.Instance.escapeInputName == m_InputName &&
			    UIWindowManager.Instance.escapedUsed)
				return;

			try {
				if (Input.GetButton(m_InputName))
					m_OnButton.Invoke();

				if (Input.GetButtonDown(m_InputName))
					m_OnButtonDown.Invoke();

				if (Input.GetButtonUp(m_InputName))
					m_OnButtonUp.Invoke();
			} catch (ArgumentException) {
				enabled = false;
				Debug.LogWarning("Input \"" + m_InputName + "\" used by game object \"" + gameObject.name +
				                 "\" is not defined.");
			}
		}
#pragma warning disable 0649
		[SerializeField] private string m_InputName;

		[SerializeField] private UnityEvent m_OnButton;
		[SerializeField] private UnityEvent m_OnButtonDown;
		[SerializeField] private UnityEvent m_OnButtonUp;
#pragma warning restore 0649

	}
}