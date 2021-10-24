using System;
using Asgla.Data.Avatar;
using Asgla.Data.Monster;
using Asgla.Map;

namespace Asgla.Data.Area {
	[Serializable]
	public class AreaLocalMonster {

		public string areaName;

		public MonsterData monster;
		public AvatarStats stats;

		public MapArea Area => Main.Singleton.MapManager.Map.AreaByName(areaName);

	}
}