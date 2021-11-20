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
			for (int i = 0; i < 300; i++) {
				bool b = Random.Range(0, 1) == 1;
				playerInventoryLoad.inventory.Add(new PlayerInventory {
					databaseId = Random.Range(1, 10000000),
					equipped = false,
					quantity = Random.Range(1, 10),

					item = new ItemData {
						databaseId = Random.Range(1, 10000000),

						name = Random.Range(1, 10000000).ToString(),
						description = Random.Range(1, 10000000).ToString(),

						bundle = b ? "items/armor/armor1" : "items/weapon/one handed/rose",
						asset = b ? "armor/armor1/armor1.asset" : "weapon/one handed/rose/rose.asset",

						rarity = (RarityData) Random.Range(1, 7),

						requiredLevel = Random.Range(1, 100),

						Type = new TypeItemData {
							Category = (PartCategory) Random.Range(0, 17),
							Equipment = (SlotCategory) Random.Range(0, 17),

							Icon = "axe",

							Name = Random.Range(1, 9999999).ToString(),

							//Weapon = Random.Range(1, 9999999).ToString(),
						}
					}
				});
			}

			player.Inventory(playerInventoryLoad.inventory);
		}

	}
}