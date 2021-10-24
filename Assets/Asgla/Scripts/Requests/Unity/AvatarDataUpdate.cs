using System.Collections.Generic;
using Asgla.Avatar;
using Asgla.Data.Avatar;
using Asgla.Data.Map;
using Asgla.Data.Request;
using Asgla.UI.Loading;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class AvatarDataUpdate : IRequest {
		
		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public RequestAvatar.Entity entity = null;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public AvatarStats stats = null;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public AvatarState state = AvatarState.NONE;

		public void onRequest(Main main, string json) {
			AvatarDataUpdate avatarDataUpdate = JsonMapper.ToObject<AvatarDataUpdate>(json);
			
			AvatarMain avatar = avatarDataUpdate.entity.Avatar;

			if (avatar is null) {
				return;
			}

			if (avatarDataUpdate.stats != null) {
				avatar.Stats(avatarDataUpdate.stats);
			}

			if (avatarDataUpdate.state != AvatarState.NONE) {
				avatar.State(avatarDataUpdate.state);
			}
		}

	}
}