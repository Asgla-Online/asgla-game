using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class Demo_CharacterSelectList_Character : MonoBehaviour {

		private OnCharacterDeleteEvent m_OnCharacterDelete;
		private OnCharacterSelectEvent m_OnCharacterSelected;

		/// <summary>
		///     Gets the character info.
		/// </summary>
		public Demo_CharacterInfo characterInfo { get; private set; }

		/// <summary>
		///     Gets a value indicating wheather this character is selected.
		/// </summary>
		public bool isSelected => m_Toggle != null ? m_Toggle.isOn : false;

		protected void Awake() {
			m_OnCharacterSelected = new OnCharacterSelectEvent();
			m_OnCharacterDelete = new OnCharacterDeleteEvent();
		}

		protected void OnEnable() {
			if (m_Toggle != null) {
				m_Toggle.isOn = false;
				m_Toggle.onValueChanged.AddListener(OnToggleValueChanged);
			}

			if (m_Delete != null)
				m_Delete.onClick.AddListener(OnDeleteClick);
		}

		protected void OnDisable() {
			if (m_Toggle != null)
				m_Toggle.onValueChanged.RemoveListener(OnToggleValueChanged);

			if (m_Delete != null)
				m_Delete.onClick.RemoveListener(OnDeleteClick);
		}

		/// <summary>
		///     Sets the character info.
		/// </summary>
		/// <param name="info">The character info.</param>
		public void SetCharacterInfo(Demo_CharacterInfo info) {
			if (info == null)
				return;

			if (m_NameText != null) m_NameText.text = info.name.ToUpper();
			if (m_LevelText != null) m_LevelText.text = info.level.ToString();
			if (m_RaceText != null) m_RaceText.text = info.raceString;
			if (m_ClassText != null) m_ClassText.text = info.classString;

			// Set the character info
			characterInfo = info;
		}

		/// <summary>
		///     Sets the avatar sprite.
		/// </summary>
		/// <param name="sprite">The avatar sprite.</param>
		public void SetAvatar(Sprite sprite) {
			if (m_Avatar != null)
				m_Avatar.sprite = sprite;
		}

		/// <summary>
		///     Sets the toggle group.
		/// </summary>
		/// <param name="group">The toggle group.</param>
		public void SetToggleGroup(ToggleGroup group) {
			if (m_Toggle != null)
				m_Toggle.group = group;
		}

		/// <summary>
		///     Sets the selected state.
		/// </summary>
		/// <param name="selected">The selected state.</param>
		public void SetSelected(bool selected) {
			if (m_Toggle != null)
				m_Toggle.isOn = selected;
		}

		private void OnToggleValueChanged(bool value) {
			if (value && m_OnCharacterSelected != null)
				m_OnCharacterSelected.Invoke(this);
		}

		private void OnDeleteClick() {
			if (m_OnCharacterDelete != null)
				m_OnCharacterDelete.Invoke(this);
		}

		/// <summary>
		///     Adds on select listener.
		/// </summary>
		/// <param name="call">The on select listener.</param>
		public void AddOnSelectListener(UnityAction<Demo_CharacterSelectList_Character> call) {
			m_OnCharacterSelected.AddListener(call);
		}

		/// <summary>
		///     Removes on select listener.
		/// </summary>
		/// <param name="call">The on select listener.</param>
		public void RemoveOnSelectListener(UnityAction<Demo_CharacterSelectList_Character> call) {
			m_OnCharacterSelected.RemoveListener(call);
		}

		/// <summary>
		///     Adds on delete listener.
		/// </summary>
		/// <param name="call">The delete listener.</param>
		public void AddOnDeleteListener(UnityAction<Demo_CharacterSelectList_Character> call) {
			m_OnCharacterDelete.AddListener(call);
		}

		/// <summary>
		///     Removes on delete listener.
		/// </summary>
		/// <param name="call">The delete listener.</param>
		public void RemoveOnDeleteListener(UnityAction<Demo_CharacterSelectList_Character> call) {
			m_OnCharacterDelete.RemoveListener(call);
		}

		[Serializable]
		public class OnCharacterSelectEvent : UnityEvent<Demo_CharacterSelectList_Character> {

		}

		[Serializable]
		public class OnCharacterDeleteEvent : UnityEvent<Demo_CharacterSelectList_Character> {

		}

#pragma warning disable 0649
		[SerializeField] private Toggle m_Toggle;
		[SerializeField] private Button m_Delete;

		[Header("Texts")] [SerializeField] private Text m_NameText;

		[SerializeField] private Text m_LevelText;
		[SerializeField] private Text m_RaceText;
		[SerializeField] private Text m_ClassText;
		[SerializeField] private Image m_Avatar;
#pragma warning restore 0649

	}
}