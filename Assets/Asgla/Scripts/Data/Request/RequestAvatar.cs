using Asgla.Avatar;
using Asgla.Data.Avatar;
using Asgla.Data.Effect;
using Asgla.Data.Entity;
using Asgla.Data.Player;
using Asgla.Data.Skill;
using Asgla.Data.Type;
using System.Collections.Generic;

namespace Asgla.Data.Request {
    public class RequestAvatar {

        public class DataUpdate2 {
            public Entity Entity = null;

            public AvatarStats Stats = null;
            public PlayerData Data = null;
        }

        public class Entity {
            public int EntityID = -1;
            public EntityType EntityType;

            public AvatarMain Avatar => EntityType == EntityType.PLAYER ? (AvatarMain)Main.Singleton.MapManager.PlayerByID(EntityID) : (AvatarMain)Main.Singleton.MapManager.MonsterByID(EntityID);
        }

        public class CombatSkill {
            public int SlotID = -1;
            public float Cooldown = -1;
            public float CastTime = -1;
        }

        public class CombatAnimation {
            public SkillTarget Target = SkillTarget.TARGET;

            public string Area = null;
            public string Effect = null;
            public string Animation = null;

            public EffectData EffectData => Main.Singleton.GameAsset.GetByName(Effect);
        }

        public class CombatResult {
            public bool IsDead;
            public int Damage;

            public SkillDamageType Type;

            public Entity Entity;
        }

    }
}
