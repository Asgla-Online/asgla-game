using Asgla.Avatar.Player;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class MoveToLocal : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string area;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		
		public int playerId;
		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string position;

		public void onRequest(Main main, string json) {
			MoveToLocal moveToLocal = JsonMapper.ToObject<MoveToLocal>(json);

			Player player = main.MapManager.PlayerByID(moveToLocal.playerId);

			if (player is null)
				return;

			main.MapManager.UpdatePlayerArea(player, main.MapManager.Map.AreaByName(moveToLocal.area));
		}

	}
}