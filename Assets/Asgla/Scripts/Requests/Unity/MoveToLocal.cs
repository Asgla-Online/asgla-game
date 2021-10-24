using Asgla.Avatar.Player;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class MoveToLocal : IRequest {

		public void onRequest(Main main, string json) {
			Asgla.Data.Avatar.Helper.MoveToLocal moveToLocal =
				JsonMapper.ToObject<Asgla.Data.Avatar.Helper.MoveToLocal>(json);

			Player player = main.MapManager.PlayerByID(moveToLocal.playerId);

			if (player is null)
				return;

			main.MapManager.UpdatePlayerArea(player, main.MapManager.Map.AreaByName(moveToLocal.area));
		}

	}
}