using System;
using System.Collections.Generic;
using Asgla.Data.Avatar.Player;

namespace Asgla.Data.Area {
	[Serializable]
	public class AreaData {

		public int roomId = -1;

		public int databaseId = -1;

		public string name;

		public string bundle;
		public string asset;

		public List<AreaLocal> locals;

		public List<AreaLocalMonster> monsters;

		public List<PlayerData> players; //ignore

	}
}