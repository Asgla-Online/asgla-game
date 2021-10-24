using Asgla.Avatar;
using Asgla.Avatar.Player;
using BestHTTP.JSON.LitJson;
using static Asgla.Data.Request.RequestAvatar;

namespace Asgla.Requests.Unity {
	public class AvatarRespawn : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public Entity entity = null;

		public void onRequest(Main main, string json) {
			AvatarRespawn avatarRespawn = JsonMapper.ToObject<AvatarRespawn>(json);

			AvatarMain target = avatarRespawn.entity.Avatar;

			switch (target) {
				case null:
					return;
				case Player player: {
					if (player.Data().isControlling)
						main.Game.WindowRespawn.Hide();

					main.MapManager.UpdatePlayerArea(player, main.MapManager.Map.AreaByName("Enter"));

					player.ResetCharacter();
					break;
				}
				default:
					//TODO: function set animation idle, hide gameObject ~reset~
					target.gameObject.SetActive(true);
					break;
			}
		}

	}
}