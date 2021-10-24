using Asgla.Data.Avatar;
using Asgla.Data.Item;
using Asgla.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using Asgla.Requests.Unity;

namespace Asgla.Data.Player {

    [Serializable]
    public class PlayerData {

        public int playerID = -1;

        public int databaseID = -1;

        public string username = "";

        public EquipPart Ear = null;
        public EquipPart Eye = null;
        public EquipPart Hair = null;
        public EquipPart Mouth = null;
        public EquipPart Nose = null;

        public string colorSkin;

        public string colorEye;
        public string colorHair;
        public string colorMouth;
        public string colorNose;

        public MoveToLocal Area = null;

        public double x;
        public double y;

        public int level;

        public bool isAway;
        public bool isControlling;

        public AvatarState state = AvatarState.NONE;

        public List<EquipPart> Part = null;

        public List<PlayerInventory> inventory;

        public MapArea MapArea() => Main.Singleton.MapManager.Map.AreaByName(Area.area);

        public bool IsNeutral() {
            return state == AvatarState.NORMAL;
        }

        public bool OnCombat() {
            return state == AvatarState.COMBAT;
        }

        public bool IsDead() {
            return state == AvatarState.DEAD;
        }

        public PlayerInventory InventoryById(int databaseId) => inventory.First(playerInventory => playerInventory.databaseId == databaseId);

        public PlayerInventory InventoryByItemId(int databaseId) => inventory.First(playerInventory => playerInventory.item.databaseId == databaseId);

    }

    [Serializable]
    public class PlayerInventory {

        public int databaseId;
        public bool equipped;
        public int quantity;

        public ItemData item;

        public void DecreaseQuantity(int amount) => quantity -= amount;

        public void IncreaseQuantity(int amount) => quantity += amount;

    }

}