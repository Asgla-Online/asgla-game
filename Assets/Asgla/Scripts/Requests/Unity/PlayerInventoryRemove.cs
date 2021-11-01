using System.Collections.Generic;
using Asgla.Data.Avatar.Player;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class PlayerInventoryRemove : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public List<PlayerInventory> inventory;

		public void onRequest(Main main, string json) {
			PlayerInventoryRemove playerInventoryRemove = JsonMapper.ToObject<PlayerInventoryRemove>(json);


			foreach (PlayerInventory playerInventory in playerInventoryRemove.inventory)
				main.Game.AvatarController.Player.InventoryRemove(playerInventory.databaseId, playerInventory.quantity);
		}

	}
}