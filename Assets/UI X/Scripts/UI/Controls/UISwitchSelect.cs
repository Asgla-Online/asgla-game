using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/Switch Select Field", 58)]
	public class UISwitchSelect : MonoBehaviour {

		/// <summary>
		///     New line-delimited list of items.
		/// </summary>
		[SerializeField] private List<string> m_Options = new List<string>();

		/// <summary>
		///     Event delegates triggered when the selected option changes.
		/// </summary>
		public ChangeEvent onChange = new ChangeEvent();

		/// <summary>
		///     The list of options.
		/// </summary>
		public List<string> options => m_Options;

		/// <summary>
		///     Currently selected option.
		/// </summary>
		public string value {
			get => m_SelectedItem;
			set => SelectOption(value);
		}

		/// <summary>
		///     Gets the index of the selected option.
		/// </summary>
		/// <value>The index of the selected option.</value>
		public int selectedOptionIndex => GetOptionIndex(m_SelectedItem);

		protected void OnEnable() {
			// Hook the buttons click events
			if (m_PrevButton != null)
				m_PrevButton.onClick.AddListener(OnPrevButtonClick);
			if (m_NextButton != null)
				m_NextButton.onClick.AddListener(OnNextButtonClick);
		}

		protected void OnDisable() {
			// Unhook the buttons click events
			if (m_PrevButton != null)
				m_PrevButton.onClick.RemoveListener(OnPrevButtonClick);
			if (m_NextButton != null)
				m_NextButton.onClick.RemoveListener(OnNextButtonClick);
		}

		protected void OnPrevButtonClick() {
			int prevIndex = selectedOptionIndex - 1;

			// Check if the option index is valid
			if (prevIndex < 0)
				prevIndex = m_Options.Count - 1;

			if (prevIndex >= m_Options.Count)
				prevIndex = 0;

			// Select the new option
			SelectOptionByIndex(prevIndex);
		}

		protected void OnNextButtonClick() {
			int nextIndex = selectedOptionIndex + 1;

			// Check if the option index is valid
			if (nextIndex < 0)
				nextIndex = m_Options.Count - 1;

			if (nextIndex >= m_Options.Count)
				nextIndex = 0;

			// Select the new option
			SelectOptionByIndex(nextIndex);
		}

		/// <summary>
		///     Gets the index of the given option.
		/// </summary>
		/// <returns>The option index. (-1 if the option was not found)</returns>
		/// <param name="optionValue">Option value.</param>
		public int GetOptionIndex(string optionValue) {
			// Find the option index in the options list
			if (m_Options != null && m_Options.Count > 0 && !string.IsNullOrEmpty(optionValue))
				for (int i = 0; i < m_Options.Count; i++)
					if (optionValue.Equals(m_Options[i], StringComparison.OrdinalIgnoreCase))
						return i;

			// Default
			return -1;
		}

		/// <summary>
		///     Selects the option by index.
		/// </summary>
		/// <param name="optionIndex">Option index.</param>
		public void SelectOptionByIndex(int index) {
			// Check if the option index is valid
			if (index < 0 || index >= m_Options.Count)
				return;

			string newOption = m_Options[index];

			// If the options changes
			if (!newOption.Equals(m_SelectedItem)) {
				// Set as selected
				m_SelectedItem = newOption;

				// Trigger change
				TriggerChangeEvent();
			}
		}

		/// <summary>
		///     Selects the option by value.
		/// </summary>
		/// <param name="optionValue">The option value.</param>
		public void SelectOption(string optionValue) {
			if (string.IsNullOrEmpty(optionValue))
				return;

			// Get the option
			int index = GetOptionIndex(optionValue);

			// Check if the option index is valid
			if (index < 0 || index >= m_Options.Count)
				return;

			// Select the option
			SelectOptionByIndex(index);
		}

		/// <summary>
		///     Adds an option.
		/// </summary>
		/// <param name="optionValue">Option value.</param>
		public void AddOption(string optionValue) {
			if (m_Options != null)
				m_Options.Add(optionValue);
		}

		/// <summary>
		///     Adds an option at given index.
		/// </summary>
		/// <param name="optionValue">Option value.</param>
		/// <param name="index">Index.</param>
		public void AddOptionAtIndex(string optionValue, int index) {
			if (m_Options == null)
				return;

			// Check if the index is outside the list
			if (index >= m_Options.Count)
				m_Options.Add(optionValue);
			else
				m_Options.Insert(index, optionValue);
		}

		/// <summary>
		///     Removes the option.
		/// </summary>
		/// <param name="optionValue">Option value.</param>
		public void RemoveOption(string optionValue) {
			if (m_Options == null)
				return;

			// Remove the option if exists
			if (m_Options.Contains(optionValue)) {
				m_Options.Remove(optionValue);
				ValidateSelectedOption();
			}
		}

		/// <summary>
		///     Removes the option at the given index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveOptionAtIndex(int index) {
			if (m_Options == null)
				return;

			// Remove the option if the index is valid
			if (index >= 0 && index < m_Options.Count) {
				m_Options.RemoveAt(index);
				ValidateSelectedOption();
			}
		}

		/// <summary>
		///     Validates the selected option and makes corrections if it's missing.
		/// </summary>
		public void ValidateSelectedOption() {
			if (m_Options == null)
				return;

			// Fix the selected option if it no longer exists
			if (!m_Options.Contains(m_SelectedItem))
				// Select the first option
				SelectOptionByIndex(0);
		}

		/// <summary>
		///     Tiggers the change event.
		/// </summary>
		protected virtual void TriggerChangeEvent() {
			// Apply the string to the label componenet
			if (m_Text != null)
				m_Text.text = m_SelectedItem;

			// Invoke the on change event
			if (onChange != null)
				onChange.Invoke(selectedOptionIndex, m_SelectedItem);
		}

		[Serializable]
		public class ChangeEvent : UnityEvent<int, string> {

		}
#pragma warning disable 0649
		[SerializeField] private Text m_Text;
		[SerializeField] private Button m_PrevButton;
		[SerializeField] private Button m_NextButton;

		// Currently selected item
		[HideInInspector] [SerializeField] private string m_SelectedItem;
#pragma warning restore 0649

	}
}