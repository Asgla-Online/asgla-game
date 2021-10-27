using Asgla.Data.Item;
using UnityEngine;

namespace AsglaUI.UI {
	public interface IUIItemSlot {

		ItemData GetItemInfo();

		bool Assign(ItemData itemInfo, Object source);

		void Unassign();

	}
}