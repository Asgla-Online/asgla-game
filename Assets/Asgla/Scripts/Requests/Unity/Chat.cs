using Asgla.Avatar;
using BestHTTP.JSON.LitJson;
using static Asgla.Data.Request.RequestAvatar;

namespace Asgla.Requests.Unity {
	public class Chat : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public int channel;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public Entity entity;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string entityTag;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string message;

		public void onRequest(Main main, string json) {
			Chat chat = JsonMapper.ToObject<Chat>(json);

			if (chat.entity == null) {
				main.Game.Chat.ChatMessage(chat.channel, chat.entityTag, chat.message);
			} else {
				AvatarMain avatar = chat.entity.Avatar;

				main.Game.Chat.ChatMessage(chat.channel, chat.entityTag, avatar.Name(), chat.message);

				avatar.Utility().Bubble.Show(chat.message);
			}
		}

	}
}