namespace AsglaUI.UI {
    using Asgla.Data.Skill;

    public interface IUISlotHasCooldown {

        SkillData GetSkillData();
        UISlotCooldown cooldownComponent { get; }
        void SetCooldownComponent(UISlotCooldown cooldown);

    }
}
