using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class Chat : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public int channel;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string entityName;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string entityTag;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string message;

		public void onRequest(Main main, string json) {
			Chat chat = JsonMapper.ToObject<Chat>(json);

			main.Game.Chat.ReceiveChatMessage(chat.channel, chat.message, chat.entityName, chat.entityTag);
		}

	}
}