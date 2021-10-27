using System;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AsglaUI.UI {
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	[AddComponentMenu("UI/UI Scene/Scene")]
	public class UIScene : MonoBehaviour {

		public enum Transition {

			None,
			Animation,
			CrossFade,
			SlideFromRight,
			SlideFromLeft,
			SlideFromTop,
			SlideFromBottom

		}

		public enum Type {

			Preloaded,
			Prefab,
			Resource

		}

		/// <summary>
		///     Invoked when the scene is activated.
		/// </summary>
		public OnActivateEvent onActivate = new OnActivateEvent();

		/// <summary>
		///     Invoked when the scene is deactivated.
		/// </summary>
		public OnDeactivateEvent onDeactivate = new OnDeactivateEvent();

		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		private bool m_AnimationState;

		// The canvas group
		private CanvasGroup m_CanvasGroup;

		private UISceneRegistry m_SceneManager;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UIScene() {
			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_FloatTweenRunner.Init(this);
		}

		/// <summary>
		///     Gets the scene id.
		/// </summary>
		public int id => m_Id;

		/// <summary>
		///     Gets or sets value indicating whether the scene is activated.
		/// </summary>
		public bool isActivated {
			get => m_IsActivated;
			set{
				if (value)
					Activate();
				else
					Deactivate();
			}
		}

		/// <summary>
		///     Gets the scene type.
		/// </summary>
		public Type type => m_Type;

		/// <summary>
		///     Gets or sets the scene content holder.
		/// </summary>
		public Transform content {
			get => m_Content;
			set => m_Content = value;
		}

		/// <summary>
		///     Gets or sets the scene transition.
		/// </summary>
		public Transition transition {
			get => m_Transition;
			set => m_Transition = value;
		}

		/// <summary>
		///     Gets or sets the transition duration.
		/// </summary>
		public float transitionDuration {
			get => m_TransitionDuration;
			set => m_TransitionDuration = value;
		}

		/// <summary>
		///     Gets or set the transition easing.
		/// </summary>
		public TweenEasing transitionEasing {
			get => m_TransitionEasing;
			set => m_TransitionEasing = value;
		}

		/// <summary>
		///     Gets or sets the animate in trigger.
		/// </summary>
		public string animateInTrigger {
			get => m_AnimateInTrigger;
			set => m_AnimateInTrigger = value;
		}

		/// <summary>
		///     Gets or sets the animate out trigger.
		/// </summary>
		public string animateOutTrigger {
			get => m_AnimateOutTrigger;
			set => m_AnimateOutTrigger = value;
		}

		/// <summary>
		///     Gets the rect transform.
		/// </summary>
		public RectTransform rectTransform => transform as RectTransform;

		/// <summary>
		///     Gets the animator.
		/// </summary>
		/// <value>The animator.</value>
		public Animator animator => gameObject.GetComponent<Animator>();

		protected virtual void Awake() {
			// Get the scene mangaer
			m_SceneManager = UISceneRegistry.instance;

			if (m_SceneManager == null) {
				Debug.LogWarning("Scene registry does not exist.");
				enabled = false;
				return;
			}

			// Set the initial animation state
			m_AnimationState = m_IsActivated;

			// Get the canvas group
			m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();

			// Set the first selected game object for the navigation
			if (Application.isPlaying && isActivated && isActiveAndEnabled && m_FirstSelected != null)
				EventSystem.current.SetSelectedGameObject(m_FirstSelected);
		}

		protected virtual void Start() {
		}

		protected void Update() {
			if (animator != null && !string.IsNullOrEmpty(m_AnimateInTrigger) &&
			    !string.IsNullOrEmpty(m_AnimateOutTrigger)) {
				AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

				// Check which is the current state
				if (state.IsName(m_AnimateInTrigger) && !m_AnimationState) {
					if (state.normalizedTime >= state.length) {
						// Flag as opened
						m_AnimationState = true;

						// On animation finished
						OnTransitionIn();
					}
				} else if (state.IsName(m_AnimateOutTrigger) && m_AnimationState) {
					if (state.normalizedTime >= state.length) {
						// Flag as closed
						m_AnimationState = false;

						// On animation finished
						OnTransitionOut();
					}
				}
			}
		}

		protected virtual void OnEnable() {
			// Register the scene
			if (m_SceneManager != null)
				// Register only if in the scene
				if (gameObject.activeInHierarchy)
					m_SceneManager.RegisterScene(this);

			// Trigger the on activate event
			if (isActivated && onActivate != null)
				onActivate.Invoke(this);
		}

		protected virtual void OnDisable() {
			// Unregister the scene
			if (m_SceneManager != null)
				m_SceneManager.UnregisterScene(this);
		}

#if UNITY_EDITOR
		protected virtual void OnValidate() {
			// Check for duplicate id
			if (m_SceneManager != null) {
				UIScene[] scenes = m_SceneManager.scenes;

				foreach (UIScene scene in scenes)
					if (!scene.Equals(this) && scene.id == m_Id) {
						Debug.LogWarning("Duplicate scene ids, change the id of scene: " + gameObject.name);
						m_Id = m_SceneManager.GetAvailableSceneId();
						break;
					}
			}

			// Activate or deactivate the scene
			if (m_Type == Type.Preloaded) {
				if (m_IsActivated) {
					// Enable the game object
					if (m_Content != null)
						m_Content.gameObject.SetActive(true);
				} else {
					// Disable the game object
					if (m_Content != null)
						m_Content.gameObject.SetActive(false);
				}
			}

			m_TransitionDuration = Mathf.Max(m_TransitionDuration, 0f);
		}
#endif

		/// <summary>
		///     Activate the scene, no transition is used.
		/// </summary>
		public void Activate() {
			// Make sure the scene is active and enabled
			if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
				return;

			// If it's prefab
			if (m_Type == Type.Prefab || m_Type == Type.Resource) {
				GameObject prefab = null;

				if (m_Type == Type.Prefab) {
					// Check the prefab
					if (m_Prefab == null) {
						Debug.LogWarning("You are activating a prefab scene and no prefab is specified.");
						return;
					}

					prefab = m_Prefab;
				}

				if (m_Type == Type.Resource) {
					// Try loading the resource
					if (string.IsNullOrEmpty(m_Resource)) {
						Debug.LogWarning("You are activating a resource scene and no resource path is specified.");
						return;
					}

					prefab = Resources.Load<GameObject>(m_Resource);
				}

				// Instantiate the prefab
				if (prefab != null) {
					// Instantiate the prefab
					GameObject obj = Instantiate(prefab);

					// Set the content variable
					m_Content = obj.transform;

					// Set parent
					m_Content.SetParent(transform);

					// Check if it's a rect transform
					if (m_Content is RectTransform) {
						// Get the rect transform
						RectTransform rectTransform = m_Content as RectTransform;

						// Prepare the rect
						rectTransform.localScale = Vector3.one;
						rectTransform.localPosition = Vector3.zero;

						// Set anchor and pivot
						rectTransform.anchorMin = new Vector2(0f, 0f);
						rectTransform.anchorMax = new Vector2(1f, 1f);
						rectTransform.pivot = new Vector2(0.5f, 0.5f);

						// Get the canvas size
						Canvas canvas = UIUtility.FindInParents<Canvas>(gameObject);

						if (canvas == null)
							canvas = gameObject.GetComponentInChildren<Canvas>();

						if (canvas != null) {
							RectTransform crt = canvas.transform as RectTransform;

							rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, crt.sizeDelta.x);
							rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, crt.sizeDelta.y);
						}

						// Set position
						rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
					}
				}
			}

			// Enable the game object
			if (m_Content != null)
				m_Content.gameObject.SetActive(true);

			// Set the first selected for the navigation
			if (isActiveAndEnabled && m_FirstSelected != null)
				EventSystem.current.SetSelectedGameObject(m_FirstSelected);

			// Set the active variable
			m_IsActivated = true;

			// Invoke the event
			if (onActivate != null)
				onActivate.Invoke(this);
		}

		/// <summary>
		///     Deactivate the scene, no transition is used.
		/// </summary>
		public void Deactivate() {
			// Disable the game object
			if (m_Content != null)
				m_Content.gameObject.SetActive(false);

			// If prefab destroy the object
			if (m_Type == Type.Prefab || m_Type == Type.Resource)
				Destroy(m_Content.gameObject);

			// Unload unused assets
			Resources.UnloadUnusedAssets();

			// Set the active variable
			m_IsActivated = false;

			// Invoke the event
			if (onDeactivate != null)
				onDeactivate.Invoke(this);
		}

		/// <summary>
		///     Transition to the scene.
		/// </summary>
		public void TransitionTo() {
			// Make sure the scene is active and enabled
			if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
				return;

			if (m_SceneManager != null)
				m_SceneManager.TransitionToScene(this);
		}

		/// <summary>
		///     Transition the scene in.
		/// </summary>
		public void TransitionIn() {
			TransitionIn(m_Transition, m_TransitionDuration, m_TransitionEasing);
		}

		/// <summary>
		///     Transition the scene in.
		/// </summary>
		/// <param name="transition">The transition.</param>
		/// <param name="duration">The transition duration.</param>
		/// <param name="easing">The transition easing.</param>
		public void TransitionIn(Transition transition, float duration, TweenEasing easing) {
			// Make sure the scene is active and enabled
			if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
				return;

			if (m_CanvasGroup == null)
				return;

			// If no transition is used
			if (transition == Transition.None) {
				Activate();
				return;
			}

			// If the transition is animation
			if (transition == Transition.Animation) {
				Activate();
				TriggerAnimation(m_AnimateInTrigger);
				return;
			}

			// Make the scene non interactable
			//this.m_CanvasGroup.interactable = false;
			//this.m_CanvasGroup.blocksRaycasts = false;

			// Prepare some variable
			Vector2 rectSize = rectTransform.rect.size;

			// Prepare the rect transform
			if (transition == Transition.SlideFromLeft || transition == Transition.SlideFromRight ||
			    transition == Transition.SlideFromTop || transition == Transition.SlideFromBottom) {
				// Anchor and pivot top left
				rectTransform.pivot = new Vector2(0f, 1f);
				rectTransform.anchorMin = new Vector2(0f, 1f);
				rectTransform.anchorMax = new Vector2(0f, 1f);
				rectTransform.sizeDelta = rectSize;
			}

			// Prepare the tween
			FloatTween floatTween = new FloatTween();
			floatTween.duration = duration;

			switch (transition) {
				case Transition.CrossFade:
					m_CanvasGroup.alpha = 0f;
					floatTween.startFloat = 0f;
					floatTween.targetFloat = 1f;
					floatTween.AddOnChangedCallback(SetCanvasAlpha);
					break;
				case Transition.SlideFromRight:
					rectTransform.anchoredPosition = new Vector2(rectSize.x, 0f);
					floatTween.startFloat = rectSize.x;
					floatTween.targetFloat = 0f;
					floatTween.AddOnChangedCallback(SetPositionX);
					break;
				case Transition.SlideFromLeft:
					rectTransform.anchoredPosition = new Vector2(rectSize.x * -1f, 0f);
					floatTween.startFloat = rectSize.x * -1f;
					floatTween.targetFloat = 0f;
					floatTween.AddOnChangedCallback(SetPositionX);
					break;
				case Transition.SlideFromBottom:
					rectTransform.anchoredPosition = new Vector2(0f, rectSize.y * -1f);
					floatTween.startFloat = rectSize.y * -1f;
					floatTween.targetFloat = 0f;
					floatTween.AddOnChangedCallback(SetPositionY);
					break;
				case Transition.SlideFromTop:
					rectTransform.anchoredPosition = new Vector2(0f, rectSize.y);
					floatTween.startFloat = rectSize.y;
					floatTween.targetFloat = 0f;
					floatTween.AddOnChangedCallback(SetPositionY);
					break;
			}

			// Activate the scene
			Activate();

			// Start the transition
			floatTween.AddOnFinishCallback(OnTransitionIn);
			floatTween.ignoreTimeScale = true;
			floatTween.easing = easing;
			m_FloatTweenRunner.StartTween(floatTween);
		}

		/// <summary>
		///     Transition the scene out.
		/// </summary>
		public void TransitionOut() {
			TransitionOut(m_Transition, m_TransitionDuration, m_TransitionEasing);
		}

		/// <summary>
		///     Transition the scene out.
		/// </summary>
		/// <param name="transition">The transition.</param>
		/// <param name="duration">The transition duration.</param>
		/// <param name="easing">The transition easing.</param>
		public void TransitionOut(Transition transition, float duration, TweenEasing easing) {
			// Make sure the scene is active and enabled
			if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
				return;

			if (m_CanvasGroup == null)
				return;

			// If no transition is used
			if (transition == Transition.None) {
				Deactivate();
				return;
			}

			// If the transition is animation
			if (transition == Transition.Animation) {
				TriggerAnimation(m_AnimateOutTrigger);
				return;
			}

			// Make the scene non interactable
			//this.m_CanvasGroup.interactable = false;
			//this.m_CanvasGroup.blocksRaycasts = false;

			// Prepare some variable
			Vector2 rectSize = rectTransform.rect.size;

			// Prepare the rect transform
			if (transition == Transition.SlideFromLeft || transition == Transition.SlideFromRight ||
			    transition == Transition.SlideFromTop || transition == Transition.SlideFromBottom) {
				// Anchor and pivot top left
				rectTransform.pivot = new Vector2(0f, 1f);
				rectTransform.anchorMin = new Vector2(0f, 1f);
				rectTransform.anchorMax = new Vector2(0f, 1f);
				rectTransform.sizeDelta = rectSize;
				rectTransform.anchoredPosition = new Vector2(0f, 0f);
			}

			// Prepare the tween
			FloatTween floatTween = new FloatTween();
			floatTween.duration = duration;

			switch (transition) {
				case Transition.CrossFade:
					m_CanvasGroup.alpha = 1f;
					// Start the tween
					floatTween.startFloat = m_CanvasGroup.alpha;
					floatTween.targetFloat = 0f;
					floatTween.AddOnChangedCallback(SetCanvasAlpha);
					break;
				case Transition.SlideFromRight:
					// Start the tween
					floatTween.startFloat = 0f;
					floatTween.targetFloat = rectSize.x * -1f;
					floatTween.AddOnChangedCallback(SetPositionX);
					break;
				case Transition.SlideFromLeft:
					// Start the tween
					floatTween.startFloat = 0f;
					floatTween.targetFloat = rectSize.x;
					floatTween.AddOnChangedCallback(SetPositionX);
					break;
				case Transition.SlideFromBottom:
					// Start the tween
					floatTween.startFloat = 0f;
					floatTween.targetFloat = rectSize.y;
					floatTween.AddOnChangedCallback(SetPositionY);
					break;
				case Transition.SlideFromTop:
					// Start the tween
					floatTween.startFloat = 0f;
					floatTween.targetFloat = rectSize.y * -1f;
					floatTween.AddOnChangedCallback(SetPositionY);
					break;
			}

			// Start the transition
			floatTween.AddOnFinishCallback(OnTransitionOut);
			floatTween.ignoreTimeScale = true;
			floatTween.easing = easing;
			m_FloatTweenRunner.StartTween(floatTween);
		}

		/// <summary>
		///     Starts alpha tween.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="easing">Easing.</param>
		/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
		/// <param name="callback">Event to be called on transition finish.</param>
		public void StartAlphaTween(float targetAlpha,
			float duration,
			TweenEasing easing,
			bool ignoreTimeScale,
			UnityAction callback) {
			if (m_CanvasGroup == null)
				return;

			// Start the tween
			FloatTween floatTween = new FloatTween
				{duration = duration, startFloat = m_CanvasGroup.alpha, targetFloat = targetAlpha};
			floatTween.AddOnChangedCallback(SetCanvasAlpha);
			floatTween.AddOnFinishCallback(callback);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			floatTween.easing = easing;
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
		}

		/// <summary>
		///     Sets the rect transform anchored position X.
		/// </summary>
		/// <param name="x">The X position.</param>
		public void SetPositionX(float x) {
			rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
		}

		/// <summary>
		///     Sets the rect transform anchored position Y.
		/// </summary>
		/// <param name="y">The Y position.</param>
		public void SetPositionY(float y) {
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
		}

		/// <summary>
		///     Triggers animation.
		/// </summary>
		/// <param name="triggername"></param>
		private void TriggerAnimation(string triggername) {
			// Get the animator on the target game object
			Animator animator = gameObject.GetComponent<Animator>();

			if (animator == null || !animator.enabled || !animator.isActiveAndEnabled ||
			    animator.runtimeAnimatorController == null || !animator.hasBoundPlayables ||
			    string.IsNullOrEmpty(triggername))
				return;

			animator.ResetTrigger(m_AnimateInTrigger);
			animator.ResetTrigger(m_AnimateOutTrigger);
			animator.SetTrigger(triggername);
		}

		/// <summary>
		///     Raises the transition in finished event.
		/// </summary>
		protected virtual void OnTransitionIn() {
			// Re-enable the canvas group interaction
			if (m_CanvasGroup != null) {
				//this.m_CanvasGroup.interactable = true;
				//this.m_CanvasGroup.blocksRaycasts = true;
			}
		}

		/// <summary>
		///     Raises the transition out finished event.
		/// </summary>
		protected virtual void OnTransitionOut() {
			// Deactivate the scene
			Deactivate();

			// Re-enable the canvas group interaction
			if (m_CanvasGroup != null) {
				//this.m_CanvasGroup.interactable = true;
				//this.m_CanvasGroup.blocksRaycasts = true;
			}

			// Reset the alpha
			SetCanvasAlpha(1f);

			// Reset the position of the transform
			SetPositionX(0f);
		}

		[Serializable]
		public class OnActivateEvent : UnityEvent<UIScene> {

		}

		[Serializable]
		public class OnDeactivateEvent : UnityEvent<UIScene> {

		}

#pragma warning disable 0649
		[SerializeField] private int m_Id;
		[SerializeField] private bool m_IsActivated = true;
		[SerializeField] private Type m_Type = Type.Preloaded;
		[SerializeField] private Transform m_Content;
		[SerializeField] private GameObject m_Prefab;
		[SerializeField] [ResourcePath] private string m_Resource;
		[SerializeField] private Transition m_Transition = Transition.None;
		[SerializeField] private float m_TransitionDuration = 0.2f;
		[SerializeField] private TweenEasing m_TransitionEasing = TweenEasing.InOutQuint;
		[SerializeField] private string m_AnimateInTrigger = "AnimateIn";
		[SerializeField] private string m_AnimateOutTrigger = "AnimateOut";
		[SerializeField] private GameObject m_FirstSelected;
#pragma warning restore 0649

	}
}