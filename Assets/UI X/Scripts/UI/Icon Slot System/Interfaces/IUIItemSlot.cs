using Asgla.Data.Item;
using Object = UnityEngine.Object;

namespace AsglaUI.UI {
    public interface IUIItemSlot {
        ItemData GetItemInfo();
        bool Assign(ItemData itemInfo, Object source);
        void Unassign();
    }
}
