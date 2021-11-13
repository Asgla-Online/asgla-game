using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AsglaUI.UI {
	[RequireComponent(typeof(ToggleGroup))]
	public class Demo_CharacterSelectList : MonoBehaviour {

		[Header("Demo Properties")] [SerializeField]
		private bool m_IsDemo;

		[SerializeField] private int m_AddCharacters = 5;

		[Header("Events")] [SerializeField]
		private OnCharacterSelectedEvent m_OnCharacterSelected = new OnCharacterSelectedEvent();

		[SerializeField] private OnCharacterDeleteEvent m_OnCharacterDelete = new OnCharacterDeleteEvent();
		private Demo_CharacterSelectList_Character m_DeletingCharacter;

		private ToggleGroup m_ToggleGroup;

		protected void Awake() {
			m_ToggleGroup = gameObject.GetComponent<ToggleGroup>();
		}

		protected void Start() {
			// Clear the characters container
			if (m_CharactersContainer != null)
				foreach (Transform t in m_CharactersContainer)
					Destroy(t.gameObject);

			// Add characters for the demo
			if (m_IsDemo && m_CharacterPrefab)
				for (int i = 0; i < m_AddCharacters; i++) {
					string[] names = new string[10]
						{"Annika", "Evita", "Herb", "Thad", "Myesha", "Lucile", "Sharice", "Tatiana", "Isis", "Allen"};
					string[] races = new string[5] {"Human", "Elf", "Orc", "Undead", "Programmer"};
					string[] classes = new string[5] {"Warrior", "Mage", "Hunter", "Priest", "Designer"};

					Demo_CharacterInfo info = new Demo_CharacterInfo();
					info.name = names[Random.Range(0, 10)];
					info.raceString = races[Random.Range(0, 5)];
					info.classString = classes[Random.Range(0, 5)];
					info.level = Random.Range(1, 61);

					AddCharacter(info, i == 0);
				}
		}

		/// <summary>
		///     Adds a character to the character list.
		/// </summary>
		/// <param name="info">The character info.</param>
		/// <param name="selected">In the character should be selected.</param>
		public void AddCharacter(Demo_CharacterInfo info, bool selected) {
			if (m_CharacterPrefab == null || m_CharactersContainer == null)
				return;

			// Add the character
			GameObject model = Instantiate(m_CharacterPrefab);
			model.layer = m_CharactersContainer.gameObject.layer;
			model.transform.SetParent(m_CharactersContainer, false);
			model.transform.localScale = m_CharacterPrefab.transform.localScale;
			model.transform.localPosition = m_CharacterPrefab.transform.localPosition;
			model.transform.localRotation = m_CharacterPrefab.transform.localRotation;

			// Get the character component
			Demo_CharacterSelectList_Character character = model.GetComponent<Demo_CharacterSelectList_Character>();

			if (character != null) {
				// Set the info
				character.SetCharacterInfo(info);

				// Set the toggle group
				character.SetToggleGroup(m_ToggleGroup);

				// Set the selected state
				character.SetSelected(selected);

				// Add on select listener
				character.AddOnSelectListener(OnCharacterSelected);

				// Add on delete listener
				character.AddOnDeleteListener(OnCharacterDeleteRequested);
			}
		}

		/// <summary>
		///     Event invoked when when a character in the list is selected.
		/// </summary>
		/// <param name="character">The character.</param>
		private void OnCharacterSelected(Demo_CharacterSelectList_Character character) {
			if (m_OnCharacterSelected != null)
				m_OnCharacterSelected.Invoke(character.characterInfo);
		}

		/// <summary>
		///     Event invoked when when a character delete button is pressed.
		/// </summary>
		/// <param name="character">The character.</param>
		private void OnCharacterDeleteRequested(Demo_CharacterSelectList_Character character) {
			// Save the deleting character reference
			m_DeletingCharacter = character;

			// Create a modal box
			UIModalBox box = UIModalBoxManager.Instance.Create(gameObject);
			if (box != null) {
				box.SetText1("Do you really want to delete this character?");
				box.SetText2(
					"You wont be able to reverse this operation and yourcharcater will be permamently removed.");
				box.SetConfirmButtonText("DELETE");
				box.onConfirm.AddListener(OnCharacterDeleteConfirm);
				box.onCancel.AddListener(OnCharacterDeleteCancel);
				box.Show();
			}
		}

		/// <summary>
		///     Event invoked when a character deletion is confirmed.
		/// </summary>
		private void OnCharacterDeleteConfirm() {
			if (m_DeletingCharacter == null)
				return;

			// If this character is selected
			if (m_DeletingCharacter.isSelected && m_CharactersContainer != null)
				// Find and select new character
				foreach (Transform t in m_CharactersContainer) {
					Demo_CharacterSelectList_Character character =
						t.gameObject.GetComponent<Demo_CharacterSelectList_Character>();

					// If the character is not the one we are deleting
					if (!character.Equals(m_DeletingCharacter)) {
						character.SetSelected(true);
						break;
					}
				}

			// Invoke the on delete event
			if (m_OnCharacterDelete != null)
				m_OnCharacterDelete.Invoke(m_DeletingCharacter.characterInfo);

			// Delete the character game object
			Destroy(m_DeletingCharacter.gameObject);
		}

		/// <summary>
		///     Event invoked when a character deletion is canceled.
		/// </summary>
		private void OnCharacterDeleteCancel() {
			m_DeletingCharacter = null;
		}

		[Serializable]
		public class OnCharacterSelectedEvent : UnityEvent<Demo_CharacterInfo> {

		}

		[Serializable]
		public class OnCharacterDeleteEvent : UnityEvent<Demo_CharacterInfo> {

		}

#pragma warning disable 0649
		[SerializeField] private GameObject m_CharacterPrefab;
		[SerializeField] private Transform m_CharactersContainer;
#pragma warning restore 0649

	}
}