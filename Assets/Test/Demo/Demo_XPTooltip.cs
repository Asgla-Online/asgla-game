using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class Demo_XPTooltip : UIBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler {

		[SerializeField] [Tooltip("How long of a delay to expect before showing the tooltip.")] [Range(0f, 10f)]
		private float m_Delay = 1f;

		private bool m_IsTooltipShown;

		protected override void Awake() {
			base.Awake();

			if (m_TooltipObject != null) {
				RectTransform tooltipRect = m_TooltipObject.transform as RectTransform;
				tooltipRect.anchorMin = new Vector2(0f, 1f);
				tooltipRect.anchorMax = new Vector2(0f, 1f);
				tooltipRect.pivot = new Vector2(0.5f, 0f);
				m_TooltipObject.SetActive(false);
			}
		}

		protected override void OnEnable() {
			base.OnEnable();

			if (m_ProgressBar != null)
				m_ProgressBar.onChange.AddListener(OnProgressChange);
		}

		protected override void OnDisable() {
			base.OnDisable();

			if (m_ProgressBar != null)
				m_ProgressBar.onChange.RemoveListener(OnProgressChange);
		}

        /// <summary>
        /// Raises the pointer enter event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerEnter(PointerEventData eventData) {
            // Check if tooltip is enabled
            if (this.enabled && this.IsActive()) {
                // Start the tooltip delayed show coroutine
                // If delay is set at all
                if (this.m_Delay > 0f) {
                    this.StartCoroutine("DelayedShow");
                } else {
                    this.InternalShowTooltip();
                }
            }
        }

        /// <summary>
        /// Raises the pointer exit event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerExit(PointerEventData eventData) {
            this.InternalHideTooltip();
        }

        /// <summary>
		/// Internal call for show tooltip.
		/// </summary>
		protected void InternalShowTooltip() {
            // Call the on tooltip only if it's currently not shown
            if (!this.m_IsTooltipShown) {
                this.m_IsTooltipShown = true;
                this.OnTooltip(true);
            }
        }

        /// <summary>
        /// Internal call for hide tooltip.
        /// </summary>
        protected void InternalHideTooltip() {
            // Cancel the delayed show coroutine
            this.StopCoroutine("DelayedShow");

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

		public void UpdatePosition() {
			if (m_ProgressBar == null || m_TooltipObject == null)
				return;

			RectTransform tooltipRect = m_TooltipObject.transform as RectTransform;
			RectTransform fillRect = m_ProgressBar.type == UIProgressBar.Type.Filled
				? m_ProgressBar.targetImage.transform as RectTransform
				: m_ProgressBar.targetTransform.parent as RectTransform;

			Transform parent = tooltipRect.parent;

			// Change the parent so we can calculate the position correctly
			tooltipRect.SetParent(fillRect, true);

			// Change the position based on fill
			tooltipRect.anchoredPosition = new Vector2(fillRect.rect.width * m_ProgressBar.fillAmount, m_OffsetY);

			// Bring to top
			tooltipRect.SetParent(parent, true);

			// Set the percent text
			if (m_PercentText != null)
				m_PercentText.text = (m_ProgressBar.fillAmount * 100f).ToString("0") + "%";
		}

#pragma warning disable 0649
		[SerializeField] private GameObject m_TooltipObject;
		[SerializeField] private UIProgressBar m_ProgressBar;
		[SerializeField] private Text m_PercentText;
		[SerializeField] private float m_OffsetY;
#pragma warning restore 0649

	}
}