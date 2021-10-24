using System.Collections.Generic;
using Asgla.Data.Item;

namespace Asgla.Data.Shop {

	public class ShopData {

		public int DatabaseID;

		public bool IsStaffOnly;

		public List<ShopItem> Items;

		public string Name;

	}

	public class ShopItem {

		public int DatabaseID;

		public bool IsLimited;

		public ItemData Item;

		public int QuantityRemain;

	}

}