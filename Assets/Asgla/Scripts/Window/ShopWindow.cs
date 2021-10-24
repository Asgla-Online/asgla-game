using System.Collections.Generic;
using Asgla.Data.Shop;
using UnityEngine;

namespace Asgla.Window {
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class ShopWindow : ItemListMain {

		public void Init(List<ShopItem> inventory_items) {
			Clear();
			foreach (ShopItem shopItem in inventory_items)
				AddItem(shopItem.DatabaseID, shopItem.Item);
		}

	}
}