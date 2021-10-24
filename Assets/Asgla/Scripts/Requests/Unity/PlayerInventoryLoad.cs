using System.Collections.Generic;
using Asgla.Avatar.Player;
using Asgla.Data.Player;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class PlayerInventoryLoad : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public List<PlayerInventory> inventory;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public int playerId;

		public void onRequest(Main main, string json) {
			PlayerInventoryLoad playerInventoryLoad = JsonMapper.ToObject<PlayerInventoryLoad>(json);

			Player player = main.MapManager.PlayerByID(playerInventoryLoad.playerId);

			if (player is null)
				return;

			player.Inventory(playerInventoryLoad.inventory);
		}

	}
}