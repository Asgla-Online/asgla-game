using System;
using System.Collections.Generic;
using Asgla.Avatar;
using Asgla.Data.Avatar;
using Asgla.Data.Monster;
using Asgla.Data.NPC;
using Asgla.Data.Player;
using Asgla.Map;

namespace Asgla.Data.Map {

	[Serializable]
	public class AreaData {

		public int MapID = -1;

		public int DatabaseID = -1;

		public string Name;

		public string Bundle;
		public string Asset;

		public List<MapAreaData> Areas;

		public List<MapMonster> Monsters;

		public List<PlayerData> Players; //ignore

	}

	[Serializable]
	public class MapAreaData {

		public string Name;

		public double Scale;
		public double Speed;

		public bool IsSafe;

		public List<MapAreaNPC> NPCs;

	}

	[Serializable]
	public class MapAreaNPC {

		public int DatabaseID;
		public NPCData NPC;

	}

	[Serializable]
	public class MapMonster {

		public string AreaName;

		public MonsterData Monster;
		public AvatarStats Stats;

		public MapArea Area => Main.Singleton.MapManager.Map.AreaByName(AreaName);

	}

	[Serializable]
	public class MapAvatar {

		public AvatarMain Entity;
		public MapArea Area;

	}

	public enum MapMoveType {

		MoveToArea,
		JoinMap

	}

}