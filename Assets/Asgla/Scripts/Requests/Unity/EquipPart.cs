using Asgla.Avatar.Player;
using Asgla.Data.Type;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class EquipPart : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string asset = null;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public string bundle = null;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public Equipment equipment;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public int playerId = -1;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public TypeItemData type;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public int uniqueId = -1;

		public void onRequest(Main main, string json) {
			EquipPart equipPart = JsonMapper.ToObject<EquipPart>(json);

			Player player = main.MapManager.PlayerByID(equipPart.playerId);

			if (player is null)
				return;

			player.Equip(equipPart);
		}

	}
}