using System;
using System.Collections.Generic;
using System.Linq;
using Asgla.Area;
using Asgla.Data.Avatar.Helper;
using Asgla.Data.Item;

namespace Asgla.Data.Avatar.Player {

	[Serializable]
	public class PlayerData {

		public int playerID = -1;

		public int databaseID = -1;

		public string username = "";

		public string colorSkin;

		public string colorEye;
		public string colorHair;
		public string colorMouth;
		public string colorNose;

		public double x;
		public double y;

		public int level;

		public bool isAway;
		public bool isControlling;

		public AvatarState state = AvatarState.NONE;

		public List<PlayerInventory> inventory;

		public MoveToLocal area = null;

		public EquipPart Ear = null;
		public EquipPart Eye = null;
		public EquipPart Hair = null;
		public EquipPart Mouth = null;
		public EquipPart Nose = null;

		public List<EquipPart> Part = null;

		public AreaLocal MapArea() {
			return Main.Singleton.Game.AreaController.Map.AreaByName(area.area);
		}

		public bool IsNeutral() {
			return state == AvatarState.NORMAL;
		}

		public bool OnCombat() {
			return state == AvatarState.COMBAT;
		}

		public bool IsDead() {
			return state == AvatarState.DEAD;
		}

		public PlayerInventory InventoryById(int databaseId) =>
			inventory.FirstOrDefault(playerInventory => playerInventory.databaseId == databaseId);

		public PlayerInventory InventoryByItemId(int databaseId) =>
			inventory.FirstOrDefault(playerInventory => playerInventory.item.databaseId == databaseId);

	}

	[Serializable]
	public class PlayerInventory {

		public int databaseId;
		public bool equipped;
		public int quantity;

		public ItemData item;

		public void DecreaseQuantity(int amount) {
			quantity -= amount;
		}

		public void IncreaseQuantity(int amount) {
			quantity += amount;
		}

	}

}