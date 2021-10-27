using System;
using System.Collections.Generic;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AsglaUI.UI {

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[AddComponentMenu("UI/Window", 58)]
	[RequireComponent(typeof(CanvasGroup))]
	public class UIWindow : MonoBehaviour, IEventSystemHandler, ISelectHandler, IPointerDownHandler {

		public enum EscapeKeyAction {

			None,
			Hide,
			HideIfFocused,
			Toggle

		}

		public enum Transition {

			Instant,
			Fade

		}

		public enum VisualState {

			Shown,
			Hidden

		}

		protected static UIWindow m_FucusedWindow;

		[SerializeField] private UIWindowID m_WindowId = UIWindowID.None;
		[SerializeField] private VisualState m_StartingState = VisualState.Hidden;
		[SerializeField] private EscapeKeyAction m_EscapeKeyAction = EscapeKeyAction.Hide;
		[SerializeField] private bool m_UseBlackOverlay;

		[SerializeField] private bool m_FocusAllowReparent = true;

		[SerializeField] private Transition m_Transition = Transition.Instant;
		[SerializeField] private TweenEasing m_TransitionEasing = TweenEasing.InOutQuint;
		[SerializeField] private float m_TransitionDuration = 0.1f;

		/// <summary>
		///     The transition begin (invoked when a transition begins).
		/// </summary>
		public TransitionBeginEvent onTransitionBegin = new TransitionBeginEvent();

		/// <summary>
		///     The transition complete event (invoked when a transition completes).
		/// </summary>
		public TransitionCompleteEvent onTransitionComplete = new TransitionCompleteEvent();

		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		private CanvasGroup m_CanvasGroup;

		private VisualState m_CurrentVisualState = VisualState.Hidden;
		/*[SerializeField]*/

		protected bool m_IsFocused;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UIWindow() {
			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_FloatTweenRunner.Init(this);
		}

		public static UIWindow FocusedWindow {
			get => m_FucusedWindow;
			private set => m_FucusedWindow = value;
		}

		/// <summary>
		///     Gets or sets the window identifier.
		/// </summary>
		/// <value>The window identifier.</value>
		public UIWindowID ID {
			get => m_WindowId;
			set => m_WindowId = value;
		}

		/// <summary>
		///     Gets or sets the custom window identifier.
		/// </summary>
		/// <value>The custom window identifier.</value>
		public int CustomID { get; set; }

		/// <summary>
		///     Gets or sets the escape key action.
		/// </summary>
		public EscapeKeyAction escapeKeyAction {
			get => m_EscapeKeyAction;
			set => m_EscapeKeyAction = value;
		}

		/// <summary>
		///     Gets or sets a value indicating whether this window should use the black overlay.
		/// </summary>
		public bool useBlackOverlay {
			get => m_UseBlackOverlay;
			set{
				m_UseBlackOverlay = value;

				if (Application.isPlaying && m_UseBlackOverlay && isActiveAndEnabled) {
					UIBlackOverlay overlay = UIBlackOverlay.GetOverlay(gameObject);

					if (overlay != null) {
						if (value) onTransitionBegin.AddListener(overlay.OnTransitionBegin);
						else onTransitionBegin.RemoveListener(overlay.OnTransitionBegin);
					}
				}
			}
		}

		/// <summary>
		///     Allow re-parenting on focus.
		/// </summary>
		public bool focusAllowReparent {
			get => m_FocusAllowReparent;
			set => m_FocusAllowReparent = value;
		}

		/// <summary>
		///     Gets or sets the transition.
		/// </summary>
		/// <value>The transition.</value>
		public Transition transition {
			get => m_Transition;
			set => m_Transition = value;
		}

		/// <summary>
		///     Gets or sets the transition easing.
		/// </summary>
		/// <value>The transition easing.</value>
		public TweenEasing transitionEasing {
			get => m_TransitionEasing;
			set => m_TransitionEasing = value;
		}

		/// <summary>
		///     Gets or sets the duration of the transition.
		/// </summary>
		/// <value>The duration of the transition.</value>
		public float transitionDuration {
			get => m_TransitionDuration;
			set => m_TransitionDuration = value;
		}

		/// <summary>
		///     Gets a value indicating whether this window is visible.
		/// </summary>
		/// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
		public bool IsVisible => m_CanvasGroup != null && m_CanvasGroup.alpha > 0f ? true : false;

		/// <summary>
		///     Gets a value indicating whether this window is open.
		/// </summary>
		/// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
		public bool IsOpen => m_CurrentVisualState == VisualState.Shown;

		/// <summary>
		///     Gets a value indicating whether this instance is focused.
		/// </summary>
		/// <value><c>true</c> if this instance is focused; otherwise, <c>false</c>.</value>
		public bool IsFocused => m_IsFocused;

		protected virtual void Awake() {
			// Get the canvas group
			m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();

			// Transition to the starting state
			if (Application.isPlaying)
				ApplyVisualState(m_StartingState);
		}

		protected virtual void Start() {
			// Assign new custom ID
			if (CustomID == 0)
				CustomID = NextUnusedCustomID;

			// Make sure we have a window manager in the scene if required
			if (m_EscapeKeyAction != EscapeKeyAction.None) {
				UIWindowManager manager = FindObjectOfType<UIWindowManager>();

				// Add a manager if not present
				if (manager == null) {
					GameObject newObj = new GameObject("Window Manager");
					newObj.AddComponent<UIWindowManager>();
					newObj.transform.SetAsFirstSibling();
				}
			}
		}

		protected virtual void OnEnable() {
			if (Application.isPlaying && m_UseBlackOverlay) {
				UIBlackOverlay overlay = UIBlackOverlay.GetOverlay(gameObject);

				if (overlay != null)
					onTransitionBegin.AddListener(overlay.OnTransitionBegin);
			}
		}

		protected virtual void OnDisable() {
			if (Application.isPlaying && m_UseBlackOverlay) {
				UIBlackOverlay overlay = UIBlackOverlay.GetOverlay(gameObject);

				if (overlay != null)
					onTransitionBegin.RemoveListener(overlay.OnTransitionBegin);
			}
		}

#if UNITY_EDITOR
		protected virtual void OnValidate() {
			m_TransitionDuration = Mathf.Max(m_TransitionDuration, 0f);
		}
#endif

		/// <summary>
		///     Raises the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData) {
			// Focus the window
			Focus();
		}

		/// <summary>
		///     Raises the select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSelect(BaseEventData eventData) {
			// Focus the window
			Focus();
		}

		public UIWindow Window() {
			return this;
		}

		/// <summary>
		///     Determines whether this window is active.
		/// </summary>
		/// <returns><c>true</c> if this instance is active; otherwise, <c>false</c>.</returns>
		protected virtual bool IsActive() {
			return enabled && gameObject.activeInHierarchy;
		}

		/// <summary>
		///     Focuses this window.
		/// </summary>
		public virtual void Focus() {
			if (m_IsFocused)
				return;

			// Flag as focused
			m_IsFocused = true;

			// Call the static on focused window
			OnBeforeFocusWindow(this);

			// Bring the window forward
			BringToFront();
		}

		/// <summary>
		///     Brings the window to the front.
		/// </summary>
		public void BringToFront() {
			UIUtility.BringToFront(gameObject, m_FocusAllowReparent);
		}

		/// <summary>
		///     Toggle the window Show/Hide.
		/// </summary>
		public virtual void Toggle() {
			if (m_CurrentVisualState == VisualState.Shown)
				Hide();
			else
				Show();
		}

		/// <summary>
		///     Show the window.
		/// </summary>
		public virtual void Show() {
			Show(false);
		}

		/// <summary>
		///     Show the window.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		public virtual void Show(bool instant) {
			if (!IsActive())
				return;

			// Focus the window
			Focus();

			// Check if the window is already shown
			if (m_CurrentVisualState == VisualState.Shown)
				return;

			// Transition
			EvaluateAndTransitionToVisualState(VisualState.Shown, instant);
		}

		/// <summary>
		///     Hide the window.
		/// </summary>
		public virtual void Hide() {
			Hide(false);
		}

		/// <summary>
		///     Hide the window.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		public virtual void Hide(bool instant) {
			if (!IsActive())
				return;

			// Check if the window is already hidden
			if (m_CurrentVisualState == VisualState.Hidden)
				return;

			// Transition
			EvaluateAndTransitionToVisualState(VisualState.Hidden, instant);
		}

		/// <summary>
		///     Evaluates and transitions to the specified visual state.
		/// </summary>
		/// <param name="state">The state to transition to.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void EvaluateAndTransitionToVisualState(VisualState state, bool instant) {
			float targetAlpha = state == VisualState.Shown ? 1f : 0f;

			// Call the transition begin event
			if (onTransitionBegin != null)
				onTransitionBegin.Invoke(this, state, instant || m_Transition == Transition.Instant);

			// Do the transition
			if (m_Transition == Transition.Fade) {
				float duration = instant ? 0f : m_TransitionDuration;

				// Tween the alpha
				StartAlphaTween(targetAlpha, duration, true);
			} else {
				// Set the alpha directly
				SetCanvasAlpha(targetAlpha);

				// Call the transition complete event, since it's instant
				if (onTransitionComplete != null)
					onTransitionComplete.Invoke(this, state);
			}

			// Save the state
			m_CurrentVisualState = state;

			// If we are transitioning to show, enable the canvas group raycast blocking
			if (state == VisualState.Shown) m_CanvasGroup.blocksRaycasts = true;
			//this.m_CanvasGroup.interactable = true;
		}

		/// <summary>
		///     Instantly applies the visual state.
		/// </summary>
		/// <param name="state">The state to transition to.</param>
		public virtual void ApplyVisualState(VisualState state) {
			float targetAlpha = state == VisualState.Shown ? 1f : 0f;

			// Set the alpha directly
			SetCanvasAlpha(targetAlpha);

			// Save the state
			m_CurrentVisualState = state;

			// If we are transitioning to show, enable the canvas group raycast blocking
			if (state == VisualState.Shown) m_CanvasGroup.blocksRaycasts = true;
			//this.m_CanvasGroup.interactable = true;
			else m_CanvasGroup.blocksRaycasts = false;
			//this.m_CanvasGroup.interactable = false;
		}

		/// <summary>
		///     Starts alpha tween.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
		public void StartAlphaTween(float targetAlpha, float duration, bool ignoreTimeScale) {
			if (m_CanvasGroup == null)
				return;

			FloatTween floatTween = new FloatTween
				{duration = duration, startFloat = m_CanvasGroup.alpha, targetFloat = targetAlpha};
			floatTween.AddOnChangedCallback(SetCanvasAlpha);
			floatTween.AddOnFinishCallback(OnTweenFinished);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			floatTween.easing = m_TransitionEasing;
			m_FloatTweenRunner.StartTween(floatTween);
		}

		/// <summary>
		///     Sets the canvas alpha.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		public void SetCanvasAlpha(float alpha) {
			if (m_CanvasGroup == null)
				return;

			// Set the alpha
			m_CanvasGroup.alpha = alpha;

			// If the alpha is zero, disable block raycasts
			// Enabling them back on is done in the transition method
			if (alpha == 0f) m_CanvasGroup.blocksRaycasts = false;
			//this.m_CanvasGroup.interactable = false;
		}

		/// <summary>
		///     Raises the list tween finished event.
		/// </summary>
		protected virtual void OnTweenFinished() {
			// Call the transition complete event
			if (onTransitionComplete != null)
				onTransitionComplete.Invoke(this, m_CurrentVisualState);
		}

		[Serializable]
		public class TransitionBeginEvent : UnityEvent<UIWindow, VisualState, bool> {

		}

		[Serializable]
		public class TransitionCompleteEvent : UnityEvent<UIWindow, VisualState> {

		}

		#region Static Methods

		/// <summary>
		///     Get all the windows in the scene (Including inactive).
		/// </summary>
		/// <returns>The windows.</returns>
		public static List<UIWindow> GetWindows() {
			List<UIWindow> windows = new List<UIWindow>();

			UIWindow[] ws = Resources.FindObjectsOfTypeAll<UIWindow>();

			foreach (UIWindow w in ws) // Check if the window is active in the hierarchy
				if (w.gameObject.activeInHierarchy)
					windows.Add(w);

			return windows;
		}

		public static int SortByCustomWindowID(UIWindow w1, UIWindow w2) {
			return w1.CustomID.CompareTo(w2.CustomID);
		}

		/// <summary>
		///     Gets the next unused custom ID for a window.
		/// </summary>
		/// <value>The next unused ID.</value>
		public static int NextUnusedCustomID {
			get{
				// Get the windows
				List<UIWindow> windows = GetWindows();

				if (GetWindows().Count > 0) {
					// Sort the windows by id
					windows.Sort(SortByCustomWindowID);

					// Return the last window id plus one
					return windows[windows.Count - 1].CustomID + 1;
				}

				// No windows, return 0
				return 0;
			}
		}

		/// <summary>
		///     Gets the window with the given ID.
		/// </summary>
		/// <returns>The window.</returns>
		/// <param name="id">Identifier.</param>
		public static UIWindow GetWindow(UIWindowID id) {
			// Get the windows and try finding the window with the given id
			foreach (UIWindow window in GetWindows())
				if (window.ID == id)
					return window;

			return null;
		}

		/// <summary>
		///     Gets the window with the given custom ID.
		/// </summary>
		/// <returns>The window.</returns>
		/// <param name="id">The custom identifier.</param>
		public static UIWindow GetWindowByCustomID(int customId) {
			// Get the windows and try finding the window with the given id
			foreach (UIWindow window in GetWindows())
				if (window.CustomID == customId)
					return window;

			return null;
		}

		/// <summary>
		///     Focuses the window with the given ID.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public static void FocusWindow(UIWindowID id) {
			// Focus the window
			if (GetWindow(id) != null)
				GetWindow(id).Focus();
		}

		/// <summary>
		///     Raises the before focus window event.
		/// </summary>
		/// <param name="window">The window.</param>
		protected static void OnBeforeFocusWindow(UIWindow window) {
			if (m_FucusedWindow != null)
				m_FucusedWindow.m_IsFocused = false;

			m_FucusedWindow = window;
		}

		#endregion

	}

}