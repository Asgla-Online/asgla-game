using System.Collections.Generic;
using Asgla.Avatar;
using Asgla.Avatar.Player;
using Asgla.Data.Avatar;
using Asgla.Data.Player;
using Asgla.Data.Request;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class PlayerInventoryLoad : IRequest {
		
		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public int playerId;
		
		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public List<PlayerInventory> inventory;
		
		public void onRequest(Main main, string json) {
			PlayerInventoryLoad playerInventoryLoad = JsonMapper.ToObject<PlayerInventoryLoad>(json);
			
			Player player = main.MapManager.PlayerByID(playerInventoryLoad.playerId);

			if (player is null) {
				return;
			}

			player.Inventory(playerInventoryLoad.inventory);
		}

	}
}