using System.Collections.Generic;
using Asgla.Data.Map;
using Asgla.UI.Loading;
using BestHTTP.JSON.LitJson;

// ReSharper disable InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global

namespace Asgla.Requests.Unity {
	public class JoinMap : IRequest {
		
		public AreaData area;

		public void onRequest(Main main, string json) {
			JoinMap joinMap = JsonMapper.ToObject<JoinMap>(json);
			
			LoadingMapOverlay loadingMap = main.UIManager.LoadingOverlay == null ? main.UIManager.CreateLoadingMap() : (LoadingMapOverlay) main.UIManager.LoadingOverlay;

			main.AvatarManager.Monsters = new List<MapAvatar>();

			loadingMap.LoadMap(joinMap.area);
		}

	}
}