using System;
using BestHTTP.JSON.LitJson;
using UnityEngine;

// ReSharper disable InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global

namespace Asgla.Requests.Unity {
	public class Chat : IRequest {
		
		public int channel;

		public string username;
		public string message;

		public void onRequest(Main main, string json) {
			Chat chat = JsonMapper.ToObject<Chat>(json);
			
			main.Game.Chat.ReceiveChatMessage(
				chat.channel,
				$"<size=22>{DateTime.Now.ToShortTimeString()}</size> <b><color=#d9d9d9>{chat.username}</color></b> <color=#{CommonColorBuffer.ColorToString(Color.white)}>{chat.message}</color>"
			);
		}

	}
}