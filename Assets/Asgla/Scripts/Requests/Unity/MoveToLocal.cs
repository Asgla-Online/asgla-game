using Asgla.Avatar.Player;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class MoveToLocal : IRequest {

		public void onRequest(Main main, string json) {
			Asgla.Data.Avatar.Helper.MoveToLocal moveToLocal =
				JsonMapper.ToObject<Asgla.Data.Avatar.Helper.MoveToLocal>(json);

			Player player = main.Game.AreaController.PlayerByID(moveToLocal.playerId);

			if (player is null)
				return;

			main.Game.AreaController.UpdatePlayerArea(player,
				main.Game.AreaController.Map.AreaByName(moveToLocal.area));
		}

	}
}