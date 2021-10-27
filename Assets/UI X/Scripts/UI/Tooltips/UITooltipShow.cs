using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Tooltip Show", 58)]
	[DisallowMultipleComponent]
	public class UITooltipShow : UIBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler,
		IPointerDownHandler, IPointerUpHandler {

		public enum Position {

			Floating,
			Anchored

		}

		public enum WidthMode {

			Default,
			Preferred

		}

		[SerializeField] private Position m_Position = Position.Floating;
		[SerializeField] private WidthMode m_WidthMode = WidthMode.Default;
		[SerializeField] private bool m_OverrideOffset;
		[SerializeField] private Vector2 m_Offset = Vector2.zero;

		[SerializeField] [Tooltip("How long of a delay to expect before showing the tooltip.")] [Range(0f, 10f)]
		private float m_Delay = 1f;

		[SerializeField] private UITooltipLineContent[] m_ContentLines = new UITooltipLineContent[0];

		private bool m_IsTooltipShown;

		/// <summary>
		///     Gets or sets the tooltip content lines.
		/// </summary>
		public UITooltipLineContent[] contentLines {
			get => m_ContentLines;
			set => m_ContentLines = value;
		}

		/// <summary>
		///     Raises the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData) {
			// Hide the tooltip
			InternalHideTooltip();
		}

		/// <summary>
		///     Raises the pointer enter event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerEnter(PointerEventData eventData) {
			// Check if tooltip is enabled
			if (enabled && IsActive()) {
				// Instantiate the tooltip now
				UITooltip.InstantiateIfNecessary(gameObject);

				// Start the tooltip delayed show coroutine
				// If delay is set at all
				if (m_Delay > 0f)
					StartCoroutine("DelayedShow");
				else
					InternalShowTooltip();
			}
		}

		/// <summary>
		///     Raises the pointer exit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerExit(PointerEventData eventData) {
			InternalHideTooltip();
		}

		/// <summary>
		///     Raises the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData) {
		}

		/// <summary>
		///     Raises the tooltip event.
		/// </summary>
		/// <param name="show">If set to <c>true</c> show.</param>
		public virtual void OnTooltip(bool show) {
			if (!enabled || !IsActive())
				return;

			// If we are showing the tooltip
			if (show) {
				UITooltip.InstantiateIfNecessary(gameObject);

				for (int i = 0; i < m_ContentLines.Length; i++) {
					UITooltipLineContent line = m_ContentLines[i];

					if (line.IsSpacer) {
						UITooltip.AddSpacer();
					} else {
						if (line.LineStyle != UITooltipLines.LineStyle.Custom)
							UITooltip.AddLine(line.Content, line.LineStyle);
						else
							UITooltip.AddLine(line.Content, line.CustomLineStyle);
					}
				}

				if (m_WidthMode == WidthMode.Preferred)
					UITooltip.SetHorizontalFitMode(ContentSizeFitter.FitMode.PreferredSize);

				// Anchor to this slot
				if (m_Position == Position.Anchored)
					UITooltip.AnchorToRect(transform as RectTransform);

				// Handle offset override
				if (m_OverrideOffset) {
					UITooltip.OverrideOffset(m_Offset);
					UITooltip.OverrideAnchoredOffset(m_Offset);
				}

				// Show the tooltip
				UITooltip.Show();
			} else {
				// Hide the tooltip
				UITooltip.Hide();
			}
		}

		/// <summary>
		///     Internal call for show tooltip.
		/// </summary>
		protected void InternalShowTooltip() {
			// Call the on tooltip only if it's currently not shown
			if (!m_IsTooltipShown) {
				m_IsTooltipShown = true;
				OnTooltip(true);
			}
		}

		/// <summary>
		///     Internal call for hide tooltip.
		/// </summary>
		protected void InternalHideTooltip() {
			// Cancel the delayed show coroutine
			StopCoroutine("DelayedShow");

			// Call the on tooltip only if it's currently shown
			if (m_IsTooltipShown) {
				m_IsTooltipShown = false;
				OnTooltip(false);
			}
		}

		protected IEnumerator DelayedShow() {
			yield return new WaitForSeconds(m_Delay);
			InternalShowTooltip();
		}

	}
}