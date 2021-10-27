using Asgla.Data.Skill;

namespace AsglaUI.UI {

	public interface IUISlotHasCooldown {

		UISlotCooldown cooldownComponent { get; }

		SkillData GetSkillData();

		void SetCooldownComponent(UISlotCooldown cooldown);

	}
}