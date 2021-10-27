using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AsglaUI.UI {
	[AddComponentMenu("UI/Icon Slots/Base Slot")]
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public class UISlotBase : UIBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler,
		IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
		IDropHandler {

		public enum DragKeyModifier {

			None,
			Control,
			Alt,
			Shift

		}

		public enum Transition {

			None,
			ColorTint,
			SpriteSwap,
			Animation

		}

		/// <summary>
		///     The target icon graphic.
		/// </summary>
		public Graphic iconGraphic;

		public Transition hoverTransition = Transition.None;
		public Graphic hoverTargetGraphic;
		public Color hoverNormalColor = Color.white;
		public Color hoverHighlightColor = Color.white;
		public float hoverTransitionDuration = 0.15f;
		public Sprite hoverOverrideSprite;
		public string hoverNormalTrigger = "Normal";
		public string hoverHighlightTrigger = "Highlighted";

		public Transition pressTransition = Transition.None;
		public Graphic pressTargetGraphic;
		public Color pressNormalColor = Color.white;
		public Color pressPressColor = new Color(0.6117f, 0.6117f, 0.6117f, 1f);
		public float pressTransitionDuration = 0.15f;
		public Sprite pressOverrideSprite;
		public string pressNormalTrigger = "Normal";
		public string pressPressTrigger = "Pressed";

		[SerializeField] [Tooltip("Should the pressed state transition to normal state instantly.")]
		private bool m_PressTransitionInstaOut = true;

		[SerializeField] [Tooltip("Should the pressed state force normal state transition on the hover target.")]
		private bool m_PressForceHoverNormal = true;

		private bool isPointerDown;
		private bool isPointerInside;

		/// <summary>
		///     The current dragged object.
		/// </summary>
		protected GameObject m_CurrentDraggedObject;

		/// <summary>
		///     The current dragging plane.
		/// </summary>
		protected RectTransform m_CurrentDraggingPlane;

		private bool m_DragHasBegan;
		private bool m_IsTooltipShown;

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="UISlotBase" /> drag and drop is enabled.
		/// </summary>
		/// <value><c>true</c> if drag and drop enabled; otherwise, <c>false</c>.</value>
		public bool dragAndDropEnabled {
			get => m_DragAndDropEnabled;
			set => m_DragAndDropEnabled = value;
		}

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="UISlotBase" /> is static.
		/// </summary>
		/// <value><c>true</c> if is static; otherwise, <c>false</c>.</value>
		public bool isStatic {
			get => m_IsStatic;
			set => m_IsStatic = value;
		}

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="UISlotBase" /> can be throw away.
		/// </summary>
		/// <value><c>true</c> if allow throw away; otherwise, <c>false</c>.</value>
		public bool allowThrowAway {
			get => m_AllowThrowAway;
			set => m_AllowThrowAway = value;
		}

		/// <summary>
		///     Gets or sets the drag key modifier.
		/// </summary>
		/// <value>The drag key modifier.</value>
		public DragKeyModifier dragKeyModifier {
			get => m_DragKeyModifier;
			set => m_DragKeyModifier = value;
		}

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="UISlotBase" /> tooltip should be enabled.
		/// </summary>
		/// <value><c>true</c> if tooltip enabled; otherwise, <c>false</c>.</value>
		public bool tooltipEnabled {
			get => m_TooltipEnabled;
			set => m_TooltipEnabled = value;
		}

		/// <summary>
		///     Gets or sets the tooltip delay.
		/// </summary>
		/// <value>The tooltip delay.</value>
		public float tooltipDelay {
			get => m_TooltipDelay;
			set => m_TooltipDelay = value;
		}

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="UISlotBase" /> pressed state should transition out
		///     instantly.
		/// </summary>
		/// <value><c>true</c> if press transition insta out; otherwise, <c>false</c>.</value>
		public bool pressTransitionInstaOut {
			get => m_PressTransitionInstaOut;
			set => m_PressTransitionInstaOut = value;
		}

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="UISlotBase" /> pressed state should force normal state
		///     transition on the hover target.
		/// </summary>
		/// <value><c>true</c> if press force hover normal; otherwise, <c>false</c>.</value>
		public bool pressForceHoverNormal {
			get => m_PressForceHoverNormal;
			set => m_PressForceHoverNormal = value;
		}

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="UISlotBase" /> drop was preformed.
		/// </summary>
		/// <value><c>true</c> if drop preformed; otherwise, <c>false</c>.</value>
		public bool dropPreformed { get; set; }

		protected override void Start() {
			// Check if the slot is not assigned but the icon graphic is active
			if (!IsAssigned() && iconGraphic != null &&
			    iconGraphic.gameObject.activeSelf) // Disable the icon graphic object
				iconGraphic.gameObject.SetActive(false);
		}

		protected override void OnEnable() {
			base.OnEnable();

			// Instant transition
			EvaluateAndTransitionHoveredState(true);
			EvaluateAndTransitionPressedState(true);
		}

		protected override void OnDisable() {
			base.OnDisable();

			isPointerInside = false;
			isPointerDown = false;

			// Instant transition
			EvaluateAndTransitionHoveredState(true);
			EvaluateAndTransitionPressedState(true);
		}

#if UNITY_EDITOR
		protected override void OnValidate() {
			hoverTransitionDuration = Mathf.Max(hoverTransitionDuration, 0f);
			pressTransitionDuration = Mathf.Max(pressTransitionDuration, 0f);

			if (isActiveAndEnabled) {
				DoSpriteSwap(hoverTargetGraphic, null);
				DoSpriteSwap(pressTargetGraphic, null);

				if (!EditorApplication.isPlayingOrWillChangePlaymode) {
					// Instant transition
					EvaluateAndTransitionHoveredState(true);
					EvaluateAndTransitionPressedState(true);
				} else {
					// Regular transition
					EvaluateAndTransitionHoveredState(false);
					EvaluateAndTransitionPressedState(false);
				}
			}
		}
#endif

		/// <summary>
		///     Raises the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData) {
			if (!enabled || !IsAssigned() || !m_DragAndDropEnabled) {
				eventData.Reset();
				return;
			}

			// Check if we have a key modifier and if it's held down
			if (!DragKeyModifierIsDown()) {
				eventData.Reset();
				return;
			}

			// Start the drag
			m_DragHasBegan = true;

			// Create the temporary icon for dragging
			CreateTemporaryIcon(eventData);

			// Prevent event propagation
			eventData.Use();
		}

		/// <summary>
		///     Raises the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData) {
			// Check if the dragging has been started
			if (m_DragHasBegan) // Update the dragged object's position
				if (m_CurrentDraggedObject != null)
					UpdateDraggedPosition(eventData);
		}

		/// <summary>
		///     Raises the drop event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrop(PointerEventData eventData) {
			// Get the source slot
			UISlotBase source = eventData.pointerPress != null
				? eventData.pointerPress.GetComponent<UISlotBase>()
				: null;

			// Make sure we have the source slot
			if (source == null || !source.IsAssigned() || !source.dragAndDropEnabled)
				return;

			// Notify the source that a drop was performed so it does not unassign
			source.dropPreformed = true;

			// Check if this slot is enabled and it's drag and drop feature is enabled
			if (!enabled || !m_DragAndDropEnabled)
				return;

			// Prepare a variable indicating whether the assign process was successful
			bool assignSuccess = false;

			// Normal empty slot assignment
			if (!IsAssigned()) {
				// Assign the target slot with the info from the source
				assignSuccess = Assign(source);

				Debug.Log("UISlotBase 1 > " + assignSuccess);

				// Unassign the source on successful assignment and the source is not static
				if (assignSuccess && !source.isStatic)
					source.Unassign();
			}
			// The target slot is assigned
			else {
				// If the target slot is not static
				// and we have a source slot that is not static
				if (!isStatic && !source.isStatic) {
					// Check if we can swap
					if (CanSwapWith(source) && source.CanSwapWith(this)) {
						// Swap the slots
						assignSuccess = source.PerformSlotSwap(this);
						Debug.Log("UISlotBase 2 > " + assignSuccess);
					}
				}
				// If the target slot is not static
				// and the source slot is a static one
				else if (!isStatic && source.isStatic) {
					assignSuccess = Assign(source);
					Debug.Log("UISlotBase 3 > " + assignSuccess);
				}
			}

			// If this slot failed to be assigned
			if (!assignSuccess)
				OnAssignBySlotFailed(source);
		}

		/// <summary>
		///     Raises the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData) {
			// Check if a drag was initialized at all
			if (!m_DragHasBegan)
				return;

			// Reset the drag begin bool
			m_DragHasBegan = false;

			// Destroy the dragged icon object
			if (m_CurrentDraggedObject != null)
				Destroy(m_CurrentDraggedObject);

			// Reset the variables
			m_CurrentDraggedObject = null;
			m_CurrentDraggingPlane = null;

			// Check if we are returning the icon to the same slot
			// By checking if the slot is highlighted
			if (IsHighlighted(eventData))
				return;

			// Check if no drop was preformed
			if (!dropPreformed) // Try to throw away the assigned content
				OnThrowAway();
			else // Reset the drop preformed variable
				dropPreformed = false;
		}

		/// <summary>
		///     Raises the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData) {
		}

		/// <summary>
		///     Raises the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData) {
			isPointerDown = true;
			EvaluateAndTransitionPressedState(false);

			// Hide the tooltip
			InternalHideTooltip();
		}

		/// <summary>
		///     Raises the pointer enter event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerEnter(PointerEventData eventData) {
			isPointerInside = true;
			EvaluateAndTransitionHoveredState(false);

			// Check if tooltip is enabled
			if (enabled && IsActive() && m_TooltipEnabled) {
				// Start the tooltip delayed show coroutine
				// If delay is set at all
				if (m_TooltipDelay > 0f)
					StartCoroutine("TooltipDelayedShow");
				else
					InternalShowTooltip();
			}
		}

		/// <summary>
		///     Raises the pointer exit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerExit(PointerEventData eventData) {
			isPointerInside = false;
			EvaluateAndTransitionHoveredState(false);
			InternalHideTooltip();
		}

		/// <summary>
		///     Raises the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData) {
			isPointerDown = false;
			EvaluateAndTransitionPressedState(m_PressTransitionInstaOut);
		}

		/// <summary>
		///     Raises the tooltip event.
		/// </summary>
		/// <param name="show">If set to <c>true</c> show.</param>
		public virtual void OnTooltip(bool show) {
		}

		/// <summary>
		///     Determines whether this slot is highlighted based on the specified eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance is highlighted the specified eventData; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		protected bool IsHighlighted(BaseEventData eventData) {
			if (!IsActive())
				return false;

			if (eventData is PointerEventData) {
				PointerEventData pointerEventData = eventData as PointerEventData;
				return isPointerDown && !isPointerInside && pointerEventData.pointerPress == gameObject ||
				       !isPointerDown && isPointerInside && pointerEventData.pointerPress == gameObject ||
				       !isPointerDown && isPointerInside && pointerEventData.pointerPress == null;
			}

			return false;
		}

		/// <summary>
		///     Determines whether this slot is pressed based on the specified eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance is pressed the specified eventData; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Event data.</param>
		protected bool IsPressed(BaseEventData eventData) {
			return IsActive() && isPointerInside && isPointerDown;
		}

		/// <summary>
		///     Evaluates and transitions the hovered state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void EvaluateAndTransitionHoveredState(bool instant) {
			if (!IsActive() || hoverTargetGraphic == null || !hoverTargetGraphic.gameObject.activeInHierarchy)
				return;

			// Determine what should the state of the hover target be
			bool highlighted = m_PressForceHoverNormal ? isPointerInside && !isPointerDown : isPointerInside;

			// Do the transition
			switch (hoverTransition) {
				case Transition.ColorTint: {
					StartColorTween(hoverTargetGraphic, highlighted ? hoverHighlightColor : hoverNormalColor,
						instant ? 0f : hoverTransitionDuration);
					break;
				}
				case Transition.SpriteSwap: {
					DoSpriteSwap(hoverTargetGraphic, highlighted ? hoverOverrideSprite : null);
					break;
				}
				case Transition.Animation: {
					TriggerHoverStateAnimation(highlighted ? hoverHighlightTrigger : hoverNormalTrigger);
					break;
				}
			}
		}

		/// <summary>
		///     Evaluates and transitions the pressed state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void EvaluateAndTransitionPressedState(bool instant) {
			if (!IsActive() || pressTargetGraphic == null || !pressTargetGraphic.gameObject.activeInHierarchy)
				return;

			// Do the transition
			switch (pressTransition) {
				case Transition.ColorTint: {
					StartColorTween(pressTargetGraphic, isPointerDown ? pressPressColor : pressNormalColor,
						instant ? 0f : pressTransitionDuration);
					break;
				}
				case Transition.SpriteSwap: {
					DoSpriteSwap(pressTargetGraphic, isPointerDown ? pressOverrideSprite : null);
					break;
				}
				case Transition.Animation: {
					TriggerPressStateAnimation(isPointerDown ? pressPressTrigger : pressNormalTrigger);
					break;
				}
			}

			// If we should force normal state transition on the hover target
			if (m_PressForceHoverNormal)
				EvaluateAndTransitionHoveredState(false);
		}

		/// <summary>
		///     Starts a color tween.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="targetColor">Target color.</param>
		/// <param name="duration">Duration.</param>
		protected virtual void StartColorTween(Graphic target, Color targetColor, float duration) {
			if (target == null)
				return;

			target.CrossFadeColor(targetColor, duration, true, true);
		}

		/// <summary>
		///     Does a sprite swap.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="newSprite">New sprite.</param>
		protected virtual void DoSpriteSwap(Graphic target, Sprite newSprite) {
			if (target == null)
				return;

			Image image = target as Image;

			if (image == null)
				return;

			image.overrideSprite = newSprite;
		}

		/// <summary>
		///     Triggers the hover state animation.
		/// </summary>
		/// <param name="triggername">Triggername.</param>
		private void TriggerHoverStateAnimation(string triggername) {
			if (hoverTargetGraphic == null)
				return;

			// Get the animator on the target game object
			Animator animator = hoverTargetGraphic.gameObject.GetComponent<Animator>();

			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(triggername))
				return;

			animator.ResetTrigger(hoverNormalTrigger);
			animator.ResetTrigger(hoverHighlightTrigger);
			animator.SetTrigger(triggername);
		}

		/// <summary>
		///     Triggers the pressed state animation.
		/// </summary>
		/// <param name="triggername">Triggername.</param>
		private void TriggerPressStateAnimation(string triggername) {
			if (pressTargetGraphic == null)
				return;

			// Get the animator on the target game object
			Animator animator = pressTargetGraphic.gameObject.GetComponent<Animator>();

			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(triggername))
				return;

			animator.ResetTrigger(pressNormalTrigger);
			animator.ResetTrigger(pressPressTrigger);
			animator.SetTrigger(triggername);
		}

		/// <summary>
		///     Determines whether this slot is assigned.
		/// </summary>
		/// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
		public virtual bool IsAssigned() {
			return GetIconSprite() != null || GetIconTexture() != null;
		}

		/// <summary>
		///     Assign the specified slot by icon sprite.
		/// </summary>
		/// <param name="icon">Icon.</param>
		public bool Assign(Sprite icon) {
			if (icon == null)
				return false;

			// Set the icon
			SetIcon(icon);

			return true;
		}

		/// <summary>
		///     Assign the specified slot by icon texture.
		/// </summary>
		/// <param name="icon">Icon.</param>
		public bool Assign(Texture icon) {
			if (icon == null)
				return false;

			// Set the icon
			SetIcon(icon);

			return true;
		}

		/// <summary>
		///     Assign the specified slot by object.
		/// </summary>
		/// <param name="source">Source.</param>
		public virtual bool Assign(Object source) {
			if (source is UISlotBase) {
				UISlotBase sourceSlot = source as UISlotBase;

				if (sourceSlot != null) {
					// Assign by sprite or texture
					if (sourceSlot.GetIconSprite() != null)
						return Assign(sourceSlot.GetIconSprite());
					if (sourceSlot.GetIconTexture() != null)
						return Assign(sourceSlot.GetIconTexture());
				}
			}

			return false;
		}

		/// <summary>
		///     Unassign this slot.
		/// </summary>
		public virtual void Unassign() {
			// Remove the icon
			ClearIcon();
		}

		/// <summary>
		///     Gets the icon sprite of this slot if it's set and the icon graphic is <see cref="UnityEngine.UI.Image" />.
		/// </summary>
		/// <returns>The icon.</returns>
		public Sprite GetIconSprite() {
			// Check if the icon graphic valid image
			if (iconGraphic == null || !(iconGraphic is Image))
				return null;

			return (iconGraphic as Image).sprite;
		}

		/// <summary>
		///     Gets the icon texture of this slot if it's set and the icon graphic is <see cref="UnityEngine.UI.RawImage" />.
		/// </summary>
		/// <returns>The icon.</returns>
		public Texture GetIconTexture() {
			// Check if the icon graphic valid image
			if (iconGraphic == null || !(iconGraphic is RawImage))
				return null;

			return (iconGraphic as RawImage).texture;
		}

		/// <summary>
		///     Gets the icon as object.
		/// </summary>
		/// <returns>The icon as object.</returns>
		public Object GetIconAsObject() {
			if (iconGraphic == null)
				return null;

			if (iconGraphic is Image)
				return GetIconSprite();
			if (iconGraphic is RawImage)
				return GetIconTexture();

			// Default
			return null;
		}

		/// <summary>
		///     Sets the icon of this slot.
		/// </summary>
		/// <param name="iconSprite">The icon sprite.</param>
		public void SetIcon(Sprite iconSprite) {
			// Check if the icon graphic valid image
			if (iconGraphic == null || !(iconGraphic is Image))
				return;

			// Set the sprite
			(iconGraphic as Image).sprite = iconSprite;

			// Enable or disabled the icon graphic game object
			if (iconSprite != null && !iconGraphic.gameObject.activeSelf) iconGraphic.gameObject.SetActive(true);
			if (iconSprite == null && iconGraphic.gameObject.activeSelf) iconGraphic.gameObject.SetActive(false);
		}

		/// <summary>
		///     Sets the icon of this slot.
		/// </summary>
		/// <param name="iconTex">The icon texture.</param>
		public void SetIcon(Texture iconTex) {
			// Check if the icon graphic valid raw image
			if (iconGraphic == null || !(iconGraphic is RawImage))
				return;

			// Set the sprite
			(iconGraphic as RawImage).texture = iconTex;

			// Enable or disabled the icon graphic game object
			if (iconTex != null && !iconGraphic.gameObject.activeSelf) iconGraphic.gameObject.SetActive(true);
			if (iconTex == null && iconGraphic.gameObject.activeSelf) iconGraphic.gameObject.SetActive(false);
		}

		/// <summary>
		///     Clears the icon of this slot.
		/// </summary>
		public void ClearIcon() {
			// Check if the icon graphic valid
			if (iconGraphic == null)
				return;

			// In case of image
			if (iconGraphic is Image)
				(iconGraphic as Image).sprite = null;

			// In case of raw image
			if (iconGraphic is RawImage)
				(iconGraphic as RawImage).texture = null;

			// Disable the game object
			iconGraphic.gameObject.SetActive(false);
		}

		/// <summary>
		///     Is the drag key modifier down.
		/// </summary>
		/// <returns><c>true</c>, if key modifier is down, <c>false</c> otherwise.</returns>
		public virtual bool DragKeyModifierIsDown() {
			switch (m_DragKeyModifier) {
				case DragKeyModifier.Control:
					return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				case DragKeyModifier.Alt:
					return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
				case DragKeyModifier.Shift:
					return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			}

			// Default should be true
			return true;
		}

		/// <summary>
		///     Determines whether this slot can swap with the specified target slot.
		/// </summary>
		/// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
		/// <param name="target">Target.</param>
		public virtual bool CanSwapWith(Object target) {
			return target is UISlotBase;
		}

		/// <summary>
		///     Performs a slot swap.
		/// </summary>
		/// <param name="targetObject">Target slot.</param>
		public virtual bool PerformSlotSwap(Object targetObject) {
			// Get the source slot
			UISlotBase targetSlot = targetObject as UISlotBase;

			// Get the target slot icon
			Object targetIcon = targetSlot.GetIconAsObject();

			// Assign the target slot with this one
			bool assign1 = targetSlot.Assign(this);

			// Assign this slot by the target slot icon
			bool assign2 = Assign(targetIcon);

			// Return the status
			return assign1 && assign2;
		}

		/// <summary>
		///     Called when the slot fails to assign by another slot.
		/// </summary>
		protected virtual void OnAssignBySlotFailed(Object source) {
			Debug.Log("UISlotBase (" + gameObject.name + ") failed to get assigned by (" +
			          (source as UISlotBase).gameObject.name + ").");
		}

		/// <summary>
		///     This method is raised to confirm throwing away the slot.
		/// </summary>
		protected virtual void OnThrowAway() {
			// Check if throwing away is allowed
			if (m_AllowThrowAway) // Throw away successful, unassign the slot
				Unassign();
			else // Throw away was denied
				OnThrowAwayDenied();
		}

		/// <summary>
		///     This method is raised when the slot is denied to be thrown away and returned to it's source.
		/// </summary>
		protected virtual void OnThrowAwayDenied() {
		}

		/// <summary>
		///     Creates the temporary icon.
		/// </summary>
		/// <returns>The temporary icon.</returns>
		protected virtual void CreateTemporaryIcon(PointerEventData eventData) {
			Canvas canvas = UIUtility.FindInParents<Canvas>(gameObject);

			if (canvas == null || iconGraphic == null)
				return;

			// Create temporary panel
			GameObject iconObj = Instantiate(m_CloneTarget == null ? iconGraphic.gameObject : m_CloneTarget);

			iconObj.transform.localScale = new Vector3(1f, 1f, 1f);
			iconObj.transform.SetParent(canvas.transform, false);
			iconObj.transform.SetAsLastSibling();
			(iconObj.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);

			// The icon will be under the cursor.
			// We want it to be ignored by the event system.
			iconObj.AddComponent<UIIgnoreRaycast>();

			// Save the dragging plane
			m_CurrentDraggingPlane = canvas.transform as RectTransform;

			// Set as the current dragging object
			m_CurrentDraggedObject = iconObj;

			// Update the icon position
			UpdateDraggedPosition(eventData);
		}

		/// <summary>
		///     Updates the dragged icon position.
		/// </summary>
		/// <param name="data">Data.</param>
		private void UpdateDraggedPosition(PointerEventData data) {
			RectTransform rt = m_CurrentDraggedObject.GetComponent<RectTransform>();
			Vector3 globalMousePos;

			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_CurrentDraggingPlane, data.position,
				data.pressEventCamera, out globalMousePos)) {
				rt.position = globalMousePos;
				rt.rotation = m_CurrentDraggingPlane.rotation;
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
			StopCoroutine("TooltipDelayedShow");

			// Call the on tooltip only if it's currently shown
			if (m_IsTooltipShown) {
				m_IsTooltipShown = false;
				OnTooltip(false);
			}
		}

		protected IEnumerator TooltipDelayedShow() {
			yield return new WaitForSeconds(m_TooltipDelay);
			InternalShowTooltip();
		}

#pragma warning disable 0649
		[SerializeField] [Tooltip("The game object that should be cloned on drag.")]
		private GameObject m_CloneTarget;

		[SerializeField] [Tooltip("Should the drag and drop functionallty be enabled.")]
		private bool m_DragAndDropEnabled = true;

		[SerializeField] [Tooltip("If set to static the slot won't be unassigned when drag and drop is preformed.")]
		private bool m_IsStatic;

		[SerializeField] [Tooltip("Should the icon assigned to the slot be throwable.")]
		private bool m_AllowThrowAway = true;

		[SerializeField] [Tooltip("The key which should be held down in order to begin the drag.")]
		private DragKeyModifier m_DragKeyModifier = DragKeyModifier.None;

		[SerializeField] [Tooltip("Should the tooltip functionallty be enabled.")]
		private bool m_TooltipEnabled = true;

		[SerializeField] [Tooltip("How long of a delay to expect before showing the tooltip.")]
		private float m_TooltipDelay = 1f;
#pragma warning restore 0649

	}
}