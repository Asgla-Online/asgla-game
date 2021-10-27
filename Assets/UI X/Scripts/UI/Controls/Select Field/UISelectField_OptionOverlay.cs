using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	public class UISelectField_OptionOverlay : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler,
		IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

		public enum Transition {

			None,
			ColorTint

		}

		public enum VisualState {

			Normal,
			Highlighted,
			Selected,
			Pressed

		}

		[SerializeField] private Transition m_Transition = Transition.None;
		[SerializeField] private ColorBlock m_ColorBlock = ColorBlock.defaultColorBlock;

		[SerializeField] [Tooltip("Graphic that will have the selected transtion applied.")]
		private Graphic m_TargetGraphic;

		private readonly bool m_Selected = false;

		private bool m_Highlighted;
		private bool m_Pressed;

		/// <summary>
		///     Gets or sets the transition type.
		/// </summary>
		/// <value>The transition.</value>
		public Transition transition {
			get => m_Transition;
			set => m_Transition = value;
		}

		/// <summary>
		///     Gets or set the color block.
		/// </summary>
		public ColorBlock colorBlock {
			get => m_ColorBlock;
			set => m_ColorBlock = value;
		}

		/// <summary>
		///     Gets or sets the target graphic.
		/// </summary>
		/// <value>The target graphic.</value>
		public Graphic targetGraphic {
			get => m_TargetGraphic;
			set => m_TargetGraphic = value;
		}

		protected void OnEnable() {
			InternalEvaluateAndTransitionToNormalState(true);
		}

		protected void OnDisable() {
			InstantClearState();
		}

		public virtual void OnPointerDown(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (!m_Highlighted)
				return;

			m_Pressed = true;
			DoStateTransition(VisualState.Pressed, false);
		}

		public void OnPointerEnter(PointerEventData eventData) {
			m_Highlighted = true;

			if (!m_Selected && !m_Pressed)
				DoStateTransition(VisualState.Highlighted, false);
		}

		public void OnPointerExit(PointerEventData eventData) {
			m_Highlighted = false;

			if (!m_Selected && !m_Pressed)
				DoStateTransition(VisualState.Normal, false);
		}

		public virtual void OnPointerUp(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			m_Pressed = false;

			VisualState newState = VisualState.Normal;

			if (m_Selected)
				newState = VisualState.Selected;
			else if (m_Highlighted)
				newState = VisualState.Highlighted;

			DoStateTransition(newState, false);
		}

		/// <summary>
		///     Instantly clears the visual state.
		/// </summary>
		protected void InstantClearState() {
			switch (m_Transition) {
				case Transition.ColorTint:
					StartColorTween(Color.white, true);
					break;
			}
		}

		/// <summary>
		///     Internally evaluates and transitions to normal state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		public void InternalEvaluateAndTransitionToNormalState(bool instant) {
			DoStateTransition(VisualState.Normal, instant);
		}

		/// <summary>
		///     Does the state transition.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void DoStateTransition(VisualState state, bool instant) {
			// Check if the script is enabled
			if (!enabled || !gameObject.activeInHierarchy)
				return;

			Color color = m_ColorBlock.normalColor;

			// Prepare the transition values
			switch (state) {
				case VisualState.Normal:
					color = m_ColorBlock.normalColor;
					break;
				case VisualState.Highlighted:
					color = m_ColorBlock.highlightedColor;
					break;
				case VisualState.Pressed:
					color = m_ColorBlock.pressedColor;
					break;
			}

			// Do the transition
			switch (m_Transition) {
				case Transition.ColorTint:
					StartColorTween(color * m_ColorBlock.colorMultiplier, instant);
					break;
			}
		}

		/// <summary>
		///     Starts the color tween.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void StartColorTween(Color targetColor, bool instant) {
			if (m_TargetGraphic == null)
				return;

			if (instant || m_ColorBlock.fadeDuration == 0f || !Application.isPlaying)
				m_TargetGraphic.canvasRenderer.SetColor(targetColor);
			else
				m_TargetGraphic.CrossFadeColor(targetColor, m_ColorBlock.fadeDuration, true, true);
		}

	}
}