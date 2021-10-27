using System;
using System.Collections.Generic;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Effect Transition")]
	public class UIEffectTransition : MonoBehaviour, IEventSystemHandler, ISelectHandler, IDeselectHandler,
		IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

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

		private bool m_Active;
		private bool m_GroupsAllowInteraction = true;

		private bool m_Highlighted;
		private bool m_Pressed;

		private Selectable m_Selectable;
		private bool m_Selected;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UIEffectTransition() {
			if (m_ColorTweenRunner == null)
				m_ColorTweenRunner = new TweenRunner<ColorTween>();

			m_ColorTweenRunner.Init(this);
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

			if (isActiveAndEnabled)
				InternalEvaluateAndTransitionToNormalState(true);
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

		public virtual bool IsInteractable() {
			if (m_Selectable != null)
				return m_Selectable.IsInteractable() && m_GroupsAllowInteraction;

			return m_GroupsAllowInteraction;
		}

		protected void OnToggleValueChange(bool value) {
			if (!m_UseToggle || m_TargetToggle == null)
				return;

			m_Active = m_TargetToggle.isOn;

			if (!m_TargetToggle.isOn)
				DoStateTransition(m_Selected ? VisualState.Selected : VisualState.Normal, false);
		}

		/// <summary>
		///     Instantly clears the visual state.
		/// </summary>
		protected void InstantClearState() {
			SetEffectColor(Color.white);
		}

		/// <summary>
		///     Internally evaluates and transitions to normal state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionToNormalState(bool instant) {
			DoStateTransition(m_Active ? VisualState.Active : VisualState.Normal, instant);
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

			// Prepare the transition values
			switch (state) {
				case VisualState.Normal:
					color = m_NormalColor;
					break;
				case VisualState.Highlighted:
					color = m_HighlightedColor;
					break;
				case VisualState.Selected:
					color = m_SelectedColor;
					break;
				case VisualState.Pressed:
					color = m_PressedColor;
					break;
				case VisualState.Active:
					color = m_ActiveColor;
					break;
			}

			StartEffectColorTween(color, false);
		}

		private void StartEffectColorTween(Color targetColor, bool instant) {
			if (m_TargetEffect == null)
				return;

			if (m_TargetEffect is Shadow == false && m_TargetEffect is Outline == false)
				return;

			if (instant || m_Duration == 0f || !Application.isPlaying) {
				SetEffectColor(targetColor);
			} else {
				ColorTween colorTween = new ColorTween
					{duration = m_Duration, startColor = GetEffectColor(), targetColor = targetColor};
				colorTween.AddOnChangedCallback(SetEffectColor);
				colorTween.ignoreTimeScale = true;

				m_ColorTweenRunner.StartTween(colorTween);
			}
		}

		/// <summary>
		///     Sets the effect color.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		private void SetEffectColor(Color targetColor) {
			if (m_TargetEffect == null)
				return;

			if (m_TargetEffect is Shadow)
				(m_TargetEffect as Shadow).effectColor = targetColor;
			else if (m_TargetEffect is Outline)
				(m_TargetEffect as Outline).effectColor = targetColor;
		}

		private Color GetEffectColor() {
			if (m_TargetEffect == null)
				return Color.white;

			if (m_TargetEffect is Shadow)
				return (m_TargetEffect as Shadow).effectColor;
			if (m_TargetEffect is Outline)
				return (m_TargetEffect as Outline).effectColor;

			return Color.white;
		}

#pragma warning disable 0649
		[SerializeField] [Tooltip("Graphic that will have the selected transtion applied.")]
		private BaseMeshEffect m_TargetEffect;

		[SerializeField] private Color m_NormalColor = ColorBlock.defaultColorBlock.normalColor;
		[SerializeField] private Color m_HighlightedColor = ColorBlock.defaultColorBlock.highlightedColor;
		[SerializeField] private Color m_SelectedColor = ColorBlock.defaultColorBlock.highlightedColor;
		[SerializeField] private Color m_PressedColor = ColorBlock.defaultColorBlock.pressedColor;
		[SerializeField] private float m_Duration = 0.1f;

		[SerializeField] private bool m_UseToggle;
		[SerializeField] private Toggle m_TargetToggle;
		[SerializeField] private Color m_ActiveColor = ColorBlock.defaultColorBlock.highlightedColor;
#pragma warning restore 0649

	}
}