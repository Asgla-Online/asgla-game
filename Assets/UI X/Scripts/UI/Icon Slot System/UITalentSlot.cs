using Asgla.Data.Skill;
using Asgla.Skill;
using Asgla.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Icon Slots/Talent Slot", 12)]
	public class UITalentSlot : UISlotBase {

		private int m_CurrentPoints;
		private SkillData m_SpellInfo;

		private UITalentInfo m_TalentInfo;

		protected override void Start() {
			base.Start();

			// Disable Drag and Drop
			dragAndDropEnabled = false;
		}

		/// <summary>
		///     Gets the spell info of the spell assigned to this slot.
		/// </summary>
		/// <returns>The spell info.</returns>
		public SkillData GetSkillData() {
			return m_SpellInfo;
		}

		/// <summary>
		///     Gets the talent info of the talent assigned to this slot.
		/// </summary>
		/// <returns>The talent info.</returns>
		public UITalentInfo GetTalentInfo() {
			return m_TalentInfo;
		}

		/// <summary>
		///     Determines whether this slot is assigned.
		/// </summary>
		/// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
		public override bool IsAssigned() {
			return m_TalentInfo != null;
		}

		/// <summary>
		///     Assign the specified slot by talentInfo and spellInfo.
		/// </summary>
		/// <param name="talentInfo">Talent info.</param>
		/// <param name="spellInfo">Spell info.</param>
		public bool Assign(UITalentInfo talentInfo, SkillData spellInfo) {
			if (talentInfo == null || spellInfo == null)
				return false;

			// Use the base class to assign the icon
			Assign(spellInfo.GetIcon);

			// Set the talent info
			m_TalentInfo = talentInfo;

			// Set the spell info
			m_SpellInfo = spellInfo;

			// Update the points label
			UpdatePointsLabel();

			// Return success
			return true;
		}

		/// <summary>
		///     Updates the points label.
		/// </summary>
		public void UpdatePointsLabel() {
			if (m_PointsText == null)
				return;

			// Set the points string on the label
			m_PointsText.text = "";

			// No points assigned
			if (m_CurrentPoints == 0) {
				m_PointsText.text += "<color=#" + CommonColorBuffer.ColorToString(m_pointsMinColor) + ">" +
				                     m_CurrentPoints + "</color>";
				m_PointsText.text += "<color=#" + CommonColorBuffer.ColorToString(m_pointsMaxColor) + ">/" +
				                     m_TalentInfo.maxPoints + "</color>";
			}
			// Assigned but not maxed
			else if (m_CurrentPoints > 0 && m_CurrentPoints < m_TalentInfo.maxPoints) {
				m_PointsText.text += "<color=#" + CommonColorBuffer.ColorToString(m_pointsMinColor) + ">" +
				                     m_CurrentPoints + "</color>";
				m_PointsText.text += "<color=#" + CommonColorBuffer.ColorToString(m_pointsMaxColor) + ">/" +
				                     m_TalentInfo.maxPoints + "</color>";
			}
			// Maxed
			else {
				m_PointsText.text += "<color=#" + CommonColorBuffer.ColorToString(m_pointsActiveColor) + ">" +
				                     m_CurrentPoints + "/" +
				                     m_TalentInfo.maxPoints + "</color>";
			}
		}

		/// <summary>
		///     Unassign this slot.
		/// </summary>
		public override void Unassign() {
			// Remove the icon
			base.Unassign();

			// Clear the talent info
			m_TalentInfo = null;

			// Clear the spell info
			m_SpellInfo = null;
		}

		/// <summary>
		///     Raises the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnPointerClick(PointerEventData eventData) {
			if (!IsAssigned())
				return;

			// Redirect right click
			if (eventData.button == PointerEventData.InputButton.Right) {
				OnRightPointerClick(eventData);
				return;
			}

			// Check if the talent is maxed
			if (m_CurrentPoints >= m_TalentInfo.maxPoints)
				return;

			// Increase the points
			m_CurrentPoints = m_CurrentPoints + 1;

			// Update the label string
			UpdatePointsLabel();
		}

		/// <summary>
		///     Raises the right click event.
		/// </summary>
		public virtual void OnRightPointerClick(PointerEventData eventData) {
			// Check if the talent is at it's base
			if (m_CurrentPoints == 0)
				return;

			// Increase the points
			m_CurrentPoints = m_CurrentPoints - 1;

			// Update the label string
			UpdatePointsLabel();
		}

		/// <summary>
		///     Adds points.
		/// </summary>
		/// <param name="points">Points.</param>
		public void AddPoints(int points) {
			if (!IsAssigned() || points == 0)
				return;

			// Add the points
			m_CurrentPoints = m_CurrentPoints + points;

			// Make sure we dont exceed the limites
			if (m_CurrentPoints < 0)
				m_CurrentPoints = 0;

			if (m_CurrentPoints > m_TalentInfo.maxPoints)
				m_CurrentPoints = m_TalentInfo.maxPoints;

			// Update the label string
			UpdatePointsLabel();
		}

		/// <summary>
		///     Raises the tooltip event.
		/// </summary>
		/// <param name="show">If set to <c>true</c> show.</param>
		public override void OnTooltip(bool show) {
			// Make sure we have spell info, otherwise game might crash
			if (m_SpellInfo == null)
				return;

			// If we are showing the tooltip
			if (show) {
				UITooltip.InstantiateIfNecessary(gameObject);

				// Prepare the tooltip lines
				SkillMain.PrepareTooltip(m_SpellInfo);

				// Anchor to this slot
				UITooltip.AnchorToRect(transform as RectTransform);

				// Show the tooltip
				UITooltip.Show();
			} else {
				// Hide the tooltip
				UITooltip.Hide();
			}
		}
#pragma warning disable 0649
		[SerializeField] private Text m_PointsText;
		[SerializeField] private Color m_pointsMinColor = Color.white;
		[SerializeField] private Color m_pointsMaxColor = Color.white;
		[SerializeField] private Color m_pointsActiveColor = Color.white;
#pragma warning restore 0649

	}
}