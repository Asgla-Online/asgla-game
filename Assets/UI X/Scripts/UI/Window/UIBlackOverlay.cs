using System;
using AsglaUI.UI.Tweens;
using UnityEngine;

namespace AsglaUI.UI {
	/// <summary>
	///     The black overlay used behind windows such as the game menu.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class UIBlackOverlay : MonoBehaviour {

		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		private CanvasGroup m_CanvasGroup;

		private bool m_Transitioning;
		private int m_WindowsCount;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UIBlackOverlay() {
			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_FloatTweenRunner.Init(this);
		}

		protected void Awake() {
			m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
			m_CanvasGroup.interactable = false;
			m_CanvasGroup.blocksRaycasts = false;
		}

		protected void Start() {
			// Non interactable
			m_CanvasGroup.interactable = false;

			// Hide the overlay
			Hide();
		}

		protected void OnEnable() {
			// Hide the overlay
			if (!Application.isPlaying)
				Hide();
		}

		/// <summary>
		///     Instantly show the overlay.
		/// </summary>
		public void Show() {
			// Show the overlay
			SetAlpha(1f);

			// Toggle block raycast on
			m_CanvasGroup.blocksRaycasts = true;
		}

		/// <summary>
		///     Instantly hide the overlay.
		/// </summary>
		public void Hide() {
			// Hide the overlay
			SetAlpha(0f);

			// Toggle block raycast off
			m_CanvasGroup.blocksRaycasts = false;
		}

		/// <summary>
		///     If the overlay is enabled and active in the hierarchy.
		/// </summary>
		/// <returns></returns>
		public bool IsActive() {
			return enabled && gameObject.activeInHierarchy;
		}

		/// <summary>
		///     If the overlay is visible at all (alpha > 0f).
		/// </summary>
		/// <returns></returns>
		public bool IsVisible() {
			return m_CanvasGroup.alpha > 0f;
		}

		/// <summary>
		///     This method is hooked to the window on transition begin event.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <param name="state">The window visual state that we are transitioning to.</param>
		/// <param name="instant">If the transition is instant or not.</param>
		public void OnTransitionBegin(UIWindow window, UIWindow.VisualState state, bool instant) {
			if (!IsActive() || window == null)
				return;

			// Check if we are receiving hide event and we are not showing the overlay to begin with, return
			if (state == UIWindow.VisualState.Hidden && !IsVisible())
				return;

			// Prepare transition duration
			float duration = instant ? 0f : window.transitionDuration;
			TweenEasing easing = window.transitionEasing;

			// Showing a window
			if (state == UIWindow.VisualState.Shown) {
				// Increase the window count so we know when to hide the overlay
				m_WindowsCount += 1;

				// Check if the overlay is already visible
				if (IsVisible() && !m_Transitioning) {
					// Bring the window forward
					UIUtility.BringToFront(window.gameObject);

					// Break
					return;
				}

				// Bring the overlay forward
				UIUtility.BringToFront(gameObject);

				// Bring the window forward
				UIUtility.BringToFront(window.gameObject);

				// Transition
				StartAlphaTween(1f, duration, easing);

				// Toggle block raycast on
				m_CanvasGroup.blocksRaycasts = true;
			}
			// Hiding a window
			else {
				// Decrease the window count
				m_WindowsCount -= 1;

				// Never go below 0
				if (m_WindowsCount < 0)
					m_WindowsCount = 0;

				// Check if we still have windows using the overlay
				if (m_WindowsCount > 0)
					return;

				// Transition
				StartAlphaTween(0f, duration, easing);

				// Toggle block raycast on
				m_CanvasGroup.blocksRaycasts = false;
			}
		}

		private void StartAlphaTween(float targetAlpha, float duration, TweenEasing easing) {
			if (m_CanvasGroup == null)
				return;

			// Check if currently transitioning
			if (m_Transitioning)
				m_FloatTweenRunner.StopTween();

			if (duration == 0f || !Application.isPlaying) {
				SetAlpha(targetAlpha);
			} else {
				m_Transitioning = true;

				FloatTween floatTween = new FloatTween
					{duration = duration, startFloat = m_CanvasGroup.alpha, targetFloat = targetAlpha};
				floatTween.AddOnChangedCallback(SetAlpha);
				floatTween.ignoreTimeScale = true;
				floatTween.easing = easing;
				floatTween.AddOnFinishCallback(OnTweenFinished);

				m_FloatTweenRunner.StartTween(floatTween);
			}
		}

		public void SetAlpha(float alpha) {
			if (m_CanvasGroup != null)
				m_CanvasGroup.alpha = alpha;
		}

		protected void OnTweenFinished() {
			m_Transitioning = false;
		}

		/// <summary>
		///     Gets the black overlay based on relative game object. (In case we have multiple canvases.)
		/// </summary>
		/// <param name="relativeGameObject">The relative game object.</param>
		/// <returns>The black overlay component.</returns>
		public static UIBlackOverlay GetOverlay(GameObject relativeGameObject) {
			// Find the black overlay in the current canvas
			Canvas canvas = UIUtility.FindInParents<Canvas>(relativeGameObject);

			if (canvas != null) {
				// Try finding an overlay in the canvas
				UIBlackOverlay overlay = canvas.gameObject.GetComponentInChildren<UIBlackOverlay>();

				if (overlay != null)
					return overlay;

				// In case no overlay is found instantiate one
				if (UIBlackOverlayManager.Instance != null && UIBlackOverlayManager.Instance.prefab != null)
					return UIBlackOverlayManager.Instance.Create(canvas.transform);
			}

			return null;
		}

	}
}