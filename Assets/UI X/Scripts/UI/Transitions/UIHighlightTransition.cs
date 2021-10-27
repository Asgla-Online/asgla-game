using System;
using System.Collections.Generic;
using AsglaUI.UI.Tweens;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {

	[ExecuteInEditMode]
	[AddComponentMenu("UI/Highlight Transition")]
	public class UIHighlightTransition : MonoBehaviour, IEventSystemHandler, ISelectHandler, IDeselectHandler,
		IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

		public enum Transition {

			None,
			ColorTint,
			SpriteSwap,
			Animation,
			TextColor,
			CanvasGroup

		}

		public enum VisualState {

			Normal,
			Highlighted,
			Selected,
			Pressed,
			Active

		}

		private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();

		// Tween controls
		[NonSerialized] private readonly TweenRunner<ColorTween> m_ColorTweenRunner;

		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		private bool m_Active;
		private bool m_GroupsAllowInteraction = true;

		private bool m_Highlighted;
		private bool m_Pressed;

		private Selectable m_Selectable;
		private bool m_Selected;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UIHighlightTransition() {
			if (m_ColorTweenRunner == null)
				m_ColorTweenRunner = new TweenRunner<ColorTween>();

			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_ColorTweenRunner.Init(this);
			m_FloatTweenRunner.Init(this);
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
		///     Gets or sets the target canvas group.
		/// </summary>
		/// <value>The target canvas group.</value>
		public CanvasGroup targetCanvasGroup {
			get => m_TargetCanvasGroup;
			set => m_TargetCanvasGroup = value;
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
			if (m_UseToggle) {
				if (m_TargetToggle == null)
					m_TargetToggle = gameObject.GetComponent<Toggle>();

				if (m_TargetToggle != null)
					m_Active = m_TargetToggle.isOn;
			}

			m_Selectable = gameObject.GetComponent<Selectable>();
		}

		protected void OnEnable() {
			if (m_TargetToggle != null)
				m_TargetToggle.onValueChanged.AddListener(OnToggleValueChange);

			InternalEvaluateAndTransitionToNormalState(true);
		}

		protected void OnDisable() {
			if (m_TargetToggle != null)
				m_TargetToggle.onValueChanged.RemoveListener(OnToggleValueChange);

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

				if (m_Transition != Transition.CanvasGroup)
					InternalEvaluateAndTransitionToNormalState(true);
			}
		}
#endif

		public void OnDeselect(BaseEventData eventData) {
			m_Selected = false;

			if (m_Active)
				return;

			DoStateTransition(m_Highlighted ? VisualState.Highlighted : VisualState.Normal, false);
		}

		public virtual void OnPointerDown(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (!m_Highlighted)
				return;

			if (m_Active)
				return;

			m_Pressed = true;
			DoStateTransition(VisualState.Pressed, false);
		}

		public void OnPointerEnter(PointerEventData eventData) {
			m_Highlighted = true;

			if (!m_Selected && !m_Pressed && !m_Active)
				DoStateTransition(VisualState.Highlighted, false);
		}

		public void OnPointerExit(PointerEventData eventData) {
			m_Highlighted = false;

			if (!m_Selected && !m_Pressed && !m_Active)
				DoStateTransition(VisualState.Normal, false);
		}

		public virtual void OnPointerUp(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			m_Pressed = false;

			VisualState newState = VisualState.Normal;

			if (m_Active)
				newState = VisualState.Active;
			else if (m_Selected)
				newState = VisualState.Selected;
			else if (m_Highlighted)
				newState = VisualState.Highlighted;

			DoStateTransition(newState, false);
		}

		public void OnSelect(BaseEventData eventData) {
			m_Selected = true;

			if (m_Active)
				return;

			DoStateTransition(VisualState.Selected, false);
		}

		/// <summary>
		///     Internally evaluates and transitions to normal state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionToNormalState(bool instant) {
			DoStateTransition(m_Active ? VisualState.Active : VisualState.Normal, instant);
		}

		public virtual bool IsInteractable() {
			if (m_Selectable != null)
				return m_Selectable.IsInteractable() && m_GroupsAllowInteraction;

			return m_GroupsAllowInteraction;
		}

		protected void OnToggleValueChange(bool value) {
			if (!m_UseToggle || m_TargetToggle == null)
				return;

			m_Active = m_TargetToggle.isOn;

			if (m_Transition == Transition.Animation) {
				if (targetGameObject == null || animator == null || !animator.isActiveAndEnabled ||
				    animator.runtimeAnimatorController == null || string.IsNullOrEmpty(m_ActiveBool))
					return;

				animator.SetBool(m_ActiveBool, m_Active);
			}

			DoStateTransition(m_Active ? VisualState.Active :
				m_Selected ? VisualState.Selected :
				m_Highlighted ? VisualState.Highlighted : VisualState.Normal, false);
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
					SetTextColor(m_NormalColor);
					break;
				case Transition.CanvasGroup:
					SetCanvasGroupAlpha(1f);
					break;
			}
		}

		/// <summary>
		///     Does the state transition.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void DoStateTransition(VisualState state, bool instant) {
			// Check if active in the scene
			if (!gameObject.activeInHierarchy)
				return;

			// Check if it's interactable
			if (!IsInteractable())
				state = VisualState.Normal;

			Color color = m_NormalColor;
			Sprite newSprite = null;
			string triggername = m_NormalTrigger;
			float alpha = m_NormalAlpha;

			// Prepare the transition values
			switch (state) {
				case VisualState.Normal:
					color = m_NormalColor;
					newSprite = null;
					triggername = m_NormalTrigger;
					alpha = m_NormalAlpha;
					break;
				case VisualState.Highlighted:
					color = m_HighlightedColor;
					newSprite = m_HighlightedSprite;
					triggername = m_HighlightedTrigger;
					alpha = m_HighlightedAlpha;
					break;
				case VisualState.Selected:
					color = m_SelectedColor;
					newSprite = m_SelectedSprite;
					triggername = m_SelectedTrigger;
					alpha = m_SelectedAlpha;
					break;
				case VisualState.Pressed:
					color = m_PressedColor;
					newSprite = m_PressedSprite;
					triggername = m_PressedTrigger;
					alpha = m_PressedAlpha;
					break;
				case VisualState.Active:
					color = m_ActiveColor;
					newSprite = m_ActiveSprite;
					alpha = m_ActiveAlpha;
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
				case Transition.CanvasGroup:
					StartCanvasGroupTween(alpha, instant);
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
			if (targetGameObject == null || animator == null || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(triggername))
				return;

			animator.ResetTrigger(m_HighlightedTrigger);
			animator.ResetTrigger(m_SelectedTrigger);
			animator.ResetTrigger(m_PressedTrigger);
			animator.SetTrigger(triggername);
		}

		private void StartTextColorTween(Color targetColor, bool instant) {
			if (m_TargetGraphic == null)
				return;

			if (m_TargetGraphic is Text == false && m_TargetGraphic is TextMeshProUGUI == false)
				return;

			if (instant || m_Duration == 0f || !Application.isPlaying) {
				switch (m_TargetGraphic) {
					case Text t:
						t.color = targetColor;
						break;
					case TextMeshProUGUI t:
						t.color = targetColor;
						break;
				}
			} else {
				ColorTween colorTween = new ColorTween {
					duration = m_Duration,
					startColor = m_TargetGraphic is Text t ? t.color : (m_TargetGraphic as TextMeshProUGUI).color,
					targetColor = targetColor
				};

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

			switch (m_TargetGraphic) {
				case Text t:
					t.color = targetColor;
					break;
				case TextMeshProUGUI t:
					t.color = targetColor;
					break;
			}
		}

		/// <summary>
		///     Starts the color tween.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void StartCanvasGroupTween(float targetAlpha, bool instant) {
			if (m_TargetCanvasGroup == null)
				return;

			if (instant || m_Duration == 0f || !Application.isPlaying) {
				SetCanvasGroupAlpha(targetAlpha);
			} else {
				FloatTween floatTween = new FloatTween
					{duration = m_Duration, startFloat = m_TargetCanvasGroup.alpha, targetFloat = targetAlpha};
				floatTween.AddOnChangedCallback(SetCanvasGroupAlpha);
				floatTween.ignoreTimeScale = true;

				m_FloatTweenRunner.StartTween(floatTween);
			}
		}

		/// <summary>
		///     Sets the canvas group alpha value.
		/// </summary>
		/// <param name="alpha">The alpha value.</param>
		private void SetCanvasGroupAlpha(float alpha) {
			if (m_TargetCanvasGroup == null)
				return;

			m_TargetCanvasGroup.alpha = alpha;
		}

#pragma warning disable 0649
		[SerializeField] private Transition m_Transition = Transition.None;

		[SerializeField] private Color m_NormalColor = ColorBlock.defaultColorBlock.normalColor;
		[SerializeField] private Color m_HighlightedColor = ColorBlock.defaultColorBlock.highlightedColor;
		[SerializeField] private Color m_SelectedColor = ColorBlock.defaultColorBlock.highlightedColor;
		[SerializeField] private Color m_PressedColor = ColorBlock.defaultColorBlock.pressedColor;
		[SerializeField] private Color m_ActiveColor = ColorBlock.defaultColorBlock.highlightedColor;
		[SerializeField] private float m_Duration = 0.1f;

		[SerializeField] [Range(1f, 6f)] private float m_ColorMultiplier = 1f;

		[SerializeField] private Sprite m_HighlightedSprite;
		[SerializeField] private Sprite m_SelectedSprite;
		[SerializeField] private Sprite m_PressedSprite;
		[SerializeField] private Sprite m_ActiveSprite;

		[SerializeField] private string m_NormalTrigger = "Normal";
		[SerializeField] private string m_HighlightedTrigger = "Highlighted";
		[SerializeField] private string m_SelectedTrigger = "Selected";
		[SerializeField] private string m_PressedTrigger = "Pressed";
		[SerializeField] private string m_ActiveBool = "Active";

		[SerializeField] [Range(0f, 1f)] private float m_NormalAlpha;
		[SerializeField] [Range(0f, 1f)] private float m_HighlightedAlpha = 1f;
		[SerializeField] [Range(0f, 1f)] private float m_SelectedAlpha = 1f;
		[SerializeField] [Range(0f, 1f)] private float m_PressedAlpha = 1f;
		[SerializeField] [Range(0f, 1f)] private float m_ActiveAlpha = 1f;

		[SerializeField] [Tooltip("Graphic that will have the selected transtion applied.")]
		private Graphic m_TargetGraphic;

		[SerializeField] [Tooltip("GameObject that will have the selected transtion applied.")]
		private GameObject m_TargetGameObject;

		[SerializeField] [Tooltip("CanvasGroup that will have the selected transtion applied.")]
		private CanvasGroup m_TargetCanvasGroup;

		[SerializeField] private bool m_UseToggle;
		[SerializeField] private Toggle m_TargetToggle;
#pragma warning restore 0649

	}

}