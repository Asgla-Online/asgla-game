using System.Collections.Generic;
using Asgla.Data.Avatar.Player;
using UnityEngine;

namespace Asgla.UI.Window {

	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class InventoryWindow : ItemListMain {

		public void Init(List<PlayerInventory> inventoryItems) {
			Clear();
			foreach (PlayerInventory inventory in inventoryItems)
				AddItem(inventory.databaseId, inventory.item);
			Order();
		}

	}

}