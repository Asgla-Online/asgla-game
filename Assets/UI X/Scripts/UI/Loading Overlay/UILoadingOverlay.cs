using System;
using System.Collections;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AsglaUI.UI {
	/// <summary>
	///     Loading Overlay
	///     This script requires to be attached to a canvas.
	///     The canvas must have the highest sorting order.
	///     The canvas must have a GraphicRaycaster.
	///     The visibility of the overlay is controlled by a CanvasGroup alpha.
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(GraphicRaycaster))]
	[AddComponentMenu("UI/Loading Overlay", 58)]
	public class UILoadingOverlay : MonoBehaviour {

		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		private int m_LoadSceneId;

		private bool m_Showing;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UILoadingOverlay() {
			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_FloatTweenRunner.Init(this);
		}

		protected void Awake() {
			DontDestroyOnLoad(gameObject);

			// Make sure it's top most ordering number
			Canvas[] canvases = FindObjectsOfType<Canvas>();
			Canvas currentCanvas = gameObject.GetComponent<Canvas>();

			foreach (Canvas canvas in canvases) // Make sure it's not our canvas1
				if (!canvas.Equals(currentCanvas))
					if (canvas.sortingOrder > currentCanvas.sortingOrder)
						currentCanvas.sortingOrder = canvas.sortingOrder + 1;

			// Update the progress bar
			if (m_ProgressBar != null)
				m_ProgressBar.fillAmount = 0f;

			// Update the canvas group
			if (m_CanvasGroup != null)
				m_CanvasGroup.alpha = 0f;
		}

		protected void OnEnable() {
			SceneManager.sceneLoaded += OnSceneFinishedLoading;
		}

		protected void OnDisable() {
			SceneManager.sceneLoaded -= OnSceneFinishedLoading;
		}

		/// <summary>
		///     Shows the loading overlay and loads the scene.
		/// </summary>
		/// <param name="sceneName">The scene name.</param>
		public void LoadScene(string sceneName) {
			Scene scene = SceneManager.GetSceneByName(sceneName);

			if (scene.IsValid())
				LoadScene(scene.buildIndex);
		}

		/// <summary>
		///     Shows the loading overlay and loads the scene.
		/// </summary>
		/// <param name="sceneIndex">The scene build index.</param>
		public void LoadScene(int sceneIndex) {
			m_Showing = true;
			m_LoadSceneId = sceneIndex;

			// Update the progress bar
			if (m_ProgressBar != null)
				m_ProgressBar.fillAmount = 0f;

			// Update the canvas group
			if (m_CanvasGroup != null)
				m_CanvasGroup.alpha = 0f;

			// Start the tween
			StartAlphaTween(1f, m_TransitionDuration, true);
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
		protected void SetCanvasAlpha(float alpha) {
			if (m_CanvasGroup == null)
				return;

			// Set the alpha
			m_CanvasGroup.alpha = alpha;
		}

		/// <summary>
		///     Raises the list tween finished event.
		/// </summary>
		protected void OnTweenFinished() {
			// When the loading overlay is shown
			if (m_Showing) {
				m_Showing = false;
				StartCoroutine(AsynchronousLoad());
			} else {
				// Destroy the loading overlay
				Destroy(gameObject);
			}
		}

		private IEnumerator AsynchronousLoad() {
			yield return null;

			AsyncOperation ao = SceneManager.LoadSceneAsync(m_LoadSceneId);
			ao.allowSceneActivation = false;

			while (!ao.isDone) {
				// [0, 0.9] > [0, 1]
				float progress = Mathf.Clamp01(ao.progress / 0.9f);

				// Update the progress bar
				if (m_ProgressBar != null)
					m_ProgressBar.fillAmount = progress;

				// Loading completed
				if (ao.progress == 0.9f)
					ao.allowSceneActivation = true;

				yield return null;
			}
		}

		private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
			if (scene.buildIndex != m_LoadSceneId)
				return;

			// Hide the loading overlay
			StartAlphaTween(0f, m_TransitionDuration, true);
		}
#pragma warning disable 0649
		[SerializeField] private UIProgressBar m_ProgressBar;
		[SerializeField] private CanvasGroup m_CanvasGroup;

		[Header("Transition")] [SerializeField]
		private TweenEasing m_TransitionEasing = TweenEasing.InOutQuint;

		[SerializeField] private float m_TransitionDuration = 0.4f;
#pragma warning restore 0649

	}
}