using Asgla.Avatar.Player;
using BestHTTP.JSON.LitJson;

// ReSharper disable InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global

namespace Asgla.Requests.Unity {
	public class MoveToLocal : IRequest {

		public int playerId;

		public string area;
		public string position;

		public void onRequest(Main main, string json) {
			MoveToLocal moveToLocal = JsonMapper.ToObject<MoveToLocal>(json);
			
			Player player = main.MapManager.PlayerByID(moveToLocal.playerId);

			if (player is null) {
				return;
			}

			main.MapManager.UpdatePlayerArea(player, main.MapManager.Map.AreaByName(moveToLocal.area));
		}

	}
}