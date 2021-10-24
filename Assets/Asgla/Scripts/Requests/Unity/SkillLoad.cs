using System.Collections.Generic;
using Asgla.Data.Skill;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class SkillLoad : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public List<SkillData> Skills = null;

		public void onRequest(Main main, string json) {
			SkillLoad skillLoad = JsonMapper.ToObject<SkillLoad>(json);

			main.Game.ActionBar.SkillAssign(skillLoad.Skills);
		}

	}
}