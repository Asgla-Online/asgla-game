using System;
using System.Collections.Generic;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Press Transition")]
	public class UIPressTransition : MonoBehaviour, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler {

		public enum Transition {

			None,
			ColorTint,
			SpriteSwap,
			Animation,
			TextColor

		}

		public enum VisualState {

			Normal,
			Pressed

		}

		private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();

		// Tween controls
		[NonSerialized] private readonly TweenRunner<ColorTween> m_ColorTweenRunner;

		private bool m_GroupsAllowInteraction = true;

		private Selectable m_Selectable;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UIPressTransition() {
			if (m_ColorTweenRunner == null)
				m_ColorTweenRunner = new TweenRunner<ColorTween>();

			m_ColorTweenRunner.Init(this);
		}

		/// <summary>
		///     Gets or sets the transition type.
		/// </summary>
		/// <value>The transition.</value>
		public Transition transition {
			get => m_Transition;
			set => m_Transition = value;
		}

		/// <summary>
		///     Gets or sets the target graphic.
		/// </summary>
		/// <value>The target graphic.</value>
		public Graphic targetGraphic {
			get => m_TargetGraphic;
			set => m_TargetGraphic = value;
		}

		/// <summary>
		///     Gets or sets the target game object.
		/// </summary>
		/// <value>The target game object.</value>
		public GameObject targetGameObject {
			get => m_TargetGameObject;
			set => m_TargetGameObject = value;
		}

		/// <summary>
		///     Gets the animator.
		/// </summary>
		/// <value>The animator.</value>
		public Animator animator {
			get{
				if (m_TargetGameObject != null)
					return m_TargetGameObject.GetComponent<Animator>();

				// Default
				return null;
			}
		}

		protected void Awake() {
			m_Selectable = gameObject.GetComponent<Selectable>();
		}

		protected void OnEnable() {
			InternalEvaluateAndTransitionToNormalState(true);
		}

		protected void OnDisable() {
			InstantClearState();
		}

		protected void OnCanvasGroupChanged() {
			// Figure out if parent groups allow interaction
			// If no interaction is alowed... then we need
			// to not do that :)
			bool groupAllowInteraction = true;
			Transform t = transform;
			while (t != null) {
				t.GetComponents(m_CanvasGroupCache);
				bool shouldBreak = false;
				for (int i = 0; i < m_CanvasGroupCache.Count; i++) {
					// if the parent group does not allow interaction
					// we need to break
					if (!m_CanvasGroupCache[i].interactable) {
						groupAllowInteraction = false;
						shouldBreak = true;
					}

					// if this is a 'fresh' group, then break
					// as we should not consider parents
					if (m_CanvasGroupCache[i].ignoreParentGroups)
						shouldBreak = true;
				}

				if (shouldBreak)
					break;

				t = t.parent;
			}

			if (groupAllowInteraction != m_GroupsAllowInteraction) {
				m_GroupsAllowInteraction = groupAllowInteraction;
				InternalEvaluateAndTransitionToNormalState(true);
			}
		}

#if UNITY_EDITOR
		protected void OnValidate() {
			m_Duration = Mathf.Max(m_Duration, 0f);

			if (isActiveAndEnabled) {
				DoSpriteSwap(null);
				InternalEvaluateAndTransitionToNormalState(true);
			}
		}
#endif

		/// <summary>
		///     Raises the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData) {
			DoStateTransition(VisualState.Pressed, false);
		}

		/// <summary>
		///     Raises the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData) {
			DoStateTransition(VisualState.Normal, false);
		}

		public virtual bool IsInteractable() {
			if (m_Selectable != null)
				return m_Selectable.IsInteractable() && m_GroupsAllowInteraction;

			return m_GroupsAllowInteraction;
		}

		/// <summary>
		///     Instantly clears the visual state.
		/// </summary>
		protected void InstantClearState() {
			switch (m_Transition) {
				case Transition.ColorTint:
					StartColorTween(Color.white, true);
					break;
				case Transition.SpriteSwap:
					DoSpriteSwap(null);
					break;
				case Transition.TextColor:
					SetTextColor(Color.white);
					break;
			}
		}

		/// <summary>
		///     Internally evaluates and transitions to normal state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionToNormalState(bool instant) {
			DoStateTransition(VisualState.Normal, instant);
		}

		/// <summary>
		///     Does the state transition.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void DoStateTransition(VisualState state, bool instant) {
			// Check if the script is enabled
			if (!gameObject.activeInHierarchy)
				return;

			// Check if it's interactable
			if (!IsInteractable())
				state = VisualState.Normal;

			Color color = m_NormalColor;
			Sprite newSprite = null;
			string triggername = m_NormalTrigger;

			// Prepare the transition values
			switch (state) {
				case VisualState.Normal:
					color = m_NormalColor;
					newSprite = null;
					triggername = m_NormalTrigger;
					break;
				case VisualState.Pressed:
					color = m_PressedColor;
					newSprite = m_PressedSprite;
					triggername = m_PressedTrigger;
					break;
			}

			// Do the transition
			switch (m_Transition) {
				case Transition.ColorTint:
					StartColorTween(color * m_ColorMultiplier, instant);
					break;
				case Transition.SpriteSwap:
					DoSpriteSwap(newSprite);
					break;
				case Transition.Animation:
					TriggerAnimation(triggername);
					break;
				case Transition.TextColor:
					StartTextColorTween(color, false);
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

			if (instant || m_Duration == 0f || !Application.isPlaying)
				m_TargetGraphic.canvasRenderer.SetColor(targetColor);
			else
				m_TargetGraphic.CrossFadeColor(targetColor, m_Duration, true, true);
		}

		private void DoSpriteSwap(Sprite newSprite) {
			Image image = m_TargetGraphic as Image;

			if (image == null)
				return;

			image.overrideSprite = newSprite;
		}

		private void TriggerAnimation(string triggername) {
			if (targetGameObject == null)
				return;

			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(triggername))
				return;

			animator.ResetTrigger(m_PressedTrigger);
			animator.SetTrigger(triggername);
		}

		private void StartTextColorTween(Color targetColor, bool instant) {
			if (m_TargetGraphic == null)
				return;

			if (m_TargetGraphic is Text == false)
				return;

			if (instant || m_Duration == 0f || !Application.isPlaying) {
				(m_TargetGraphic as Text).color = targetColor;
			} else {
				ColorTween colorTween = new ColorTween
					{duration = m_Duration, startColor = (m_TargetGraphic as Text).color, targetColor = targetColor};
				colorTween.AddOnChangedCallback(SetTextColor);
				colorTween.ignoreTimeScale = true;

				m_ColorTweenRunner.StartTween(colorTween);
			}
		}

		/// <summary>
		///     Sets the text color.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		private void SetTextColor(Color targetColor) {
			if (m_TargetGraphic == null)
				return;

			if (m_TargetGraphic is Text)
				(m_TargetGraphic as Text).color = targetColor;
		}

#pragma warning disable 0649
		[SerializeField] private Transition m_Transition = Transition.None;

		[SerializeField] private Color m_NormalColor = ColorBlock.defaultColorBlock.normalColor;
		[SerializeField] private Color m_PressedColor = ColorBlock.defaultColorBlock.pressedColor;
		[SerializeField] private float m_Duration = 0.1f;

		[SerializeField] [Range(1f, 6f)] private float m_ColorMultiplier = 1f;

		[SerializeField] private Sprite m_PressedSprite;

		[SerializeField] private string m_NormalTrigger = "Normal";
		[SerializeField] private string m_PressedTrigger = "Pressed";

		[SerializeField] [Tooltip("Graphic that will have the selected transtion applied.")]
		private Graphic m_TargetGraphic;

		[SerializeField] [Tooltip("GameObject that will have the selected transtion applied.")]
		private GameObject m_TargetGameObject;
#pragma warning restore 0649

	}
}