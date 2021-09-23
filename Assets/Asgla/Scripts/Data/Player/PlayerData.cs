using Asgla.Data.Avatar;
using Asgla.Data.Item;
using Asgla.Map;
using System;
using System.Collections.Generic;
using System.Linq;

using static Asgla.Request.RequestAvatar;
using static Asgla.Request.RequestMap;

namespace Asgla.Data.Player {

    [Serializable]
    public class PlayerData {

        public int PlayerID = -1;

        public int DatabaseID = -1;

        public string Username = "";

        public EquipPart Ear = null;
        public EquipPart Eye = null;
        public EquipPart Hair = null;
        public EquipPart Mouth = null;
        public EquipPart Nose = null;

        public string ColorSkin = null;

        public string ColorEye = null;
        public string ColorHair = null;
        public string ColorMouth = null;
        public string ColorNose = null;

        public MoveToArea Area = null;

        public double x;
        public double y;

        public int Level;

        public bool Away = false;
        public bool Controlling = false;

        public AvatarState State = AvatarState.NONE;

        public List<EquipPart> Part = null;

        public List<PlayerInventory> Inventory = null;

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

        public PlayerInventory InventoryById(int databaseId) => Inventory.Where(inventory => inventory.DatabaseID == databaseId).FirstOrDefault();

        public PlayerInventory InventoryByItemId(int databaseId) => Inventory.Where(inventory => inventory.Item.DatabaseID == databaseId).FirstOrDefault();

    }

    [Serializable]
    public class PlayerInventory {

        public int DatabaseID;
        public bool Equipped;
        public int Quantity;

        public ItemData Item;

        public void DecreaseQuantity(int amount) => Quantity -= amount;

        public void IncreaseQuantity(int amount) => Quantity += amount;

    }

}