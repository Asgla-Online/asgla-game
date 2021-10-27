using System.Collections;
using System.Collections.Generic;
using Asgla.Data.Skill;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Icon Slots/Slot Cooldown", 28)]
	public class UISlotCooldown : MonoBehaviour {

		private static readonly Dictionary<int, CooldownInfo> skillCooldowns = new Dictionary<int, CooldownInfo>();

		private IUISlotHasCooldown m_CooldownSlot;
		private int m_CurrentSkillId;

		/// <summary>
		///     Gets a value indicating whether the cooldown is active.
		/// </summary>
		/// <value><c>true</c> if this instance is on cooldown; otherwise, <c>false</c>.</value>
		public bool IsOnCooldown { get; private set; }

		protected void Awake() {
			// Set the cooldown component variable on the target slot
			if (m_TargetSlot != null && m_TargetSlot is IUISlotHasCooldown) {
				m_CooldownSlot = m_TargetSlot as IUISlotHasCooldown;
				m_CooldownSlot.SetCooldownComponent(this);
			} else {
				Debug.LogWarning(
					"The slot cooldown script cannot operate without a target slot with a IUISlotHasCooldown interface, disabling script.");
				enabled = false;
			}
		}

		protected void Start() {
			// Disable the image and text if they are shown by any chance
			if (m_TargetGraphic != null) m_TargetGraphic.enabled = false;
			if (m_TargetText != null) m_TargetText.enabled = false;

			// Prepare the finish
			if (m_FinishGraphic != null) {
				m_FinishGraphic.enabled = false;
				m_FinishGraphic.rectTransform.anchorMin = new Vector2(m_FinishGraphic.rectTransform.anchorMin.x, 1f);
				m_FinishGraphic.rectTransform.anchorMax = new Vector2(m_FinishGraphic.rectTransform.anchorMax.x, 1f);
			}
		}

		protected void OnEnable() {
			// Check for active cooldown
			CheckForActiveCooldown();
		}

		protected void OnDisable() {
			// Disable any current cooldown display
			InterruptCooldown();
		}

		public Dictionary<int, CooldownInfo> Cooldowns() {
			return skillCooldowns;
		}

		/// <summary>
		///     Raises the assign spell event.
		/// </summary>
		public void OnAssignSpell() {
			CheckForActiveCooldown();
		}

		/// <summary>
		///     Raises the unassign event.
		/// </summary>
		public void OnUnassignSpell() {
			InterruptCooldown();
		}

		/// <summary>
		///     Checks if the target slot has an active cooldown and resumes it.
		/// </summary>
		public void CheckForActiveCooldown() {
			// Return if the slot is not valid
			if (m_CooldownSlot == null)
				return;

			// Get the spell info
			SkillData spellInfo = m_CooldownSlot.GetSkillData();

			if (spellInfo == null)
				return;

			// Check if this spell still has cooldown
			if (skillCooldowns.ContainsKey(spellInfo.DatabaseID)) {
				float cooldownTill = skillCooldowns[spellInfo.DatabaseID].endTime;

				// Check if the cooldown isnt expired
				if (cooldownTill > Time.time) {
					IsOnCooldown = true;

					// Resume the cooldown
					ResumeCooldown(spellInfo.DatabaseID);
				} else {
					// Cooldown already expired, remove the record
					skillCooldowns.Remove(spellInfo.DatabaseID);
				}
			}
		}

		/// <summary>
		///     Starts a cooldown.
		/// </summary>
		/// <param name="skillId">Spell identifier.</param>
		/// <param name="duration">Duration.</param>
		public void StartCooldown(int skillId, float duration) {
			if (!enabled || !gameObject.activeInHierarchy || m_TargetGraphic == null)
				return;

			// Save the spell id
			m_CurrentSkillId = skillId;

			// Enable the image if it's disabled
			if (!m_TargetGraphic.enabled)
				m_TargetGraphic.enabled = true;

			// Reset the fill amount
			m_TargetGraphic.fillAmount = 1f;

			// Enable the text if it's disabled
			if (m_TargetText != null) {
				if (!m_TargetText.enabled)
					m_TargetText.enabled = true;

				m_TargetText.text = duration.ToString("0");
			}

			// Prepare the finish graphic
			if (m_FinishGraphic != null) {
				m_FinishGraphic.canvasRenderer.SetAlpha(0f);
				m_FinishGraphic.enabled = true;
				m_FinishGraphic.rectTransform.anchoredPosition = new Vector2(
					m_FinishGraphic.rectTransform.anchoredPosition.x,
					m_FinishOffsetY
				);
			}

			// Set the slot on cooldown
			IsOnCooldown = true;
		}

		/// <summary>
		///     Resumes a cooldown.
		/// </summary>
		/// <param name="spellId">Spell identifier.</param>
		public void ResumeCooldown(int spellId) {
			if (!enabled || !gameObject.activeInHierarchy || m_TargetGraphic == null)
				return;

			// Check if we have the cooldown info for that spell
			if (!skillCooldowns.ContainsKey(spellId))
				return;

			// Get the cooldown info
			CooldownInfo cooldownInfo = skillCooldowns[spellId];

			// Get the remaining time
			float remainingTime = cooldownInfo.endTime - Time.time;
			float remainingTimePct = remainingTime / cooldownInfo.duration;

			// Save the spell id
			m_CurrentSkillId = spellId;

			// Enable the image if it's disabled
			if (!m_TargetGraphic.enabled)
				m_TargetGraphic.enabled = true;

			// Set the fill amount to the remaing percents
			m_TargetGraphic.fillAmount = remainingTime / cooldownInfo.duration;

			// Enable the text if it's disabled
			if (m_TargetText != null) {
				if (!m_TargetText.enabled)
					m_TargetText.enabled = true;

				m_TargetText.text = remainingTime.ToString("0");
			}

			// Update the finish
			if (m_FinishGraphic != null) {
				m_FinishGraphic.enabled = true;
				UpdateFinishPosition(remainingTimePct);
			}

			// Start the coroutine
			StartCoroutine("_StartCooldown", cooldownInfo);
		}

		/// <summary>
		///     Interrupts the current cooldown.
		/// </summary>
		public void InterruptCooldown() {
			// Cancel the coroutine
			StopCoroutine("_StartCooldown");

			// Call the finish
			OnCooldownFinished();
		}

		public void StartCooldownCoroutine(CooldownInfo info) {
			StartCoroutine("_StartCooldown", info);
		}

		private IEnumerator _StartCooldown(CooldownInfo cooldownInfo) {
			while (Time.time < cooldownInfo.startTime + cooldownInfo.duration) {
				float RemainingTime = cooldownInfo.startTime + cooldownInfo.duration - Time.time;
				float RemainingTimePct = RemainingTime / cooldownInfo.duration;

				// Update the cooldown image
				if (m_TargetGraphic != null)
					m_TargetGraphic.fillAmount = RemainingTimePct;

				// Update the text
				if (m_TargetText != null)
					m_TargetText.text = RemainingTime.ToString("0");

				// Update the finish position
				UpdateFinishPosition(RemainingTimePct);

				yield return 0;
			}

			// Call the on finish
			OnCooldownCompleted();
		}

		/// <summary>
		///     Raised when the cooldown completes it's full duration.
		/// </summary>
		private void OnCooldownCompleted() {
			// Remove from the cooldowns list
			if (skillCooldowns.ContainsKey(m_CurrentSkillId))
				skillCooldowns.Remove(m_CurrentSkillId);

			// Fire up the cooldown finished
			OnCooldownFinished();
		}

		/// <summary>
		///     Raised when the cooldown finishes or has been interrupted.
		/// </summary>
		private void OnCooldownFinished() {
			// No longer on cooldown
			IsOnCooldown = false;
			m_CurrentSkillId = 0;

			// Disable the image
			if (m_TargetGraphic != null)
				m_TargetGraphic.enabled = false;

			// Disable the text
			if (m_TargetText != null)
				m_TargetText.enabled = false;

			// Disable the finish sprite
			if (m_FinishGraphic != null)
				m_FinishGraphic.enabled = false;
		}

		/// <summary>
		///     Updates the finish position based on the remaining time percentage of the cooldown.
		/// </summary>
		/// <param name="RemainingTimePct">Remaining time pct.</param>
		protected void UpdateFinishPosition(float RemainingTimePct) {
			// Update the finish position
			if (m_FinishGraphic != null && m_TargetGraphic != null) {
				// Calculate the fill position
				float newY = 0f - m_TargetGraphic.rectTransform.rect.height +
				             m_TargetGraphic.rectTransform.rect.height * RemainingTimePct;

				// Add the offset
				newY += m_FinishOffsetY;

				// Update the finish position
				m_FinishGraphic.rectTransform.anchoredPosition = new Vector2(
					m_FinishGraphic.rectTransform.anchoredPosition.x,
					newY
				);

				float fadingPct = m_FinishFadingPct / 100;

				// Manage finish fading
				if (RemainingTimePct <= fadingPct)
					m_FinishGraphic.canvasRenderer.SetAlpha(RemainingTimePct / fadingPct);
				else if (RemainingTimePct >= 1f - fadingPct)
					m_FinishGraphic.canvasRenderer.SetAlpha(1f - (RemainingTimePct - (1f - fadingPct)) / fadingPct);
				else if (RemainingTimePct > fadingPct && RemainingTimePct < 1f - fadingPct)
					m_FinishGraphic.canvasRenderer.SetAlpha(1f);
			}
		}

		public class CooldownInfo {

			public float duration;
			public float endTime;
			public float startTime;

			public CooldownInfo(float duration, float startTime, float endTime) {
				this.duration = duration;
				this.startTime = startTime;
				this.endTime = endTime;
			}

		}

#pragma warning disable 0649
		[SerializeField] private UISlotBase m_TargetSlot;
		[SerializeField] private Image m_TargetGraphic;
		[SerializeField] private Text m_TargetText;
		[SerializeField] private Image m_FinishGraphic;
		[SerializeField] private float m_FinishOffsetY;
		[SerializeField] private float m_FinishFadingPct = 25f;
#pragma warning restore 0649

	}
}