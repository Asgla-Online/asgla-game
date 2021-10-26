using System;
using Asgla.Data.Avatar;
using Asgla.Data.Monster;

namespace Asgla.Data.Area {
	[Serializable]
	public class AreaLocalMonster {

		public string areaName;

		public MonsterData monster;
		public AvatarStats stats;

		public Asgla.Area.AreaLocal AreaLocal => Main.Singleton.MapManager.Map.AreaByName(areaName);

	}
}