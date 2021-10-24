using System;
using Asgla.Avatar.Player;
using Asgla.Data.Type;
using BestHTTP.JSON.LitJson;

// ReSharper disable InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global

namespace Asgla.Requests.Unity {
	public class EquipPart : IRequest {
		
		public int playerId = -1;

		public int uniqueId = -1;

		public TypeItemData type;

		public Equipment equipment;

		public string bundle = null;
		public string asset = null;

		public void onRequest(Main main, string json) {
			EquipPart equipPart = JsonMapper.ToObject<EquipPart>(json);
			
			Player player = main.MapManager.PlayerByID(equipPart.playerId);

			if (player is null) {
				return;
			}

			player.Equip(equipPart);
		}

	}
}