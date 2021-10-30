using System.Collections.Generic;
using Asgla.Data.Shop;
using UnityEngine;

namespace Asgla.UI.Window {
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasGroup))]
	public class ShopWindow : ItemListMain {

		public void Init(List<ShopItem> inventoryItems) {
			Clear();
			foreach (ShopItem shopItem in inventoryItems)
				AddItem(shopItem.DatabaseID, shopItem.Item);
		}

	}
}