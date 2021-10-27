using Asgla.Data.Skill;

namespace AsglaUI.UI {
	public interface IUISkillSlot {

		SkillData GetSkillData();

		bool Assign(SkillData spellInfo);

		void Unassign();

	}
}