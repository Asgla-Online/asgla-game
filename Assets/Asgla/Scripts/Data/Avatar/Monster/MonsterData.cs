using System;
using Asgla.Area;
using Asgla.Data.Avatar.Helper;

namespace Asgla.Data.Avatar.Monster {

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

		public AreaLocal MapArea() {
			return Main.Singleton.Game.AreaController.Map.AreaByName(Area.area);
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