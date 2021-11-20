using System.Collections.Generic;
using System.Linq;
using Asgla.Data.Avatar.Player;
using CharacterCreator2D;
using UnityEngine;

namespace Asgla.UI.Window {

	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	public class InventoryWindow : ItemListWindow {

		protected override void Awake() {
			base.Awake();

			Tabs = new List<Tab> {
				new Tab {
					Name = "All",
					Category = new List<PartCategory> {
						PartCategory.Armor,
						PartCategory.Boots,
						PartCategory.Ear,
						PartCategory.Eyebrow,
						PartCategory.Eyes,
						PartCategory.FacialHair,
						PartCategory.Gloves,
						PartCategory.Hair,
						PartCategory.Helmet,
						PartCategory.Mouth,
						PartCategory.Nose,
						PartCategory.Pants,
						PartCategory.SkinDetails,
						PartCategory.Weapon,
						PartCategory.Cape,
						PartCategory.Skirt,
						PartCategory.BodySkin,
					}
				},
				new Tab {
					Name = "Armor",
					Category = new List<PartCategory> {
						PartCategory.Armor
					},
				},
				new Tab {
					Name = "Weapon",
					Category = new List<PartCategory> {
						PartCategory.Weapon
					}
				},
				new Tab {
					Name = "Helm",
					Category = new List<PartCategory> {
						PartCategory.Helmet
					}
				},
				new Tab {
					Name = "Cape",
					Category = new List<PartCategory> {
						PartCategory.Cape
					}
				}
			};
		}

		public void Init(IEnumerable<PlayerInventory> inventoryItems) {
			Clear();

			foreach (PlayerInventory playerInventory in inventoryItems.OrderBy(
				inventory => inventory.item.Type.Category))
				AddItem(playerInventory.databaseId, playerInventory.item);
		}

	}

}