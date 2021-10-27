using UnityEngine;
using UnityEngine.Events;

namespace AsglaUI.UI.Tweens {
	public struct Vector3Tween : ITweenValue {

		public class Vector3TweenCallback : UnityEvent<Vector3> {

		}

		public class Vector3TweenFinishCallback : UnityEvent {

		}

		private Vector3TweenCallback m_Target;
		private Vector3TweenFinishCallback m_Finish;

		/// <summary>
		///     Gets or sets the starting Vector3.
		/// </summary>
		/// <value>The start color.</value>
		public Vector3 startVector3 { get; set; }

		/// <summary>
		///     Gets or sets the target Vector3.
		/// </summary>
		/// <value>The color of the target.</value>
		public Vector3 targetVector3 { get; set; }

		/// <summary>
		///     Gets or sets the duration of the tween.
		/// </summary>
		/// <value>The duration.</value>
		public float duration { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="UnityEngine.UI.Tweens.ColorTween" /> should ignore time
		///     scale.
		/// </summary>
		/// <value><c>true</c> if ignore time scale; otherwise, <c>false</c>.</value>
		public bool ignoreTimeScale { get; set; }

		/// <summary>
		///     Gets or sets the tween easing.
		/// </summary>
		/// <value>The easing.</value>
		public TweenEasing easing { get; set; }

		/// <summary>
		///     Tweens the color based on percentage.
		/// </summary>
		/// <param name="floatPercentage">Float percentage.</param>
		public void TweenValue(float floatPercentage) {
			if (!ValidTarget())
				return;

			m_Target.Invoke(Vector3.Lerp(startVector3, targetVector3, floatPercentage));
		}

		/// <summary>
		///     Adds a on changed callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void AddOnChangedCallback(UnityAction<Vector3> callback) {
			if (m_Target == null)
				m_Target = new Vector3TweenCallback();

			m_Target.AddListener(callback);
		}

		/// <summary>
		///     Adds a on finish callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void AddOnFinishCallback(UnityAction callback) {
			if (m_Finish == null)
				m_Finish = new Vector3TweenFinishCallback();

			m_Finish.AddListener(callback);
		}

		public bool GetIgnoreTimescale() {
			return ignoreTimeScale;
		}

		public float GetDuration() {
			return duration;
		}

		public bool ValidTarget() {
			return m_Target != null;
		}

		/// <summary>
		///     Invokes the on finish callback.
		/// </summary>
		public void Finished() {
			if (m_Finish != null)
				m_Finish.Invoke();
		}

	}
}