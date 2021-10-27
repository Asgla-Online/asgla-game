using System;
using System.Collections;
using Asgla;
using Asgla.Data.Skill;
using AsglaUI.UI.Tweens;
using UnityEngine;
using UnityEngine.UI;

namespace AsglaUI.UI {
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class UICastBar : MonoBehaviour {

		[Serializable]
		public enum DisplayTime {

			Elapsed,
			Remaining

		}

		[Serializable]
		public enum Transition {

			Instant,
			Fade

		}

		// Tween controls
		[NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		private string _requestTarget;
		private string _requestTargets;

		private SkillData _skillData;

		private float currentCastDuration;
		private float currentCastEndTime;

		// Called by Unity prior to deserialization, 
		// should not be called by users
		protected UICastBar() {
			if (m_FloatTweenRunner == null)
				m_FloatTweenRunner = new TweenRunner<FloatTween>();

			m_FloatTweenRunner.Init(this);
		}

		/// <summary>
		///     Gets a value indicating whether this cast bar is casting.
		/// </summary>
		/// <value><c>true</c> if this instance is casting; otherwise, <c>false</c>.</value>
		public bool IsCasting { get; private set; }

		/// <summary>
		///     Gets the canvas group.
		/// </summary>
		/// <value>The canvas group.</value>
		public CanvasGroup canvasGroup { get; private set; }

		protected virtual void Awake() {
			canvasGroup = gameObject.GetComponent<CanvasGroup>();
		}

		protected virtual void Start() {
			if (isActiveAndEnabled) {
				// Apply the normal color stage
				ApplyColorStage(m_NormalColors);

				// Hide the bar only while the application is playing
				if (Application.isPlaying) {
					// Hide the icon frame
					if (m_IconFrame != null)
						m_IconFrame.SetActive(false);

					// Hide the cast bar
					Hide(true);
				}
			}
		}

#if UNITY_EDITOR
		protected void OnValidate() {
			// Apply the normal color stage
			if (isActiveAndEnabled)
				ApplyColorStage(m_NormalColors);
		}
#endif

		/// <summary>
		///     Applies the color stages.
		/// </summary>
		/// <param name="stage">Stage.</param>
		public virtual void ApplyColorStage(ColorStage stage) {
			if (m_FillImage != null)
				m_FillImage.canvasRenderer.SetColor(stage.fillColor);

			if (m_TitleLabel != null)
				m_TitleLabel.canvasRenderer.SetColor(stage.titleColor);

			if (m_TimeLabel != null)
				m_TimeLabel.canvasRenderer.SetColor(stage.timeColor);
		}

		/// <summary>
		///     Show this cast bar.
		/// </summary>
		public void Show() {
			// Call show with a transition
			Show(false);
		}

		/// <summary>
		///     Show this cast bar.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		public virtual void Show(bool instant) {
			// Bring to the front
			if (m_BrindToFront)
				UIUtility.BringToFront(gameObject);

			// Do the transition
			if (instant || m_InTransition == Transition.Instant) // Set the canvas group alpha
				canvasGroup.alpha = 1f;
			else // Start a tween
				StartAlphaTween(1f, m_InTransitionDuration, true);
		}

		/// <summary>
		///     Hide this cast bar.
		/// </summary>
		public void Hide() {
			// Call hide with a transition
			Hide(false);
		}

		/// <summary>
		///     Hide this cast bar.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		public virtual void Hide(bool instant) {
			if (instant || m_OutTransition == Transition.Instant) {
				// Set the canvas group alpha
				canvasGroup.alpha = 0f;

				// Raise the on hide tween finished event
				OnHideTweenFinished();
			} else {
				// Start a tween
				StartAlphaTween(0f, m_OutTransitionDuration, true);
			}
		}

		/// <summary>
		///     Starts alpha tween.
		/// </summary>
		/// <param name="targetAlpha">Target alpha.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
		public void StartAlphaTween(float targetAlpha, float duration, bool ignoreTimeScale) {
			if (canvasGroup == null)
				return;

			FloatTween floatTween = new FloatTween
				{duration = duration, startFloat = canvasGroup.alpha, targetFloat = targetAlpha};
			floatTween.AddOnChangedCallback(SetCanvasAlpha);
			floatTween.AddOnFinishCallback(OnHideTweenFinished);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			m_FloatTweenRunner.StartTween(floatTween);
		}

		/// <summary>
		///     Sets the canvas alpha.
		/// </summary>
		/// <param name="alpha">Alpha.</param>
		protected void SetCanvasAlpha(float alpha) {
			if (canvasGroup == null)
				return;

			// Set the alpha
			canvasGroup.alpha = alpha;
		}

		/// <summary>
		///     Raises the hide tween finished event.
		/// </summary>
		protected virtual void OnHideTweenFinished() {
			// Hide the icon frame
			if (m_IconFrame != null)
				m_IconFrame.SetActive(false);

			// Unset the icon image sprite
			if (m_IconImage != null)
				m_IconImage.sprite = null;
		}

		/// <summary>
		///     Sets the fill amount.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void SetFillAmount(float amount) {
			if (m_ProgressBar != null)
				m_ProgressBar.fillAmount = amount;
		}

		private IEnumerator AnimateCast() {
			// Update the text
			if (m_TimeLabel != null)
				m_TimeLabel.text = m_DisplayTime == DisplayTime.Elapsed
					? 0f.ToString(m_TimeFormat)
					: currentCastDuration.ToString(m_TimeFormat);

			// Get the timestamp
			float startTime = currentCastEndTime > 0f ? currentCastEndTime - currentCastDuration : Time.time;

			// Fade In the notification
			while (Time.time < startTime + currentCastDuration) {
				float RemainingTime = startTime + currentCastDuration - Time.time;
				float ElapsedTime = currentCastDuration - RemainingTime;
				float ElapsedTimePct = ElapsedTime / currentCastDuration;

				// Update the elapsed cast time value
				if (m_TimeLabel != null)
					m_TimeLabel.text = m_DisplayTime == DisplayTime.Elapsed
						? ElapsedTime.ToString(m_TimeFormat)
						: RemainingTime.ToString(m_TimeFormat);

				// Update the fill sprite
				SetFillAmount(ElapsedTimePct);

				yield return 0;
			}

			// Update the fill sprite to full
			SetFillAmount(1f);

			// Make sure it's maxed
			if (m_TimeLabel != null)
				m_TimeLabel.text = m_DisplayTime == DisplayTime.Elapsed
					? currentCastDuration.ToString(m_TimeFormat)
					: 0f.ToString(m_TimeFormat);

			// Call that we finished
			OnFinishedCasting();

			// Hide with a delay
			StartCoroutine("DelayHide");
		}

		private IEnumerator DelayHide() {
			// Wait for the hide delay
			yield return new WaitForSeconds(m_HideDelay);

			// Do not show the casting anymore
			Hide();
		}

		/// <summary>
		///     Starts the casting of the specified spell.
		/// </summary>
		/// <param name="skillData">Skill info.</param>
		/// <param name="duration">Duration.</param>
		/// <param name="endTime">End time.</param>
		/// <param name="target">Target string.</param>
		/// <param name="targets">Targets string list.</param>
		public virtual void StartCasting(SkillData skillData,
			float duration,
			float endTime,
			string target,
			string targets) {
			// Make sure we can start casting it
			if (IsCasting)
				return;

			_skillData = skillData;

			_requestTarget = target;
			_requestTargets = targets;

			// Stop the coroutine might be still running on the hide delay
			StopCoroutine("AnimateCast");
			StopCoroutine("DelayHide");

			// Apply the normal colors
			ApplyColorStage(m_NormalColors);

			// Change the fill pct
			SetFillAmount(0f);

			// Set the spell name
			if (m_TitleLabel != null)
				m_TitleLabel.text = skillData.Name;

			// Set the full time cast text
			if (m_FullTimeLabel != null)
				m_FullTimeLabel.text = skillData.CastTime.ToString(m_FullTimeFormat);

			// Set the icon if we have enabled icons
			if (m_UseSpellIcon) // Check if we have a sprite
				if (skillData.Icon != null) {
					// Check if the icon image is set
					if (m_IconImage != null)
						m_IconImage.sprite = skillData.GetIcon;

					// Enable the frame
					if (m_IconFrame != null)
						m_IconFrame.SetActive(true);
				}

			// Set some info about the cast
			currentCastDuration = duration;
			currentCastEndTime = endTime;

			// Define that we start casting animation
			IsCasting = true;

			// Show the cast bar
			Show();

			// Start the cast animation
			StartCoroutine("AnimateCast");
		}

		/// <summary>
		///     Interrupts the current cast if any.
		/// </summary>
		public virtual void Interrupt() {
			if (IsCasting) {
				// Stop the coroutine if it's assigned
				StopCoroutine("AnimateCast");

				// No longer casting
				IsCasting = false;

				// Apply the interrupt colors
				ApplyColorStage(m_OnInterruptColors);

				// Hide with a delay
				StartCoroutine("DelayHide");
			}
		}

		protected void OnFinishedCasting() {
			// Define that we are no longer casting
			IsCasting = false;

			// Apply the finish colors
			ApplyColorStage(m_OnFinishColors);

			Main.Singleton.Request.Send("Combat", _skillData.SlotID, _requestTarget, _requestTargets);
		}

		[Serializable]
		public class ColorStage {

			public Color fillColor = Color.white;
			public Color titleColor = Color.white;
			public Color timeColor = Color.white;

		}

#pragma warning disable 0649
		[SerializeField] private UIProgressBar m_ProgressBar;
		[SerializeField] private Text m_TitleLabel;
		[SerializeField] private Text m_TimeLabel;
		[SerializeField] private DisplayTime m_DisplayTime = DisplayTime.Remaining;
		[SerializeField] private string m_TimeFormat = "0.0 sec";
		[SerializeField] private Text m_FullTimeLabel;
		[SerializeField] private string m_FullTimeFormat = "0.0 sec";

		[SerializeField] private bool m_UseSpellIcon;
		[SerializeField] private GameObject m_IconFrame;
		[SerializeField] private Image m_IconImage;

		[SerializeField] private Image m_FillImage;
		[SerializeField] private ColorStage m_NormalColors = new ColorStage();
		[SerializeField] private ColorStage m_OnInterruptColors = new ColorStage();
		[SerializeField] private ColorStage m_OnFinishColors = new ColorStage();

		[SerializeField] private Transition m_InTransition = Transition.Instant;
		[SerializeField] private float m_InTransitionDuration = 0.1f;
		[SerializeField] private bool m_BrindToFront = true;

		[SerializeField] private Transition m_OutTransition = Transition.Fade;
		[SerializeField] private float m_OutTransitionDuration = 0.1f;
		[SerializeField] private float m_HideDelay = 0.3f;
#pragma warning restore 0649

	}
}