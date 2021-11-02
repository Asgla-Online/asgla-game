using System;
using System.Collections.Generic;
using System.Linq;
using Asgla.Controller;
using AsglaUI.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = System.Random;

namespace Asgla.UI {
	public class Chat : MonoBehaviour {

		public static string Game = "game";
		public static string Warning = "warning";
		public static string Staff = "staff";

		private static readonly List<Channel> Channels = new List<Channel> {
			new Channel {ID = 0, Name = "All"},
			new Channel {ID = 1, Name = "Global"},
			new Channel {ID = 2, Name = "Zone"},
			new Channel {ID = 3, Name = "Trade"},
			new Channel {ID = 4, Name = "Party"},
			new Channel {ID = 5, Name = "Guild"}
		};

		private static readonly Dictionary<string, string> Tags = new Dictionary<string, string> {
			{"Warning", "#ff0000"},
			{"Game", "#faa84b"},

			{"Administrator", "#ed5d54"},
			{"Moderator", "#edd554"},

			{"Hero", "#54a9ed"},
			{"Player", "#a4a4a4"},
		};

		[SerializeField] private TMP_InputField inputField;

		[Header("Buttons")] [SerializeField] private Button submit;

		[SerializeField] private Button scrollTopButton;
		[SerializeField] private Button scrollBottomButton;
		[SerializeField] private Button scrollUpButton;
		[SerializeField] private Button scrollDownButton;

		[Header("Tab Properties")] [SerializeField]
		private UITab tab;

		[SerializeField] private TabContent content;

		[SerializeField] private GameObject tabsMenu;
		[SerializeField] private GameObject tabsContent;

		[Header("Text Properties")] [SerializeField]
		private TMP_FontAsset textFont;

		private Channel _activeChannel;

		public TMP_InputField ChatInput => inputField;

		/// <summary>
		///     Finds the active tab based on the tab buttons toggle state.
		/// </summary>
		/// <returns>The active tab info.</returns>
		private static Channel FindActiveTab() {
			return Channels.FirstOrDefault(channel => channel.Tab.isOn);
		}

		/// <summary>
		///     Gets the tab info for the specified tab by id.
		/// </summary>
		/// <param name="tabId">Tab id.</param>
		/// <returns></returns>
		private static Channel GetTabInfo(int tabId) {
			return Channels.FirstOrDefault(channel => channel.ID == tabId);
		}

		/// <summary>
		///     Sends a chat message.
		/// </summary>
		/// <param name="text">The message.</param>
		private void SendChatMessage(string text) {
			//Main.Singleton.Request.Send("Chat", _activeChannel?.ID ?? 1, text);

			Debug.Log("1");
			ReceiveChatMessage(_activeChannel?.ID ?? 0, text, "anthony",
				Tags.ElementAt(new Random().Next(Tags.Count)).Key);

			// Clear the input field
			if (inputField != null)
				inputField.text = "";
		}

		public void ReceiveChatMessage(int tabId, string text) {
		}

		/// <summary>
		///     Adds a chat message to the specified tab.
		/// </summary>
		/// <param name="tabId">The tab id.</param>
		/// <param name="text">The message.</param>
		/// <param name="entityName">Entity name</param>
		/// <param name="entityTag">Entity tag</param>
		public void ReceiveChatMessage(int tabId, string text, string entityName, string entityTag) {
			Channel channel = GetTabInfo(tabId);

			// Make sure we have tab info
			if (channel == null)
				return;

			// Create the text line
			GameObject obj =
				new GameObject("Text " + channel.Content.content.transform.childCount, typeof(RectTransform)) {
					layer = gameObject.layer
				};

			// Get the rect transform
			RectTransform rectTransform = obj.transform as RectTransform;

			// Prepare the rect transform
			// ReSharper disable once PossibleNullReferenceException
			rectTransform.localScale = new Vector3(1f, 1f, 1f);
			rectTransform.pivot = new Vector2(0f, 1f);
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(0f, 1f);

			// Set the parent
			rectTransform.SetParent(channel.Content.content.transform, false);

			// Add the text component
			TextMeshProUGUI textComp = obj.AddComponent<TextMeshProUGUI>();

			// Prepare the text component
			textComp.font = textFont;
			textComp.fontSize = 30;
			textComp.lineSpacing = 0;
			textComp.color = Color.white;

			Debug.Log(entityTag);

			string color = Tags.First(pair => pair.Key == entityTag).Value;

			//textComp.text = $"<size=22>{DateTime.Now.ToShortTimeString()}</size> <b><color={channel.Color}>{entityName}</color></b>: <color=#FFF>{text}</color>";
			textComp.text =
				$"<b><size=21>{DateTime.Now.ToShortTimeString()}</size> <size=24><color={color}>[{entityTag}]</color></size> {entityName}</b><color={color}>:</color> <color=#FFF>{text}</color>";

			// Rebuild the content layout
			LayoutRebuilder.ForceRebuildLayoutImmediate(channel.Content.GetComponent<RectTransform>());

			// Scroll to bottom
			OnScrollToBottomClick();
		}

		private class Channel {

			public TabContent Content;

			public int ID;

			public string Name;

			public UITab Tab;

		}

		#region Unity

		protected void Awake() {
			foreach (Channel channel in Channels) {
				channel.Tab = Instantiate(tab.gameObject, tabsMenu.transform).GetComponent<UITab>();
				channel.Tab.name = channel.Name + " Tab";

				channel.Content = Instantiate(content.gameObject, tabsContent.transform).GetComponent<TabContent>();
				channel.Content.name = channel.Name + " Content";

				channel.Tab.targetContent = channel.Content.gameObject;

				channel.Tab.textTarget.text = channel.Name;
				channel.Tab.group = tabsMenu.GetComponent<ToggleGroup>();

				channel.Tab.isOn = channel.ID == 0;

				//Clear tab content if child not empty
				if (channel.Content.content.transform.childCount > 0)
					UIController.ClearChild(channel.Content.transform);
			}

			// Find the active tab info
			_activeChannel = FindActiveTab();
		}

		protected void OnEnable() {
			// Hook the submit button click event
			submit.onClick.AddListener(OnSubmitClick);

			// Hook the scroll up button click event
			scrollUpButton.onClick.AddListener(OnScrollUpClick);

			// Hook the scroll down button click event
			scrollDownButton.onClick.AddListener(OnScrollDownClick);

			// Hook the scroll to top button click event
			scrollTopButton.onClick.AddListener(OnScrollToTopClick);

			// Hook the scroll to bottom button click event
			scrollBottomButton.onClick.AddListener(OnScrollToBottomClick);

			// Hook the input field end edit event
			inputField.onEndEdit.AddListener(OnInputEndEdit);

			// Hook the tab toggle change events
			foreach (Channel channel in Channels) // if we have a button
				channel.Tab.onValueChanged.AddListener(OnTabStateChange);
		}

		protected void OnDisable() {
			// Unhook the submit button click event
			submit.onClick.RemoveListener(OnSubmitClick);

			// Unhook the scroll up button click event
			scrollUpButton.onClick.RemoveListener(OnScrollUpClick);

			// Unhook the scroll down button click event
			scrollDownButton.onClick.RemoveListener(OnScrollDownClick);

			// Unhook the scroll to top button click event
			scrollTopButton.onClick.RemoveListener(OnScrollToTopClick);

			// Unhook the scroll to bottom button click event
			scrollBottomButton.onClick.RemoveListener(OnScrollToBottomClick);

			// Unhook the tab toggle change events
			foreach (Channel channel in Channels) // if we have a button
				channel.Tab.onValueChanged.RemoveListener(OnTabStateChange);
		}

		#endregion

		#region Event

		/// <summary>
		///     Fired when the submit button is clicked.
		/// </summary>
		private void OnSubmitClick() {
			string text = inputField.text;

			// Make sure we have input text
			if (!string.IsNullOrEmpty(text)) // Send the message
				SendChatMessage(text);
		}

		/// <summary>
		///     Fired when the scroll up button is pressed.
		/// </summary>
		private void OnScrollUpClick() {
			_activeChannel?.Content.scrollRect.OnScroll(new PointerEventData(EventSystem.current) {
				scrollDelta = new Vector2(0f, 1f)
			});
		}

		/// <summary>
		///     Fired when the scroll down button is pressed.
		/// </summary>
		private void OnScrollDownClick() {
			_activeChannel?.Content.scrollRect.OnScroll(new PointerEventData(EventSystem.current) {
				scrollDelta = new Vector2(0f, -1f)
			});
		}

		/// <summary>
		///     Fired when the scroll to top button is pressed.
		/// </summary>
		private void OnScrollToTopClick() {
			if (_activeChannel == null)
				return;

			// Scroll to top
			_activeChannel.Content.scrollRect.verticalNormalizedPosition = 1f;
		}

		/// <summary>
		///     Fired when the scroll to bottom button is pressed.
		/// </summary>
		private void OnScrollToBottomClick() {
			if (_activeChannel == null)
				return;

			// Scroll to bottom
			_activeChannel.Content.scrollRect.verticalNormalizedPosition = 0f;
		}

		/// <summary>
		///     Fired when the input field is submitted.
		/// </summary>
		/// <param name="text"></param>
		private void OnInputEndEdit(string text) {
			// Make sure we have input text
			// Make sure the return key is pressed
			if (!string.IsNullOrEmpty(text) && Input.GetKey(KeyCode.Return))
				SendChatMessage(text);
		}

		/// <summary>
		///     Fired when a tab button is toggled.
		/// </summary>
		/// <param name="state"></param>
		private void OnTabStateChange(bool state) {
			if (state)
				_activeChannel = FindActiveTab();
		}

		#endregion

	}
}