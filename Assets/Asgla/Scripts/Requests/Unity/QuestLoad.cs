using System.Collections.Generic;
using Asgla.Data.Quest;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class QuestLoad : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public List<QuestData> quests;

		public void onRequest(Main main, string json) {
			QuestLoad questLoad = JsonMapper.ToObject<QuestLoad>(json);

			main.Game.WindowQuest.Init(questLoad.quests);
			main.Game.WindowQuest.Window().Show();
		}

	}
}