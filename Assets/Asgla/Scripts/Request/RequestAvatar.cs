using Asgla.Avatar;
using Asgla.Data.Avatar;
using Asgla.Data.Effect;
using Asgla.Data.Entity;
using Asgla.Data.Player;
using Asgla.Data.Skill;
using Asgla.Data.Type;
using System;
using System.Collections.Generic;

namespace Asgla.Request {
    public class RequestAvatar {

        [Serializable]
        public class LoginRequest {
            public bool Status;
            public string Message;
        }

        [Serializable]
        public class DataLoad {
            public List<DataUpdate2> Players;
        }

        [Serializable]
        public class InventoryLoad {
            public int PlayerID;
            public List<PlayerInventory> Inventory;
        }

        [Serializable]
        public class InventoryUpdate {
            public PlayerInventory Inventory;
        }

        [Serializable]
        public class InventoryRemove {
            public List<PlayerInventory> Inventory;
        }

        [Serializable]
        public class InventoryRemoveList {
            public int DatabaseID = -1;
            public int Quantity = -1;
        }

        [Serializable]
        public class DataUpdate {
            public Entity Entity = null;

            public AvatarStats Stats = null;

            public AvatarState State = AvatarState.NONE;
        }

        [Serializable]
        public class DataUpdate2 {
            public Entity Entity = null;

            public AvatarStats Stats = null;
            public PlayerData Data = null;
        }

        [Serializable]
        public class EquipPart {
            public int PlayerID = -1;

            public int UniqueID = -1;

            public TypeItemData Type = null;

            public Equipment Equipment;

            public string Bundle = null;
            public string Asset = null;
        }

        [Serializable]
        public class ChatRequest {
            public int Channel;

            public string Username;
            public string Message;
        }

        [Serializable]
        public class Respawn {
            public Entity Entity = null;
        }

        [Serializable]
        public class SkillLoad {
            public List<SkillData> Skills = null;
        }

        public class Entity {
            public int EntityID = -1;
            public EntityType EntityType;

            public AvatarMain Avatar => EntityType == EntityType.PLAYER ? (AvatarMain)Main.Singleton.MapManager.PlayerByID(EntityID) : (AvatarMain)Main.Singleton.MapManager.MonsterByID(EntityID);
        }

        [Serializable]
        public class Combat {
            public string Message = null;
            public CombatSkill Skill = null;
            public Entity Entity = null;
            public CombatAnimation Animation = null;
            public List<CombatResult> Result = null;
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
