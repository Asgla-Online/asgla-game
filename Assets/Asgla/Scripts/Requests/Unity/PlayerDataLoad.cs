using System.Collections.Generic;
using Asgla.Avatar.Player;
using BestHTTP.JSON.LitJson;
using static Asgla.Data.Request.RequestAvatar;

// ReSharper disable InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global

namespace Asgla.Requests.Unity {
	public class PlayerDataLoad : IRequest {
		
		public List<DataUpdate2> players;

		public void onRequest(Main main, string json) {
			PlayerDataLoad playerDataLoad = JsonMapper.ToObject<PlayerDataLoad>(json);
			
			foreach (DataUpdate2 du in playerDataLoad.players) {
				Player player = main.MapManager.PlayerByID(du.Data.playerID);

				if (player is null) {
					continue;
				}

				player.Data(du.Data);

				if (player.Data().Part != null) {
					player.Data().Part.ForEach(part => player.Equip(part));
				}

				player.Data().Part = null;

				player.Stats(du.Stats);
			}

			main.Request.Send("PlayerInventory");
		}

	}
}