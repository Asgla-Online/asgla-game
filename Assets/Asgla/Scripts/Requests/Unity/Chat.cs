using System;
using Asgla.Utility;
using BestHTTP.JSON.LitJson;
using UnityEngine;

namespace Asgla.Requests.Unity {
	public class Chat : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public int channel;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string message;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string username;

		public void onRequest(Main main, string json) {
			Chat chat = JsonMapper.ToObject<Chat>(json);

			main.Game.Chat.ReceiveChatMessage(
				chat.channel,
				$"<size=22>{DateTime.Now.ToShortTimeString()}</size> <b><color=#d9d9d9>{chat.username}</color></b> <color=#{CommonColorBuffer.ColorToString(Color.white)}>{chat.message}</color>"
			);
		}

	}
}