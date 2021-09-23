using System;
using UnityEngine;

namespace Asgla.Data.Type {

    [Serializable]
    public enum RarityData : int {
        Poor = 1,
        Common = 2,
        Uncommon = 3,
        Rare = 4,
        Epic = 5,
        Legendary = 6,
        Contraband = 7,
    }

    public class RarityColor {

        public const string Poor = "9d9d9dff";
        public const string Common = "ffffffff";
        public const string Uncommon = "1eff00ff";
        public const string Rare = "0070ffff";
        public const string Epic = "a335eeff";
        public const string Legendary = "ff8000ff";
        public const string Contraband = "ff3030ff";

        public static string GetHexColor(RarityData r) {
            switch (r) {
                case RarityData.Poor: return Poor;
                case RarityData.Common: return Common;
                case RarityData.Uncommon: return Uncommon;
                case RarityData.Rare: return Rare;
                case RarityData.Epic: return Epic;
                case RarityData.Legendary: return Legendary;
                case RarityData.Contraband: return Contraband;
                default: return Poor;
            }
        }

        public static Color GetColor(RarityData r) {
            return CommonColorBuffer.StringToColor(GetHexColor(r));
        }
    }

}
