using Asgla.Data.Avatar;
using Asgla.Map;
using System;
using static Asgla.Request.RequestMap;

namespace Asgla.Data.Monster {

    [Serializable]
    public class MonsterData {

        public int UniqueID = -1;

        public int DatabaseID = -1;

        public string Name = null;

        public string Bundle = null;
        public string Asset = null;

        public bool IsAggressive = false;

        public MoveToArea Area = null;

        public double x;
        public double y;

        public int Level;

        public AvatarState State = AvatarState.NONE;

        public MapArea MapArea() => Main.Singleton.MapManager.Map.AreaByName(Area.Area);

        public bool IsNeutral() {
            return State == AvatarState.NORMAL;
        }

        public bool OnCombat() {
            return State == AvatarState.COMBAT;
        }

        public bool IsDead() {
            return State == AvatarState.DEAD;
        }

    }

}