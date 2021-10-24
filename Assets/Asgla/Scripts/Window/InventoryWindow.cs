using System.Collections.Generic;
using Asgla.Data.Avatar.Player;
using UnityEngine;

namespace Asgla.Window {

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class InventoryWindow : ItemListMain {

		public void Init(List<PlayerInventory> inventory_items) {
			Clear();
			foreach (PlayerInventory inventory in inventory_items)
				AddItem(inventory.databaseId, inventory.item);
			Order();
		}

	}

}