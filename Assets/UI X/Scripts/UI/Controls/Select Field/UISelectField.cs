using System;
using System.Collections.Generic;
using AsglaUI.UI.Tweens;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/Select Field", 58)]
	[RequireComponent(typeof(Image))]
	public class UISelectField : Toggle {

		public enum Direction {

			Auto,
			Down,
			Up

		}

		public enum ListAnimationType {

			None,
			Fade,
			Animation

		}

		public enum OptionTextEffectType {

			None,
			Shadow,
			Outline

		}

		public enum OptionTextTransitionType {

			None,
			CrossFade

		}

		public enum VisualState {

			Normal,
			Highlighted,
			Pressed,
			Active,
			ActiveHighlighted,
			ActivePressed,
			Disabled

		}

		// Currently selected item
		[HideInInspector] [SerializeField] private string m_SelectedItem;

		[SerializeField] private Direction m_Direction = Direction.Auto;

		/// <summary>
		///     Private list of the select options.
		/// </summary>
		[SerializeField] [FormerlySerializedAs("options")]
		private List<string> m_Options = new List<string>();

#pragma warning disable 0649
		// The label text
		[SerializeField] private TextMeshProUGUI m_LabelText;
#pragma warning restore 0649

		// Select Field layout properties
		public new ColorBlockExtended colors = ColorBlockExtended.defaultColorBlock;
		public new SpriteStateExtended spriteState;
		public new AnimationTriggersExtended animationTriggers = new AnimationTriggersExtended();

		// List layout properties
		public Sprite listBackgroundSprite;
		public Image.Type listBackgroundSpriteType = Image.Type.Sliced;
		public Color listBackgroundColor = Color.white;
		public RectOffset listMargins;
		public RectOffset listPadding;
		public float listSpacing;
		public ListAnimationType listAnimationType = ListAnimationType.Fade;
		public float listAnimationDuration = 0.1f;
		public RuntimeAnimatorController listAnimatorController;
		public string listAnimationOpenTrigger = "Open";
		public string listAnimationCloseTrigger = "Close";

		// Scroll rect properties
		public bool allowScrollRect = true;
		public ScrollRect.MovementType scrollMovementType = ScrollRect.MovementType.Clamped;
		public float scrollElasticity = 0.1f;
		public bool scrollInertia;
		public float scrollDecelerationRate = 0.135f;
		public float scrollSensitivity = 1f;
		public int scrollMinOptions = 5;
		public float scrollListHeight = 512f;
		public GameObject scrollBarPrefab;
		public float scrollbarSpacing = 34f;

		// Option text layout properties
		public Font optionFont = FontData.defaultFontData.font;
		public int optionFontSize = FontData.defaultFontData.fontSize;
		public FontStyle optionFontStyle = FontData.defaultFontData.fontStyle;
		public Color optionColor = Color.white;
		public OptionTextTransitionType optionTextTransitionType = OptionTextTransitionType.CrossFade;
		public ColorBlockExtended optionTextTransitionColors = ColorBlockExtended.defaultColorBlock;
		public RectOffset optionPadding;

		// Option text effect properties
		public OptionTextEffectType optionTextEffectType = OptionTextEffectType.None;
		public Color optionTextEffectColor = new Color(0f, 0f, 0f, 128f);
		public Vector2 optionTextEffectDistance = new Vector2(1f, -1f);
		public bool optionTextEffectUseGraphicAlpha = true;

		// Option background properties
		public Sprite optionBackgroundSprite;
		public Color optionBackgroundSpriteColor = Color.white;
		public Image.Type optionBackgroundSpriteType = Image.Type.Sliced;
		public Transition optionBackgroundTransitionType = Transition.None;
		public ColorBlockExtended optionBackgroundTransColors = ColorBlockExtended.defaultColorBlock;
		public SpriteStateExtended optionBackgroundSpriteStates;
		public AnimationTriggersExtended optionBackgroundAnimationTriggers = new AnimationTriggersExtended();
		public RuntimeAnimatorController optionBackgroundAnimatorController;
		public Sprite optionHoverOverlay;
		public Color optionHoverOverlayColor = Color.white;
		public ColorBlock optionHoverOverlayColorBlock = ColorBlock.defaultColorBlock;
		public Sprite optionPressOverlay;
		public Color optionPressOverlayColor = Color.white;
		public ColorBlock optionPressOverlayColorBlock = ColorBlock.defaultColorBlock;

		// List separator properties
		public Sprite listSeparatorSprite;
		public Image.Type listSeparatorType = Image.Type.Simple;
		public Color listSeparatorColor = Color.white;
		public float listSeparatorHeight;
		public bool startSeparator;

		/// <summary>
		///     Event delegate triggered when the selected option changes.
		/// </summary>
		public ChangeEvent onChange = new ChangeEvent();

		/// <summary>
		///     Event delegate triggered when the select field transition to a visual state.
		/// </summary>
		public TransitionEvent onTransition = new TransitionEvent();

		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		private readonly List<UISelectField_Option> m_OptionObjects = new List<UISelectField_Option>();

		private GameObject m_Blocker;
		private VisualState m_CurrentVisualState = VisualState.Normal;
		private Vector2 m_LastListSize = Vector2.zero;
		private Navigation.Mode m_LastNavigationMode;
		private GameObject m_LastSelectedGameObject;
		private CanvasGroup m_ListCanvasGroup;
		private GameObject m_ListContentObject;

		private GameObject m_ListObject;
		private bool m_PointerWasUsedOnOption;
		private ScrollRect m_ScrollRect;

		private GameObject m_StartSeparatorObject;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UISelectField() {
			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_FloatTweenRunner.Init(this);
		}

		/// <summary>
		///     The direction in which the list should pop.
		/// </summary>
		public Direction direction {
			get => m_Direction;
			set => m_Direction = value;
		}

		/// <summary>
		///     Gets the list of options.
		/// </summary>
		public List<string> options => m_Options;

		/// <summary>
		///     Currently selected option.
		/// </summary>
		public string value {
			get => m_SelectedItem;
			set => SelectOption(value);
		}

		/// <summary>
		///     Gets the index of the selected option.
		/// </summary>
		/// <value>The index of the selected option.</value>
		public int selectedOptionIndex => GetOptionIndex(m_SelectedItem);

		/// <summary>
		///     Gets a value indicating whether the list is open.
		/// </summary>
		/// <value><c>true</c> if the list is open; otherwise, <c>false</c>.</value>
		public bool IsOpen => isOn;

		protected override void Awake() {
			base.Awake();

			// Get the background image
			if (targetGraphic == null)
				targetGraphic = GetComponent<Image>();
		}

		protected override void Start() {
			base.Start();

			// Prepare the toggle
			toggleTransition = ToggleTransition.None;
		}

		protected override void OnEnable() {
			base.OnEnable();

			// Hook the on change event
			onValueChanged.AddListener(OnToggleValueChanged);
		}

		protected override void OnDisable() {
			base.OnDisable();

			// Unhook the on change event
			onValueChanged.RemoveListener(OnToggleValueChanged);

			// Close if open
			isOn = false;

			// Transition to the current state
			DoStateTransition(SelectionState.Disabled, true);
		}

#if UNITY_EDITOR
		protected override void OnValidate() {
			base.OnValidate();

			// Make sure we always have a font
			if (optionFont == null)
				optionFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		}
#endif

		/// <summary>
		///     Open the select field list.
		/// </summary>
		public void Open() {
			isOn = true;
		}

		/// <summary>
		///     Closes the select field list.
		/// </summary>
		public void Close() {
			isOn = false;
		}

		/// <summary>
		///     Gets the index of the given option.
		/// </summary>
		/// <returns>The option index. (-1 if the option was not found)</returns>
		/// <param name="optionValue">Option value.</param>
		public int GetOptionIndex(string optionValue) {
			// Find the option index in the options list
			if (m_Options != null && m_Options.Count > 0 && !string.IsNullOrEmpty(optionValue))
				for (int i = 0; i < m_Options.Count; i++)
					if (optionValue.Equals(m_Options[i], StringComparison.OrdinalIgnoreCase))
						return i;

			// Default
			return -1;
		}

		/// <summary>
		///     Selects the option by index.
		/// </summary>
		/// <param name="optionIndex">Option index.</param>
		public void SelectOptionByIndex(int index) {
			// If the list is open, use the toggle to select the option
			if (IsOpen) {
				UISelectField_Option option = m_OptionObjects[index];

				if (option != null)
					option.isOn = true;
			} else // otherwise set as selected
			{
				// Set as selected
				m_SelectedItem = m_Options[index];

				// Trigger change
				TriggerChangeEvent();
			}
		}

		/// <summary>
		///     Selects the option by value.
		/// </summary>
		/// <param name="optionValue">The option value.</param>
		public void SelectOption(string optionValue) {
			if (string.IsNullOrEmpty(optionValue))
				return;

			// Get the option
			int index = GetOptionIndex(optionValue);

			// Check if the option index is valid
			if (index < 0 || index >= m_Options.Count)
				return;

			// Select the option
			SelectOptionByIndex(index);
		}

		/// <summary>
		///     Adds an option.
		/// </summary>
		/// <param name="optionValue">Option value.</param>
		public void AddOption(string optionValue) {
			if (m_Options != null) {
				m_Options.Add(optionValue);
				OptionListChanged();
			}
		}

		/// <summary>
		///     Adds an option at given index.
		/// </summary>
		/// <param name="optionValue">Option value.</param>
		/// <param name="index">Index.</param>
		public void AddOptionAtIndex(string optionValue, int index) {
			if (m_Options == null)
				return;

			// Check if the index is outside the list
			if (index >= m_Options.Count)
				m_Options.Add(optionValue);
			else
				m_Options.Insert(index, optionValue);

			OptionListChanged();
		}

		/// <summary>
		///     Removes the option.
		/// </summary>
		/// <param name="optionValue">Option value.</param>
		public void RemoveOption(string optionValue) {
			if (m_Options == null)
				return;

			// Remove the option if exists
			if (m_Options.Contains(optionValue)) {
				m_Options.Remove(optionValue);
				OptionListChanged();
				ValidateSelectedOption();
			}
		}

		/// <summary>
		///     Removes the option at the given index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveOptionAtIndex(int index) {
			if (m_Options == null)
				return;

			// Remove the option if the index is valid
			if (index >= 0 && index < m_Options.Count) {
				m_Options.RemoveAt(index);
				OptionListChanged();
				ValidateSelectedOption();
			}
		}

		/// <summary>
		///     Clears the option list.
		/// </summary>
		public void ClearOptions() {
			if (m_Options == null)
				return;

			m_Options.Clear();
			OptionListChanged();
		}

		/// <summary>
		///     Validates the selected option and makes corrections if it's missing.
		/// </summary>
		public void ValidateSelectedOption() {
			if (m_Options == null)
				return;

			// Fix the selected option if it no longer exists
			if (!m_Options.Contains(m_SelectedItem))
				// Select the first option
				SelectOptionByIndex(0);
		}

		/// <summary>
		///     Raises the option select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="option">Option.</param>
		public void OnOptionSelect(string option) {
			if (string.IsNullOrEmpty(option))
				return;

			// Save the current string to compare later
			string current = m_SelectedItem;

			// Save the string
			m_SelectedItem = option;

			// Trigger change event
			if (!current.Equals(m_SelectedItem))
				TriggerChangeEvent();

			// Close the list if it's opened and the pointer was used to select the option
			if (IsOpen && m_PointerWasUsedOnOption) {
				// Reset the value
				m_PointerWasUsedOnOption = false;

				// Close the list
				Close();

				// Deselect the toggle
				base.OnDeselect(new BaseEventData(EventSystem.current));
			}
		}

		/// <summary>
		///     Raises the option pointer up event (Used to close the list).
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnOptionPointerUp(BaseEventData eventData) {
			// Flag to close the list on selection
			m_PointerWasUsedOnOption = true;
		}

		/// <summary>
		///     Tiggers the change event.
		/// </summary>
		protected virtual void TriggerChangeEvent() {
			// Apply the string to the label componenet
			if (m_LabelText != null)
				m_LabelText.text = m_SelectedItem;

			// Invoke the on change event
			if (onChange != null)
				onChange.Invoke(selectedOptionIndex, m_SelectedItem);
		}

		/// <summary>
		///     Raises the toggle value changed event (used to toggle the list).
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		private void OnToggleValueChanged(bool state) {
			if (!Application.isPlaying)
				return;

			// Transition to the current state
			DoStateTransition(currentSelectionState, false);

			// Open / Close the list
			ToggleList(isOn);

			// Destroy the block on close
			if (!isOn && m_Blocker != null)
				Destroy(m_Blocker);
		}

		/// <summary>
		///     Raises the deselect event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnDeselect(BaseEventData eventData) {
			// Check if the mouse is over our options list
			if (m_ListObject != null) {
				UISelectField_List list = m_ListObject.GetComponent<UISelectField_List>();

				if (list.IsHighlighted(eventData))
					return;
			}

			// Check if the mouse is over one of our options
			foreach (UISelectField_Option option in m_OptionObjects)
				if (option.IsHighlighted(eventData))
					return;

			// When the select field loses focus
			// close the list by deactivating the toggle
			Close();

			// Pass to base
			base.OnDeselect(eventData);
		}

		/// <summary>
		///     Raises the move event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnMove(AxisEventData eventData) {
			// Handle navigation for opened list
			if (IsOpen) {
				int prevIndex = selectedOptionIndex - 1;
				int nextIndex = selectedOptionIndex + 1;

				// Highlight the new option
				switch (eventData.moveDir) {
					case MoveDirection.Up: {
						if (prevIndex >= 0)
							SelectOptionByIndex(prevIndex);
						break;
					}
					case MoveDirection.Down: {
						if (nextIndex < m_Options.Count)
							SelectOptionByIndex(nextIndex);
						break;
					}
				}

				// Use the event
				eventData.Use();
			} else {
				// Pass to base
				base.OnMove(eventData);
			}
		}

		/// <summary>
		///     Dos the state transition of the select field.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected override void DoStateTransition(SelectionState state, bool instant) {
			if (!gameObject.activeInHierarchy)
				return;

			Color color = colors.normalColor;
			Sprite newSprite = null;
			string triggername = animationTriggers.normalTrigger;

			// Check if this is the disabled state before any others
			if (state == SelectionState.Disabled) {
				m_CurrentVisualState = VisualState.Disabled;
				color = colors.disabledColor;
				newSprite = spriteState.disabledSprite;
				triggername = animationTriggers.disabledTrigger;
			} else {
				// Prepare the state values
				switch (state) {
					case SelectionState.Normal:
						m_CurrentVisualState = isOn ? VisualState.Active : VisualState.Normal;
						color = isOn ? colors.activeColor : colors.normalColor;
						newSprite = isOn ? spriteState.activeSprite : null;
						triggername = isOn ? animationTriggers.activeTrigger : animationTriggers.normalTrigger;
						break;
					case SelectionState.Highlighted:
					case SelectionState.Selected:
						m_CurrentVisualState = isOn ? VisualState.ActiveHighlighted : VisualState.Highlighted;
						color = isOn ? colors.activeHighlightedColor : colors.highlightedColor;
						newSprite = isOn ? spriteState.activeHighlightedSprite : spriteState.highlightedSprite;
						triggername = isOn
							? animationTriggers.activeHighlightedTrigger
							: animationTriggers.highlightedTrigger;
						break;
					case SelectionState.Pressed:
						m_CurrentVisualState = isOn ? VisualState.ActivePressed : VisualState.Pressed;
						color = isOn ? colors.activePressedColor : colors.pressedColor;
						newSprite = isOn ? spriteState.activePressedSprite : spriteState.pressedSprite;
						triggername = isOn ? animationTriggers.activePressedTrigger : animationTriggers.pressedTrigger;
						break;
				}
			}

			// Do the transition
			switch (transition) {
				case Transition.ColorTint:
					StartColorTween(color * colors.colorMultiplier, instant ? 0f : colors.fadeDuration);
					break;
				case Transition.SpriteSwap:
					DoSpriteSwap(newSprite);
					break;
				case Transition.Animation:
					TriggerAnimation(triggername);
					break;
			}

			// Invoke the transition event
			if (onTransition != null)
				onTransition.Invoke(m_CurrentVisualState, instant);
		}

		/// <summary>
		///     Starts the color tween of the select field.
		/// </summary>
		/// <param name="color">Color.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void StartColorTween(Color color, float duration) {
			if (targetGraphic == null)
				return;

			targetGraphic.CrossFadeColor(color, duration, true, true);
		}

		/// <summary>
		///     Does the sprite swap of the select field.
		/// </summary>
		/// <param name="newSprite">New sprite.</param>
		private void DoSpriteSwap(Sprite newSprite) {
			Image image = targetGraphic as Image;

			if (image == null)
				return;

			image.overrideSprite = newSprite;
		}

		/// <summary>
		///     Triggers the animation of the select field.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		private void TriggerAnimation(string trigger) {
			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(trigger))
				return;

			animator.ResetTrigger(animationTriggers.normalTrigger);
			animator.ResetTrigger(animationTriggers.pressedTrigger);
			animator.ResetTrigger(animationTriggers.highlightedTrigger);
			animator.ResetTrigger(animationTriggers.activeTrigger);
			animator.ResetTrigger(animationTriggers.activeHighlightedTrigger);
			animator.ResetTrigger(animationTriggers.activePressedTrigger);
			animator.ResetTrigger(animationTriggers.disabledTrigger);
			animator.SetTrigger(trigger);
		}

		/// <summary>
		///     Toggles the list.
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		protected virtual void ToggleList(bool state) {
			if (!IsActive())
				return;

			// Check if the list is not yet created
			if (m_ListObject == null)
				CreateList();

			// Make sure the creating of the list was successful
			if (m_ListObject == null)
				return;

			// Make sure we have the canvas group
			if (m_ListCanvasGroup != null)
				// Disable or enable list interaction
				m_ListCanvasGroup.blocksRaycasts = state;

			// Make sure navigation is enabled in open state
			if (state) {
				m_LastNavigationMode = navigation.mode;
				m_LastSelectedGameObject = EventSystem.current.currentSelectedGameObject;

				Navigation newNav = navigation;
				newNav.mode = Navigation.Mode.Vertical;
				navigation = newNav;

				// Set the select field as selected
				EventSystem.current.SetSelectedGameObject(gameObject);
			} else {
				Navigation newNav = navigation;
				newNav.mode = m_LastNavigationMode;
				navigation = newNav;

				if (!EventSystem.current.alreadySelecting && m_LastSelectedGameObject != null)
					EventSystem.current.SetSelectedGameObject(m_LastSelectedGameObject);
			}

			// Bring to front
			if (state) UIUtility.BringToFront(m_ListObject);

			// Start the opening/closing animation
			if (listAnimationType == ListAnimationType.None || listAnimationType == ListAnimationType.Fade) {
				float targetAlpha = state ? 1f : 0f;

				// Fade In / Out
				TweenListAlpha(targetAlpha, listAnimationType == ListAnimationType.Fade ? listAnimationDuration : 0f,
					true);
			} else if (listAnimationType == ListAnimationType.Animation) {
				TriggerListAnimation(state ? listAnimationOpenTrigger : listAnimationCloseTrigger);
			}
		}

		/// <summary>
		///     Creates the list and it's options.
		/// </summary>
		protected void CreateList() {
			// Reset the last list size
			m_LastListSize = Vector2.zero;

			// Clear the option texts list
			m_OptionObjects.Clear();

			// Create the list game object with the necessary components
			m_ListObject = new GameObject("UISelectField - List", typeof(RectTransform));
			m_ListObject.layer = gameObject.layer;

			// Change the parent of the list
			m_ListObject.transform.SetParent(transform, false);

			// Get the select field list component
			UISelectField_List listComp = m_ListObject.AddComponent<UISelectField_List>();

			// Make sure it's the top-most element
			UIAlwaysOnTop aot = m_ListObject.AddComponent<UIAlwaysOnTop>();
			aot.order = UIAlwaysOnTop.SelectFieldOrder;

			// Get the list canvas group component
			m_ListCanvasGroup = m_ListObject.AddComponent<CanvasGroup>();

			// Change the anchor and pivot of the list
			RectTransform rect = m_ListObject.transform as RectTransform;
			rect.localScale = new Vector3(1f, 1f, 1f);
			rect.localPosition = Vector3.zero;
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.zero;
			rect.pivot = new Vector2(0f, 1f);

			// Prepare the position of the list
			rect.anchoredPosition = new Vector3(listMargins.left, listMargins.top * -1f, 0f);

			// Prepare the width of the list
			float width = (transform as RectTransform).sizeDelta.x;
			if (listMargins.left > 0) width -= listMargins.left;
			else width += Math.Abs(listMargins.left);
			if (listMargins.right > 0) width -= listMargins.right;
			else width += Math.Abs(listMargins.right);
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

			// Hook the Dimensions Change event
			listComp.onDimensionsChange.AddListener(ListDimensionsChanged);

			// Apply the background sprite
			Image image = m_ListObject.AddComponent<Image>();
			if (listBackgroundSprite != null)
				image.sprite = listBackgroundSprite;
			image.type = listBackgroundSpriteType;
			image.color = listBackgroundColor;

			if (allowScrollRect && m_Options.Count >= scrollMinOptions) {
				// Set the list height
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scrollListHeight);

				// Add scroll rect
				GameObject scrollRectGo = new GameObject("Scroll Rect", typeof(RectTransform));
				scrollRectGo.layer = m_ListObject.layer;
				scrollRectGo.transform.SetParent(m_ListObject.transform, false);

				RectTransform scrollRectRect = scrollRectGo.transform as RectTransform;
				scrollRectRect.localScale = new Vector3(1f, 1f, 1f);
				scrollRectRect.localPosition = Vector3.zero;
				scrollRectRect.anchorMin = Vector2.zero;
				scrollRectRect.anchorMax = Vector2.one;
				scrollRectRect.pivot = new Vector2(0f, 1f);
				scrollRectRect.anchoredPosition = Vector2.zero;
				scrollRectRect.offsetMin = new Vector2(listPadding.left, listPadding.bottom);
				scrollRectRect.offsetMax = new Vector2(listPadding.right * -1f, listPadding.top * -1f);

				// Add scroll rect component
				m_ScrollRect = scrollRectGo.AddComponent<ScrollRect>();
				m_ScrollRect.horizontal = false;
				m_ScrollRect.movementType = scrollMovementType;
				m_ScrollRect.elasticity = scrollElasticity;
				m_ScrollRect.inertia = scrollInertia;
				m_ScrollRect.decelerationRate = scrollDecelerationRate;
				m_ScrollRect.scrollSensitivity = scrollSensitivity;

				// Create the viewport
				GameObject viewPortGo = new GameObject("View Port", typeof(RectTransform));
				viewPortGo.layer = m_ListObject.layer;
				viewPortGo.transform.SetParent(scrollRectGo.transform, false);

				RectTransform viewPortRect = viewPortGo.transform as RectTransform;
				viewPortRect.localScale = new Vector3(1f, 1f, 1f);
				viewPortRect.localPosition = Vector3.zero;
				viewPortRect.anchorMin = Vector2.zero;
				viewPortRect.anchorMax = Vector2.one;
				viewPortRect.pivot = new Vector2(0f, 1f);
				viewPortRect.anchoredPosition = Vector2.zero;
				viewPortRect.offsetMin = Vector2.zero;
				viewPortRect.offsetMax = Vector2.zero;

				// Add image to the viewport
				Image viewImage = viewPortGo.AddComponent<Image>();
				viewImage.raycastTarget = false;

				// Add mask to the viewport
				Mask viewMask = viewPortGo.AddComponent<Mask>();
				viewMask.showMaskGraphic = false;

				// Create content
				m_ListContentObject = new GameObject("Content", typeof(RectTransform));
				m_ListContentObject.layer = m_ListObject.layer;
				m_ListContentObject.transform.SetParent(viewPortRect, false);

				RectTransform contentRect = m_ListContentObject.transform as RectTransform;
				contentRect.localScale = new Vector3(1f, 1f, 1f);
				contentRect.localPosition = Vector3.zero;
				contentRect.anchorMin = new Vector2(0f, 1f);
				contentRect.anchorMax = new Vector2(0f, 1f);
				contentRect.pivot = new Vector2(0f, 1f);
				contentRect.anchoredPosition = Vector2.zero;
				contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.sizeDelta.x);

				// Add image to the content for easy scrolling
				Image contentImage = m_ListContentObject.AddComponent<Image>();
				contentImage.color = new Color(1f, 1f, 1f, 0f);

				// Get the select field list component
				UISelectField_List contentListComp = m_ListContentObject.AddComponent<UISelectField_List>();
				contentListComp.onDimensionsChange.AddListener(ScrollContentDimensionsChanged);

				// Set the content and viewport to the scroll rect
				m_ScrollRect.content = contentRect;
				m_ScrollRect.viewport = viewPortRect;

				// Prepare the scroll bar
				if (scrollBarPrefab != null) {
					GameObject scrollBarGo = Instantiate(scrollBarPrefab, scrollRectGo.transform);

					m_ScrollRect.verticalScrollbar = scrollBarGo.GetComponent<Scrollbar>();
					m_ScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
					m_ScrollRect.verticalScrollbarSpacing = scrollbarSpacing;
				}

				// Prepare the vertical layout group without list padding
				m_ListContentObject.AddComponent<VerticalLayoutGroup>();
			} else {
				// Use the list object as list content object
				m_ListContentObject = m_ListObject;

				// Prepare the vertical layout group with list padding
				VerticalLayoutGroup layoutGroup = m_ListContentObject.AddComponent<VerticalLayoutGroup>();
				layoutGroup.padding = listPadding;
				layoutGroup.spacing = listSpacing;
			}

			// Prepare the content size fitter
			ContentSizeFitter fitter = m_ListContentObject.AddComponent<ContentSizeFitter>();
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			// Get the list toggle group
			ToggleGroup toggleGroup = m_ListObject.AddComponent<ToggleGroup>();

			// Create the options
			for (int i = 0; i < m_Options.Count; i++) {
				if (i == 0 && startSeparator)
					m_StartSeparatorObject = CreateSeparator(i - 1);

				// Create the option
				CreateOption(i, toggleGroup);

				// Create a separator if this is not the last option
				if (i < m_Options.Count - 1)
					CreateSeparator(i);
			}

			// Prepare the list for the animation
			if (listAnimationType == ListAnimationType.None || listAnimationType == ListAnimationType.Fade) {
				// Starting alpha should be zero
				m_ListCanvasGroup.alpha = 0f;
			} else if (listAnimationType == ListAnimationType.Animation) {
				// Attach animator component
				Animator animator = m_ListObject.AddComponent<Animator>();

				// Set the animator controller
				animator.runtimeAnimatorController = listAnimatorController;

				// Set the animation triggers so we can use them to detect when animations finish
				listComp.SetTriggers(listAnimationOpenTrigger, listAnimationCloseTrigger);

				// Hook a callback on the finish event
				listComp.onAnimationFinish.AddListener(OnListAnimationFinish);
			}

			// Check if the navigation is disabled
			if (navigation.mode == Navigation.Mode.None)
				CreateBlocker();

			// If we are using a scroll rect invoke the list dimensions change
			if (allowScrollRect && m_Options.Count >= scrollMinOptions)
				ListDimensionsChanged();
		}

		protected virtual void CreateBlocker() {
			// Create blocker GameObject.
			GameObject blocker = new GameObject("Blocker");

			// Setup blocker RectTransform to cover entire root canvas area.
			RectTransform blockerRect = blocker.AddComponent<RectTransform>();
			blockerRect.SetParent(transform, false);
			blockerRect.localScale = Vector3.one;
			blockerRect.localPosition = Vector3.zero;

			// Add image since it's needed to block, but make it clear.
			Image blockerImage = blocker.AddComponent<Image>();
			blockerImage.color = Color.clear;

			// Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
			Button blockerButton = blocker.AddComponent<Button>();
			blockerButton.onClick.AddListener(Close);

			// Make sure it's the top-most element
			UIAlwaysOnTop aot = blocker.AddComponent<UIAlwaysOnTop>();
			aot.order = UIAlwaysOnTop.SelectFieldBlockerOrder;

			UIUtility.BringToFront(blocker);

			blockerRect.anchoredPosition = Vector2.zero;
			blockerRect.pivot = new Vector2(0.5f, 0.5f);
			blockerRect.anchorMin = new Vector2(0f, 0f);
			blockerRect.anchorMax = new Vector2(1f, 1f);
			blockerRect.sizeDelta = new Vector2(0f, 0f);

			m_Blocker = blocker;
		}

		/// <summary>
		///     Creates a option.
		/// </summary>
		/// <param name="index">Index.</param>
		protected void CreateOption(int index, ToggleGroup toggleGroup) {
			if (m_ListContentObject == null)
				return;

			// Create the option game object with it's components
			GameObject optionObject = new GameObject("Option " + index, typeof(RectTransform));
			optionObject.layer = gameObject.layer;

			// Change parents
			optionObject.transform.SetParent(m_ListContentObject.transform, false);
			optionObject.transform.localScale = new Vector3(1f, 1f, 1f);
			optionObject.transform.localPosition = Vector3.zero;

			// Get the option component
			UISelectField_Option optionComp = optionObject.AddComponent<UISelectField_Option>();

			// Prepare the option background
			if (optionBackgroundSprite != null) {
				Image image = optionObject.AddComponent<Image>();
				image.sprite = optionBackgroundSprite;
				image.type = optionBackgroundSpriteType;
				image.color = optionBackgroundSpriteColor;

				// Add the graphic as the option transition target
				optionComp.targetGraphic = image;
			}

			// Prepare the option for animation
			if (optionBackgroundTransitionType == Transition.Animation) {
				// Attach animator component
				Animator animator = optionObject.AddComponent<Animator>();

				// Set the animator controller
				animator.runtimeAnimatorController = optionBackgroundAnimatorController;
			}

			// Apply the option padding
			VerticalLayoutGroup vlg = optionObject.AddComponent<VerticalLayoutGroup>();
			vlg.padding = optionPadding;

			// Create the option text
			GameObject textObject = new GameObject("Label", typeof(RectTransform));

			// Change parents
			textObject.transform.SetParent(optionObject.transform, false);
			textObject.transform.localScale = Vector3.one;
			textObject.transform.localPosition = Vector3.zero;

			// Apply pivot
			(textObject.transform as RectTransform).pivot = new Vector2(0f, 1f);

			// Prepare the text
			Text text = textObject.AddComponent<Text>();
			text.font = optionFont;
			text.fontSize = optionFontSize;
			text.fontStyle = optionFontStyle;
			text.color = optionColor;

			if (m_Options != null)
				text.text = m_Options[index];

			// Apply normal state transition color
			if (optionTextTransitionType == OptionTextTransitionType.CrossFade)
				text.canvasRenderer.SetColor(optionTextTransitionColors.normalColor);

			// Add and prepare the text effect
			if (optionTextEffectType != OptionTextEffectType.None) {
				if (optionTextEffectType == OptionTextEffectType.Shadow) {
					Shadow effect = textObject.AddComponent<Shadow>();
					effect.effectColor = optionTextEffectColor;
					effect.effectDistance = optionTextEffectDistance;
					effect.useGraphicAlpha = optionTextEffectUseGraphicAlpha;
				} else if (optionTextEffectType == OptionTextEffectType.Outline) {
					Outline effect = textObject.AddComponent<Outline>();
					effect.effectColor = optionTextEffectColor;
					effect.effectDistance = optionTextEffectDistance;
					effect.useGraphicAlpha = optionTextEffectUseGraphicAlpha;
				}
			}

			// Prepare the option hover overlay
			if (optionHoverOverlay != null) {
				GameObject hoverOverlayObj = new GameObject("Hover Overlay", typeof(RectTransform));
				hoverOverlayObj.layer = gameObject.layer;
				hoverOverlayObj.transform.localScale = Vector3.one;
				hoverOverlayObj.transform.localPosition = Vector3.zero;

				// Add layout element
				LayoutElement hoverLayoutElement = hoverOverlayObj.AddComponent<LayoutElement>();
				hoverLayoutElement.ignoreLayout = true;

				// Change parents
				hoverOverlayObj.transform.SetParent(optionObject.transform, false);
				hoverOverlayObj.transform.localScale = new Vector3(1f, 1f, 1f);

				// Add image
				Image hoImage = hoverOverlayObj.AddComponent<Image>();
				hoImage.sprite = optionHoverOverlay;
				hoImage.color = optionHoverOverlayColor;
				hoImage.type = Image.Type.Sliced;

				// Apply pivot
				(hoverOverlayObj.transform as RectTransform).pivot = new Vector2(0f, 1f);

				// Apply anchors
				(hoverOverlayObj.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
				(hoverOverlayObj.transform as RectTransform).anchorMax = new Vector2(1f, 1f);

				// Apply offsets
				(hoverOverlayObj.transform as RectTransform).offsetMin = new Vector2(0f, 0f);
				(hoverOverlayObj.transform as RectTransform).offsetMax = new Vector2(0f, 0f);

				// Add the highlight transition component
				UISelectField_OptionOverlay hoht = optionObject.AddComponent<UISelectField_OptionOverlay>();
				hoht.targetGraphic = hoImage;
				hoht.transition = UISelectField_OptionOverlay.Transition.ColorTint;
				hoht.colorBlock = optionHoverOverlayColorBlock;
				hoht.InternalEvaluateAndTransitionToNormalState(true);
			}

			// Prepare the option press overlay
			if (optionPressOverlay != null) {
				GameObject pressOverlayObj = new GameObject("Press Overlay", typeof(RectTransform));
				pressOverlayObj.layer = gameObject.layer;
				pressOverlayObj.transform.localScale = Vector3.one;
				pressOverlayObj.transform.localPosition = Vector3.zero;

				// Add layout element
				LayoutElement pressLayoutElement = pressOverlayObj.AddComponent<LayoutElement>();
				pressLayoutElement.ignoreLayout = true;

				// Change parents
				pressOverlayObj.transform.SetParent(optionObject.transform, false);
				pressOverlayObj.transform.localScale = new Vector3(1f, 1f, 1f);

				// Add image
				Image poImage = pressOverlayObj.AddComponent<Image>();
				poImage.sprite = optionPressOverlay;
				poImage.color = optionPressOverlayColor;
				poImage.type = Image.Type.Sliced;

				// Apply pivot
				(pressOverlayObj.transform as RectTransform).pivot = new Vector2(0f, 1f);

				// Apply anchors
				(pressOverlayObj.transform as RectTransform).anchorMin = new Vector2(0f, 0f);
				(pressOverlayObj.transform as RectTransform).anchorMax = new Vector2(1f, 1f);

				// Apply offsets
				(pressOverlayObj.transform as RectTransform).offsetMin = new Vector2(0f, 0f);
				(pressOverlayObj.transform as RectTransform).offsetMax = new Vector2(0f, 0f);

				// Add the highlight transition component
				UISelectField_OptionOverlay poht = optionObject.AddComponent<UISelectField_OptionOverlay>();
				poht.targetGraphic = poImage;
				poht.transition = UISelectField_OptionOverlay.Transition.ColorTint;
				poht.colorBlock = optionPressOverlayColorBlock;
				poht.InternalEvaluateAndTransitionToNormalState(true);
			}

			// Initialize the option component
			optionComp.Initialize(this, text);

			// Set active if it's the selected one
			if (index == selectedOptionIndex)
				optionComp.isOn = true;

			// Register to the toggle group
			if (toggleGroup != null)
				optionComp.group = toggleGroup;

			// Hook some events
			optionComp.onSelectOption.AddListener(OnOptionSelect);
			optionComp.onPointerUp.AddListener(OnOptionPointerUp);

			// Add it to the list
			if (m_OptionObjects != null)
				m_OptionObjects.Add(optionComp);
		}

		/// <summary>
		///     Creates a separator.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>The separator game object.</returns>
		protected GameObject CreateSeparator(int index) {
			if (m_ListContentObject == null || listSeparatorSprite == null)
				return null;

			GameObject separatorObject = new GameObject("Separator " + index, typeof(RectTransform));

			// Change parent
			separatorObject.transform.SetParent(m_ListContentObject.transform, false);
			separatorObject.transform.localScale = Vector3.one;
			separatorObject.transform.localPosition = Vector3.zero;

			// Apply the sprite
			Image image = separatorObject.AddComponent<Image>();
			image.sprite = listSeparatorSprite;
			image.type = listSeparatorType;
			image.color = listSeparatorColor;

			// Apply preferred height
			LayoutElement le = separatorObject.AddComponent<LayoutElement>();
			le.preferredHeight = listSeparatorHeight > 0f ? listSeparatorHeight : listSeparatorSprite.rect.height;

			return separatorObject;
		}

		/// <summary>
		///     Does a list cleanup (Destroys the list and clears the option objects list).
		/// </summary>
		protected virtual void ListCleanup() {
			if (m_ListObject != null)
				Destroy(m_ListObject);

			m_OptionObjects.Clear();
		}

		/// <summary>
		///     Positions the list for the given direction (Auto is not handled in this method).
		/// </summary>
		/// <param name="direction">Direction.</param>
		public virtual void PositionListForDirection(Direction direction) {
			// Make sure the creating of the list was successful
			if (m_ListObject == null)
				return;

			// Get the list rect transforms
			RectTransform listRect = m_ListObject.transform as RectTransform;

			// Determine the direction of the pop
			if (direction == Direction.Auto) {
				// Get the list world corners
				Vector3[] listWorldCorner = new Vector3[4];
				listRect.GetWorldCorners(listWorldCorner);

				Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, listWorldCorner[0]);

				// Check if the list is going outside to the bottom
				if (screenPoint.y < 0f)
					direction = Direction.Up;
				else
					direction = Direction.Down;
			}

			// Handle up or down direction
			if (direction == Direction.Down) {
				listRect.SetParent(transform, true);
				listRect.pivot = new Vector2(0f, 1f);
				listRect.anchorMin = new Vector2(0f, 0f);
				listRect.anchorMax = new Vector2(0f, 0f);
				listRect.anchoredPosition = new Vector2(listRect.anchoredPosition.x, listMargins.top * -1f);

				UIUtility.BringToFront(listRect.gameObject);
			} else {
				listRect.SetParent(transform, true);
				listRect.pivot = new Vector2(0f, 0f);
				listRect.anchorMin = new Vector2(0f, 1f);
				listRect.anchorMax = new Vector2(0f, 1f);
				listRect.anchoredPosition = new Vector2(listRect.anchoredPosition.x, listMargins.bottom);

				if (m_StartSeparatorObject != null)
					m_StartSeparatorObject.transform.SetAsLastSibling();

				UIUtility.BringToFront(listRect.gameObject);
			}
		}

		/// <summary>
		///     Event invoked when the list dimensions change.
		/// </summary>
		protected virtual void ListDimensionsChanged() {
			if (!IsActive() || m_ListObject == null)
				return;

			// Check if the list size has changed
			if (m_LastListSize.Equals((m_ListObject.transform as RectTransform).sizeDelta))
				return;

			// Update the last list size
			m_LastListSize = (m_ListObject.transform as RectTransform).sizeDelta;

			// Update the list direction
			PositionListForDirection(m_Direction);
		}

		/// <summary>
		///     Event invoked when the scroll rect content dimensions change.
		/// </summary>
		protected virtual void ScrollContentDimensionsChanged() {
			if (!IsActive() || m_ScrollRect == null)
				return;

			float contentHeight = m_ScrollRect.content.sizeDelta.y;
			float optionHeight = contentHeight / m_Options.Count;
			float optionPosition = optionHeight * selectedOptionIndex;

			m_ScrollRect.content.anchoredPosition =
				new Vector2(m_ScrollRect.content.anchoredPosition.x, optionPosition);
		}

		/// <summary>
		///     Event invoked when the option list changes.
		/// </summary>
		protected virtual void OptionListChanged() {
		}

		/// <summary>
		///     Tweens the list alpha.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
		private void TweenListAlpha(float targetAlpha, float duration, bool ignoreTimeScale) {
			if (m_ListCanvasGroup == null)
				return;

			float currentAlpha = m_ListCanvasGroup.alpha;

			if (currentAlpha.Equals(targetAlpha))
				return;

			FloatTween floatTween = new FloatTween
				{duration = duration, startFloat = currentAlpha, targetFloat = targetAlpha};
			floatTween.AddOnChangedCallback(SetListAlpha);
			floatTween.AddOnFinishCallback(OnListTweenFinished);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			m_FloatTweenRunner.StartTween(floatTween);
		}

		/// <summary>
		///     Sets the list alpha.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		private void SetListAlpha(float alpha) {
			if (m_ListCanvasGroup == null)
				return;

			// Set the alpha
			m_ListCanvasGroup.alpha = alpha;
		}

		/// <summary>
		///     Triggers the list animation.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		private void TriggerListAnimation(string trigger) {
			if (m_ListObject == null || string.IsNullOrEmpty(trigger))
				return;

			Animator animator = m_ListObject.GetComponent<Animator>();

			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables)
				return;

			animator.ResetTrigger(listAnimationOpenTrigger);
			animator.ResetTrigger(listAnimationCloseTrigger);
			animator.SetTrigger(trigger);
		}

		/// <summary>
		///     Raises the list tween finished event.
		/// </summary>
		protected virtual void OnListTweenFinished() {
			// If the list is closed do a cleanup
			if (!IsOpen)
				ListCleanup();
		}

		/// <summary>
		///     Raises the list animation finish event.
		/// </summary>
		/// <param name="state">State.</param>
		protected virtual void OnListAnimationFinish(UISelectField_List.State state) {
			// If the list is closed do a cleanup
			if (state == UISelectField_List.State.Closed && !IsOpen)
				ListCleanup();
		}

		[Serializable]
		public class ChangeEvent : UnityEvent<int, string> {

		}

		[Serializable]
		public class TransitionEvent : UnityEvent<VisualState, bool> {

		}

	}
}