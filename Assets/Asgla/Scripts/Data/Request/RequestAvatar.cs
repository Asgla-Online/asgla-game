using Asgla.Avatar;
using Asgla.Data.Avatar;
using Asgla.Data.Effect;
using Asgla.Data.Entity;
using Asgla.Data.Player;
using Asgla.Data.Skill;

namespace Asgla.Data.Request {
	public class RequestAvatar {

		public class DataUpdate2 {

			public PlayerData Data = null;
			public Entity Entity = null;

			public AvatarStats Stats = null;

		}

		public class Entity {

			public int EntityID = -1;
			public EntityType EntityType;

			public AvatarMain Avatar => EntityType == EntityType.PLAYER
				? Main.Singleton.MapManager.PlayerByID(EntityID)
				: (AvatarMain) Main.Singleton.MapManager.MonsterByID(EntityID);

		}

		public class CombatSkill {

			public float CastTime = -1;
			public float Cooldown = -1;
			public int SlotID = -1;

		}

		public class CombatAnimation {

			public string Animation = null;

			public string Area = null;
			public string Effect = null;
			public SkillTarget Target = SkillTarget.TARGET;

			public EffectData EffectData => Main.Singleton.GameAsset.GetByName(Effect);

		}

		public class CombatResult {

			public int Damage;

			public Entity Entity;
			public bool IsDead;

			public SkillDamageType Type;

		}

	}
}