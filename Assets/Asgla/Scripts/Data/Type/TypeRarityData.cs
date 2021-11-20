using System;
using Asgla.Utility;
using UnityEngine;

namespace Asgla.Data.Type {

	[Serializable]
	public enum RarityData {

		Poor = 1,
		Common = 2,
		Uncommon = 3,
		Rare = 4,
		Epic = 5,
		Legendary = 6,
		Contraband = 7

	}

	public static class RarityColor {

		private const string Poor = "9d9d9dff";
		private const string Common = "ffffffff";
		private const string Uncommon = "1eff00ff";
		private const string Rare = "0070ffff";
		private const string Epic = "a335eeff";
		private const string Legendary = "ff8000ff";
		private const string Contraband = "ff3030ff";

		public static string GetHexColor(RarityData r) {
			return r switch {
				RarityData.Poor => Poor,
				RarityData.Common => Common,
				RarityData.Uncommon => Uncommon,
				RarityData.Rare => Rare,
				RarityData.Epic => Epic,
				RarityData.Legendary => Legendary,
				RarityData.Contraband => Contraband,
				_ => Poor
			};
		}

		public static Color GetColor(RarityData r) {
			return CommonColorBuffer.StringToColor(GetHexColor(r));
		}

	}

}