using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Asgla.UI.Chat.Buddle {
	public class ChatBubble : MonoBehaviour {

		private const float MAXBubbleWidth = 650f;

		private const float MINDuration = 3f;
		private const float MAXDuration = 10f;

		private const int MAXBubbles = 3;

		//Extra time to display the message (in seconds per character).
		private const float ExtraDurationPerCharacter = 0.1f;

		//The time to transition the bubble and transitionObjects.
		private const float TransitionDuration = 0.25f;

		//These objects will be hidden when there are no active bubbles.
		[SerializeField] private GameObject arrow;

		[SerializeField] private Transform container;

		[SerializeField] private ChatBubbleMessage chatBubbleMessage;

		private Vector3 _arrowScale;

		// contains the active chat bubbles
		private LinkedList<ChatBubbleMessage> _chatBubbles;

		// stores the coroutines of the `transitionObjects`
		private LinkedList<Coroutine> _transitionRoutines;

		#region Unity

		public void Start() {
			_arrowScale = arrow.transform.localScale;
			arrow.transform.localScale = Vector3.zero;

			_chatBubbles = new LinkedList<ChatBubbleMessage>();
			_transitionRoutines = new LinkedList<Coroutine>();
			foreach (Transform child in container)
				child.gameObject.SetActive(false);

			RectTransform rt = container.GetComponent<RectTransform>();
			rt.sizeDelta = new Vector2(MAXBubbleWidth, rt.sizeDelta.y);
		}

		#endregion

		/// <summary>
		///     This function is called to initiate the process of displaying a given chat message.
		/// </summary>
		public void Show(string message) {
			// If too many bubbles, remove the oldest (regardless of remaining duration)
			if (_chatBubbles.Count >= MAXBubbles) {
				ChatBubbleMessage oldestBubble = _chatBubbles.Last.Value;
				StopCoroutine(oldestBubble.BubbleRoutine);
				StartCoroutine(RemoveBubble(oldestBubble));
			}

			// Show the new message
			Coroutine bubbleRoutine = StartCoroutine(ShowBubbleRoutine(message));
			// Store reference to the coroutine in case it needs to be terminated early
			_chatBubbles.First.Value.BubbleRoutine = bubbleRoutine;

			//if (bubbleEntryPrefab != null && messagesContainer != null && bubbleEntryPrefab.messageText != null) {
			//}
		}

		/// <summary>
		///     This coroutine creates a new chat bubble, transitions it in, waits for the message's duration, then
		///     transitions it out.
		/// </summary>
		private IEnumerator ShowBubbleRoutine(string message) {
			ChatBubbleMessage bubble = AddBubble(message);

			// Wait for transition before starting timer
			UpdateTransitionObjects();
			yield return bubble.TransitionIn();

			// Show message for given duration
			float duration = Mathf.Min(MAXDuration,
				MINDuration + ExtraDurationPerCharacter * bubble.messageText.text.Length);

			yield return new WaitForSeconds(duration);

			yield return RemoveBubble(bubble);
		}

		/// <summary>
		///     Instantiates and configures the chat bubble.
		/// </summary>
		private ChatBubbleMessage AddBubble(string message) {
			ChatBubbleMessage bubble = Instantiate(chatBubbleMessage, container);

			bubble.messageText.text = message;
			bubble.TransitionDuration = TransitionDuration;

			_chatBubbles.AddFirst(bubble);

			bubble.gameObject.SetActive(true);

			return bubble;
		}

		/// <summary>
		///     Transitions the chat bubble out, then destroys it.
		/// </summary>
		private IEnumerator RemoveBubble(ChatBubbleMessage bubble) {
			// Wait until all previous messages have been removed before removing this one.
			while (_chatBubbles.Last.Value != bubble)
				yield return null;

			_chatBubbles.Remove(bubble);

			UpdateTransitionObjects();

			yield return bubble.TransitionOut();

			Destroy(bubble.gameObject);
		}

		/// <summary>
		///     This displays the `transitionObjects` if there are at least 1 active chat bubbles, else it hides them
		/// </summary>
		private void UpdateTransitionObjects() {
			foreach (Coroutine coroutine in _transitionRoutines.Where(c => c != null))
				StopCoroutine(coroutine);

			_transitionRoutines.Clear();

			//Transition in & out
			Vector3 targetScale = _chatBubbles.Count > 0 ? _arrowScale : Vector3.zero;

			Coroutine c = StartCoroutine(TransitionToScale(arrow.transform, targetScale, TransitionDuration));

			_transitionRoutines.AddFirst(c);
		}

		/// <summary>
		///     Scales transform `t` to the `targetScale` in `transitionDuration` seconds. Used to transition the bubbles
		///     and other elements in and out. Used by this class and by `ChatBubbleEntry`.
		/// </summary>
		/// <param name="t">The transform to apply the scaling to</param>
		/// <param name="targetScale">The scale to transition to</param>
		/// <param name="transitionDuration">The time in seconds to transition</param>
		public static IEnumerator TransitionToScale(Transform t, Vector3 targetScale, float transitionDuration) {
			if (t == null)
				yield break;

			float smoothing = 1f;

			Vector3 difference = t.localScale - targetScale;

			while (smoothing >= 0f) {
				t.localScale = targetScale + difference * smoothing;
				smoothing -= Time.deltaTime / transitionDuration;
				yield return null;
			}

			t.localScale = targetScale;
		}

	}
}