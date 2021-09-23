using System;
using UnityEngine;

namespace Asgla.Data.Skill {

    [Serializable]
    public class SkillData {

        public int DatabaseID;

        public int SlotID;

        public string Name;
        public string Description;

        public int Damage;
        public int Energy;

        public string Icon;
        public string Animation;
        public string Effect;

        public SkillTarget Target;

        public float Range;
        public float Cooldown;
        public float CastTime;

        public SkillCastType CastType { get => CastTime == 0f ? SkillCastType.INSTANT : SkillCastType.CAST; set => CastType = value; }
        public SkillFlag FlagType;

        public int HitTargets;

        public Sprite GetIcon => Resources.Load<Sprite>("Sprites/Skill Icon/" + Icon);

    }

    public enum SkillTarget {
        SELF = 0,
        TARGET = 1,
        ALLIES = 2
    }

    public enum SkillCastType {
        INSTANT = 0,
        CAST = 1
    }

    public enum SkillFlag {
        SKILL = 0,
        PASSIVE = 1
    }

    public enum SkillDamageType {
        CRIT = 0,
        DODGE = 1,
        HIT = 2,
        MISS = 3
    }

}
