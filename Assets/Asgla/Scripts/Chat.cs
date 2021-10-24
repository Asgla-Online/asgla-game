using System;
using System.Collections.Generic;
using AsglaUI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Asgla {
	public class Chat : MonoBehaviour {

		[Serializable]
		public enum TextEffect {

			None,
			Shadow,
			Outline

		}

		[Header("Events")]
		/// <summary>
		/// Fired when the clients sends a chat message.
		/// First paramenter - int tabId.
		/// Second parameter - string messageText.
		/// </summary>
		public SendMessageEvent onSendMessage = new SendMessageEvent();
		//public ReceiveMessageEvent onReceiveMessage = new ReceiveMessageEvent();

		private TabInfo m_ActiveTabInfo;

		public static Chat Singleton { get; protected set; }

		public TMP_InputField ChatInput => m_InputField;

		/// <summary>
		///     Finds the active tab based on the tab buttons toggle state.
		/// </summary>
		/// <returns>The active tab info.</returns>
		private TabInfo FindActiveTab() {
			// If we have tabs
			if (m_Tabs != null && m_Tabs.Count > 0)
				foreach (TabInfo info in m_Tabs) // if we have a button
					if (info.button != null) // If this button is active
						if (info.button.isOn)
							return info;

			return null;
		}

		/// <summary>
		///     Gets the tab info for the specified tab by id.
		/// </summary>
		/// <param name="tabId">Tab id.</param>
		/// <returns></returns>
		public TabInfo GetTabInfo(int tabId) {
			// If we have tabs
			if (m_Tabs != null && m_Tabs.Count > 0)
				foreach (TabInfo info in m_Tabs) // If this is the tab we are looking for
					if (info.id == tabId)
						return info;

			return null;
		}

		/// <summary>
		///     Sends a chat message.
		/// </summary>
		/// <param name="text">The message.</param>
		private void SendChatMessage(string text) {
			int tabId = m_ActiveTabInfo != null ? m_ActiveTabInfo.id : 0;

			// Trigger the event
			if (onSendMessage != null)
				onSendMessage.Invoke(tabId, text);

			// Clear the input field
			if (m_InputField != null)
				m_InputField.text = "";
		}

		/// <summary>
		///     Adds a chat message to the specified tab.
		/// </summary>
		/// <param name="tabId">The tab id.</param>
		/// <param name="text">The message.</param>
		public void ReceiveChatMessage(int tabId, string text) {
			//Debug.Log("<color=cyan>[Chat]</color> " + tabId + " " + text);

			TabInfo tabInfo = GetTabInfo(tabId);

			// Make sure we have tab info
			if (tabInfo == null || tabInfo.content == null)
				return;

			// Create the text line
			GameObject obj = new GameObject("Text " + tabInfo.content.childCount, typeof(RectTransform));

			// Prepare the game object
			obj.layer = gameObject.layer;

			// Get the rect transform
			RectTransform rectTransform = obj.transform as RectTransform;

			// Prepare the rect transform
			rectTransform.localScale = new Vector3(1f, 1f, 1f);
			rectTransform.pivot = new Vector2(0f, 1f);
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(0f, 1f);

			// Set the parent
			rectTransform.SetParent(tabInfo.content, false);

			// Add the text component
			Text textComp = obj.AddComponent<Text>();

			// Prepare the text component
			textComp.font = m_TextFont;
			textComp.fontSize = m_TextFontSize;
			textComp.lineSpacing = m_TextLineSpacing;
			textComp.color = m_TextColor;
			textComp.text = text;

			// Prepare the text effect
			if (m_TextEffect != TextEffect.None)
				switch (m_TextEffect) {
					case TextEffect.Shadow:
						Shadow shadow = obj.AddComponent<Shadow>();
						shadow.effectColor = m_TextEffectColor;
						shadow.effectDistance = m_TextEffectDistance;
						break;
					case TextEffect.Outline:
						Outline outline = obj.AddComponent<Outline>();
						outline.effectColor = m_TextEffectColor;
						outline.effectDistance = m_TextEffectDistance;
						break;
				}

			// Rebuild the content layout
			LayoutRebuilder.ForceRebuildLayoutImmediate(tabInfo.content as RectTransform);

			// Scroll to bottom
			OnScrollToBottomClick();
		}

		public void OnSendChatMessage(int tabId, string text) {
			if (text.StartsWith("/")) {
				text = text.Substring(1, text.Length - 12);
				string[] stringArr = text.Split(' ');

				string command = stringArr[0];

				Main.Singleton.Request.Send("Command", command, stringArr);

				/*switch (stringArr[0].ToLower()) {
				    case "item":
				        Debug.Log("item");
				        break;
				    case "test":
				        Debug.Log("test");
				        break;
				    default:
				        Debug.Log("not found");
				        //not found
				        break;
				}*/
			} else {
				Main.Singleton.Request.Send("Chat", tabId, text);
			}
		}

		[Serializable]
		public class SendMessageEvent : UnityEvent<int, string> {

		}

		//[Serializable]
		//public class ReceiveMessageEvent : UnityEvent<int, string> { }

		[Serializable]
		public class TabInfo {

			public int id;
			public UITab button;
			public Transform content;
			public ScrollRect scrollRect;

		}

#pragma warning disable 0649
		[SerializeField] private TMP_InputField m_InputField;

		[Header("Buttons")] [SerializeField] private Button m_Submit;

		[SerializeField] private Button m_ScrollTopButton;
		[SerializeField] private Button m_ScrollBottomButton;
		[SerializeField] private Button m_ScrollUpButton;
		[SerializeField] private Button m_ScrollDownButton;

		[Header("Tab Properties")] [SerializeField]
		private List<TabInfo> m_Tabs = new List<TabInfo>();

		[Header("Text Properties")] [SerializeField]
		private Font m_TextFont = FontData.defaultFontData.font;

		[SerializeField] private int m_TextFontSize = FontData.defaultFontData.fontSize;
		[SerializeField] private float m_TextLineSpacing = FontData.defaultFontData.lineSpacing;
		[SerializeField] private Color m_TextColor = Color.white;
		[SerializeField] private TextEffect m_TextEffect = TextEffect.None;
		[SerializeField] private Color m_TextEffectColor = Color.black;
		[SerializeField] private Vector2 m_TextEffectDistance = new Vector2(1f, -1f);
#pragma warning restore 0649

		#region Unity

		protected void Awake() {
			// Find the active tab info
			m_ActiveTabInfo = FindActiveTab();

			// Clear the lines of text
			if (m_Tabs != null && m_Tabs.Count > 0)
				foreach (TabInfo info in m_Tabs) // if we have a button
					if (info.content != null)
						foreach (Transform t in info.content)
							Destroy(t.gameObject);
		}

		protected void OnEnable() {
			// Hook the submit button click event
			if (m_Submit != null)
				m_Submit.onClick.AddListener(OnSubmitClick);

			// Hook the scroll up button click event
			if (m_ScrollUpButton != null)
				m_ScrollUpButton.onClick.AddListener(OnScrollUpClick);

			// Hook the scroll down button click event
			if (m_ScrollDownButton != null)
				m_ScrollDownButton.onClick.AddListener(OnScrollDownClick);

			// Hook the scroll to top button click event
			if (m_ScrollTopButton != null)
				m_ScrollTopButton.onClick.AddListener(OnScrollToTopClick);

			// Hook the scroll to bottom button click event
			if (m_ScrollBottomButton != null)
				m_ScrollBottomButton.onClick.AddListener(OnScrollToBottomClick);

			// Hook the input field end edit event
			if (m_InputField != null)
				m_InputField.onEndEdit.AddListener(OnInputEndEdit);

			// Hook the tab toggle change events
			if (m_Tabs != null && m_Tabs.Count > 0)
				foreach (TabInfo info in m_Tabs) // if we have a button
					if (info.button != null)
						info.button.onValueChanged.AddListener(OnTabStateChange);
		}

		protected void OnDisable() {
			// Unhook the submit button click event
			if (m_Submit != null)
				m_Submit.onClick.RemoveListener(OnSubmitClick);

			// Unhook the scroll up button click event
			if (m_ScrollUpButton != null)
				m_ScrollUpButton.onClick.RemoveListener(OnScrollUpClick);

			// Unhook the scroll down button click event
			if (m_ScrollDownButton != null)
				m_ScrollDownButton.onClick.RemoveListener(OnScrollDownClick);

			// Unhook the scroll to top button click event
			if (m_ScrollTopButton != null)
				m_ScrollTopButton.onClick.RemoveListener(OnScrollToTopClick);

			// Unhook the scroll to bottom button click event
			if (m_ScrollBottomButton != null)
				m_ScrollBottomButton.onClick.RemoveListener(OnScrollToBottomClick);

			// Unhook the tab toggle change events
			if (m_Tabs != null && m_Tabs.Count > 0)
				foreach (TabInfo info in m_Tabs) // if we have a button
					if (info.button != null)
						info.button.onValueChanged.RemoveListener(OnTabStateChange);
		}

		#endregion

		#region Event

		/// <summary>
		///     Fired when the submit button is clicked.
		/// </summary>
		public void OnSubmitClick() {
			// Get the input text
			if (m_InputField != null) {
				string text = m_InputField.text;

				// Make sure we have input text
				if (!string.IsNullOrEmpty(text)) // Send the message
					SendChatMessage(text);
			}
		}

		/// <summary>
		///     Fired when the scroll up button is pressed.
		/// </summary>
		public void OnScrollUpClick() {
			if (m_ActiveTabInfo == null || m_ActiveTabInfo.scrollRect == null)
				return;

			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.scrollDelta = new Vector2(0f, 1f);

			m_ActiveTabInfo.scrollRect.OnScroll(pointerEventData);
		}

		/// <summary>
		///     Fired when the scroll down button is pressed.
		/// </summary>
		public void OnScrollDownClick() {
			if (m_ActiveTabInfo == null || m_ActiveTabInfo.scrollRect == null)
				return;

			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.scrollDelta = new Vector2(0f, -1f);

			m_ActiveTabInfo.scrollRect.OnScroll(pointerEventData);
		}

		/// <summary>
		///     Fired when the scroll to top button is pressed.
		/// </summary>
		public void OnScrollToTopClick() {
			if (m_ActiveTabInfo == null || m_ActiveTabInfo.scrollRect == null)
				return;

			// Scroll to top
			m_ActiveTabInfo.scrollRect.verticalNormalizedPosition = 1f;
		}

		/// <summary>
		///     Fired when the scroll to bottom button is pressed.
		/// </summary>
		public void OnScrollToBottomClick() {
			if (m_ActiveTabInfo == null || m_ActiveTabInfo.scrollRect == null)
				return;

			// Scroll to bottom
			m_ActiveTabInfo.scrollRect.verticalNormalizedPosition = 0f;
		}

		/// <summary>
		///     Fired when the input field is submitted.
		/// </summary>
		/// <param name="text"></param>
		public void OnInputEndEdit(string text) {
			// Make sure we have input text
			if (!string.IsNullOrEmpty(text)) // Make sure the return key is pressed
				if (Input.GetKey(KeyCode.Return)) // Send the message
					SendChatMessage(text);
		}

		/// <summary>
		///     Fired when a tab button is toggled.
		/// </summary>
		/// <param name="state"></param>
		public void OnTabStateChange(bool state) {
			// If a tab was activated
			if (state) // Find the active tab
				m_ActiveTabInfo = FindActiveTab();
		}

		#endregion

		//public void OnReceiveChatMessage(int tabId, string text) => ReceiveChatMessage(tabId, text);

	}
}