using Asgla.Data.Item;

namespace Asgla.Data.Item {

    public enum ItemListType {
        Equip,
        Buy,
        Sell,
        Quest,
    }

    //TODO: More "item list" stuff, color..
    public class ItemListData {
        public int DatabaseID;

        public bool Equipped;
        public int Quantity;

        public ItemData Item;
    }

}