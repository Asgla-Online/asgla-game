using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CharacterCreator2D.UI {
	public class EmotionTypeItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

		public Text title;
		public EmotionType emotionType;

		UIEmote emoteUI;

		void Awake() {
			emoteUI = GetComponentInParent<UIEmote>();
		}

		public void OnPointerClick(PointerEventData eventData) {
			if (emoteUI == null) return;
			emoteUI.OpenType(emotionType);
		}

		public void OnPointerEnter(PointerEventData eventData) {
			if (emoteUI == null) return;
			emoteUI.character.Emote(emotionType);
		}

		public void OnPointerExit(PointerEventData eventData) {
			if (emoteUI == null) return;
			emoteUI.character.ResetEmote();
		}

		public void Initialize(EmotionType eType) {
			emotionType = eType;
			string name = emoteUI.character.emotes.getIndex(eType).name;
			if (string.IsNullOrEmpty(name))
				title.text = Enum.GetName(typeof(EmotionType), eType);
			else
				title.text = name;
		}

		public void Initialize(string t) {
			title.text = t;
		}

	}
}