using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AsglaUI.UI {
	public class Demo_CharacterSelectMgr : MonoBehaviour {

		private int m_SelectedIndex = -1;
		private Transform m_SelectedTransform;

        protected void Awake()
        {
            // Save a reference to the instance
            m_Mgr = this;

			// Get a camera if not set
			if (m_Camera == null) m_Camera = Camera.main;

			// Disable the info container
			if (m_InfoContainer != null) m_InfoContainer.SetActive(false);
		}

		protected void Start() {
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

					AddCharacter(info, m_CharacterPrefab, i);
				}

			// Select the first character
			SelectFirstAvailable();
		}

		protected void Update() {
			if (isActiveAndEnabled && m_Slots.Count == 0)
				return;

			// Make sure we have a slot transform
			if (m_SelectedTransform != null) {
				Vector3 targetPos = m_SelectedTransform.position + m_CameraDirection * m_CameraDistance;
				targetPos.y = m_Camera.transform.position.y;

				m_Camera.transform.position =
					Vector3.Lerp(m_Camera.transform.position, targetPos, Time.deltaTime * m_CameraSpeed);
			}
		}

		protected void OnDestroy() {
			instance = null;
		}

        /// <summary>
        /// Adds a character to the slot at the specified index.
        /// </summary>
        /// <param name="info">The character info.</param>
        /// <param name="modelPrefab">The character model prefab.</param>
        /// <param name="index">Slot index.</param>
        public void AddCharacter(Demo_CharacterInfo info, GameObject modelPrefab, int index)
        {
            if (this.m_Slots.Count == 0 || this.m_Slots.Count < (index + 1))
                return;

			if (modelPrefab == null)
				return;

			// Get the slot
			Transform slotTrans = m_Slots[index];

			// Make sure we have a slot transform
			if (slotTrans == null)
				return;

			// Get the character script
			Demo_CharacterSelectSlot csc = slotTrans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

			// Set the character info
			if (csc != null) {
				csc.info = info;
				csc.index = index;
			}

			// Remove any child objects
			foreach (Transform child in slotTrans)
				Destroy(child.gameObject);

			// Add the character model
			GameObject model = Instantiate(modelPrefab);
			model.layer = slotTrans.gameObject.layer;
			model.transform.SetParent(slotTrans, false);
			model.transform.localScale = modelPrefab.transform.localScale;
			model.transform.localPosition = modelPrefab.transform.localPosition;
			model.transform.localRotation = modelPrefab.transform.localRotation;
		}

        /// <summary>
        /// Selects the first available character if any.
        /// </summary>
        public void SelectFirstAvailable()
        {
            if (this.m_Slots.Count == 0)
                return;

			foreach (Transform trans in m_Slots) {
				if (trans == null)
					continue;

				// Get the character script
				Demo_CharacterSelectSlot slot = trans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

				// Select the character
				if (slot != null && slot.info != null) {
					SelectCharacter(slot);
					break;
				}
			}
		}

        /// <summary>
        /// Selects the character slot at the given index.
        /// </summary>
        /// <param name="index"></param>
        public void SelectCharacter(int index)
        {
            if (this.m_Slots.Count == 0)
                return;

			// Get the slot
			Transform slotTrans = m_Slots[index];

			if (slotTrans == null)
				return;

			// Get the character script
			Demo_CharacterSelectSlot slot = slotTrans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

			// Select the character
			if (slot != null) SelectCharacter(slot);
		}

        /// <summary>
        /// Selects the character slot.
        /// </summary>
        /// <param name="slot">The character slot component.</param>
        public void SelectCharacter(Demo_CharacterSelectSlot slot)
        {
            // Check if already selected
            if (this.m_SelectedIndex == slot.index)
                return;
            
            // Deselect
            if (this.m_SelectedIndex > -1)
            {
                // Get the slot
                Transform selectedSlotTrans = this.m_Slots[this.m_SelectedIndex];

				if (selectedSlotTrans != null) {
					// Get the character script
					Demo_CharacterSelectSlot selectedSlot =
						selectedSlotTrans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

					// Deselect
					if (selectedSlot != null) selectedSlot.OnDeselected();
				}
			}

			// Set the selected
			m_SelectedIndex = slot.index;
			m_SelectedTransform = slot.transform;

			if (slot.info != null) {
				if (m_InfoContainer != null) m_InfoContainer.SetActive(true);
				if (m_NameText != null) m_NameText.text = slot.info.name.ToUpper();
				if (m_LevelText != null) m_LevelText.text = slot.info.level.ToString();
				if (m_RaceText != null) m_RaceText.text = slot.info.raceString;
				if (m_ClassText != null) m_ClassText.text = slot.info.classString;

				// Invoke the on character selected event
				if (m_OnCharacterSelected != null)
					m_OnCharacterSelected.Invoke(slot.info);
			} else {
				if (m_InfoContainer != null) m_InfoContainer.SetActive(false);
				if (m_NameText != null) m_NameText.text = "";
				if (m_LevelText != null) m_LevelText.text = "";
				if (m_RaceText != null) m_RaceText.text = "";
				if (m_ClassText != null) m_ClassText.text = "";
			}

			slot.OnSelected();
		}

        /// <summary>
        /// Gets the character in the specified direction (1 or -1).
        /// </summary>
        /// <param name="direction">The direction 1 or -1.</param>
        /// <returns>The character slot.</returns>
        public Demo_CharacterSelectSlot GetCharacterInDirection(float direction)
        {
            if (this.m_Slots.Count == 0)
                return null;

			if (m_SelectedTransform == null && m_Slots[0] != null)
				return m_Slots[0].gameObject.GetComponent<Demo_CharacterSelectSlot>();

			Demo_CharacterSelectSlot closest = null;
			float lastDistance = 0f;

			foreach (Transform trans in m_Slots) {
				// Skip the selected one
				if (trans.Equals(m_SelectedTransform))
					continue;

				float curDirection = trans.position.x - m_SelectedTransform.position.x;

				// Check direction
				if (direction > 0f && curDirection > 0f || direction < 0f && curDirection < 0f) {
					// Get the character component
					Demo_CharacterSelectSlot slot = trans.GetComponent<Demo_CharacterSelectSlot>();

					// Make sure we have slot component
					if (slot == null)
						continue;

					// Make sure it's populated
					if (slot.info == null)
						continue;

					// If we have no closest assigned yet
					if (closest == null) {
						closest = slot;
						lastDistance = Vector3.Distance(m_SelectedTransform.position, trans.position);
						continue;
					}

					// Comapre distance
					if (Vector3.Distance(m_SelectedTransform.position, trans.position) <= lastDistance) {
						closest = slot;
						lastDistance = Vector3.Distance(m_SelectedTransform.position, trans.position);
					}
				}
			}

			return closest;
		}

        /// <summary>
        /// Selects the next character slot.
        /// </summary>
        public void SelectNext()
        {
            Demo_CharacterSelectSlot next = this.GetCharacterInDirection(1f);

			if (next != null)
				SelectCharacter(next);
		}

        /// <summary>
        /// Selects the previous character slot.
        /// </summary>
        public void SelectPrevious()
        {
            Demo_CharacterSelectSlot prev = this.GetCharacterInDirection(-1f);

			if (prev != null)
				SelectCharacter(prev);
		}

        /// <summary>
        /// Remove the character at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveCharacter(int index)
        {
            if (this.m_Slots.Count == 0)
                return;

			// Get the slot
			Transform slotTrans = m_Slots[index];

			if (slotTrans == null)
				return;

			// Get the character script
			Demo_CharacterSelectSlot slot = slotTrans.gameObject.GetComponent<Demo_CharacterSelectSlot>();

			// Invoke the on character delete event
			if (m_OnCharacterDelete != null && slot.info != null)
				m_OnCharacterDelete.Invoke(slot.info);

			// Unset the character info
			if (slot != null) slot.info = null;

			// Deselect
			if (slot != null) slot.OnDeselected();

			// Remove the child objects
			foreach (Transform child in slotTrans)
				Destroy(child.gameObject);

			// Unset the character info texts
			if (index == m_SelectedIndex) {
				if (m_InfoContainer != null) m_InfoContainer.SetActive(false);
				if (m_NameText != null) m_NameText.text = "";
				if (m_LevelText != null) m_LevelText.text = "";
				if (m_RaceText != null) m_RaceText.text = "";
				if (m_ClassText != null) m_ClassText.text = "";

				SelectFirstAvailable();
			}
		}

        /// <summary>
        /// Deletes the selected character.
        /// </summary>
        public void DeleteSelected()
        {
            if (this.m_SelectedIndex > -1)
            {
                this.RemoveCharacter(this.m_SelectedIndex);
            }
        }
        
        public void OnPlayClick()
        {
            UILoadingOverlay loadingOverlay = UILoadingOverlayManager.Instance.Create();

            if (loadingOverlay != null)
                loadingOverlay.LoadScene(this.m_IngameSceneId);
        }
    }
}
