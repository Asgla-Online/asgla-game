using Asgla.Avatar.Player;
using BestHTTP.JSON.LitJson;
using UnityEngine;

// ReSharper disable InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global

namespace Asgla.Requests.Unity {
	public class LeaveArea : IRequest {

		public int playerId;

		public void onRequest(Main main, string json) {
			LeaveArea leaveMap = JsonMapper.ToObject<LeaveArea>(json);

			Player player = main.MapManager.PlayerByID(leaveMap.playerId);

			if (player is null) {
				return;
			}

			Object.Destroy(player.gameObject);

			main.MapManager.RemovePlayerFromArea(leaveMap.playerId);
		}

	}
}