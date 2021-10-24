using System.Collections.Generic;
using Asgla.Data.Map;
using Asgla.UI.Loading;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class JoinMap : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public AreaData area;

		public void onRequest(Main main, string json) {
			JoinMap joinMap = JsonMapper.ToObject<JoinMap>(json);

			LoadingMapOverlay loadingMap = main.UIManager.LoadingOverlay == null
				? main.UIManager.CreateLoadingMap()
				: (LoadingMapOverlay) main.UIManager.LoadingOverlay;

			main.AvatarManager.Monsters = new List<MapAvatar>();

			loadingMap.LoadMap(joinMap.area);
		}

	}
}