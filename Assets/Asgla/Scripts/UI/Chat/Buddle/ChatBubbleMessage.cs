using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Asgla.UI.Chat.Buddle {
	public class ChatBubbleMessage : MonoBehaviour {

		public Text messageText;

		private Vector2 _defaultScale;
		private RectTransform _rectTransform;

		public Coroutine BubbleRoutine;

		// Assigned by the ChatBubble class
		public float TransitionDuration { get; set; }

		/// <summary>
		///     Transitions the chat bubble in.
		/// </summary>
		public IEnumerator TransitionIn() {
			_rectTransform = (RectTransform) transform;

			_defaultScale = _rectTransform.localScale;
			_rectTransform.localScale = Vector3.zero;

			_rectTransform.pivot = new Vector2(0.5f, 0f);
			yield return ChatBubble.TransitionToScale(_rectTransform, _defaultScale, TransitionDuration);
		}

		/// <summary>
		///     Transitions the chat bubble out.
		/// </summary>
		public IEnumerator TransitionOut() {
			yield return ChatBubble.TransitionToScale(transform, Vector3.zero, TransitionDuration);
		}

	}
}