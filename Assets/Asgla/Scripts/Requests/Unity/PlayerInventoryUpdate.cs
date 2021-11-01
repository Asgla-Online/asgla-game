using Asgla.Data.Avatar.Player;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class PlayerInventoryUpdate : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public PlayerInventory inventory;

		public void onRequest(Main main, string json) {
			PlayerInventoryUpdate playerInventoryUpdate = JsonMapper.ToObject<PlayerInventoryUpdate>(json);

			main.Game.AvatarController.Player.Inventory(playerInventoryUpdate.inventory);
		}

	}
}