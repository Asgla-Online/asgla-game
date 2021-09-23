using Asgla.Data.Item;
using System.Collections.Generic;

namespace Asgla.Data.Shop {

    public class ShopData {
        public int DatabaseID;

        public string Name;

        public bool IsStaffOnly;

        public List<ShopItem> Items;
    }

    public class ShopItem {
        public int DatabaseID;

        public bool IsLimited;

        public int QuantityRemain;

        public ItemData Item;
    }

}
