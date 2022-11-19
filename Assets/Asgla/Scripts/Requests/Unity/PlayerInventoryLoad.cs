using System.Collections.Generic;
using Asgla.Avatar.Player;
using Asgla.Data.Avatar.Player;
using Asgla.Data.Item;
using Asgla.Data.Type;
using BestHTTP.JSON.LitJson;
using CharacterCreator2D;
using UnityEngine;

namespace Asgla.Requests.Unity {
	public class PlayerInventoryLoad : IRequest {

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public List<PlayerInventory> inventory;

		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public int playerId;

		public void onRequest(Main main, string json) {
			PlayerInventoryLoad playerInventoryLoad = JsonMapper.ToObject<PlayerInventoryLoad>(json);

			Player player = main.Game.AreaController.PlayerByID(playerInventoryLoad.playerId);

			if (player is null)
				return;

			//TEST
			playerInventoryLoad.inventory = new List<PlayerInventory>();
			
			TypeItemData armor = new() {
				Category = PartCategory.Armor,
				Equipment = SlotCategory.Armor,

				Icon = "armor",

				Name = "Armor",
			};
			
			TypeItemData weapon = new() {
				Category = PartCategory.Weapon,
				Equipment = SlotCategory.MainHand,

				Icon = "sword",

				Name = "Weapon",
			};

			playerInventoryLoad.inventory.Add(new PlayerInventory {
				databaseId = Random.Range(1, 10000000),
				equipped = false,
				quantity = Random.Range(1, 10),

				item = new ItemData {
					databaseId = Random.Range(1, 10000000),

					name = Random.Range(1, 10000000) + "items/armor/armor1",
					description = Random.Range(1, 10000000).ToString(),


					bundle = "items/armor/armor1",
					asset = "armor/armor1/armor1.asset",

					rarity = (RarityData) Random.Range(1, 7),

					requiredLevel = Random.Range(1, 100),

					Type = armor
				}
			});

			playerInventoryLoad.inventory.Add(new PlayerInventory {
				databaseId = Random.Range(1, 10000000),
				equipped = false,
				quantity = Random.Range(1, 10),

				item = new ItemData {
					databaseId = Random.Range(1, 10000000),

					name = Random.Range(1, 10000000) + "items/armor/armor2",
					description = Random.Range(1, 10000000).ToString(),


					bundle = "items/armor/armor2",
					asset = "armor/armor2/armor2.asset",

					rarity = (RarityData) Random.Range(1, 7),

					requiredLevel = Random.Range(1, 100),

					Type = armor
				}
			});

			playerInventoryLoad.inventory.Add(new PlayerInventory {
				databaseId = Random.Range(1, 10000000),
				equipped = false,
				quantity = Random.Range(1, 10),

				item = new ItemData {
					databaseId = Random.Range(1, 10000000),

					name = Random.Range(1, 10000000) + "items/armor/armor3",
					description = Random.Range(1, 10000000).ToString(),


					bundle = "items/armor/armor3",
					asset = "armor/armor3/armor3.asset",

					rarity = (RarityData) Random.Range(1, 7),

					requiredLevel = Random.Range(1, 100),

					Type = armor
				}
			});

			playerInventoryLoad.inventory.Add(new PlayerInventory {
				databaseId = Random.Range(1, 10000000),
				equipped = false,
				quantity = Random.Range(1, 10),

				item = new ItemData {
					databaseId = Random.Range(1, 10000000),

					name = Random.Range(1, 10000000) + "items/armor/rose",
					description = Random.Range(1, 10000000).ToString(),


					bundle = "items/weapon/one handed/rose",
					asset = "weapon/one handed/rose/rose.asset",

					rarity = (RarityData) Random.Range(1, 7),

					requiredLevel = Random.Range(1, 100),

					Type = weapon
				}
			});

			playerInventoryLoad.inventory.Add(new PlayerInventory {
				databaseId = Random.Range(1, 10000000),
				equipped = false,
				quantity = Random.Range(1, 10),

				item = new ItemData {
					databaseId = Random.Range(1, 10000000),

					name = Random.Range(1, 10000000) + "items/armor/rose",
					description = Random.Range(1, 10000000).ToString(),


					bundle = "items/weapon/one handed/alice",
					asset = "weapon/one handed/alice/alice.asset",

					rarity = (RarityData) Random.Range(1, 7),

					requiredLevel = Random.Range(1, 100),

					Type = weapon
				}
			});

			player.Inventory(playerInventoryLoad.inventory);
		}

	}
}