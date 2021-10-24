using System.Collections.Generic;
using Asgla.Avatar.Player;
using BestHTTP.JSON.LitJson;
using static Asgla.Data.Request.RequestAvatar;

namespace Asgla.Requests.Unity {
	public class PlayerDataLoad : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public List<DataUpdate2> players;

		public void onRequest(Main main, string json) {
			PlayerDataLoad playerDataLoad = JsonMapper.ToObject<PlayerDataLoad>(json);

			foreach (DataUpdate2 du in playerDataLoad.players) {
				Player player = main.MapManager.PlayerByID(du.Data.playerID);

				if (player is null)
					continue;

				player.Data(du.Data);

				if (player.Data().Part != null)
					player.Data().Part.ForEach(part => player.Equip(part));

				player.Data().Part = null;

				player.Stats(du.Stats);
			}

			main.Request.Send("PlayerInventory");
		}

	}
}