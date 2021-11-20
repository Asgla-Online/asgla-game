using System.Collections.Generic;
using Asgla.Data.Shop;
using UnityEngine;

namespace Asgla.UI.Window {

	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class ShopWindow : ItemListWindow {

		public void Init(List<ShopItem> inventoryItems) {
			Clear();

			foreach (ShopItem shopItem in inventoryItems)
				AddItem(shopItem.DatabaseID, shopItem.Item);
		}

	}

}