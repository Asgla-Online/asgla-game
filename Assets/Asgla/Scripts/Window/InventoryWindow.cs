using Asgla.Data.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Asgla.Window {

    [DisallowMultipleComponent, ExecuteInEditMode, RequireComponent(typeof(CanvasGroup))]
    public class InventoryWindow : ItemListMain {

        public void Init(List<PlayerInventory> inventory_items) {
            Clear();
            foreach (PlayerInventory inventory in inventory_items)
                AddItem(inventory.DatabaseID, inventory.Item);
            Order();
        }

    }

}