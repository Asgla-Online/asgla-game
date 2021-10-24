using Asgla.Data.Avatar.Player;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class PlayerUpdate : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public PlayerData player;

		public void onRequest(Main main, string json) {
			PlayerUpdate update = JsonMapper.ToObject<PlayerUpdate>(json);

			main.AvatarManager.Create(update.player);
		}

	}
}