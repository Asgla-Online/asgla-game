using System;
using Asgla.Avatar;
using Asgla.Data.Avatar;
using Asgla.Data.Avatar.Helper;
using Asgla.Data.Avatar.Player;
using Asgla.Data.Effect;
using Asgla.Data.Skill;

namespace Asgla.Data.Request {
	public class RequestAvatar {

		public class DataUpdate2 {

			public PlayerData data = null;
			public Entity entity = null;

			public AvatarStats stats = null;

		}

		public class Entity {

			public int EntityID = -1;
			public EntityType EntityType;

			public AvatarMain Avatar => EntityType switch {
				EntityType.Player => Main.Singleton.Game.AreaController.PlayerByID(EntityID),
				EntityType.Monster => Main.Singleton.Game.AreaController.MonsterByID(EntityID),
				EntityType.Npc => throw new ArgumentOutOfRangeException(),
				EntityType.Pet => throw new ArgumentOutOfRangeException(),
				_ => throw new ArgumentOutOfRangeException()
			};

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