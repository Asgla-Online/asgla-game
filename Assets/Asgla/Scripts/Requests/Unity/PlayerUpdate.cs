using Asgla.Data.Player;
using BestHTTP.JSON.LitJson;

// ReSharper disable InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global

namespace Asgla.Requests.Unity {
	public class PlayerUpdate : IRequest {

		public PlayerData player;

		public void onRequest(Main main, string json) {
			PlayerUpdate update = JsonMapper.ToObject<PlayerUpdate>(json);
			
			main.AvatarManager.Create(update.player);
		}

	}
}