using Asgla.Data.Shop;
using BestHTTP.JSON.LitJson;

namespace Asgla.Requests.Unity {
	public class ShopLoad : IRequest {
		
		// ReSharper disable once InconsistentNaming UnassignedField.Global MemberCanBePrivate.Global CollectionNeverUpdated.Global FieldCanBeMadeReadOnly.Global ConvertToConstant.Global
		public ShopData shop;

		public void onRequest(Main main, string json) {
			ShopLoad shopLoad = JsonMapper.ToObject<ShopLoad>(json);
			
			main.Game.WindowShop.Init(shopLoad.shop.Items);
			main.Game.WindowShop.Window().Show();
		}

	}
}