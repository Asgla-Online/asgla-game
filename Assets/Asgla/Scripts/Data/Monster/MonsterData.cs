using System;
using Asgla.Data.Avatar;
using Asgla.Map;
using Asgla.Requests.Unity;

namespace Asgla.Data.Monster {

	[Serializable]
	public class MonsterData {

		public int UniqueID = -1;

		public int DatabaseID = -1;

		public string Name;

		public string Bundle;
		public string Asset;

		public bool IsAggressive;

		public double x;
		public double y;

		public int Level;

		public AvatarState State = AvatarState.NONE;

		public MoveToLocal Area = null;

		public MapArea MapArea() {
			return Main.Singleton.MapManager.Map.AreaByName(Area.area);
		}

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