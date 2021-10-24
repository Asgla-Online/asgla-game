using System.Collections.Generic;
using System.Linq;
using Asgla.Data.Item;
using Asgla.Data.Type;
using Asgla.UI.Item;
using AsglaUI.UI;
using UnityEngine;

namespace Asgla.Window {
	public abstract class ItemListMain : UIWindow {

		[SerializeField] private ItemListType _type = ItemListType.Equip;

		[SerializeField] protected Transform _all;
		[SerializeField] protected Transform _class;
		[SerializeField] protected Transform _armor;
		[SerializeField] protected Transform _weapon;
		[SerializeField] protected Transform _helm;

		[SerializeField] private ItemSlot _slot;

		[SerializeField] private List<ItemSlot> _slots;

		public void AddItem(int databaseId, ItemData item) {
			AddSlot(databaseId, item, _all);
			switch (item.Type.Category) {
				case Category.Armor:
					AddSlot(databaseId, item, _armor);
					break;
				case Category.Boot:
					break;
				case Category.Ear:
					break;
				case Category.Eyebrow:
					break;
				case Category.Eye:
					break;
				case Category.FacialHair:
					break;
				case Category.Glove:
					break;
				case Category.Hair:
					break;
				case Category.Helmet:
					AddSlot(databaseId, item, _helm);
					break;
				case Category.Mouth:
					break;
				case Category.Nose:
					break;
				case Category.Pant:
					break;
				case Category.SkinDetail:
					break;
				case Category.Weapon:
					AddSlot(databaseId, item, _weapon);
					break;
				case Category.Cape:
					break;
				case Category.Skirt:
					break;
				case Category.BodySkin:
					break;
				case Category.Class:
					AddSlot(databaseId, item, _class);
					break;
			}
		}

		public void RemoveItem(int databaseId) {
			ItemSlot slot = _slots.Where(s => s.Id() == databaseId).FirstOrDefault();
			if (slot != null)
				Destroy(slot.gameObject);
		}

		private void AddSlot(int databaseId, ItemData item, Transform content) {
			_slots.Add(Instantiate(_slot.gameObject, content)
				.GetComponent<ItemSlot>()
				.Init(databaseId, _type, item));
		}

		protected void Order() {
			int i = 0;
			foreach (ItemSlot slot in from s in _slots orderby s.Item().Type.Category select s) {
				//Debug.LogFormat("{0} {1}", slot.Item().Name, i);
				slot.transform.SetSiblingIndex(i);
				i++;
			}
		}

		protected void Clear() {
			Main.Singleton.UIManager.ClearChild(_all.transform, _armor.transform, _weapon.transform, _helm.transform);
		}

	}
}