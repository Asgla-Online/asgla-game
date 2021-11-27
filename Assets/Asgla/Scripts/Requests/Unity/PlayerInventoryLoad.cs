using System.Collections.Generic;
using System.Linq;
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

			Dictionary<string, string> bundleAsset = new Dictionary<string, string> {
				{"items/armor/armor1", "armor/armor1/armor1.asset"},
				{"items/armor/armor2", "armor/armor2/armor2.asset"},
				{"items/armor/armor3", "armor/armor3/armor3.asset"},
				{"items/weapon/one handed/rose", "weapon/one handed/rose/rose.asset"},
				{"items/weapon/one handed/alice", "weapon/one handed/alice/alice.asset"},
			};

			//TEST
			playerInventoryLoad.inventory = new List<PlayerInventory>();
			for (int i = 0; i < 300; i++) {
				KeyValuePair<string, string> ba = bundleAsset.ElementAt(Random.Range(0, bundleAsset.Count - 1));

				playerInventoryLoad.inventory.Add(new PlayerInventory {
					databaseId = Random.Range(1, 10000000),
					equipped = false,
					quantity = Random.Range(1, 10),

					item = new ItemData {
						databaseId = Random.Range(1, 10000000),

						name = Random.Range(1, 10000000).ToString() + ba.Key,
						description = Random.Range(1, 10000000).ToString(),


						bundle = ba.Key,
						asset = ba.Value,

						rarity = (RarityData) Random.Range(1, 7),

						requiredLevel = Random.Range(1, 100),

						Type = new TypeItemData {
							Category = (PartCategory) Random.Range(0, 17),
							Equipment = (SlotCategory) Random.Range(0, 17),

							Icon = "axe",

							Name = ba.Key + Random.Range(1, 9999999).ToString(),

							//Weapon = Random.Range(1, 9999999).ToString(),
						}
					}
				});
			}

			player.Inventory(playerInventoryLoad.inventory);
		}

	}
}