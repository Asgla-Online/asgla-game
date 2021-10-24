using System.Collections.Generic;
using Asgla.Data.Area;
using Asgla.UI.Loading;
using BestHTTP.JSON.LitJson;
using UnityEngine;

namespace Asgla.Requests.Unity {
	public class JoinMap : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public AreaData area;

		public void onRequest(Main main, string json) {
			JoinMap joinMap = JsonMapper.ToObject<JoinMap>(json);

			Debug.Log(joinMap.area);

			LoadingMapOverlay loadingMap = main.UIManager.LoadingOverlay == null
				? main.UIManager.CreateLoadingMap()
				: (LoadingMapOverlay) main.UIManager.LoadingOverlay;

			main.AvatarManager.Monsters = new List<AreaAvatar>();

			loadingMap.LoadMap(joinMap.area);
		}

	}
}