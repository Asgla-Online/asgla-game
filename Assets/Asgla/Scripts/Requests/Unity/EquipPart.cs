using Asgla.Avatar.Player;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class EquipPart : IRequest {

		public void onRequest(Main main, string json) {
			Asgla.Data.Avatar.Helper.EquipPart
				equipPart = JsonMapper.ToObject<Asgla.Data.Avatar.Helper.EquipPart>(json);

			Player player = main.Game.AreaController.PlayerByID(equipPart.playerId);

			if (player is null)
				return;

			player.Equip(equipPart);
		}

	}
}