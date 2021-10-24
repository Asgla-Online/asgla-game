using System;
using System.Collections.Generic;

namespace Asgla.Data.Area {
	[Serializable]
	public class AreaLocal {

		public string name;

		public double scale;
		public double speed;

		public bool isSafe;

		public List<AreaLocalNpc> npcs;

	}
}